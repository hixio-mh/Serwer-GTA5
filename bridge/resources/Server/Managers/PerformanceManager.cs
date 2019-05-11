using System;
using System.Collections.Generic;
using System.Text;
using Database;
using Main;
using GTANetworkAPI;
using Extend;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Managers
{
    public class CPerformanceManager
    {
        private readonly float MinCPUDifference = 0.01f;
        private readonly float MinRAMDifference = 1.0f;
        private readonly string ProcessName;
        private readonly DateTime Start;
        private Dictionary<ETiming, CTiming> Timing;
        readonly public Stack<CCPU> sCPU;
        readonly public Stack<CRAM> sRAM;
        public Tuple<double, double> lastUsage;

        public enum ETiming
        {
            Test1,
            Test2,
            End,
        }

        public class CTiming
        {
            public uint Count { get; private set; }
            public double Time { get; private set; }
            public double TimeMS { get => Time / 10000; }
            public double MinTime { get; private set; } = 9999999999.0;
            public double MaxTime { get; private set; } = 0.0f;

            double Timing;
            bool Started = false;

            public bool Start()
            {
                if (Started) return false;
                Timing = Globals.Utils.GetTickCount(true);
                return true;
            }
            public bool Stop()
            {
                if (Started) return false;
                double stop = Globals.Utils.GetTickCount(true);
                double elapsed = stop - Timing;
                MaxTime = Math.Max(MaxTime, elapsed);
                MinTime = Math.Min(MinTime, elapsed);
                
                Timing = 0;
                Count++;
                Time += elapsed;

                return true;
            }
        }

        public class CCPU
        {
            public DateTime Time { get; set; }
            public double Usage { get; set; }
        }
        
        public class CRAM
        {
            public DateTime Time { get; set; }
            public double Usage { get; set; }
        }

        void UpdateLastUsage()
        {
            var cpu = new PerformanceCounter("Process", "% Processor Time", ProcessName, true);
            var ram = new PerformanceCounter("Process", "Private Bytes", ProcessName, true);

            // If system has multiple cores, that should be taken into account
            double cpuUsage = Math.Round(cpu.NextValue() / Environment.ProcessorCount, 2);
            // Returns number of MB consumed by application
            double ramUsage = Math.Round(ram.NextValue() / 1024 / 1024, 2);

            lastUsage = new Tuple<double, double>(cpuUsage, ramUsage);
        }

        public CCPU GetLastCPUUsage() => sCPU.Peek();

        public CRAM GetLastRAMUsage() => sRAM.Peek();

        void CheckCPURAMUsage()
        {
            UpdateLastUsage();

            double difference1 = Math.Abs(GetLastCPUUsage().Usage - lastUsage.Item1);
            double difference2 = Math.Abs(GetLastRAMUsage().Usage - lastUsage.Item2);

            if (Math.Abs(GetLastCPUUsage().Usage - lastUsage.Item1) > MinCPUDifference)
                sCPU.Push(new CCPU { Time = DateTime.Now, Usage = lastUsage.Item1 });
            
            if(Math.Abs(GetLastRAMUsage().Usage - lastUsage.Item2) > MinRAMDifference)
                sRAM.Push(new CRAM { Time = DateTime.Now, Usage = lastUsage.Item2 });

            NAPI.Task.Run(CheckCPURAMUsage, 1000);
        }

        public bool StartTiming(ETiming eTiming) => Timing[eTiming].Start();

        public bool StopTiming(ETiming eTiming) => Timing[eTiming].Stop();

        public CTiming GetTimingStats(ETiming eTiming) => Timing[eTiming];

        public CPerformanceManager()
        {
            lastUsage = new Tuple<double, double>(0, 0);
            // Getting information about current process
            var process = Process.GetCurrentProcess();

            // Preparing variable for application instance name
            var name = string.Empty;

            foreach (var instance in new PerformanceCounterCategory("Process").GetInstanceNames())
            {
                if (instance.StartsWith(process.ProcessName))
                {
                    using (var processId = new PerformanceCounter("Process", "ID Process", instance, true))
                    {
                        if (process.Id == (int)processId.RawValue)
                        {
                            name = instance;
                            break;
                        }
                    }
                }
            }

            Start = DateTime.Now;
            ProcessName = name;
            Timing = new Dictionary<ETiming, CTiming>((int)ETiming.End);

            Enumerable.Range(0, (int)ETiming.End - 1).ToList().ForEach((i) =>
            {
                Timing[(ETiming)i] = new CTiming();
            });


            sCPU = new Stack<CCPU>();
            sRAM = new Stack<CRAM>();
            sCPU.Push(new CCPU { Time = DateTime.Now, Usage = 0 });
            sRAM.Push(new CRAM { Time = DateTime.Now, Usage = 0 });
            CheckCPURAMUsage();
            /*
            StartTiming(ETiming.Test1);
            for(int i=1;i<10000;i++)
            { }
            StopTiming(ETiming.Test1);
            CDebug.Debug("timing1 Test1:", GetTimingStats(ETiming.Test1).Serialize());

            StartTiming(ETiming.Test1);
            for(int i=1;i<100;i++)
            { }
            StopTiming(ETiming.Test1);
            CDebug.Debug("timing2 Test1:", GetTimingStats(ETiming.Test1).Serialize());*/
        }
    }
}
