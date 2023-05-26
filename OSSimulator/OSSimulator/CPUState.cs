using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// To log the current state of CPU
    /// </summary>
    internal class CPUState
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CPUState() {
            ProcessID = -1;
            RemainingTimeForThisJob = -1;
            IORequestCompletedList = new List<int>();
            JobState = JobState.Idling;
        }

        /// <summary>
        /// Current clock time
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// ID of currently running process
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// Remaining execution time for this job
        /// </summary>
        public long RemainingTimeForThisJob { get; set; }

        /// <summary>
        /// If there is an I/O request for the current process
        /// </summary>
        public bool IORequest { get; set; }

        /// <summary>
        /// List of processes for whom IO request was completed in current clock cycle
        /// </summary>
        public List<int> IORequestCompletedList { get; set; }

        /// <summary>
        /// State of current job
        /// </summary>
        public JobState JobState { get; set; }

        /// <summary>
        /// String representation of this state
        /// </summary>
        /// <returns>string version of the state</returns>
        public override string ToString()
        {
            string result = "";

            result += Time;
            result += ":" + (ProcessID >= 0 ? ProcessID : "*");
            result += ":" + (ProcessID >= 0 ? RemainingTimeForThisJob : "x");
            result += ":" + IORequest.ToString().ToLower();

            if(IORequestCompletedList.Count == 0)
            {
                result += ":none";
            }
            else
            {
                result += ":";
                for(int i = 0; i <  IORequestCompletedList.Count; i++)
                {
                    if(i == 0)
                    {
                        result += IORequestCompletedList[i];
                    }
                    else
                    {
                        result += "," + IORequestCompletedList[i];
                    }
                }
            }

            switch (JobState)
            {
                case JobState.PreEmpted:
                    result += ":preempted";
                    break;
                case JobState.StillRunning:
                    result += ":still running";
                    break;
                case JobState.Sleeping:
                    result += ":sleeping";
                    break;
                case JobState.Idling:
                    result += ":idling";
                    break;
                case JobState.Exited:
                    result += ":* exited";
                    break;
            }

            return result;
        }
    }
}
