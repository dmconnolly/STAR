using System;
using System.Collections.Generic;
using System.Linq;

namespace STAR.Model {
    class WriteCommandPacket : DataPacket {
        private byte m_destinationKey;
        private byte[] m_sourcePathAddress;
        private byte m_sourceLogicalAddress;
        private ushort m_transactionId;
        private byte m_extendedWriteAddress;
        private uint m_writeAddress;
        private uint m_dataLength;
        private byte m_headerCRC;
        private byte[] m_dataBytes;
        private byte m_dataCRC;

        public WriteCommandPacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode)
                : base(entryPort, exitPort, dateString, packetBytes, endCode) {

            m_sourcePathAddress = new byte[0];
            m_dataBytes = new byte[0];

            int byteCount = m_remainingBytes.Count();
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

                byte[] tIdBytes = {
                    m_remainingBytes[++byteIndex],
                    m_remainingBytes[++byteIndex]
                };

                if(BitConverter.IsLittleEndian) {
                    Array.Reverse(tIdBytes);
                }

                m_transactionId = BitConverter.ToUInt16(tIdBytes, 0);
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

                byte[] writeAddressBytes = {
                    m_remainingBytes[++byteIndex],
                    m_remainingBytes[++byteIndex],
                    m_remainingBytes[++byteIndex],
                    m_remainingBytes[++byteIndex]
                };

                if(BitConverter.IsLittleEndian) {
                    Array.Reverse(writeAddressBytes);
                }

                m_writeAddress = BitConverter.ToUInt32(writeAddressBytes, 0);
            }

            // Data length
            {
                if((3+byteIndex) >= byteCount) {
                    return;
                }

                byte[] dataLengthBytes = {
                        m_remainingBytes[++byteIndex],
                        m_remainingBytes[++byteIndex],
                        m_remainingBytes[++byteIndex],
                        0
                    };

                if(BitConverter.IsLittleEndian) {
                    Array.Reverse(dataLengthBytes);
                }

                m_dataLength = BitConverter.ToUInt32(dataLengthBytes, 0);
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

            m_dataCRC = m_remainingBytes[byteIndex];
        }
    }
}
