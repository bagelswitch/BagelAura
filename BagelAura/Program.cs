﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using AuraServiceLib;

namespace BagelAura
{
    internal class Program
    {
        // Create SDK instance
        static IAuraSdk3 sdk = new AuraSdk() as IAuraSdk3;

        // enumerate all devices
        static IAuraSyncDeviceCollection devices = sdk.Enumerate(0);

        static IAuraSyncDevice stickOne = null;
        static IAuraSyncDevice stickTwo = null;
        static IAuraSyncDevice mBoard = null;

        static List<IAuraRgbLight> stickOneLights = null;
        static List<IAuraRgbLight> stickTwoLights = null;
        static List<IAuraRgbLight> mBoardLights = null;

        static uint[] colors = {
                0x0000FF00,
                0x0000DF3F,
                0x0000BF5F,
                0x00009F7F,
                0x00007F9F,
                0x00005FBF,
                0x00003FDF,
                0x000000FF};

        static void ObtainControl()
        {
            // Aquire control
            sdk.ReleaseControl(0);
            sdk.SwitchMode();

            // Traverse all devices
            foreach (IAuraSyncDevice dev in devices)
            {
                if (dev.Name.Equals("ENE_RGB_For_ASUS0"))
                {
                    stickOne = dev;
                    stickOneLights = stickOne.Lights.Cast<IAuraRgbLight>().ToList();
                    stickOneLights.Reverse();
                }
                else if (dev.Name.Equals("ENE_RGB_For_ASUS1"))
                {
                    stickTwo = dev;
                    stickTwoLights = stickTwo.Lights.Cast<IAuraRgbLight>().ToList();
                    stickTwoLights.Reverse();
                }
                else if (dev.Name.Equals("Mainboard_Master"))
                {
                    mBoard = dev;
                    mBoardLights = mBoard.Lights.Cast<IAuraRgbLight>().ToList();
                    mBoardLights.Reverse();
                }
            }
        }

        static void Main(string[] args)
        {
            var cpu = new PerformanceCounter
            {
                CategoryName = "Processor Information",
                CounterName = "% Processor Utility",
                InstanceName = "_Total"
            };

            int k = 1;
            while (true)
            {
                uint cpuLoad = (uint)(cpu.NextValue() * 100);

                if (k == 1 || k == 600)
                {
                    ObtainControl();
                    k = 1;
                }

                // Set LEDs on mboard i/o panel
                if (cpuLoad > 500)
                {
                    mBoardLights[0].Color = colors[0];
                } else
                {
                    mBoardLights[0].Color = 0x00000000;
                }

                if (cpuLoad > 2500)
                {
                    mBoardLights[1].Color = colors[4];
                }
                else
                {
                    mBoardLights[1].Color = 0x00000000;
                }

                if (cpuLoad > 4000)
                {
                    mBoardLights[2].Color = colors[7];
                }
                else
                {
                    mBoardLights[2].Color = 0x00000000;
                }

                mBoard.Apply();

                // Traverse all LEDs on stick one
                int i = 1;
                foreach (IAuraRgbLight light in stickOneLights)
                {
                    if(cpuLoad > (i*500))
                    {
                        light.Color = colors[i-1];
                    } else
                    {
                        light.Color = 0x00000000;
                    }
                    i++;
                }
                stickOne.Apply();

                // Traverse all LEDs on stick two
                int j = 1;
                foreach (IAuraRgbLight light in stickTwoLights)
                {
                    if (cpuLoad > (j * 500))
                    {
                        light.Color = colors[j - 1];
                    } else
                    {
                        light.Color = 0x00000000;
                    }
                    j++;
                }
                stickTwo.Apply();

                System.Threading.Thread.Sleep(500);
                k++;
            }
            sdk.ReleaseControl(0);
        }
    }
}
