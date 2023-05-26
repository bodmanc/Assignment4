using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// Process
    /// </summary>
    internal class Process : IComparer<Process>, IComparable<Process>
    {
        /// <summary>
        /// Private constructor
        /// </summary>
        private Process() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="processString">String to be read from file and parsed</param>
        public Process(String processString)
        {
            Alpha = 0.5;
            string[] parts = processString.Split(':');

            ProcessID = int.Parse(parts[0]);
            ArrivalTime = long.Parse(parts[1]);
            ServiceTime = long.Parse(parts[2]);
            Priority = long.Parse(parts[3]);

            LastTimestamp = ArrivalTime;
            TimeSpentInReadyQueue = 0;
            TimeSpentInIOQueue = 0;
            TimeRemainingToCompletion = ServiceTime;

            // applicable only for PreEmptiveShortestJobFirst
            EstimatedTimeTillNextIO = Math.Min(TimeRemainingToCompletion, (long)(Alpha * TimeRemainingToCompletion));
            if(EstimatedTimeTillNextIO == 0)
            {
                EstimatedTimeTillNextIO = 1;
            }
        }

        /// <summary>
        /// Statistical factor for SJF
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        /// Unique ID of this process
        /// </summary>
        public int ProcessID { get; private set; }

        /// <summary>
        /// Time at which this process arrives in the system
        /// </summary>
        public long ArrivalTime { get; private set; }

        /// <summary>
        /// Time required for this process to fully finish
        /// </summary>
        public long ServiceTime { get; private set; }

        /// <summary>
        /// Priority of this process
        /// </summary>
        public long Priority { get; internal set; }

        /// <summary>
        /// Total time spent in ready queue
        /// </summary>
        public long TimeSpentInReadyQueue { get; set; }

        /// <summary>
        /// Time spent in IO queue
        /// </summary>
        public long TimeSpentInIOQueue { get; set; }

        /// <summary>
        /// Time remaining to completion
        /// </summary>
        public long TimeRemainingToCompletion { get; internal set; }

        /// <summary>
        /// Time remaining till next I/O
        /// </summary>
        public long EstimatedTimeTillNextIO { get; internal set; }

        /// <summary>
        /// Last timestamp of this process
        /// </summary>
        public long LastTimestamp { get; set; }

        /// <summary>
        /// Whether this process has an IO request
        /// </summary>
        /// <returns>True or False</returns>
        public bool HasIORequest()
        {
            if (TimeRemainingToCompletion > 0)
            {
                return CommonParameters.RandomNumberGenerator.Next(0, CommonParameters.IORequestChance) == 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Whether the IO is completed for this process
        /// </summary>
        /// <returns>True or False</returns>
        public bool HasIOCompleted()
        {
            return CommonParameters.RandomNumberGenerator.Next(0, CommonParameters.IOCompletionChance) == 0;
        }

        public override int GetHashCode()
        {
            return ProcessID;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Process))
            {
                return false;
            }
            else
            {
                return this.ProcessID == ((Process)obj).ProcessID;
            }
        }

        public override string ToString()
        {
            return ProcessID + " : " + ArrivalTime + " : " + ServiceTime + " : " + Priority;
        }

        public int Compare(Process? x, Process? y)
        {
            int diff = (int)(x.EstimatedTimeTillNextIO - y.EstimatedTimeTillNextIO); 
            if(diff == 0)
            {
                return x.ProcessID - y.ProcessID;
            }
            else
            {
                return diff;
            }
        }

        public int CompareTo(Process? other)
        {
            int diff =  (int)(this.EstimatedTimeTillNextIO - other.EstimatedTimeTillNextIO);
            if(diff == 0)
            {
                return this.ProcessID - other.ProcessID;
            }
            else
            {
                return diff;
            }
        }
    }
}
