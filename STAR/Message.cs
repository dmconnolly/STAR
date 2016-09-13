using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Message {
        private const String timeFormat = "dd-MM-yyyy HH:mm:ss.fff";
        private DateTime timestamp;

        // Gets the timestamp as millionths of a second
        // useful for ordering packets and errors on the UI
        public long Time {
            get {
                return this.timestamp.Ticks;
            }
        }

        // Gets the timestamp as a formatted string
        public String TimeString {
            get {
                return String.Format("{0:" + timeFormat + "}", timestamp);
            }
        }

        public static String timeString(DateTime dateTime) {
            return String.Format("{0:" + timeFormat + "}", dateTime);
        }

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        public Message(String dateString) {
            // Take date as string and store as DateTime
            this.timestamp = parseDateString(dateString);
        }

        // Parse the date string and return a DateTime
        public static DateTime parseDateString(String dateString) {
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
