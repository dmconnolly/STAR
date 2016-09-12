using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    public class Packet {
        private DateTime timestamp;
        private Byte protocolID;
        private Byte[] address;
        private Byte[] cargo;
        private bool error;

        // Packet constructor
        public Packet(String dateString, List<Byte> packet, bool error) {
            // Take date as string and store as DateTime
            this.timestamp = parseDateString(dateString);

            int packetBytes = packet.Count();

            // Parse packet bytes for address (up to and including first byte >= 32)
            List<Byte> addressBytes = new List<Byte>();
            for(int i=0; i<packetBytes; ++i) {
                Byte curByte = packet[i];

                // Add current byte to the temporary list
                addressBytes.Add(curByte);

                // If the byte is 32 or larger, this is the last byte of the address
                if(curByte >= 32) {
                    // Store address bytes in address class member array
                    address = addressBytes.ToArray();

                    // Next byte is the protocol ID
                    protocolID = packet[++i];

                    // Store the rest of the packet bytes in address class member array
                    cargo = packet.Skip<Byte>(++i).ToArray();

                    break;
                }
            }

            this.error = error;
        }

        // Parse the date string and return a DateTime
        private DateTime parseDateString(String dateString) {
            DateTime timestamp = new DateTime();

            CultureInfo provider = CultureInfo.InvariantCulture;
            const String format = "dd-mm-yyyy HH:mm:ss.FFFF";

            if(!DateTime.TryParseExact(dateString, format, null, DateTimeStyles.None, out timestamp)) {
                Console.WriteLine("Unable to convert '{0}' to a date and time.", dateString);
            }

            return timestamp;
        }
    }
}
