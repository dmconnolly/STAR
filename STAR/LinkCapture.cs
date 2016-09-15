using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STAR {
    class LinkCapture {
        private List<Packet> packets;
        private Statistics stats;

        public Statistics Stats {
            get {
                return stats;
            }
        }

        public LinkCapture() {
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
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;
            byte port;

            if(lineCount >= 2) {
                // First two lines are timestamp for measurement start and port
                {
                    DateTime tmpTime = Packet.parseDateString(lines[0]);
                    if(startTime == DateTime.MinValue || tmpTime.Ticks < startTime.Ticks) {
                        startTime = tmpTime;
                        if(endTime == DateTime.MinValue || startTime.Ticks > endTime.Ticks) {
                            endTime = startTime;
                        }
                    }
                }
                port = Convert.ToByte(lines[1]);

                int lineIndex = 2;

                while(lineIndex < lineCount) {
                    string time, startCode, endCode, bytes, errorText;

                    // Next line is a timestamp
                    time = lines[lineIndex];

                    // Store this time in case file ends
                    {
                        DateTime tmpTime = Packet.parseDateString(time);
                        if(tmpTime.Ticks > endTime.Ticks) {
                            endTime = tmpTime;
                        }
                    }

                    if(++lineIndex >= lines.Length) {
                        break;
                    }

                    startCode = lines[lineIndex];

                    if(++lineIndex >= lines.Length) {
                        break;
                    }

                    if(startCode.Equals("E", StringComparison.Ordinal)) {
                        // This is an error packet
                        errorText = lines[lineIndex];
                        ErrorPacket errorMessage = new ErrorPacket(time, errorText);
                        packets.Add(errorMessage);
                    } else if(startCode.Equals("P", StringComparison.Ordinal)) {
                        // This is a packet
                        bytes = lines[lineIndex];

                        if(++lineIndex >= lines.Length) {
                            break;
                        }

                        endCode = lines[lineIndex];
                        DataPacket packet = new DataPacket(port, time, bytes, endCode);
                        packets.Add(packet);
                    } else {
                        // Unknown start code
                        // throw error?
                        break;
                    }

                    ++lineIndex;
                }
            }

            // Sort packet by timestamp (DateTime Ticks)
            packets.OrderBy(packet => packet.Time);

            // Collect statistics
            stats.collect(startTime, endTime, packets);
        }
    }
}
