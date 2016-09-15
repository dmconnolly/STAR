using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STAR {
    class Data {
        private List<Packet> packets;
        private Statistics stats;

        public Statistics Stats {
            get {
                return stats;
            }
        }

        public Data() {
            packets = new List<Packet>();
            stats = new Statistics();
        }

        // Reads and processes a file
        public void processFile(string path) {
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            byte port;

            // Clear anything in the packet list
            packets.Clear();

            string[] lines = File.ReadLines(@path).ToArray();

            // First two lines are timestamp for measurement start and port
            startTime = Packet.parseDateString(lines[0].Trim());
            port = Convert.ToByte(lines[1].Trim());

            int lineIndex = 3;
            while(lineIndex < lines.Length - 1) {
                string time, startCode, endCode, bytes, errorText;
                // Skip whitespace
                while(string.IsNullOrWhiteSpace(lines[lineIndex])) {
                    ++lineIndex;
                }

                // Next line is a timestamp
                time = lines[lineIndex++].Trim();

                // Store this time in case file ends
                endTime = Packet.parseDateString(time);

                // If blank line is after the timestamp
                if(lineIndex >= lines.Length || string.IsNullOrWhiteSpace(lines[lineIndex])) {
                    // This is end of the file
                    break;
                }

                startCode = lines[lineIndex].Trim();

                if(startCode.Equals("E", StringComparison.Ordinal)) {
                    // This is an error packet
                    errorText = lines[++lineIndex].Trim();
                    ErrorPacket errorMessage = new ErrorPacket(time, errorText);
                    packets.Add(errorMessage);
                } else if(startCode.Equals("P", StringComparison.Ordinal)) {
                    // This is a packet
                    bytes = lines[++lineIndex].Trim();
                    endCode = lines[++lineIndex].Trim();
                    DataPacket packet = new DataPacket(time, bytes, endCode);
                    packets.Add(packet);
                } else {
                    // Unknown start code
                    // throw error?
                }

                lineIndex++;
            }

            stats.collect(port, startTime, endTime, packets);
        }
    }
}
