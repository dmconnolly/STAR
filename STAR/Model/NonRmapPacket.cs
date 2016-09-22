using System.Collections.Generic;

namespace STAR.Model {
    class NonRmapPacket : DataPacket {
        private uint sequenceNumber;
        private byte[] cargo;

        public NonRmapPacket(byte entryPort, byte exitPort, string dateString, List<byte> packetBytes, string endCode, int sequenceIndex) 
            : base(entryPort, exitPort, dateString, packetBytes, endCode) {
            // Empty for now
        }
    }
}
