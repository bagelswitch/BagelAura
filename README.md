# BagelAura

Simple Windows Forms app that does a couple things:
  * Uses the Asus Aura COM interface to control the RGB lights on Asus motherboard I/O panel and G.Skill DRAM sticks. Lights are illuminated in gradually cross-faded blue->green->yellow->red, according to total CPU utilization.
  * Renders a continuous/rolling graph of total CPU utilization history to a small translucent window on monitor 0, using 50 connected polygons. Coloring matches that used for the Aura lighting.

The app is meant to run continously (eg. be placed in the startup folder) and has no dock or tray icons when running.
