using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// Class to perform logging to console
    /// </summary>
    internal class Logger
    {
        /// <summary>
        /// Log a statement if VerboseOutput is desired
        /// </summary>
        /// <param name="statement">ststement to be logged</param>
        public static void Log(string statement)
        {
            if (CommonParameters.VerboseOutput)
            {
                Console.WriteLine(statement);
            }
        }

        /// <summary>
        /// Log a statement always
        /// </summary>
        /// <param name="statement">Statement to be logged</param>
        public static void LogAlways(string statement)
        {
            Console.WriteLine(statement);
        }
    }
}
