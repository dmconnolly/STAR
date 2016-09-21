using System.Collections.Generic;

namespace STAR.Model {
    class WriteCommandPacket : DataPacket {
        private byte   m_destinationKey;
        private byte[] m_sourcePathAddress;
        private byte   m_sourceLogicalAddress;
        private ushort m_transactionId;
        private byte   m_extendedWriteAddress;
        private uint   m_writeAddress;
        private uint   m_dataLength;
        private byte   m_headerCRC;
        private byte[] m_dataBytes;
        private byte   m_dataCRC;

        public byte DestinationKey { get { return m_destinationKey; }}
        public byte[] SourcePathAddress { get { return m_sourcePathAddress; }}
        public byte SourceLogicalAddress { get { return m_sourceLogicalAddress; }}
        public ushort TransactionId { get { return m_transactionId; }}
        public byte ExtendedWriteAddress { get { return m_extendedWriteAddress; }}
        public uint WriteAddress { get { return m_writeAddress; }}
        public uint DataLength { get { return m_dataLength; }}
        public byte HeaderCRC { get { return m_headerCRC; }}
        public byte[] DataBytes { get { return m_dataBytes; }}
        public byte DataCRC { get { return m_dataCRC; }}

        public WriteCommandPacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode)
                : base(entryPort, exitPort, dateString, packetBytes, endCode) {

            m_sourcePathAddress = new byte[0];
            m_dataBytes = new byte[0];

            int byteCount = m_remainingBytes.Count;
            int byteIndex = 0;

            if(byteIndex >= byteCount) {
                return;
            }

            // Destination key
            m_destinationKey = m_remainingBytes[byteIndex];

            if(++byteIndex >= byteCount) {
                return;
            }

            // Source path address
            // Source logical address
            {
                List<byte> tmpBytes = new List<byte>();
                for(; byteIndex<byteCount; ++byteIndex) {
                    if(byteIndex >= byteCount) {
                        return;
                    }
                    if(m_remainingBytes[byteIndex] >= 32) {
                        m_sourceLogicalAddress = m_remainingBytes[byteIndex];
                        break;
                    }
                    tmpBytes.Add(m_remainingBytes[byteIndex]);
                }
                m_sourcePathAddress = tmpBytes.ToArray();
            }

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

            // Extended write address
            if(++byteIndex >= byteCount) {
                return;
            }
            m_extendedWriteAddress = m_remainingBytes[byteIndex];

            // Write address
            {
                if((4+byteIndex) >= byteCount) {
                    return;
                }

                m_writeAddress = (uint)(
                    m_remainingBytes[4+byteIndex] +
                    (m_remainingBytes[3+byteIndex] << 8) +
                    (m_remainingBytes[2+byteIndex] << 16) +
                    (m_remainingBytes[1+byteIndex] << 24));

                byteIndex += 4;
            }

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
            if((++byteIndex) >= byteCount) {
                return;
            }
            m_headerCRC = m_remainingBytes[byteIndex];

            // Data
            {
                List<byte> tmpBytes = new List<byte>();
                for(; byteIndex<(byteCount-1); ++byteIndex) {
                    tmpBytes.Add(m_remainingBytes[byteIndex]);
                }
            }

            // Data CRC
            m_dataCRC = m_remainingBytes[byteIndex];
        }
    }
}
