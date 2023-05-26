using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// Class to manage the queues
    /// </summary>
    internal class QueueManager
    {
        /// <summary>
        /// The ready queues
        /// </summary>
        public Queue<Process>[] ReadyQueues { get; private set; }

        /// <summary>
        /// Queue for SJF
        /// </summary>
        public SortedSet<Process> SJFQueue { get; private set; }

        /// <summary>
        /// Pool of processes waiting for I/O completion
        /// </summary>
        public SortedSet<Process> IOPool { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public QueueManager()
        {
            ReadyQueues = new Queue<Process>[8];
            IOPool = new SortedSet<Process>();

            for (int i = 0; i < ReadyQueues.Length; i++)
            {
                ReadyQueues[i] = new Queue<Process>();
            }

            SJFQueue = new SortedSet<Process>();
        }

        /// <summary>
        /// Adds a new process to this system
        /// </summary>
        /// <param name="process">Process to be added to this system</param>
        public void addNewProcess(Process process)
        {
            if (CommonParameters.Policy == SchedulingPolicy.PreEmptiveShortestJobFirst)
            {
                if (!SJFQueue.Add(process))
                {
                    throw new Exception("Failed to add " + process.ToString() + " to SJF pool !!");
                }
            }
            else
            {
                ReadyQueues[process.Priority].Enqueue(process);
            }
        }

        /// <summary>
        /// Pick the next process to execute
        /// </summary>
        /// <returns>Process to be executed next</returns>
        public Process PickNextProcess()
        {
            if (CommonParameters.Policy == SchedulingPolicy.PreEmptiveShortestJobFirst)
            {
                if (SJFQueue.Count != 0)
                {
                    Process process = SJFQueue.First();
                    SJFQueue.Remove(process);
                    process.TimeSpentInReadyQueue += Clock.Instance.Time - process.LastTimestamp;
                    process.LastTimestamp = Clock.Instance.Time;
                    return process;
                }
            }
            else
            {
                for (int i = 0; i < ReadyQueues.Length; i++)
                {
                    if (ReadyQueues[i].Count != 0)
                    {
                        Process process = ReadyQueues[i].Dequeue();
                        process.TimeSpentInReadyQueue += Clock.Instance.Time - process.LastTimestamp;
                        process.LastTimestamp = Clock.Instance.Time;
                        return process;
                    }
                }
            }

            // reached till this point
            // see if any process waiting for I/O
            foreach(Process process in IOPool)
            {
                if (process.HasIOCompleted())
                {
                    RemoveFromIOPool(process);
                    return process;
                }
            }

            return null;
        }


        /// <summary>
        /// Sends a process to IO queue
        /// </summary>
        /// <param name="process">Process to be sent to IO queue</param>
        public void SendToIOWaitingPool(Process process)
        {
            process.LastTimestamp = Clock.Instance.Time;
            IOPool.Add(process);
        }

        /// <summary>
        /// Removes a process from IO queue
        /// </summary>
        /// <param name="process">Process to be removed from IO pool</param>
        public void RemoveFromIOPool(Process process)
        {
            IOPool.Remove(process);
            process.TimeSpentInIOQueue += Clock.Instance.Time - process.LastTimestamp;
            process.LastTimestamp = Clock.Instance.Time;

            switch (CommonParameters.Policy)
            {
                case SchedulingPolicy.NonAggressivePreEmptive:
                    process.Priority = 0;
                    ReadyQueues[0].Enqueue(process);
                    break;
                case SchedulingPolicy.AggressivePreEmptive:
                    process.Priority = Math.Max(0, process.Priority - 1);
                    ReadyQueues[process.Priority].Enqueue(process);
                    break;
                case SchedulingPolicy.PreEmptiveShortestJobFirst:
                    process.Alpha = 1.5 * process.Alpha; // exponential increase in estimated CPU requirement
                    process.EstimatedTimeTillNextIO = Math.Min(process.TimeRemainingToCompletion, (long)(process.Alpha * process.TimeRemainingToCompletion));
                    if(process.EstimatedTimeTillNextIO == 0)
                    {
                        process.EstimatedTimeTillNextIO = 1;
                    }

                    SJFQueue.Add(process);
                    break;
            }
        }

        /// <summary>
        /// Pre-empt a process
        /// </summary>
        /// <param name="process">Process to be pre-empted</param>
        public void Preempt(Process process)
        {
            switch (CommonParameters.Policy)
            {
                case SchedulingPolicy.NonAggressivePreEmptive:
                    process.Priority = Math.Min(ReadyQueues.Length - 1, process.Priority + 1);
                    ReadyQueues[process.Priority].Enqueue(process);
                    break;
                case SchedulingPolicy.AggressivePreEmptive:
                    process.Priority = Math.Max(0, process.Priority - 1);
                    ReadyQueues[process.Priority].Enqueue(process);
                    break;
                case SchedulingPolicy.PreEmptiveShortestJobFirst:
                    SJFQueue.Add(process);
                    break;
            }
        }

        /// <summary>
        /// Print current state of queue
        /// </summary>
        public void Print()
        {
            for(int i = 0; i < ReadyQueues.Length; i++)
            {
                Logger.LogAlways("ReadyQueue[" + i + "] : " + ReadyQueues[i].Count);
            }

            Logger.LogAlways("SJFQueue : " + SJFQueue.Count);
            Logger.LogAlways("IOPool : " + IOPool.Count);
        }
    }
}

