using System;
using System.Collections.Generic;
using System.Linq;

namespace STAR.Model {
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

        public static Type GetType(List<byte> packetBytes) {
            int i=0;
            for(; i<packetBytes.Count(); ++i) {
                if(packetBytes[i] >= 32) {
                    break;
                }
            }

            if(++i >= packetBytes.Count()) {
                return typeof(DataPacket);
            }

            byte flagsByte;
            if(packetBytes[i] == 0) {
                if(++i >= packetBytes.Count()) {
                    return typeof(DataPacket);
                }
                flagsByte = packetBytes[i];
            } else {
                if(3+i >= packetBytes.Count()) {
                    return typeof(DataPacket);
                }
                flagsByte = packetBytes[3 + i];
            }

            /* Flag Bits
             * 0 - Reserved
             * 1 - 0=response, 1=command
             * 2 - 0=read, 1=write
             * 3 - 0=don't verify, 1=verify
             * 4 - 0=no ack, 1=ack
             * 5 - (Increment/No inc. address)  ?
             * 6 - Source Path Address Length bit 1
             * 6 - Source Path Address Length bit 2 */
            bool command = (flagsByte & (1 << 1)) != 0;
            bool write = (flagsByte & (1 << 2)) != 0;
            if(command) {
                return write ? typeof(WriteCommandPacket) : typeof(ReadCommandPacket);
            }
            return write ? typeof(WriteResponsePacket) : typeof(ReadResponsePacket);
        }

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
