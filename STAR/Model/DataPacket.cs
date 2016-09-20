using System;
using System.Collections.Generic;
using System.Linq;

namespace STAR {
    class DataPacket : Packet {
        private ushort m_protocolID;
        private byte[] m_address;
        private string m_endCode;
        private bool m_valid;
        private List<byte> m_remainingBytes;
        private long m_cargoByteCount;

        // Accessors for class member variables
        public ushort Protocol { get { return m_protocolID; }}
        public byte[] AddressBytes { get { return m_address; }}
        //public byte ID { get { return m_packetID;  }}
        public bool Valid { get { return m_valid; }}
        public string EndCode { get { return m_endCode; }}
        public long CargoByteCount { get { return m_cargoByteCount; }}

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        // List of bytes which make up the packet, including address bytes and protocol ID
        // and whether the packet ended with EOP and not EEP
        public DataPacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode)
                : base(entryPort, exitPort, dateString) {
            long byteCount = packetBytes.Count;

            // Parse packet bytes for address (up to and including first byte >= 32)
            List<byte> addressBytes = new List<byte>();
            for(int i=0; i<byteCount; ++i) {
                // Add current byte to the temporary list
                addressBytes.Add(packetBytes[i]);

                // If the byte is 32 or larger, this is the last byte of the address
                if(packetBytes[i] >= 32) {
                    // Store address bytes in address class member array
                    m_address = addressBytes.ToArray();

                    // Next byte is the protocol ID
                    if(++i < byteCount) {
                        m_cargoByteCount = byteCount - i;
                        if(packetBytes[i] != 0) {
                            m_protocolID = packetBytes[i];
                        } else {
                            if((2+i) >= byteCount) {
                                break;
                            }

                            byte[] pidBytes = {
                                packetBytes[++i],
                                packetBytes[++i]
                            };

                            if(BitConverter.IsLittleEndian) {
                                Array.Reverse(pidBytes);
                            }

                            m_protocolID = BitConverter.ToUInt16(pidBytes, 0);
                        }
                    }

                    // Store the rest of the packet bytes in address class member array
                    if(++i < byteCount) {
                       m_remainingBytes = packetBytes.Skip(i).ToList();
                    }

                    break;
                }
            }

            //End code - EEP, EOP, None
            m_endCode = endCode;

            //Only valid if endCode is EOP
            m_valid = m_endCode == "EOP";
        }
    }
}
