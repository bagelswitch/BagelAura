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
using System.Runtime.InteropServices;
using System.Text;

namespace BagelAura
{
    internal class BagelAura
    {
        static Boolean active = true;

        static Boolean dolights = false;

        static String[] others = { "HYTE.Nexus.Service", "HYTE Nexus", "wallpaper32", "AsusCertService", "asus_framework", 
                                   "steamwebhelper", "steam", "SearchIndexer", "OneDrive", "nordvpn-service", "msedgewebview2" }; 

        // Create SDK instance
        static IAuraSdk3 sdk = dolights?new AuraSdk() as IAuraSdk3:null;

        static IAuraSyncDevice stickOne = null;
        static IAuraSyncDevice stickTwo = null;
        static IAuraSyncDevice mBoard = null;

        static List<IAuraRgbLight> stickOneLights = null;
        static List<IAuraRgbLight> stickTwoLights = null;
        static List<IAuraRgbLight> mBoardLights = null;

        private static System.Timers.Timer cpuTimer;
        private static System.Timers.Timer diskTimer;
        private static System.Timers.Timer focusTimer;
        private static System.Windows.Forms.Timer restartTimer;


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
        static PerformanceCounter wRead = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", "w:");
        static PerformanceCounter wWrite = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", "w:");

        static PerformanceCounter netSend = new PerformanceCounter("Network Interface", "Bytes Sent/sec", "Intel[R] Wi-Fi 7 BE200 320MHz");
        static PerformanceCounter netReceive = new PerformanceCounter("Network Interface", "Bytes Received/sec", "Intel[R] Wi-Fi 7 BE200 320MHz");

        static SimpleMovingAverage netRecCalculator = new SimpleMovingAverage(5);
        static SimpleMovingAverage netSendCalculator = new SimpleMovingAverage(5);

        private static int diskActivityThreshold = 100000;
        private static int networkActivityThreshold = 10000;

        static SimpleMovingAverage graphCalculator = new SimpleMovingAverage(k: 5);

        static SimpleMovingAverage redCalculator = new SimpleMovingAverage(k: 20);
        static SimpleMovingAverage blueCalculator = new SimpleMovingAverage(k: 20);
        static SimpleMovingAverage greenCalculator = new SimpleMovingAverage(k: 20);

        static SimpleMovingAverage redGraphCalculator = new SimpleMovingAverage(k: 5);
        static SimpleMovingAverage blueGraphCalculator = new SimpleMovingAverage(k: 5);
        static SimpleMovingAverage greenGraphCalculator = new SimpleMovingAverage(k: 5);

        // attempt to draw rectangles directly to screen
        static CPUDisplay cpud = new CPUDisplay();

        static FocusDisplay focusd = new FocusDisplay();

        static int k = 1;

        static Boolean controlLock = false;

        static string giphyKey = "";

        static void ObtainControl(Boolean reEnum = true)
        {
            if (!controlLock)
            {
                controlLock = true;
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

                k = 2;
                controlLock = false;
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
            if (dolights)
            {
                sdk.ReleaseControl(0);
            }
            Application.Exit();
        }

        static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            var mode = e.Mode;

            if(mode == PowerModes.Suspend)
            {
                active = false;
                cpuTimer.Stop();
                diskTimer.Stop();
                focusTimer.Stop();
                if (dolights)
                {
                    sdk.ReleaseControl(0);
                }
            } else if (mode == PowerModes.Resume)
            {
                Console.WriteLine("Resumed from sleep, delaying before restart");
                SetRestartTimer();
            }
        }

        static void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            active = false;
            cpuTimer.Stop();
            diskTimer.Stop();
            focusTimer.Stop();
            if (dolights)
            {
                sdk.ReleaseControl(0);
            }
            Console.WriteLine("Display settings changed, delaying before restart");
            SetRestartTimer();
        }

        private static void SetTimers()
        {
            cpuTimer = new System.Timers.Timer(90);
            diskTimer = new System.Timers.Timer(240);
            focusTimer = new System.Timers.Timer(4500);

            cpuTimer.Elapsed += OnTimedCPUEvent;
            cpuTimer.AutoReset = true;
            cpuTimer.Enabled = true;

            diskTimer.Elapsed += OnTimedDiskEvent;
            diskTimer.AutoReset = true;
            diskTimer.Enabled = true;

            focusTimer.Elapsed += OnTimedFocusEvent;
            focusTimer.AutoReset = true;
            focusTimer.Enabled = true;
        }

        private static void SetRestartTimer()
        {
            if(restartTimer is not null)
            {
                restartTimer.Stop();
                restartTimer.Dispose();
            }
            restartTimer = new System.Windows.Forms.Timer();

            restartTimer.Tick += new EventHandler(OnTimedRestartEvent);
            restartTimer.Interval = 60000;
            restartTimer.Enabled = true;
            restartTimer.Start();
            
        }

        static void Shutdown()
        {
            Console.WriteLine("Starting Shutdown");

            try
            {
                Console.WriteLine("Disposing of timers");

                cpuTimer.Stop();
                cpuTimer.Dispose();

                diskTimer.Stop();
                diskTimer.Dispose();

                focusTimer.Stop();
                focusTimer.Dispose();

                if (restartTimer is not null)
                {
                    restartTimer.Stop();
                    restartTimer.Dispose();
                }

                Console.WriteLine("Disposing of forms");

                cpud.Close();
                cpud.Dispose();

                focusd.Shutdown();

                focusd.Close();
                focusd.Dispose();
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.WriteLine("Exiting");

                try
                {
                    Console.WriteLine("Application.Exit()");
                    Application.Exit();
                
                    Console.WriteLine("Process kill");
                    Process.GetCurrentProcess().Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            giphyKey = args[0];
            focusd.SetGiphyKey(giphyKey);

            Process process = Process.GetCurrentProcess();
            process.Exited += new EventHandler(OnExit);
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerModeChanged);
            SystemEvents.DisplaySettingsChanged += new EventHandler(OnDisplaySettingsChanged);

            Console.WriteLine("Setting timers");
            SetTimers();

            // force all background processes to Eco QoS
            Console.WriteLine("Setting Eco QoS");
            EnableEcoqos(process);

            Console.WriteLine("Running application");
            Application.Run();

            Shutdown();

            return;
        }

        private static void OnTimedFocusEvent(Object source, ElapsedEventArgs e)
        {
            String title = GetActiveWindowTitle();
            if (title == null || title.Trim().Equals("")) title = "sloth";

            var words = title.Split(new char[] { ' ', ',', '.', '/', '|', ':', ';', '-', '\\', '+', '(', ')', '@' }, StringSplitOptions.RemoveEmptyEntries);
            var query = words.OrderByDescending(n => n.Length).First().Trim();

            if (query.Equals("Program")) query = "sloth";

            focusd.SetQuery(query);
        }

        private static void OnTimedRestartEvent(Object source, EventArgs e)
        {
            restartTimer.Stop();

            Console.WriteLine("Restarting application");
            Application.Restart();
            Console.WriteLine("Application restart complete");
            Shutdown();
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        private static void OnTimedCPUEvent(Object source, ElapsedEventArgs e)
        {
            if (active)
            {
                if (dolights)
                {
                    if (k == 1)
                    {
                        ObtainControl();
                    }
                    else if (k >= 5000)
                    {
                        ObtainControl(false);
                    }
                }

                if (!controlLock)
                {
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

                    intensity = (float)(instCpuLoad) / (float)8000;
                    if (intensity > 1.0) intensity = (float)1.0;
                    if (intensity < 0.0) intensity = (float)0.0;
                    red = (int)(255 * intensity);
                    green = (int)(0 * intensity);
                    blue = (int)(255 * intensity);

                    uint color = ColorFromBytes((byte)blueCalculator.Update(blue), (byte)greenCalculator.Update((int)((float)green * 0.8)), (byte)redCalculator.Update(red));
                    
                    if (dolights)
                    {
                        // Set motherboard aura section lights
                        //mBoardLights[0].Color = color;
                        //mBoardLights[1].Color = color;
                        //mBoardLights[2].Color = color;
                        //mBoard.Apply();

                        // Traverse all LEDs on DRAM sticks one and two
                        for (int i = 0; i < 8; i++)
                        {
                            stickOneLights[i].Color = color;
                            stickTwoLights[i].Color = color;
                        }
                        stickOne.Apply();
                        stickTwo.Apply();
                    }

                    cpud.currentload = graphCpuLoad / 100;
                    cpud.currentColor = activecolor;

                    cpud.isDirty = true;
                    cpud.Invalidate();
                }
            }
            k++;
        }

        private static void OnTimedDiskEvent(Object source, ElapsedEventArgs e)
        {
            if (cWrite.NextValue() > diskActivityThreshold) { cpud.DriveCStatus = CPUDisplay.DriveStatus.Write; }
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

            if (wWrite.NextValue() > diskActivityThreshold) { cpud.DriveWStatus = CPUDisplay.DriveStatus.Write; }
            else if (wRead.NextValue() > diskActivityThreshold) { cpud.DriveWStatus = CPUDisplay.DriveStatus.Read; }
            else cpud.DriveWStatus = CPUDisplay.DriveStatus.Idle;

            float netSendVal = netSendCalculator.Update((int) netSend.NextValue());
            float netRecVal = netRecCalculator.Update((int) netReceive.NextValue());
            if (netSendVal > netRecVal && netSendVal > networkActivityThreshold) { cpud.NetStatus = CPUDisplay.NetworkStatus.Send; }
            else if (netRecVal > netSendVal && netRecVal > networkActivityThreshold) { cpud.NetStatus = CPUDisplay.NetworkStatus.Receive; }
            else cpud.NetStatus = CPUDisplay.NetworkStatus.Idle;
        }
    }
}
