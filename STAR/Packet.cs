using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Packet : Message {
        private Byte protocolID;
        private Byte[] address;
        private Byte[] cargo;
        private bool valid;

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.FFFF
        // List of bytes which make up the packet, including address bytes and protocol ID
        // and whether the packet ended with EOP and not EEP
        public Packet(String dateString, String packetByteString, bool valid) : base(dateString) {
            String[] packetByteStringSplit = packetByteString.Split(' ');

            int byteCount = packetByteStringSplit.Count();

            Byte[] packetBytes = new Byte[byteCount];
            for(int i=0; i<byteCount; ++i) {
                packetBytes[i] = Convert.ToByte(packetByteStringSplit[i], 16);
            }

            // Parse packet bytes for address (up to and including first byte >= 32)
            List<Byte> addressBytes = new List<Byte>();
            for(int i=0; i<byteCount; ++i) {
                Byte curByte = packetBytes[i];

                // Add current byte to the temporary list
                addressBytes.Add(curByte);

                // If the byte is 32 or larger, this is the last byte of the address
                if(curByte >= 32) {
                    // Store address bytes in address class member array
                    address = addressBytes.ToArray();

                    // Next byte is the protocol ID
                    protocolID = packetBytes[++i];

                    // Store the rest of the packet bytes in address class member array
                    cargo = packetBytes.Skip<Byte>(++i).ToArray();

                    break;
                }
            }

            this.valid = valid;
        }

        // Accessor for protocol ID
        public Byte Protocol() {
            return protocolID;
        }

        // Accessor for address bytes
        public Byte[] AddressBytes() {
            return address;
        }

        // Accessor for cargo bytes
        public Byte[] CargoBytes() {
            return cargo;
        }

        // Returns true if the packet was valid
        // i.e. ended with EOP rather than EEP
        public bool Valid() {
            return valid;
        }
    }
}
