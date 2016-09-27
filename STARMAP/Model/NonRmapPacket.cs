using System.Linq;
using System.Collections.Generic;

namespace STARMAP.Model {
    class NonRmapPacket : DataPacket {
        public uint SequenceNumber { get; private set; }
        public byte[] Cargo { get; private set; }

        public NonRmapPacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode, int sequenceIndex)
            : base(entryPort, exitPort, dateString, packetBytes, endCode) {

            sequenceIndex -= m_totalPacketBytes - m_remainingBytes.Count;

            Cargo = new byte[0];

            if(sequenceIndex >= m_remainingBytes.Count || sequenceIndex < 0) {
                return;
            }

            SequenceNumber = m_remainingBytes[sequenceIndex];

            if((sequenceIndex + 1) >= m_remainingBytes.Count) {
                return;
            }

            Cargo = m_remainingBytes.Skip(sequenceIndex + 1).ToArray();

            m_remainingBytes = null;
        }
    }
}
