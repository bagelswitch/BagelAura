using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AuraServiceLib;
using Microsoft.Win32;

using System.Drawing;
using System.Windows.Forms;
using System.Timers;

namespace BagelAura
{
    internal class BagelAura
    {
        static Boolean active = true;

        // Create SDK instance
        static IAuraSdk3 sdk = new AuraSdk() as IAuraSdk3;

        static IAuraSyncDevice stickOne = null;
        static IAuraSyncDevice stickTwo = null;
        static IAuraSyncDevice mBoard = null;

        static List<IAuraRgbLight> stickOneLights = null;
        static List<IAuraRgbLight> stickTwoLights = null;
        static List<IAuraRgbLight> mBoardLights = null;

        private static System.Timers.Timer aTimer;

        static PerformanceCounter cpu = new PerformanceCounter
        {
            CategoryName = "Processor Information",
            CounterName = "% Processor Utility",
            InstanceName = "_Total"
        };

        static SimpleMovingAverage calculator = new SimpleMovingAverage(k: 20);

        // attempt to draw rectangles directly to screen
        static CPUDisplay cpud = new CPUDisplay();

        static int k = 1;

        static void ObtainControl(Boolean reEnum = true)
        {
            // Aquire control
            sdk.ReleaseControl(0);
            sdk.SwitchMode();

            if (reEnum)
            {
                // enumerate all devices
                IAuraSyncDeviceCollection devices = sdk.Enumerate(0);

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
        }

        static uint AdjustColorIntensity(uint color, float intensity)
        {
            byte[] bytes = BitConverter.GetBytes(color);

            byte[] newBytes = {
                (byte) ((float) bytes[0] * intensity),
                (byte) ((float) bytes[1] * intensity),
                (byte) ((float) bytes[2] * intensity),
                (byte) ((float) bytes[3] * intensity)
            };

            return BitConverter.ToUInt32(newBytes, 0);
        }

        static uint ColorFromBytes(byte blue, byte green, byte red)
        {
            byte[] bytes = {
                0x00,
                blue,
                green,
                red
            };
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        // Sets the process priority to Idle
        static void SetProcessPriority(Process process)
        {
            process.PriorityClass = ProcessPriorityClass.Idle;
        }

        // cleanup on exit
        private static void OnExit(object sender, System.EventArgs e)
        {
            active = false;
            sdk.ReleaseControl(0);
            Application.Exit();
        }

        static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if(e.Mode == PowerModes.Suspend)
            {
                //cpud.Hide();
                active = false;
                sdk.ReleaseControl(0);
            } else if (e.Mode == PowerModes.Resume)
            {
                active = false;
                ObtainControl();
                active = true;
                //cpud.Show();
            }
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(60);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        static void Main(string[] args)
        {
            Process process = Process.GetCurrentProcess();
            SetProcessPriority(process);
            process.Exited += new EventHandler(OnExit);
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerModeChanged);

            SetTimer();

            Application.Run();

            aTimer.Stop();
            aTimer.Dispose();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (active)
            {
                int instCpuLoad = (int)(cpu.NextValue() * 100);
                int cpuLoad = calculator.Update(instCpuLoad - 500);

                if (k == 1)
                {
                    ObtainControl();
                }
                else if (k >= 5000)
                {
                    ObtainControl(false);
                    k = 1;
                }

                // Set LEDs on mboard i/o panel
                int blue = 0;
                int green = 0;
                int red = 0;
                if (cpuLoad > 6000)
                {
                    float intensity = (float)(cpuLoad - 6000) / (float)1500;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    blue = 0;
                    red = 255;
                    green = 255 - (int)(255 * intensity);
                }
                else if (cpuLoad > 4500)
                {
                    float intensity = (float)(cpuLoad - 4500) / (float)1500;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    blue = 0;
                    red = (int)(255 * intensity);
                    green = 255;
                }
                else if (cpuLoad > 3000)
                {
                    float intensity = (float)(cpuLoad - 3000) / (float)1500;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    red = 0;
                    blue = 255 - (int)(255 * intensity);
                    green = 255;
                }
                else if (cpuLoad > 1500)
                {
                    float intensity = (float)(cpuLoad - 1500) / (float)1500;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    red = 0;
                    green = (int)(255 * intensity);
                    blue = 255;
                }
                else
                {
                    float intensity = (float)(cpuLoad) / (float)1500;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    red = 0;
                    green = 0;
                    blue = (int)(255 * intensity);
                }
                uint iocolor = ColorFromBytes((byte)blue, (byte)green, (byte)red);
                mBoardLights[0].Color = iocolor;
                mBoardLights[1].Color = iocolor;
                mBoardLights[2].Color = iocolor;
                mBoard.Apply();

                // Traverse all LEDs on DRAM sticks one and two
                for (int i = 0; i < 8; i++)
                {
                    stickOneLights[i].Color = iocolor;
                    stickTwoLights[i].Color = iocolor;
                }
                stickOne.Apply();
                stickTwo.Apply();
                
                Color activecolor = Color.FromArgb(red, green, blue);
                cpud.currentload = instCpuLoad / 100;
                cpud.currentcolor = activecolor;
                cpud.Invalidate();
            }
            k++;
        }
    }
}
