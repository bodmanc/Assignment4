using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSSimulator
{
    /// <summary>
    /// Class to simulate system clock
    /// </summary>
    internal class Clock
    {
        /// <summary>
        /// Private conmstructor to force Singleton pattern
        /// </summary>
        private Clock()
        {
            Time = 0;
        }

        /// <summary>
        /// Private instance of the clock
        /// </summary>
        private static Clock? instance = null;

        /// <summary>
        /// Timer for the clock
        /// </summary>
        public long Time { get; private set; }

        /// <summary>
        /// Singleton instance for the clock
        /// </summary>
        public static Clock Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Clock();
                }
                return instance;
            }
        }

        /// <summary>
        /// Increment the timer
        /// </summary>
        public void Tick()
        {
            ++Time;
        }
    }
}
