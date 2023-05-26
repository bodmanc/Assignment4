using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// Coordinator to take care of the overall looping
    /// </summary>
    internal class Coordinator
    {
        /// <summary>
        /// List of all processes initialized after loading from input file
        /// </summary>
        private List<Process> processList;

        /// <summary>
        /// Queue manager
        /// </summary>
        private QueueManager queueManager;

        /// <summary>
        /// Dictionary containing the processes in ascending order of arrival times
        /// </summary>
        private Dictionary<long, SortedList<int, Process>> arrivalList;

        /// <summary>
        /// Total number of processes
        /// </summary>
        private int processCount;

        /// <summary>
        /// Total number of processes completed
        /// </summary>
        private int completedCount;

        /// <summary>
        /// Constructor
        /// </summary>
        public Coordinator()
        {
            queueManager = new QueueManager();
            processList = new List<Process>();

            // read the input file and load all processes here
            if (File.Exists(CommonParameters.InputFilePath))
            {
                const int bufferSize = 256;
                using (var fileStream = File.OpenRead(CommonParameters.InputFilePath))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Process process = new Process(line);
                        processList.Add(process);
                    }
                }
            }
            else
            {
                Logger.LogAlways(CommonParameters.InputFilePath + " not found !");
                Environment.Exit(-1);
            }

            processCount = processList.Count;
            completedCount = 0;

            // arrange in ascending order of arrival times
            // next, break ties by process ID
            arrivalList = new Dictionary<long, SortedList<int, Process>>();
            foreach (Process process in processList)
            {
                SortedList<int, Process> list = arrivalList.GetValueOrDefault(process.ArrivalTime, new SortedList<int, Process>());
                list.Add(process.ProcessID, process);
                arrivalList[process.ArrivalTime] = list;
            }
        }

        /// <summary>
        /// Execute the main loop
        /// </summary>
        public void execute()
        {
            Logger.LogAlways("Starting the Scheduler with these parameters");
            Process? currentProcess = null;
            long timeSlice = 0;

            while (completedCount < processCount)
            {
                CPUState state = new CPUState();
                state.Time = Clock.Instance.Time;

                // check if any new process has arrived
                if (arrivalList.ContainsKey(Clock.Instance.Time))
                {
                    SortedList<int, Process> list = arrivalList[Clock.Instance.Time];

                    foreach (int processId in list.Keys)
                    {
                        // add in correct queue according to priority
                        queueManager.addNewProcess(list[processId]);
                    }

                    arrivalList.Remove(Clock.Instance.Time);
                }

                // need to pick a new process                
                if (currentProcess == null)
                {
                    currentProcess = queueManager.PickNextProcess();
                    if (currentProcess != null)
                    {
                        if (CommonParameters.Policy == SchedulingPolicy.PreEmptiveShortestJobFirst)
                        {
                            timeSlice = currentProcess.EstimatedTimeTillNextIO;
                        }
                        else
                        {
                            timeSlice = CommonParameters.TimeSlices[currentProcess.Priority];
                        }

                        currentProcess.TimeSpentInReadyQueue += Clock.Instance.Time - currentProcess.LastTimestamp;
                    }
                }

                // a process available for execution
                if (currentProcess != null)
                {
                    --currentProcess.TimeRemainingToCompletion;
                    --timeSlice;

                    state.ProcessID = currentProcess.ProcessID;
                    state.RemainingTimeForThisJob = currentProcess.TimeRemainingToCompletion;

                    if (currentProcess.TimeRemainingToCompletion == 0)
                    {
                        ++completedCount;
                        state.JobState = JobState.Exited;
                        timeSlice = 0;
                        currentProcess = null;
                    }

                    if (timeSlice == 0)
                    {
                        if (currentProcess != null)
                        {
                            if (currentProcess.TimeRemainingToCompletion > 0)
                            {
                                queueManager.Preempt(currentProcess);
                                state.JobState = JobState.PreEmpted;
                                currentProcess = null;
                            }
                        }
                    }
                    else if (currentProcess != null)
                    {
                        //currentProcess.TimeRemainingToCompletion > 0
                        // timeSlice > 0
                        if (currentProcess.HasIORequest())
                        {
                            queueManager.SendToIOWaitingPool(currentProcess);
                            state.IORequest = true;
                            state.JobState = JobState.Sleeping;
                            currentProcess = null;
                        }

                        // this process has no I/O request
                        // check if any other waiting processes completed their I/O requests
                        List<Process> ioCompleted = new List<Process>();
                        foreach (Process process in queueManager.IOPool)
                        {
                            if (process.HasIOCompleted())
                            {
                                ioCompleted.Add(process);
                                state.IORequestCompletedList.Add(process.ProcessID);
                            }
                        }

                        foreach (Process process in ioCompleted)
                        {
                            queueManager.RemoveFromIOPool(process);
                        }

                        if (CommonParameters.Policy == SchedulingPolicy.AggressivePreEmptive)
                        {
                            if (ioCompleted.Count > 0)
                            {
                                if (currentProcess != null)
                                {
                                    queueManager.Preempt(currentProcess);
                                    state.JobState = JobState.PreEmpted;
                                    currentProcess = null;
                                }
                            }
                            else
                            {
                                if (currentProcess != null)
                                {
                                    if (timeSlice == 0)
                                    {
                                        queueManager.Preempt(currentProcess);
                                        state.JobState = JobState.PreEmpted;
                                        currentProcess = null;
                                    }
                                    else
                                    {
                                        state.JobState = JobState.StillRunning;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (currentProcess != null)
                            {
                                if (timeSlice == 0)
                                {
                                    queueManager.Preempt(currentProcess);
                                    state.JobState = JobState.PreEmpted;
                                    currentProcess = null;
                                }
                                else
                                {
                                    state.JobState = JobState.StillRunning;
                                }
                            }
                        }
                    }
                }

                if (state.JobState == JobState.Exited)
                {
                    Logger.LogAlways(state.ToString());
                }
                else
                {
                    Logger.Log(state.ToString());
                }

                Clock.Instance.Tick();
            }

            Logger.LogAlways("All processes completed !");
            GenerateStatistics();
        }

        /// <summary>
        /// Generate statistics of all processes in a csv file
        /// </summary>
        public void GenerateStatistics()
        {
            String fileName = "Statistics_" + CommonParameters.Policy + ".csv";

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("ProcessID,Arrival Time,Total CPU Time,Time spent in Ready Queue,Time spent in I/O pool");

                foreach (Process process in processList)
                {
                    String text = process.ProcessID + "," + process.ArrivalTime + "," + process.ServiceTime + "," + process.TimeSpentInReadyQueue + "," + process.TimeSpentInIOQueue;
                    writer.WriteLine(text);
                }
            }
        }
    }
}
