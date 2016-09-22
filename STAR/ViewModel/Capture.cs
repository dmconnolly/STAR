using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using STAR.Model;

namespace STAR.ViewModel {
    /*
     * Class responsible for handling captured packet data
     * files from multiple ports can be loaded, parsed and their contents
     * stored in a meaningful format.
     */
    class Capture {
        private List<Packet> m_packets; // List of all captured packets
        private Statistics m_stats; // Storage of statistics for the whole data set
        private List<byte> m_portsLoaded; // List of the entry ports we have loaded data from

        private DateTime startTime = DateTime.MinValue;
        private DateTime endTime = DateTime.MinValue;

        // Accessors for class member data
        public Packet[] Packets { get { return m_packets.ToArray(); }}
        public Statistics Stats { get { return m_stats; }}
        public byte[] PortsLoaded { get { return m_portsLoaded.ToArray(); }}

        // Constructor
        public Capture() {
            m_packets = new List<Packet>();
            m_stats = new Statistics();
            m_portsLoaded = new List<byte>();
        }

        // Clears any stored packet data
        public void Clear() {
            startTime = DateTime.MinValue;
            endTime = DateTime.MinValue;
            m_packets.Clear();
            m_stats.Clear();
            m_portsLoaded.Clear();
        }

        // Process an array of files
        public void processFiles(string[] paths) {
            foreach(string path in paths) {
                processFile(path);
            }

            // Sort packet by timestamp (DateTime Ticks)
            m_packets.OrderBy(packet => packet.TimeStamp);

            // Collect statistics
            m_stats.collect(startTime, endTime, m_packets);
        }

        // Processes a file and extracts the packet data
        private void processFile(string path) {
            string[] lines;
            {
                List<string> linesList = File.ReadLines(@path).ToList();
                linesList.RemoveAll(string.IsNullOrEmpty);
                lines = linesList.ToArray();
            }
            int lineCount = lines.Length;
            byte entryPort;
            byte exitPort;

            List<string> allPacketTimes = new List<string>();
            List<string> allPacketEndMarkers = new List<string>();
            List<List<byte>> allPacketBytes = new List<List<byte>>();

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

                    // Start code (E or P)
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

                        allPacketTimes.Add(time);
                        allPacketBytes.Add(packetBytes);
                        allPacketEndMarkers.Add(endCode);
                    } else {
                        // Unknown start code
                        // throw error?
                        break;
                    }

                    ++lineIndex;
                }

                int sequenceIdIndex = -1;
                for(int i=0; i<allPacketBytes.Count; i++) {
                    Type packetType = DataPacket.GetPacketType(allPacketBytes[i]);
                    object[] args;
                    if(packetType == typeof(NonRmapPacket)) {
                        if(sequenceIdIndex == -1) {
                            sequenceIdIndex = getSequenceIdIndex(allPacketBytes);
                        }
                        args = new object[] {
                            entryPort,
                            exitPort,
                            allPacketTimes[i],
                            allPacketBytes[i],
                            allPacketEndMarkers[i],
                            sequenceIdIndex
                        };
                    } else {
                        args = new object[] {
                            entryPort,
                            exitPort,
                            allPacketTimes[i],
                            allPacketBytes[i],
                            allPacketEndMarkers[i]
                        };
                    }
                    dynamic packet = Activator.CreateInstance(packetType, args);
                    m_packets.Add(packet);
                }
            }
        }

        private int getSequenceIdIndex(List<List<byte>> allPacketBytes) {
            const int sequenceCountReq = 5;
            const int bytesToCheck = 10;
            const int packetsToCheck = 6;
            
            for(int i=0; i<(bytesToCheck+1); ++i) {
                byte sequenceCount = 0;
                byte lastValue = 0;
                byte firstValue = 0;

                for(int j=0; j<packetsToCheck; ++j) {
                    List<byte> packetBytes = allPacketBytes[j];
                    if(i >= packetBytes.Count()) {
                        return -1;
                    }

                    int k = 0;
                    for(; k<packetBytes.Count(); ++k) {
                        if(packetBytes[k] >= 32) {
                            ++k;
                            break;
                        }
                    }

                    if(j == 0) {
                        firstValue = packetBytes[k+i];
                        lastValue = firstValue;
                    } else {
                        if((packetBytes[k+i] == firstValue + j) ||
                                ((lastValue == 255) && (packetBytes[k+i] == 0))) {
                            ++sequenceCount;
                            lastValue = packetBytes[k+i];
                            if(lastValue == 255) {
                                firstValue = (byte)(0 - j);
                            }
                        } else {
                            break;
                        }
                    }
                    if(sequenceCount >= sequenceCountReq) {
                        return k+i;
                    }
                }
            }
            return -1;
        }
    }
}
