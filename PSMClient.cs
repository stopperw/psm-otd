using System.Numerics;
using System.Reflection;
using System.Timers;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using Timer = System.Timers.Timer;

namespace PSM.OTD;

#pragma warning disable CS8618

[PluginName(DisplayName)]
// ReSharper disable once ClassNeverInstantiated.Global
public class PSMClient : IPositionedPipelineElement<IDeviceReport>, IDisposable
{
    public const string DisplayName = "Pain Studio Mask client";
    public const string Version = "0.0.2";
    
    public static PSMClient? Instance;

    public bool Connected => _active && _connection != null && (_connection?.Connected ?? false);
    
    private bool _active = false;
    private readonly Connection? _connection;
    
    private bool _firstEvent = true;
    private bool _proximity = false;

    public const double ProximityTimerInterval = 2000; // 2000ms (2 sec.)
    [BooleanProperty(
        "Timeout-based proximity loss",
        "Report pen being out of range when there is no packets in the last 2 seconds"
    )]
    [DefaultPropertyValue(true)]
    public bool TimeoutProximity { get; set; } = true;

    private Timer _proximityTimer;
    private Timer ProximityTimer
    {
        get => _proximityTimer;
        set
        {
            _proximityTimer = value;
            _proximityTimer.Elapsed += OnProximityTimerElapsed;
        }
    }

    public PSMClient()
    {
        Log.Write(nameof(PSMClient), $"PSM Client v{Version} enabled!");
        Instance = this;
        _active = true;
        if (_connection != null)
            _connection.Start();
        else
            _connection = new Connection();
        ProximityTimer = new Timer(ProximityTimerInterval);
    }
    
    public void Dispose()
    {
        Log.Write(nameof(PSMClient), $"Client disabled.");
        _active = false;
        _proximity = false;
        _connection?.Terminate();
    }
    
    public void Consume(IDeviceReport value)
    {
        Emit?.Invoke(value);
        
        if (_firstEvent)
        {
            _firstEvent = false;
            DetectOutputMode();
            string config = ConfigGenerator.GenerateConfig(AbsoluteOutputMode!, Tablet!);
            string pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string configPath = Path.Join(pluginFolder, "psm.json");
            File.WriteAllText(configPath, config);
            Log.Write(nameof(PSMClient), $"Default config for your tablet was written to {configPath}");
            Log.Write(nameof(PSMClient), $"Put it in target app's folder alongside PSM's wintab32.dll");
        }

        if (!Connected || _connection == null) return;

        if (value is ITabletReport report)
        {
            // Log.Write(nameof(PSMClient),
            //     $"TAB {report.Position} | {report.Pressure} | {LogHelper.LogButtons(report.PenButtons)}");

            float pressure = report.Pressure;
            float pressureValue = pressure / Tablet!.Properties.Specifications.Pen.MaxPressure;
            uint normalPressure = (uint)(pressureValue * 32767f);

            bool mainBtn = pressureValue > 0.01f;
            uint buttons = (uint)(mainBtn ? 1 : 0);
            for (int i = 0; i < report.PenButtons.Length; i++)
            {
                buttons |= ((uint)(report.PenButtons[i] ? 1 : 0) << (i + 1));
            }

            float x = report.Position.X;
            float y = report.Position.Y;
            Vector2 areaPos = AbsoluteOutputMode.Output.Position -
                              new Vector2(AbsoluteOutputMode.Output.Width, AbsoluteOutputMode.Output.Height) / 2;
            x -= areaPos.X;
            y -= areaPos.Y;
            
            // Out of range check
            if (
                x < 0 ||
                y < 0 ||
                x > AbsoluteOutputMode.Output.Width ||
                y > AbsoluteOutputMode.Output.Height
            ) return;
            
            _connection.SendPacket(new C2SPackets.TabletEvent
            {
                Buttons = buttons,
                X = (uint)(x * 1000),
                Y = (uint)(y * 1000),
                NormalPressure = normalPressure,
            });

            ProximityTimer.Start();
        }

        if (value is IProximityReport proximity)
        {
            // Log.Write(nameof(PSMClient), $"PROX {proximity.HoverDistance} | NEAR: {proximity.NearProximity}");
            UpdateProximity(proximity.NearProximity);
        }

        if (value is OutOfRangeReport outOfRange)
        {
            // Log.Write(nameof(PSMClient), $"Out of range.");
            UpdateProximity(false);
        }

        Emit?.Invoke(value);
    }

    private void UpdateProximity(bool value)
    {
        if (value != _proximity && _active && _connection != null)
        {
            _connection.SendPacket(new C2SPackets.Proximity
            {
                Value = value
            });
        }

        _proximity = value;
    }

    private void OnProximityTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (!TimeoutProximity) return;
        if (!Connected || _connection == null) return;
        UpdateProximity(false);
    }

    public event Action<IDeviceReport>? Emit;
    public PipelinePosition Position => PipelinePosition.PostTransform;
    [TabletReference] public TabletReference Tablet { get; set; }
    [Resolved] public IDriver Driver { get; set; }
    public AbsoluteOutputMode AbsoluteOutputMode { get; set; }

    public void DetectOutputMode()
    {
        if (Driver is not OpenTabletDriver.Driver driver) return;
        IOutputMode output = driver.InputDevices
            .Where(dev => dev?.OutputMode?.Elements?.Contains(this) ?? false)
            .Select(dev => dev?.OutputMode).FirstOrDefault()!;

        if (output is AbsoluteOutputMode absolute)
            AbsoluteOutputMode = absolute;
    }
}