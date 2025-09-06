using System.Text.Json.Serialization;

namespace PSM.OTD;

#pragma warning disable CS8618

public static class C2SPackets
{
    public interface IPacket {}
        
    public class Hi : IPacket
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "Hi";
        [JsonPropertyName("name")] public string Name { get; set; }
    }
    
    public class TabletEvent : IPacket
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "TabletEvent";
        [JsonPropertyName("status")] public uint Status { get; set; }
        [JsonPropertyName("buttons")] public uint Buttons { get; set; }
        [JsonPropertyName("x")] public uint X { get; set; }
        [JsonPropertyName("y")] public uint Y { get; set; }
        [JsonPropertyName("z")] public uint Z { get; set; }
        [JsonPropertyName("normal_pressure")] public uint NormalPressure { get; set; }
        [JsonPropertyName("tangential_pressure")] public uint TangentialPressure { get; set; }
    }
    
    public class Proximity : IPacket
    {
        [JsonPropertyName("type")] public string Type { get; set; } = "Proximity";
        [JsonPropertyName("value")] public bool Value { get; set; }
    }
}

public static class S2CPackets
{
    public class TypeBase
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
    
    public class Hi
    {
        [JsonPropertyName("compatible")]
        public uint Compatible { get; set; }
    }
}