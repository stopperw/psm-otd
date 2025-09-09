using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace PSM.OTD;

public static class ConfigGenerator
{
    public static string GenerateConfig(AbsoluteOutputMode absolute, TabletReference tablet)
    {
        var outputWidth = (uint)absolute.Output.Width;
        var outputHeight = (uint)absolute.Output.Height;
        string config = $@"{{
    ""preset"": {{
        ""status"": 0,
        ""packet_rate"": 255,
        ""packet_mode"": 0,
        ""move_mask"": 4294967295,
        ""in_org_x"": 0,
        ""in_org_y"": 0,
        ""in_org_z"": -1023,
        ""in_ext_x"": {outputWidth * 1000},
        ""in_ext_y"": {outputHeight * 1000},
        ""in_ext_z"": 2047,
        ""out_org_x"": 0,
        ""out_org_y"": 0,
        ""out_org_z"": -1023,
        ""out_ext_x"": {outputWidth * 1000},
        ""out_ext_y"": {outputHeight * 1000},
        ""out_ext_z"": 2047,
        ""sys_org_x"": 0,
        ""sys_org_y"": 0,
        ""sys_ext_x"": {outputWidth},
        ""sys_ext_y"": {outputHeight},
        ""hardware"": 12,
        ""x_margin"": 0,
        ""y_margin"": 0,
        ""z_margin"": 0,
        ""device_x"": {{
            ""min"": 0,
            ""max"": {outputWidth * 1000},
            ""units"": 2,
            ""resolution"": 65536000
        }},
        ""device_y"": {{
            ""min"": 0,
            ""max"": {outputHeight * 1000},
            ""units"": 2,
            ""resolution"": 65536000
        }},
        ""device_z"": {{
            ""min"": -1023,
            ""max"": 1023,
            ""units"": 2,
            ""resolution"": 65536000
        }},
        ""normal_pressure"": {{
            ""min"": 0,
            ""max"": 32767,
            ""units"": 0,
            ""resolution"": 0
        }},
        ""tangential_pressure"": {{
            ""min"": 0,
            ""max"": 1023,
            ""units"": 0,
            ""resolution"": 0
        }},
        ""orientation"": [
            {{
                ""min"": 0,
                ""max"": 65535,
                ""units"": 0,
                ""resolution"": 65536000
            }},
            {{
                ""min"": 0,
                ""max"": 65535,
                ""units"": 0,
                ""resolution"": 65536000
            }},
            {{
                ""min"": 0,
                ""max"": 65535,
                ""units"": 0,
                ""resolution"": 65536000
            }}
        ],
        ""rotation"": [
            {{
                ""min"": 0,
                ""max"": 65535,
                ""units"": 0,
                ""resolution"": 65536000
            }},
            {{
                ""min"": 0,
                ""max"": 65535,
                ""units"": 0,
                ""resolution"": 65536000
            }},
            {{
                ""min"": 0,
                ""max"": 65535,
                ""units"": 0,
                ""resolution"": 65536000
            }}
        ]
    }}
}}";
        return config;
    }
}