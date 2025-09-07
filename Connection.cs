using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using OpenTabletDriver.Plugin;

namespace PSM.OTD;

public class Connection
{
    private bool Active { get; set; }
    public bool Connected => Active && Client.Connected;
    private Thread Thread { get; set; }
    private TcpClient Client { get; set; }

    public Connection()
    {
        Active = true;
        Client = new TcpClient();
        Thread = new Thread(ThreadMain);
        Thread.Start();
    }

    void ThreadMain()
    {
        Log.Write(LogGroup, "Connection thread started.");
        while (true)
        {
            if (Active) 
                EstablishConnection();
            Thread.Sleep(15000);
        }
    }

    void EstablishConnection()
    {
        try
        {
            Client = new TcpClient();
            Client.Connect("127.0.0.1", 40302);
        }
        catch (Exception)
        {
            // Log.Exception(e);
            // ignored
        }
        Log.Write(LogGroup, "Successfully connected to a PSM server!");
        SendPacket(new C2SPackets.Hi
        {
            Name = "PSM-OTD 0.0.1"
        });
        // Connection may cut like that when closing the app with injected PSM,
        // so this check is needed to not crash the plugin/OTD.
        if (!Connected) return;
        NetworkStream stream = Client.GetStream();
        try
        {
            while (true)
            {
                byte[] sizeBytes = new byte[4];
                stream.ReadExactly(sizeBytes);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(sizeBytes);
                uint size = BitConverter.ToUInt32(sizeBytes);
                byte[] data = new byte[size];
                stream.ReadExactly(data);
                string json = Encoding.UTF8.GetString(data);
                // Log.Write(LogGroup, json);
                
                var typeBase = JsonSerializer.Deserialize<S2CPackets.TypeBase>(json);
                if (typeBase == null)
                    continue;
                
                if (typeBase.Type == "Hi")
                {
                    var packet = JsonSerializer.Deserialize<S2CPackets.Hi>(json);
                    if (packet == null)
                        continue;
                    if (packet.Compatible != NetCode.CompatibleVersion)
                    {
                        Log.Write(LogGroup, $"Server is running an incompatible PSM version! S{packet.Compatible} != C{NetCode.CompatibleVersion}");
                        break;
                    }
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
        Terminate();
        Log.Write(LogGroup, "Disconnected from the PSM server.");
    }

    public void SendPacket<T>(T packet) where T: C2SPackets.IPacket
    {
        if (!Connected) return;
        
        string json = JsonSerializer.Serialize(packet);
        // Log.Write(LogGroup, json);

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            uint size = (uint)data.Length;
            byte[] sizeBytes = BitConverter.GetBytes(size);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(sizeBytes);
            Client.GetStream().Write(sizeBytes);
            Client.GetStream().Write(data);
        }
        catch (Exception e)
        {
            Log.Exception(e);
            Log.Write(LogGroup, "Packet write failed!");
            Terminate();
            // ignored
        }
    }
    
    public void Start()
    {
        Active = true;
        Client.Close();
        Log.Write(LogGroup, "Connection terminated.");
    }
    
    public void Terminate()
    {
        Active = false;
        Client.Close();
        Log.Write(LogGroup, "Connection terminated.");
    }

    private const string LogGroup = "PSMConnection";
}