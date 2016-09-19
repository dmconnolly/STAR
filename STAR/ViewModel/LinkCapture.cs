using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Data;

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
            byte port;

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
                port = Convert.ToByte(lines[lineIndex++]);

                if(!m_portsLoaded.Contains(port)) {
                    m_portsLoaded.Add(port);
                    m_portsLoaded.Sort();
                }

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
                        ErrorPacket errorMessage = new ErrorPacket(port, time, errorText);
                        m_packets.Add(errorMessage);
                    } else if(startCode.Equals("P", StringComparison.Ordinal)) {
                        // This is a packet
                        bytes = lines[lineIndex];

                        if(++lineIndex >= lines.Length) {
                            break;
                        }

                        endCode = lines[lineIndex];
                        DataPacket packet = new DataPacket(port, time, bytes, endCode);
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
