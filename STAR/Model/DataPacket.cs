using System;
using System.Collections.Generic;
using System.Linq;

namespace STAR {
    class DataPacket : Packet {
        private byte m_protocolID;
        private byte[] m_address;
        //private byte m_packetID;
        private byte[] m_cargo;
        private string m_endCode;
        private bool m_valid;
        private int m_characterCount;

        // Accessor for all packet bytes
        public byte[] Bytes {
            get {
                // Possibly refactor to store full packet data in class at all times
                // depends if avoiding data duplication or speed is more important

                //List<Byte> list = new List<Byte>(m_address.Length + 2 + m_cargo.Length);
                List<byte> list = new List<byte>(m_address.Length + 1 + m_cargo.Length);
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
        //public byte ID { get { return m_packetID;  }}
        public byte[] CargoBytes { get {return m_cargo; }}
        public bool Valid { get { return m_valid; }}
        public string EndCode { get { return m_endCode; }}
        public int characterCount { get { return m_characterCount; }}
        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        // List of bytes which make up the packet, including address bytes and protocol ID
        // and whether the packet ended with EOP and not EEP
        public DataPacket(byte port, string dateString, string packetByteString, string endCode) : base(port, dateString) {
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
                    if(++i < byteCount) {
                        m_protocolID = packetBytes[i];
                    }

                    // Next byte is packet ID
                    // if(++i < byteCount) {
                    //     m_packetID = packetBytes[i];
                    // }

                    // Store the rest of the packet bytes in address class member array
                    if(++i < byteCount) {
                        m_cargo = packetBytes.Skip<byte>(i).ToArray();
                    }

                    break;
                }
            }

            for (int pointer = 0; pointer < m_cargo.Length; pointer++)
            {
                m_characterCount++;
            }

            //End code - EEP, EOP, None
            m_endCode = endCode;

            //Only valid if endCode is EOP
            m_valid = m_endCode == "EOP";
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
