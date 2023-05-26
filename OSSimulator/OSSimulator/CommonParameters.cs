using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// Common Parameters
    /// </summary>
    internal static class CommonParameters
    {
        /// <summary>
        /// Time Slices for the queues
        /// </summary>
        public static int[] TimeSlices { get; internal set; } = new int[8];

        /// <summary>
        /// Seed for random number generator
        /// </summary>
        public static int RandomSeed { get; internal set; } = 1;

        /// <summary>
        /// Random number generator
        /// </summary>
        public static Random RandomNumberGenerator { get; internal set; } = new Random(RandomSeed);

        /// <summary>
        /// Probability of IO Request - 1 out of N
        /// </summary>
        public static int IORequestChance { get; internal set; }

        /// <summary>
        /// Probability of IO completion - 1 out of N
        /// </summary>
        public static int IOCompletionChance { get; internal set; }

        /// <summary>
        /// Whether Verbose output is desired
        /// </summary>
        public static bool VerboseOutput { get; internal set; }

        /// <summary>
        /// Scheduling Policy to be used
        /// </summary>
        public static SchedulingPolicy Policy { get; internal set; }

        /// <summary>
        /// Fully qualified path of the input data file
        /// </summary>
        public static String? InputFilePath { get; internal set; }

        /// <summary>
        /// Prints the parameters
        /// </summary>
        public static void Print()
        {
            Logger.LogAlways("Time slices ...");
            for (int i = 0; i < TimeSlices.Length; i++)
            {
                Logger.LogAlways("Queue[" + i + "] : " + TimeSlices[i]);
            }

            Logger.LogAlways("Random seed = " + RandomSeed);
            Logger.LogAlways("IORequest chance = 1 out of " + IORequestChance);
            Logger.LogAlways("IOCompletion chance = 1 out of " + IOCompletionChance);
            Logger.LogAlways("Verbose output = " + VerboseOutput);
            Logger.LogAlways("Scheduling policy = " + Policy.ToString());
            Logger.LogAlways("Input file = " + InputFilePath);
        }
    }
}
