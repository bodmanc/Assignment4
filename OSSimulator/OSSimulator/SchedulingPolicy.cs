using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// Scheduling Policies
    /// </summary>
    internal enum SchedulingPolicy
    {
        /// <summary>
        /// 1. Jobs are scheduled according to priority, which ranges from 0 (highest priority) 
        /// to 7 (lowest priority)
        /// 2. When a job starts a burst (that is, when it becomes ready either because it
        /// has just started or because it has finished doing I/O), it is assigned priority 0 .
        /// 3. The scheduler maintains a (FIFO) queue of jobs for each priority level. The
        /// scheduler will always run the first job of the highest priority level available (i.e.
        /// lowest-numbered non-empty queue). For example, if queues 0 and 1 are
        /// empty but queue 2 is not, the scheduler will run the first job in queue 2.
        /// 4. When a job is run, it is assigned a slice, which is a number of quanta based
        /// on the priority of the job. A job at priority level 0 has a time slice length of 1
        /// quantum, a job at level 1 has a time slice of 2 quanta, a job at level 2 has a
        /// time slice of 4 quanta, and so on. In general, a job with priority i has a time
        /// slice of 2^i quanta.
        /// 5. If a job with priority i uses up its time slice without blocking for I/O or
        /// terminating, the scheduler stops it, lowers its priority to i+1, and adds it at the
        /// tail of queue i+1, and selects a job as in rule (3). However, if the job is
        /// already in the lowest priority queue, its priority is unchanged and it returns to
        /// the end of the same queue. While it is possible that the same job will be
        /// selected again--for example, if it is the only ready job--normally a different job
        /// will be given the opportunity to run.
        /// 6. This policy is non-aggressive in the following sense: If a job becomes ready
        /// while another job is running, it is added to the tail of queue 0, but the running
        /// job is not stopped until it terminates, blocks for I/O, or uses up its time slice.
        /// </summary>
        NonAggressivePreEmptive,

        /// <summary>
        /// In this version jobs arriving at the CPU scheduler can preempt running jobs, 
        /// and the priority of a job is "remembered" from one burst until the next. 
        /// In more detail, rules (2) and (5) are modified as follows:
        /// - (2)' When a job becomes ready because it has finished doing I/O, it is given
        /// priority i-1, where i is the priority it had when it blocked for I/O. There is no level -1, so if
        /// a job finishes a burst at priority 0, it stays at priority 0. Newly created jobs are assigned
        /// priority 0.
        /// - (5)' This policy is aggressively preemptive in the following sense: If a job
        /// becomes ready while another job is running, it is added to the tail of the appropriate
        /// queue as defined by rule (2'), the running job is stopped and has 1 subtracted from its
        /// priority (unless it is already at priority 0), it is added to the tail of the appropriate queue,
        /// and another job is selected to run as in rule (2).
        /// </summary>
        AggressivePreEmptive,

        /// <summary>
        /// In this strategy the ready queue will consist of one queue ordered according to the time
        /// that the scheduler 'thinks' the job needs on the CPU.
        /// Calculate this "guess" using exponential averaging
        /// Calculate estimated time to next I/O as 10% of remaining time to completion
        /// </summary>
        PreEmptiveShortestJobFirst
    }
}
