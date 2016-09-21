using System;
using System.Globalization;

namespace STAR.Model {
    class Packet {
        private const string timeFormat = "dd-MM-yyyy HH:mm:ss.fff";

        private byte m_entryPort;
        private byte m_exitPort;
        private DateTime m_timestamp;
        private DateTime m_timeInSeconds;

        // Gets the source port
        public byte EntryPort {
            get {
                return m_entryPort;
            }
        }

        // Gets the destination port
        public byte ExitPort {
            get {
                return m_exitPort;
            }
        }

        // Gets the timestamp as millionths of a second
        // useful for ordering packets and errors on the UI
        public long Time {
            get {
                return m_timestamp.Ticks;
            }
        }

        // Gets the timestamp as a formatted string
        public string Timestamp {
            get {
                return string.Format("{0:" + timeFormat + "}", m_timestamp);
            }
        }

        public static string timeString(DateTime dateTime) {
            return string.Format("{0:" + timeFormat + "}", dateTime);
        }

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        public Packet(byte entryPort, byte exitPort, string dateString) {
            // Store source port
            m_entryPort = entryPort;

            // Store destination port
            m_exitPort = exitPort;

            // Take date as string and store as DateTime
            m_timestamp = parseDateString(dateString);

            // Gets the time with the milliseconds cut off
            m_timeInSeconds = m_timestamp.AddMilliseconds(0 - m_timestamp.Millisecond);
        }

        // Parse the date string and return a DateTime
        public static DateTime parseDateString(string dateString) {
            DateTime timestamp = new DateTime();

            try {
                timestamp = DateTime.ParseExact(dateString, timeFormat, CultureInfo.InvariantCulture);
            } catch(FormatException) {
                Console.WriteLine("{0} is not in the correct format.", dateString);
            } 

            return timestamp;
        }
    }
}
