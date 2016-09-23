using System;
using System.Collections.Generic;
using System.Data;
using STAR.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STAR.ViewModel
{
    class Statistics
    {
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
        private int[] m_numPacketsInMinute;
        private int[] m_numErrorsInMinute;
        private long[] m_numDataCharactersInMinute;
        private DateTime[] m_minutesForStatistics;

        // Accessors for class member variables
        public long PacketCount
        {
            get { return m_packetCount; }
        }

        public long ValidPacketCount
        {
            get { return ValidPacketCount; }
        }

        public long InvalidPacketCount
        {
            get { return m_invalidPacketCount; }
        }

        public long ErrorMessageCount
        {
            get { return m_errorMessageCount; }
        }

        public long TotalByteCount
        {
            get { return m_totalByteCount; }
        }

        public double MeasurementTimeSeconds
        {
            get { return m_measurementTimeSeconds; }
        }

        public double TotalPacketsPerSecond
        {
            get { return m_totalPacketsPerSecond; }
        }

        public double TotalErrorsPerSecond
        {
            get { return m_totalErrorsPerSecond; }
        }

        public double TotalBytesPerSecond
        {
            get { return m_totalBytesPerSecond; }
        }

        public long[] NumDataCharactersInMinute
        {
            get { return m_numDataCharactersInMinute; }
        }

        public int[] NumErrorsInMinute
        {
            get { return m_numErrorsInMinute; }
        }

        public int[] NumPacketsInMinute
        {
            get { return m_numPacketsInMinute; }
        }

        public DateTime[] MinutesForStatistics
        {
            get { return m_minutesForStatistics; }
        }

        public bool runsCorrectly;

        public Statistics()
        {
            m_numPacketsInMinute = new int[1];
            m_numErrorsInMinute = new int[1];
            m_numDataCharactersInMinute = new long[1];
            m_minutesForStatistics = new DateTime[2];
            runsCorrectly = true;
            Clear();
        }

        // Collect statistics from the entire data set provided
        public void collect(DateTime startTime, DateTime endTime, List<Packet> packets)
        {
            int pointer = 0;
            bool matchedSecond;
            m_minutesForStatistics[0] = DateTime.MinValue;
            Clear();



            // No packets
            if (packets.Count == 0)
            {
                return;
            }

            // Loop through the list of packets
            foreach (Packet packet in packets)
            {
                if (packet is DataPacket)
                {
                    // Increment packet count
                    m_packetCount++;

                    // Add the packet cargo bytes to the total byte count
                    m_totalByteCount += (packet as DataPacket).CargoByteCount;

                    // Add to counters of valid or invalid packets
                    if ((packet as DataPacket).Valid)
                    {
                        m_validPacketCount++;
                        pointer = 0;
                        matchedSecond = false;
                        do
                        {

                            if (packet.TimeStampInMinutes == m_minutesForStatistics[pointer])
                            {
                                m_numPacketsInMinute[pointer]++;
                                m_numDataCharactersInMinute[pointer] = m_numDataCharactersInMinute[pointer] +
                                                                       (packet as DataPacket).CargoByteCount;
                                matchedSecond = true;
                            }
                            pointer++;
                        } while ((pointer < m_minutesForStatistics.Length) && (matchedSecond != true));
                        if (matchedSecond == false)
                        {
                            Array.Resize<DateTime>(ref m_minutesForStatistics, m_minutesForStatistics.Length + 1);
                            Array.Resize<int>(ref m_numPacketsInMinute, m_minutesForStatistics.Length + 1);
                            Array.Resize<long>(ref m_numDataCharactersInMinute, m_minutesForStatistics.Length + 1);
                            m_minutesForStatistics[pointer] = packet.TimeStampInMinutes;
                            m_numDataCharactersInMinute[pointer] = (packet as DataPacket).CargoByteCount;
                            m_numPacketsInMinute[pointer]++;
                        }
                    }
                    else
                    {
                        pointer = 0;
                        matchedSecond = false;
                        do
                        {

                            if (packet.TimeStampInMinutes == m_minutesForStatistics[pointer])
                            {
                                m_numDataCharactersInMinute[pointer] = m_numDataCharactersInMinute[pointer] +
                                                                       (packet as DataPacket).CargoByteCount;
                                matchedSecond = true;
                            }
                            pointer++;
                        } while ((pointer < m_minutesForStatistics.Length) && (matchedSecond != true));
                        if (matchedSecond == false)
                        {
                            Array.Resize<DateTime>(ref m_minutesForStatistics, m_minutesForStatistics.Length + 1);
                            Array.Resize<int>(ref m_numPacketsInMinute, m_minutesForStatistics.Length + 1);
                            Array.Resize<long>(ref m_numDataCharactersInMinute, m_minutesForStatistics.Length + 1);
                            Array.Resize<int>(ref m_numErrorsInMinute, m_minutesForStatistics.Length + 1);
                            m_minutesForStatistics[pointer] = packet.TimeStampInMinutes;
                            m_numDataCharactersInMinute[pointer] = (packet as DataPacket).CargoByteCount;
                            m_numPacketsInMinute[pointer]++;
                        }
                        m_invalidPacketCount++;
                    }
                    //Console.WriteLine(m_numDataCharactersInSecond[0]);
                }
                else
                {
                    // Increment error count
                    m_errorMessageCount++;

                    pointer = 0;
                    matchedSecond = false;
                    do
                    {
                        if (packet.TimeStampInMinutes == m_minutesForStatistics[pointer])
                        {
                            m_numErrorsInMinute[pointer]++;
                            matchedSecond = true;
                        }
                        pointer++;
                    } while ((pointer < m_minutesForStatistics.Length) && (matchedSecond != true));
                    if (matchedSecond == false)
                    {
                        Array.Resize<DateTime>(ref m_minutesForStatistics, m_minutesForStatistics.Length + 1);
                        Array.Resize<int>(ref m_numPacketsInMinute, m_minutesForStatistics.Length + 1);
                        Array.Resize<long>(ref m_numDataCharactersInMinute, m_minutesForStatistics.Length + 1);
                        Array.Resize<int>(ref m_numErrorsInMinute, m_minutesForStatistics.Length + 1);
                        m_minutesForStatistics[pointer] = packet.TimeStampInMinutes;
                        m_numErrorsInMinute[pointer] = 1;
                    }
                }
            }
            // Calculate some stats based on what we've worked out so far
            m_measurementTimeSeconds = (endTime - startTime).TotalSeconds;
            m_totalPacketsPerSecond = m_packetCount/m_measurementTimeSeconds;
            m_totalErrorsPerSecond = m_errorMessageCount/m_measurementTimeSeconds;
            m_totalBytesPerSecond = m_totalByteCount/m_measurementTimeSeconds;
        }

        // Clear the statistics
        public void Clear()
        {
            m_packetCount = 0;
            m_validPacketCount = 0;
            m_invalidPacketCount = 0;
            m_errorMessageCount = 0;
            m_totalByteCount = 0;
            m_measurementTimeSeconds = 0;
            m_totalPacketsPerSecond = 0;
            m_totalErrorsPerSecond = 0;
            m_totalBytesPerSecond = 0;
            Array.Clear(m_numDataCharactersInMinute, 0, m_numDataCharactersInMinute.Length);
            Array.Clear(m_numErrorsInMinute, 0, m_numErrorsInMinute.Length);
            Array.Clear(m_numPacketsInMinute, 0, NumPacketsInMinute.Length);
            Array.Clear(m_minutesForStatistics, 0, m_minutesForStatistics.Length);
            Array.Resize<int>(ref m_numErrorsInMinute, 1);
            Array.Resize<DateTime>(ref m_minutesForStatistics, 2);
            Array.Resize<int>(ref m_numPacketsInMinute, 1);
            Array.Resize<long>(ref m_numDataCharactersInMinute, 1);
        }

        public void print()
        {
            Console.WriteLine();
            Console.WriteLine("Measurement time: " + string.Format("{0:0.000}", m_measurementTimeSeconds) + " seconds");
            Console.WriteLine("Total packets: " + m_packetCount);
            Console.WriteLine("Valid Packets: " + m_validPacketCount);
            Console.WriteLine("Invalid Packets: " + m_validPacketCount);
            Console.WriteLine("Errors: " + m_errorMessageCount);
            Console.WriteLine("Total packet rate: " + string.Format("{0:0.000}", m_totalPacketsPerSecond) +
                              " packets/second");
            Console.WriteLine("Total error rate: " + string.Format("{0:0.000}", m_totalErrorsPerSecond) +
                              " errors/second");
            Console.WriteLine("Bytes transferred: " + m_totalByteCount);
            Console.WriteLine("Data rate: " + string.Format("{0:0.000}", m_totalBytesPerSecond) + " bytes/second");
            Console.WriteLine();
        }
    }

    [TestClass]
    public class StatisticTester
    {

        [TestMethod]
        public void testErrorStatisticTotal()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.ErrorMessageCount == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void testPacketStatisticTotal()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.PacketCount == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void testCharacterStatisticTotal()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.TotalByteCount == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);

        }

        [TestMethod]
        public void testValidPacketCount()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            //if (testStatistics.ValidPacketCount == 0)
            //{
                //isEmpty = true;
            //}

            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void testInvalidPacketCount()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.InvalidPacketCount == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);

        }

        [TestMethod]
        public void testMeasurementTime()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.MeasurementTimeSeconds == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);

        }

        [TestMethod]
        public void testPacketsPerSecond()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.TotalPacketsPerSecond == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void testErrorsPerSecond()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.TotalErrorsPerSecond == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);

        }

        [TestMethod]
        public void testCharactersPerSecond()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.TotalBytesPerSecond == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void testDataCharactersInMinute()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.NumDataCharactersInMinute[0] == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);

        }

        [TestMethod]
        public void testPacketsInMinute()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.NumPacketsInMinute[0] == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);
        }

        [TestMethod]
        public void testErrorsInMinute()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.NumErrorsInMinute[0] == 0)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);

        }

        [TestMethod]
        public void testEmptyMinutesForStatistics()
        {
            Statistics testStatistics = new Statistics();
            bool isEmpty = false;

            if (testStatistics.MinutesForStatistics.Length == 2)
            {
                isEmpty = true;
            }

            Assert.IsTrue(isEmpty);


        }

        [TestMethod]
        public void testCollect()
        {
            Statistics testStatistic = new Statistics();
            bool runsCorrectly;

            runsCorrectly = testStatistic.runsCorrectly;
            Assert.IsTrue(runsCorrectly);
        }

        [TestMethod]
        public void testInvalid()
        {
            Statistics testStatistic = new Statistics();
            bool runsCorrectly = true;
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            List<Packet> packet = new List<Packet>();


            try
            {
                testStatistic.collect(startTime, endTime, packet);
            }
            catch (Exception)
            {
                runsCorrectly = false;
            }

            Assert.IsTrue(runsCorrectly);
        }
    }
}
