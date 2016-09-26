using System;
using System.Globalization;

namespace STAR.Model {
    class Packet {
        private const string timeFormatInput = "dd-MM-yyyy HH:mm:ss.fff";

        private byte m_entryPort;
        private byte m_exitPort;
        private DateTime m_timestamp;
        private DateTime m_timeInMinutes;

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
        public DateTime TimeStamp {
            get {
                return m_timestamp;
            }
            set {
                m_timestamp = value;
            }
        }

        public DateTime TimeStampInMinutes {
            get { return m_timeInMinutes; }
        }

        public static string timeString(DateTime dateTime) {
            return string.Format("{0:" + timeFormatInput + "}", dateTime);
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
            m_timeInMinutes = TimeStampInMinutes.AddSeconds(0 - TimeStampInMinutes.Second);
            m_timeInMinutes = TimeStampInMinutes.AddMilliseconds(0 - TimeStampInMinutes.Millisecond);
        }

        // Parse the date string and return a DateTime
        public static DateTime parseDateString(string dateString) {
            DateTime timestamp = new DateTime();

            try {
                timestamp = DateTime.ParseExact(dateString, timeFormatInput, CultureInfo.InvariantCulture);
            } catch(FormatException) {
                Console.WriteLine("{0} is not in the correct format.", dateString);
            }

            return timestamp;
        }
    }
}
