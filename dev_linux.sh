#!/bin/sh

# https://github.com/TheBlueOompaLoompa/BetterCalibrator/blob/56b8d873f386f6a42d502498490111a6bd50c541/debug.sh

systemctl --user stop opentabletdriver.service
killall -9 OpenTabletDriver.UX.Gtk
killall -9 OpenTabletDriver.Daemon

dotnet build

mkdir -p ~/.config/OpenTabletDriver/Plugins/PSM/
rm ~/.config/OpenTabletDriver/Plugins/PSM/psm-otd.dll
cp bin/Debug/net8.0/psm-otd.dll ~/.config/OpenTabletDriver/Plugins/PSM/psm-otd.dll

otd-gui > /dev/null 2>&1 &
otd-daemon
