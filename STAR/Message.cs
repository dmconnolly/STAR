using System;
using System.Globalization;

namespace STAR {
    class Message {
        private const string timeFormat = "dd-MM-yyyy HH:mm:ss.fff";
        private DateTime timestamp;

        // Gets the timestamp as millionths of a second
        // useful for ordering packets and errors on the UI
        public long Time {
            get {
                return timestamp.Ticks;
            }
        }

        // Gets the timestamp as a formatted string
        public string TimeString {
            get {
                return string.Format("{0:" + timeFormat + "}", timestamp);
            }
        }

        public static string timeString(DateTime dateTime) {
            return string.Format("{0:" + timeFormat + "}", dateTime);
        }

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        public Message(string dateString) {
            // Take date as string and store as DateTime
            timestamp = parseDateString(dateString);
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
