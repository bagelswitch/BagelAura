# BagelAura

Simple Windows Forms app that does a few things:
  * Renders a continuous/rolling graph of total CPU utilization history to a small translucent window on a secondary monitor, using 50 connected polygons, colored according to instantaneous utilization, as well as disk/network utilization indicators.
  * Uses the google Tenor API to present random GIFs in another window on a secondary monitor, loosely related to the current active window title.
  * Manages the state of the HYTE Nexus app and OpenRGB service, in an attempt to keep all my RGB lighting working across sleep/wake and other system events that tend to break things.

The app is meant to run continously (eg. be placed in the startup folder) and has no dock or tray icons when running.
