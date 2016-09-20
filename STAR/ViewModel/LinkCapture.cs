using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STAR {
    class LinkCapture {
        private List<Packet> m_packets;
        private Statistics m_stats;
        private List<byte> m_portsLoaded;

        public Packet[] Packets {
            get {
                return m_packets.ToArray();
            }
        }

        public Statistics Stats {
            get {
                return m_stats;
            }
        }

        public byte[] PortsLoaded {
            get {
                return m_portsLoaded.ToArray();
            }
        }

        public LinkCapture() {
            m_packets = new List<Packet>();
            m_stats = new Statistics();
            m_portsLoaded = new List<byte>();
        }

        public void Clear() {
            m_packets.Clear();
            m_stats.Clear();
            m_portsLoaded.Clear();
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
            byte entryPort;
            byte exitPort;

            if(lineCount >= 2) {
                int lineIndex = 0;

                // First two lines are timestamp for measurement start and port
                {
                    DateTime tmpTime = Packet.parseDateString(lines[lineIndex++]);
                    if(startTime == DateTime.MinValue || tmpTime.Ticks < startTime.Ticks) {
                        startTime = tmpTime;
                        if(endTime == DateTime.MinValue || startTime.Ticks > endTime.Ticks) {
                            endTime = startTime;
                        }
                    }
                }
                entryPort = Convert.ToByte(lines[lineIndex++]);

                if(!m_portsLoaded.Contains(entryPort)) {
                    m_portsLoaded.Add(entryPort);
                    m_portsLoaded.Sort();
                }

                exitPort = (byte)(entryPort + (entryPort % 2 == 0 ? -1 : 1));

                while(lineIndex < lineCount) {
                    string time, startCode, endCode, byteString, errorText;

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
                        ErrorPacket errorMessage = new ErrorPacket(entryPort, exitPort, time, errorText);
                        m_packets.Add(errorMessage);
                    } else if(startCode.Equals("P", StringComparison.Ordinal)) {
                        // This is a packet
                        byteString = lines[lineIndex];

                        if(++lineIndex >= lines.Length) {
                            break;
                        }

                        endCode = lines[lineIndex];

                        string[] byteStringSplit = byteString.Split(' ');
                        int byteCount = byteStringSplit.Count();
                        List<byte> packetBytes = new List<byte>(byteCount);
                        for(int i=0; i<byteCount; ++i) {
                            packetBytes.Add(Convert.ToByte(byteStringSplit[i], 16));
                        }

                        DataPacket packet = new DataPacket(entryPort, exitPort, time, packetBytes, endCode);
                        m_packets.Add(packet);
                    } else {
                        // Unknown start code
                        // throw error?
                        break;
                    }

                    ++lineIndex;
                }
            }

            // Sort packet by timestamp (DateTime Ticks)
            m_packets.OrderBy(packet => packet.Time);

            // Collect statistics
            m_stats.collect(startTime, endTime, m_packets);
        }
    }
}
