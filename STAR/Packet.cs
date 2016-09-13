using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Packet : Message {
        private byte m_protocolID;
        private byte[] m_address;
        //private byte m_packetID;
        private byte[] m_cargo;
        private bool m_valid;

        // Accessor for all packet bytes
        public byte[] Bytes {
            get {
                // Possibly refactor to store full packet data in class at all times
                // depends if avoiding data duplication or speed is more important

                //List<Byte> list = new List<Byte>(m_address.Length + 2 + m_cargo.Length);
                List<Byte> list = new List<Byte>(m_address.Length + 1 + m_cargo.Length);
                list.AddRange(m_address);
                list.Add(m_protocolID);
                //list.Add(m_packetID);
                list.AddRange(m_cargo);
                return list.ToArray();
            }
        }

        // Accessors for class member variables
        public byte Protocol { get { return m_protocolID; }}
        public byte[] AddressBytes { get { return m_address; }}
        //public byte ID { get { return m_packetID;  } }
        public byte[] CargoBytes { get {return m_cargo; }}
        public bool Valid { get { return m_valid; }}

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        // List of bytes which make up the packet, including address bytes and protocol ID
        // and whether the packet ended with EOP and not EEP
        public Packet(string dateString, string packetByteString, bool valid) : base(dateString) {
            string[] packetByteStringSplit = packetByteString.Split(' ');

            int byteCount = packetByteStringSplit.Count();

            byte[] packetBytes = new byte[byteCount];
            for(int i=0; i<byteCount; ++i) {
                packetBytes[i] = Convert.ToByte(packetByteStringSplit[i], 16);
            }

            // Parse packet bytes for address (up to and including first byte >= 32)
            List<byte> addressBytes = new List<byte>();
            for(int i=0; i<byteCount; ++i) {
                byte curByte = packetBytes[i];

                // Add current byte to the temporary list
                addressBytes.Add(curByte);

                // If the byte is 32 or larger, this is the last byte of the address
                if(curByte >= 32) {
                    // Store address bytes in address class member array
                    m_address = addressBytes.ToArray();

                    // Next byte is the protocol ID
                    m_protocolID = packetBytes[++i];

                    // Next byte is packet ID
                    //m_packetID = packetBytes[++i];

                    // Store the rest of the packet bytes in address class member array
                    m_cargo = packetBytes.Skip<byte>(++i).ToArray();

                    break;
                }
            }

            m_valid = valid;
        }

        public void printFields() {
            byte[] addressBytes = AddressBytes;
            Console.Write("Address: ");
            for(int i=0; i<addressBytes.Length; i++) {
                Console.Write(addressBytes[i].ToString("x2") + " ");
            }

            Console.WriteLine("\nProtocol: " + Protocol.ToString("x2"));

            //Console.WriteLine("ID: " + ID.ToString("x2"));

            byte[] cargoBytes = CargoBytes;
            Console.Write("Cargo: ");
            for(int i=0; i<cargoBytes.Length; i++) {
                Console.Write(cargoBytes[i].ToString("x2") + " ");
            }

            Console.WriteLine();
        }
    }
}
