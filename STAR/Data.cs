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

        public void processFile(string path) {
            string[] lines;
            {
                List<string> linesList = File.ReadLines(@path).ToList();
                linesList.RemoveAll(String.IsNullOrEmpty);
                lines = linesList.ToArray();
            }
            int lineCount = lines.Length;
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            byte port;

            // Clear anything in the packet list
            packets.Clear();

            if(lineCount >= 2) {
                // First two lines are timestamp for measurement start and port
                startTime = Packet.parseDateString(lines[0]);
                endTime = startTime;
                port = Convert.ToByte(lines[1]);

                int lineIndex = 2;

                while(lineIndex < lineCount) {
                    string time, startCode, endCode, bytes, errorText;

                    // Next line is a timestamp
                    time = lines[lineIndex++];

                    // Store this time in case file ends
                    endTime = Packet.parseDateString(time);

                    if(lineIndex >= lines.Length) {
                        break;
                    }

                    startCode = lines[lineIndex++];

                    if(lineIndex >= lines.Length) {
                        break;
                    }

                    if(startCode.Equals("E", StringComparison.Ordinal)) {
                        // This is an error packet
                        errorText = lines[lineIndex];
                        ErrorPacket errorMessage = new ErrorPacket(time, errorText);
                        packets.Add(errorMessage);
                    } else if(startCode.Equals("P", StringComparison.Ordinal)) {
                        // This is a packet
                        bytes = lines[lineIndex++];

                        if(lineIndex >= lines.Length) {
                            break;
                        }

                        endCode = lines[lineIndex];
                        DataPacket packet = new DataPacket(time, bytes, endCode);
                        packets.Add(packet);
                    } else {
                        // Unknown start code
                        // throw error?
                        break;
                    }

                    lineIndex++;
                }

                stats.collect(port, startTime, endTime, packets);
            }
        }
    }
}
