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

            //Untested code for comparing data obtained to a given baseline in order to find issues with the data 
            //i.e. if there are more errors than usual, or if there is less data characters generated than usual.
            //For statistic quality: 1 is good, 2 is normal, 3 is bad.

            //private int[] m_statisticQuality;
            //private float m_percentageValid;
            //private float m_percentageInvalid;
            //private float m_percentageError;
            //public int[] StatisticQuality {get {return m_statisticQuality;}}
            //public float PercentageValid {get {return m_percentageValid;}}
            //public float PercentageInvalid {get {return m_percentageInvalid;}}
            //public float PercentageError {get {return m_percentageError;}}

            //m_percentageValid = (m_validPacketCount / m_packetCount) * 100; 
            //m_percentageInvalid = (m_invalidPakcetCount / m_packetCount) * 100;
            //m_percentageError = (m_errorMessageCount / m_packetCount) * 100;
            //Console.WriteLine("Valid %: {0}, Invalid %: {1}, Error %: {2}");

            //if statement for statistic quality of packets generated
            //if (m_packetCount > 100)
            //{ 
            //  m_statisticQuality[0] = 1; Console.WriteLine("A lot of packets were generated");
            //} else if (m_packetCount > 50)
            //{
            //  m_statisticQuality[0] = 2; Console.WriteLine("A reasonable amount of packets were generated");
            //} else
            //{
            //  m_statisticQuality[0] = 3;  Console.WriteLine("Not a lot of packets were generated. This is likely due to an early disconnection in the files presented.");
            //}

            //if statement for quality of packets
            //if (m_percentageValid > 

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
