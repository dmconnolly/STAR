using System.Collections.Generic;

namespace STAR.Model {
    class ReadResponsePacket : DataPacket {
        private byte   m_status;
        private byte   m_destinationLogicalAddress;
        private ushort m_transactionId;
        private uint   m_dataLength;
        private byte   m_headerCRC;
        private byte[] m_dataBytes;
        private byte   m_dataCRC;

        public byte Status { get { return m_status; }}
        public byte DestinationLogicalAddress { get { return m_destinationLogicalAddress; }}
        public ushort TransactionId { get { return m_transactionId; }}
        public uint DataLength { get { return m_dataLength; }}
        public byte HeaderCRC { get { return m_headerCRC; }}
        public byte[] DataBytes { get { return m_dataBytes; }}
        public byte DataCRC { get { return m_dataCRC; }}

        public ReadResponsePacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode)
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

            // Next byte is reserved, skip it
            if(2+byteIndex >= byteCount) {
                return;
            }
            byteIndex += 2;

            // Data length
            {
                if((3+byteIndex) >= byteCount) {
                    return;
                }

                m_dataLength = (uint)(
                    m_remainingBytes[3+byteIndex] +
                    (m_remainingBytes[2+byteIndex] << 8) +
                    (m_remainingBytes[1+byteIndex] << 16));

                byteIndex += 3;
            }

            // Header CRC
            if(++byteIndex >= byteCount) {
                return;
            }
            m_headerCRC = m_remainingBytes[byteIndex];

            // Data
            {
                List<byte> tmpBytes = new List<byte>();
                for(; byteIndex<(byteCount-1); ++byteIndex) {
                    tmpBytes.Add(m_remainingBytes[byteIndex]);
                }
                m_dataBytes = tmpBytes.ToArray();
            }

            // Data CRC
            m_dataCRC = m_remainingBytes[byteIndex];
        }
    }
}
