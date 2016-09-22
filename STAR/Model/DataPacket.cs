using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STAR.Model {
    /*
     * Packet containing write or read command or response
     * This class contains variables and methods pertaining to only
     * the elements which are shared among all four packet types
     */
    class DataPacket : Packet {
        private ushort m_protocolId;
        private byte[] m_pathAddress;
        private byte m_logicalAddress;
        private string m_endCode;
        private bool m_valid;
        private long m_cargoByteCount;

        // Accessors for class member variables
        public ushort Protocol { get { return m_protocolId; }}
        public byte[] PathAddress { get { return m_pathAddress; } }
        public byte LogicalAddress { get { return m_logicalAddress; } }
        public bool Valid { get { return m_valid; }}
        public string EndCode { get { return m_endCode; }}
        public long CargoByteCount { get { return m_cargoByteCount; }}
        public bool CRCError { get { return m_CRCError; } }

        protected bool m_CRCError = false;
        protected List<byte> m_remainingBytes;

        // Takes a list of bytes representing the packet
        // returns the Type that the packet should be
        // used when parsing a file and storing packet data
        public static Type GetRmapPacketType(List<byte> packetBytes) {
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
                if(3+i >= packetBytes.Count()) {
                    return typeof(DataPacket);
                }
                flagsByte = packetBytes[3 + i];
            } else {
                if(++i >= packetBytes.Count()) {
                    return typeof(DataPacket);
                }
                flagsByte = packetBytes[i];
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
            var bits = new BitArray(new byte[] { flagsByte });

            // Shift left 6 bits and mask for command/response bit flag
            bool command = (flagsByte & (1 << 6)) != 0;

            // Shift left 5 bits and mask for read/write bit flag
            bool write = (flagsByte & (1 << 5)) != 0;

            // Return packet type based on the two flags
            return command ?
                write ? typeof(WriteCommandPacket) :
                typeof(ReadCommandPacket) :
                write ? typeof(WriteResponsePacket) :
                typeof(ReadResponsePacket);
        }

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        // List of bytes which make up the packet, including address bytes and protocol ID
        // and whether the packet ended with EOP and not EEP
        public DataPacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode)
                : base(entryPort, exitPort, dateString) {
            long byteCount = packetBytes.Count;

            // Parse packet bytes for address (up to and including first byte >= 32)
            List<byte> addressBytes = new List<byte>();
            int byteIndex = 0;
            for(; byteIndex<byteCount; ++byteIndex) {
                // If the byte is 32 or larger, this is the logical address
                if(packetBytes[byteIndex] >= 32) {
                    break;
                }

                // Add current byte to the temporary list
                addressBytes.Add(packetBytes[byteIndex]);
            }

            m_pathAddress = addressBytes.ToArray();

            // Store the rest of the packet bytes in address class member array
            if(++byteIndex >= byteCount) {
                return;
            }

            // Next byte is logical address
            m_logicalAddress = packetBytes[byteIndex];

            // Next byte is the protocol ID
            if(++byteIndex < byteCount) {
                m_cargoByteCount = byteCount - byteIndex;
                if(packetBytes[byteIndex] != 0) {
                    m_protocolId = packetBytes[byteIndex];
                } else {
                    if((2+byteIndex) >= byteCount) {
                        return;
                    }

                    m_protocolId = (ushort)(
                        packetBytes[2+byteIndex] +
                        (packetBytes[1+byteIndex] << 8)
                    );

                    byteIndex += 2;
                }
            }

            // Store the rest of the packet bytes in address class member array
            if(++byteIndex < byteCount) {
                m_remainingBytes = packetBytes.Skip(byteIndex).ToList();
            }

            //End code - EEP, EOP, None
            m_endCode = endCode;

            //Only valid if endCode is EOP
            m_valid = m_endCode == "EOP";
        }
    }
}
