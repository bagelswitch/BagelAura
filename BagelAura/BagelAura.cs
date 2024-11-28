using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AuraServiceLib;

namespace BagelAura
{
    internal class BagelAura
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

        static uint AdjustColorIntensity(uint color, float intensity)
        {
            byte[] bytes = BitConverter.GetBytes(color);
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(bytes);

            byte[] newBytes = {
                (byte) ((float) bytes[0] * intensity),
                (byte) ((float) bytes[1] * intensity),
                (byte) ((float) bytes[2] * intensity),
                (byte) ((float) bytes[3] * intensity)
            };

            return BitConverter.ToUInt32(newBytes, 0);
        }

        // Sets the process priority to Idle
        static void SetProcessPriority(Process process)
        {
            process.PriorityClass = ProcessPriorityClass.Idle;
        }

        // cleanup on exit
        private static void OnExit(object sender, System.EventArgs e)
        {
            sdk.ReleaseControl(0);
        }

        static void Main(string[] args)
        {
            Process process = Process.GetCurrentProcess();
            SetProcessPriority(process);
            process.Exited += new EventHandler(OnExit);

            var cpu = new PerformanceCounter
            {
                CategoryName = "Processor Information",
                CounterName = "% Processor Utility",
                InstanceName = "_Total"
            };

            var calculator = new SimpleMovingAverage(k: 20);

            int k = 1;
            while (true)
            {
                int cpuLoad = calculator.Update((int)(cpu.NextValue() * 100) - 500);

                if (k == 1 || k == 5000)
                {
                    ObtainControl();
                    k = 1;
                }

                // Set LEDs on mboard i/o panel
                float intensity = (float) (cpuLoad) / (float) 2000;
                if (intensity > 1.0) intensity = (float) 1.0;
                if (intensity < 0.0) intensity = (float) 0.0;
                mBoardLights[0].Color = AdjustColorIntensity(colors[0], intensity);

                intensity = (float) (cpuLoad - 2000) / (float) 2000;
                if (intensity > 1.0) intensity = (float) 1.0;
                if (intensity < 0.0) intensity = (float) 0.0;
                mBoardLights[1].Color = AdjustColorIntensity(colors[4], intensity);

                intensity = (float) (cpuLoad - 4000) / (float) 2000;
                if (intensity > 1.0) intensity = (float) 1.0;
                if (intensity < 0.0) intensity = (float) 0.0;
                mBoardLights[2].Color = AdjustColorIntensity(colors[7], intensity);

                mBoard.Apply();

                // Traverse all LEDs on DRAM sticks one and two
                for (int i = 0; i < 8; i++)
                {
                    intensity = (float) (cpuLoad - (i * 500)) / (float) 500;
                    if (intensity > 1.0) intensity = (float) 1.0;
                    if (intensity < 0.0) intensity = (float) 0.0;
                    stickOneLights[i].Color = AdjustColorIntensity(colors[i], intensity);
                    stickTwoLights[i].Color = AdjustColorIntensity(colors[i], intensity);
                }
                stickOne.Apply();
                stickTwo.Apply();

                System.Threading.Thread.Sleep(60);
                k++;
            }
        }
    }
}
