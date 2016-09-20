using System;
using System.Collections.Generic;

namespace STAR {
    class Statistics {
        private const double ticksPerSecond = 10000000;

        private long m_packetCount;
        private long m_validPacketCount;
        private long m_invalidPacketCount;
        private long m_errorMessageCount;
        private long m_totalByteCount;
        private double m_measurementTimeSeconds;
        private double m_totalPacketsPerSecond;
        private double m_totalErrorsPerSecond;
        private double m_totalBytesPerSecond;

        public long PacketCount { get { return m_packetCount; }}
        public long ValidPacketCount { get { return ValidPacketCount; }}
        public long InvalidPacketCount { get { return m_invalidPacketCount;  }}
        public long ErrorMessageCount { get { return m_errorMessageCount;  }}
        public long TotalByteCount { get { return m_totalByteCount;  }}
        public double MeasurementTimeSeconds { get { return m_measurementTimeSeconds;  }}
        public double TotalPacketsPerSecond { get { return m_totalPacketsPerSecond; }}
        public double TotalErrorsPerSecond { get { return m_totalErrorsPerSecond; }}
        public double TotalBytesPerSecond { get { return m_totalBytesPerSecond; }}

        public Statistics() {
            Clear();
        }

        public void collect(DateTime startTime, DateTime endTime, List<Packet> packets) {
            Clear();

            if(packets.Count == 0) {
                return;
            }

            foreach(Packet packet in packets) {
                if(packet is DataPacket) {
                    m_packetCount++;
                    m_totalByteCount += (packet as DataPacket).CargoBytes.Length;
                    if((packet as DataPacket).Valid) {
                        m_validPacketCount++;
                    } else {
                        m_invalidPacketCount++;
                    }
                } else {
                    m_errorMessageCount++;
                }
            }

            m_measurementTimeSeconds = (endTime.Ticks - startTime.Ticks) / ticksPerSecond;
            m_totalPacketsPerSecond = m_packetCount / m_measurementTimeSeconds;
            m_totalErrorsPerSecond = m_errorMessageCount / m_measurementTimeSeconds;
            m_totalBytesPerSecond = m_totalByteCount / m_measurementTimeSeconds;
        }

        public void Clear() {
            m_packetCount = 0;
            m_validPacketCount = 0;
            m_invalidPacketCount = 0;
            m_errorMessageCount = 0;
            m_totalByteCount = 0;
            m_measurementTimeSeconds = 0;
            m_totalPacketsPerSecond = 0;
            m_totalErrorsPerSecond = 0;
            m_totalBytesPerSecond = 0;
        }

        public void print() {
            Console.WriteLine();
            Console.WriteLine("Measurement time: " + string.Format("{0:0.000}", m_measurementTimeSeconds) + " seconds");
            Console.WriteLine("Total packets: " + m_packetCount);
            Console.WriteLine("Valid Packets: " + m_validPacketCount);
            Console.WriteLine("Invalid Packets: " + m_validPacketCount);
            Console.WriteLine("Errors: " + m_errorMessageCount);
            Console.WriteLine("Total packet rate: " + string.Format("{0:0.000}", m_totalPacketsPerSecond) + " packets/second");
            Console.WriteLine("Total error rate: " + string.Format("{0:0.000}", m_totalErrorsPerSecond) + " errors/second");
            Console.WriteLine("Bytes transferred: " + m_totalByteCount);
            Console.WriteLine("Data rate: " + string.Format("{0:0.000}", m_totalBytesPerSecond) + " bytes/second");
            Console.WriteLine();
        }
    }
}
