# OpenTabletDriver plugin for Pain Studio Mask

This is a OpenTabletDriver client plugin made for use with
[Pain Studio Mask](https://github.com/stopperw/pain-studio-mask?tab=readme-ov-file).
**You can use it to fix graphics tablet support in Wine**, for example
when running **CLIP STUDIO PAINT**.

# Installation

Download and setup [Pain Studio Mask](https://github.com/stopperw/pain-studio-mask?tab=readme-ov-file), if you haven't already.

Download and setup OpenTabletDriver, if you haven't already -
[Linux](https://opentabletdriver.net/Wiki/Install/Linux) |
[Windows](https://opentabletdriver.net/Wiki/Install/Windows) |
[macOS](https://opentabletdriver.net/Wiki/Install/MacOS)

Download the latest OTD plugin from [releases](https://github.com/stopperw/psm-otd/releases/latest)
and put the DLL into:

- Windows: `%localappdata%\OpenTabletDriver\Plugins\PSM\psm-otd.dll`
- Linux: `~/.config/OpenTabletDriver/Plugins/PSM/psm-otd.dll`
- macOS: `$HOME/Library/Application Support/OpenTabletDriver/Plugins/PSM/psm-otd.dll`

Create the `PSM` folder if it doesn't exist.

# Generating the config for PSM (psm.json)

Launch OTD, enable the client plugin in `Filters` tab, press Apply and interact with your tablet.

This will create `psm.json` with your display & tablet's options in the plugin folder (`~/.config/OpenTabletDriver/PSM/psm.json`).
You can copy it to your drawing app's folder (for CSP, it should be at `C:\Program Files\CELSYS\CLIP STUDIO 1.5\CLIP STUDIO PAINT\psm.json`)
alongside Pain Studio Mask's `wintab32.dll`.

> In some cases, psm.json might be generated in a wrong folder. Absolute path to it will be in the log.

# Development

- `dotnet restore`
- `dotnet build`
- The resulting DLL is located at `bin/Debug/net8.0/psm-otd.dll`.
- \[Linux\] Run `./dev_linux.sh` to automatically build the plugin and
  run OTD with it installed.

Make a release build with `dotnet build -c Release`.

# Disclaimer

The plugin will try to make a TCP connection to `127.0.0.1:40302` (PSM server) every 15 seconds.
If this is something you don't want, disable the client plugin in `Filters` (and Apply)
when you are not using PSM.

