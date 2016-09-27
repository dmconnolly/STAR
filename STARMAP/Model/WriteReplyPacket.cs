using System.Collections.Generic;
using System.Linq;

namespace STARMAP.Model {
    class WriteReplyPacket : DataPacket {
        private byte   m_packetTypeByte;
        private byte   m_status;
        private byte   m_destinationLogicalAddress;
        private ushort m_transactionId;
        private byte   m_replyCRC;

        public byte PacketTypeByte { get { return m_packetTypeByte; } }
        public byte Status { get { return m_status; } }
        public byte DestinationLogicalAddress { get { return m_destinationLogicalAddress; } }
        public ushort TransactionId { get { return m_transactionId; } }
        public byte ReplyCRC { get { return m_replyCRC; } }

        public WriteReplyPacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode)
                : base(entryPort, exitPort, dateString, packetBytes, endCode) {

            int byteCount = m_remainingBytes.Count;
            int byteIndex = 0;

            if(byteIndex >= byteCount) {
                return;
            }

            // Packet type
            m_packetTypeByte = m_remainingBytes[byteIndex];

            if(++byteIndex >= byteCount) {
                return;
            }

            // Status
            m_status = m_remainingBytes[byteIndex];

            if(++byteIndex >= byteCount) {
                return;
            }

            // Destination logical address
            m_destinationLogicalAddress = m_remainingBytes[byteIndex];

            // Transaction identifier
            {
                if((2+byteIndex) >= byteCount) {
                    return;
                }

                m_transactionId = (ushort)(
                    m_remainingBytes[2+byteIndex] +
                    (m_remainingBytes[1+byteIndex] << 8)
                );

                byteIndex += 2;
            }

            if(++byteIndex >= byteCount) {
                return;
            }

            // Reply RmapCRC
            m_replyCRC = m_remainingBytes[byteIndex];

            List<byte> headerBytes = new List<byte>(byteIndex + 3);
            headerBytes.Add(m_logicalAddress);
            headerBytes.Add((byte)m_protocolId);
            headerBytes.AddRange(m_remainingBytes.Take(byteIndex));

            if(!m_CRCError && !RmapCRC.validCRC(headerBytes.ToArray(), m_replyCRC)) {
                m_CRCError = true;
            }

            m_remainingBytes = null;
        }
    }
}
