using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AuraServiceLib;
using Microsoft.Win32;

using System.Drawing;
using System.Windows.Forms;
using System.Timers;

using static Vanara.PInvoke.Kernel32;

namespace BagelAura
{
    internal class BagelAura
    {
        static Boolean active = true;

        static String[] others = { "HYTE.Nexus.Service", "HYTE Nexus", "wallpaper32", "AsusCertService", "asus_framework", 
                                   "adb", "steamwebhelper", "steam", "nvcontainer", "NVIDIA Overlay", "NVDisplay.Container", 
                                   "SearchIndexer", "OneDrive", "nordvpn-service" };

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

        static PerformanceCounter cRead = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", "c:");
        static PerformanceCounter cWrite = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", "c:");
        static PerformanceCounter dRead = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", "d:");
        static PerformanceCounter dWrite = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", "d:");
        static PerformanceCounter eRead = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", "e:");
        static PerformanceCounter eWrite = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", "e:");
        static PerformanceCounter fRead = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", "f:");
        static PerformanceCounter fWrite = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", "f:");

        private static int diskActivityThreshold = 10000;

        static SimpleMovingAverage graphCalculator = new SimpleMovingAverage(k: 5);

        static SimpleMovingAverage redCalculator = new SimpleMovingAverage(k: 20);
        static SimpleMovingAverage blueCalculator = new SimpleMovingAverage(k: 20);
        static SimpleMovingAverage greenCalculator = new SimpleMovingAverage(k: 20);

        static SimpleMovingAverage redTextCalculator = new SimpleMovingAverage(k: 20);
        static SimpleMovingAverage blueTextCalculator = new SimpleMovingAverage(k: 20);
        static SimpleMovingAverage greenTextCalculator = new SimpleMovingAverage(k: 20);

        static SimpleMovingAverage redGraphCalculator = new SimpleMovingAverage(k: 5);
        static SimpleMovingAverage blueGraphCalculator = new SimpleMovingAverage(k: 5);
        static SimpleMovingAverage greenGraphCalculator = new SimpleMovingAverage(k: 5);

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

            // force all background processes to Eco QoS
            Process process = Process.GetCurrentProcess();
            EnableEcoqos(process);
            //cpud.Show();

            k = 2;
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

        // Set Eco QoS on some background processes
        static void EnableEcoqos(Process process)
        {
            PROCESS_POWER_THROTTLING_STATE PowerThrottling = new PROCESS_POWER_THROTTLING_STATE();
            PowerThrottling.Version = PROCESS_POWER_THROTTLING_STATE.PROCESS_POWER_THROTTLING_CURRENT_VERSION;
            PowerThrottling.ControlMask = PROCESS_POWER_THROTTLING_MASK.PROCESS_POWER_THROTTLING_EXECUTION_SPEED;
            PowerThrottling.StateMask = PROCESS_POWER_THROTTLING_MASK.PROCESS_POWER_THROTTLING_EXECUTION_SPEED;

            process.PriorityClass = ProcessPriorityClass.Idle;
            SetProcessInformation<PROCESS_POWER_THROTTLING_STATE>(process, PROCESS_INFORMATION_CLASS.ProcessPowerThrottling, PowerThrottling);

            foreach (var other in others)
            {
                Process[] otherProcs = Process.GetProcessesByName(other);
                foreach (var otherProcess in otherProcs)
                {
                    otherProcess.PriorityClass = ProcessPriorityClass.Idle;
                    SetProcessInformation<PROCESS_POWER_THROTTLING_STATE>(otherProcess, PROCESS_INFORMATION_CLASS.ProcessPowerThrottling, PowerThrottling);
                }
            }
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
                active = false;
                aTimer.Stop();
                sdk.ReleaseControl(0);
            } else if (e.Mode == PowerModes.Resume)
            {
                k = 1;
                active = true;
                SetTimer();
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
                if (k == 1)
                {
                    ObtainControl();
                }
                else if (k >= 5000)
                {
                    ObtainControl(false);
                }

                int instCpuLoad = (int)(cpu.NextValue() * 100);
                int graphCpuLoad = graphCalculator.Update(instCpuLoad);

                // Set LEDs on mboard i/o panel
                int blue = 0;
                int green = 0;
                int red = 0;
                float intensity = 0;

                if (instCpuLoad > 8000)
                {
                    intensity = (float)(instCpuLoad - 8000) / (float)2000;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    blue = 0;
                    red = 255;
                    green = 255 - (int)(255 * intensity);
                }
                else if (instCpuLoad > 6000)
                {
                    intensity = (float)(instCpuLoad - 6000) / (float)2000;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    blue = 0;
                    red = (int)(255 * intensity);
                    green = 255;
                }
                else if (instCpuLoad > 4000)
                {
                    intensity = (float)(instCpuLoad - 4000) / (float)2000;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    red = 0;
                    blue = 255 - (int)(255 * intensity);
                    green = 255;
                }
                else if (instCpuLoad > 2000)
                {
                    intensity = (float)(instCpuLoad - 2000) / (float)2000;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    red = 0;
                    green = (int)(255 * intensity);
                    blue = 255;
                }
                else
                {
                    intensity = (float)(instCpuLoad) / (float)2000;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    red = 0;
                    green = 0;
                    blue = (int)(255 * intensity);
                }

                Color activecolor = Color.FromArgb((int)redGraphCalculator.Update(red), (int)greenGraphCalculator.Update(green), (int)blueGraphCalculator.Update(blue));

                intensity = (float)(instCpuLoad) / (float) 8000;
                if (intensity > 1.0) intensity = (float)1.0;
                if (intensity < 0.0) intensity = (float)0.0;
                red = (int)(255 * intensity);
                green = (int)(255 * intensity);
                blue = (int)(255 * intensity);

                uint color = ColorFromBytes((byte)blueCalculator.Update(blue), (byte)greenCalculator.Update((int)((float)green * 0.8)), (byte)redCalculator.Update(red));
                Color textColor = Color.FromArgb((int) blueTextCalculator.Update(255 - blue), (int) greenTextCalculator.Update(255 - green), (int) redTextCalculator.Update(255 - red));

                mBoardLights[0].Color = color;
                mBoardLights[1].Color = color;
                mBoardLights[2].Color = color;
                mBoard.Apply();

                // Traverse all LEDs on DRAM sticks one and two
                for (int i = 0; i < 8; i++)
                {
                    stickOneLights[i].Color = color;
                    stickTwoLights[i].Color = color;
                }
                stickOne.Apply();
                stickTwo.Apply();

                cpud.currentload = graphCpuLoad / 100;
                cpud.currentColor = activecolor;
                cpud.currentTextColor = textColor;

                if(cWrite.NextValue() > diskActivityThreshold) { cpud.DriveCStatus = CPUDisplay.DriveStatus.Write; }
                else if (cRead.NextValue() > diskActivityThreshold) { cpud.DriveCStatus = CPUDisplay.DriveStatus.Read; }
                else cpud.DriveCStatus = CPUDisplay.DriveStatus.Idle;

                if (dWrite.NextValue() > diskActivityThreshold) { cpud.DriveDStatus = CPUDisplay.DriveStatus.Write; }
                else if (dRead.NextValue() > diskActivityThreshold) { cpud.DriveDStatus = CPUDisplay.DriveStatus.Read; }
                else cpud.DriveDStatus = CPUDisplay.DriveStatus.Idle;

                if (eWrite.NextValue() > diskActivityThreshold) { cpud.DriveEStatus = CPUDisplay.DriveStatus.Write; }
                else if (eRead.NextValue() > diskActivityThreshold) { cpud.DriveEStatus = CPUDisplay.DriveStatus.Read; }
                else cpud.DriveEStatus = CPUDisplay.DriveStatus.Idle;

                if (fWrite.NextValue() > diskActivityThreshold) { cpud.DriveFStatus = CPUDisplay.DriveStatus.Write; }
                else if (fRead.NextValue() > diskActivityThreshold) { cpud.DriveFStatus = CPUDisplay.DriveStatus.Read; }
                else cpud.DriveFStatus = CPUDisplay.DriveStatus.Idle;

                cpud.Invalidate();
            }
            k++;
        }
    }
}
