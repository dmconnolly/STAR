using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Message {
        private const String timeFormat = "dd-MM-yyyy HH:mm:ss.ffff";
        private DateTime timestamp;

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.FFFF
        public Message(String dateString) {
            // Take date as string and store as DateTime
            this.timestamp = parseDateString(dateString);
        }

        // Gets the timestamp as millionths of a second
        // useful for ordering packets and errors on the UI
        public long getTime() {
            return this.timestamp.Ticks;
        }

        // Gets the timestamp as a formatted string
        public String getTimeString() {
            return String.Format("{0:" + timeFormat + "}", timestamp);
        }

        // Parse the date string and return a DateTime
        private DateTime parseDateString(String dateString) {
            DateTime timestamp = new DateTime();

            CultureInfo provider = CultureInfo.InvariantCulture;

            try {
                timestamp = DateTime.ParseExact(dateString, timeFormat, provider);
            } catch(FormatException) {
                Console.WriteLine("{0} is not in the correct format.", dateString);
            } 

            return timestamp;
        }
    }
}
