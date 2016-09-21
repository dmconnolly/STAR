using System.Collections.Generic;

namespace STAR.Model {
    class WriteResponsePacket : DataPacket {
        private byte m_status;
        private byte m_destinationLogicalAddress;
        private ushort m_transactionId;
        private byte m_replyCRC;

        public WriteResponsePacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode)
                : base(entryPort, exitPort, dateString, packetBytes, endCode) {

            int byteCount = m_remainingBytes.Count;
            int byteIndex = 0;

            if(byteIndex >= byteCount) {
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
                    (m_remainingBytes[1+byteIndex] << 8));

                byteIndex += 2;
            }

            if(++byteIndex >= byteCount) {
                return;
            }

            // Reply CRC
            m_replyCRC = m_remainingBytes[byteIndex];
        }
    }
}
