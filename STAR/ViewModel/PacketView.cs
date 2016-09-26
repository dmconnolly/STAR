using Microsoft.VisualStudio.TestTools.UnitTesting;
using STAR.Model;
using System;
using System.Linq;

namespace STAR.ViewModel {
    /*
     * Used as an interface to the list of packets
     * in order to display information on the UI
     */
    class PacketView {
        private const string timeFormatUI = "HH:mm:ss.fff";

        public bool Valid { get; private set; }
        public bool CRCError { get; private set; }
        public bool SequenceIdError { get; private set; }
        public bool DuplicatePacketError { get; private set; }
        public bool DataPacket { get; private set; }
        public Type PacketType { get; private set; }
        public long TimeTicks { get; private set; }

        public string TimeString { get; private set; }
        public string EntryPort { get; private set; }
        public string ExitPort { get; private set; }
        public string PacketTypeString { get; private set; }
        public string Message { get; private set; }
        public string EndCode { get; private set; }
        public string Cargo { get; private set; }
        public string ProtocolId { get; private set; }
        public string DestinationKey { get; private set; }
        public string SequenceId { get; private set; }
        public string SourcePathAddress { get; private set; }
        public string SourceLogicalAddress { get; private set; }
        public string TransactionId { get; private set; }
        public string ExtendedWriteAddress { get; private set; }
        public string WriteAddress { get; private set; }
        public string ExtendedReadAddress { get; private set; }
        public string ReadAddress { get; private set; }
        public string DataLength { get; private set; }
        public string HeaderCRC { get; private set; }
        public string DataBytes { get; private set; }
        public string DataCRC { get; private set; }
        public string Status { get; private set; }
        public string ReplyCRC { get; private set; }
        public string DestinationPathAddress { get; private set; }
        public string DestinationLogicalAddress { get; private set; }

        // Constructor for PacketView
        // Takes a Packet of any time as a parameter
        // initialises the elements which will be displayed
        // on the GUI
        public PacketView(Packet packet) {
            TimeTicks = packet.TimeStamp.Ticks;
            TimeString = string.Format("{0:" + timeFormatUI + "}", packet.TimeStamp);
            EntryPort = byteToString(packet.EntryPort);
            ExitPort = byteToString(packet.ExitPort);

            PacketType = packet.GetType();

            if(PacketType == typeof(ErrorPacket)) {
                // Error packet
                PacketTypeString = "Error";
                DataPacket = false;
                Message = (packet as ErrorPacket).Message;
                return;
            }

            // Common to all data packets
            DataPacket = true;
            ProtocolId = byteToString((packet as DataPacket).Protocol);
            EndCode = (packet as DataPacket).EndCode;
            Valid = (packet as DataPacket).Valid;
            Message = Valid ? "" : "No EOP";

            SequenceIdError = (packet as DataPacket).SequenceIdError;
            if(SequenceIdError) {
                Message = "Out of sequence";
                Valid = false;
            }

            DuplicatePacketError = (packet as DataPacket).DuplicatePacketError;
            if(DuplicatePacketError) {
                Message = "Duplicate";
                Valid = false;
            }

            CRCError = (packet as DataPacket).CRCError;
            if(CRCError) {
                Message = "CRC Error";
                Valid = false;
            }

            if(PacketType == typeof(NonRmapPacket)) {
                NonRmapPacket pkt = packet as NonRmapPacket;
                PacketTypeString = "Non RMAP";
                DestinationPathAddress = byteToString(pkt.PathAddress);
                DestinationLogicalAddress = byteToString(pkt.LogicalAddress);
                SequenceId = byteToString(pkt.SequenceNumber);
                Cargo = byteToString(pkt.Cargo);
            } else if(PacketType == typeof(WriteCommandPacket)) {
                WriteCommandPacket pkt = packet as WriteCommandPacket;
                PacketTypeString = "Write command";
                DestinationPathAddress = byteToString(pkt.PathAddress);
                DestinationLogicalAddress = byteToString(pkt.LogicalAddress);
                DestinationKey = byteToString(pkt.DestinationKey);
                SourcePathAddress = byteToString(pkt.SourcePathAddress);
                SourceLogicalAddress = byteToString(pkt.SourceLogicalAddress);
                TransactionId = byteToString(pkt.TransactionId);
                ExtendedWriteAddress = byteToString(pkt.ExtendedWriteAddress);
                WriteAddress = byteToString(pkt.WriteAddress);
                DataLength = byteToString(pkt.DataLength);
                HeaderCRC = byteToString(pkt.HeaderCRC);
                DataBytes = byteToString(pkt.DataBytes);
                DataCRC = byteToString(pkt.DataCRC);
            } else if(PacketType == typeof(WriteReplyPacket)) {
                WriteReplyPacket pkt = packet as WriteReplyPacket;
                PacketTypeString = "Write reply";
                SourcePathAddress = byteToString(pkt.PathAddress);
                SourceLogicalAddress = byteToString(pkt.LogicalAddress);
                Status = byteToString(pkt.Status);
                DestinationLogicalAddress = byteToString(pkt.DestinationLogicalAddress);
                TransactionId = byteToString(pkt.TransactionId);
                ReplyCRC = byteToString(pkt.ReplyCRC);
            } else if(PacketType == typeof(ReadCommandPacket)) {
                ReadCommandPacket pkt = packet as ReadCommandPacket;
                PacketTypeString = "Read command";
                DestinationPathAddress = byteToString(pkt.PathAddress);
                DestinationLogicalAddress = byteToString(pkt.LogicalAddress);
                DestinationKey = byteToString(pkt.DestinationKey);
                SourcePathAddress = byteToString(pkt.SourcePathAddress);
                SourceLogicalAddress = byteToString(pkt.SourceLogicalAddress);
                TransactionId = byteToString(pkt.TransactionId);
                ExtendedReadAddress = byteToString(pkt.ExtendedReadAddress);
                ReadAddress = byteToString(pkt.ReadAddress);
                DataLength = byteToString(pkt.DataLength);
                HeaderCRC = byteToString(pkt.HeaderCRC);
            } else {
                ReadReplyPacket pkt = packet as ReadReplyPacket;
                PacketTypeString = "Read reply";
                SourcePathAddress = byteToString(pkt.PathAddress);
                SourceLogicalAddress = byteToString(pkt.LogicalAddress);
                Status = byteToString(pkt.Status);
                DestinationLogicalAddress = byteToString(pkt.DestinationLogicalAddress);
                TransactionId = byteToString(pkt.TransactionId);
                DataLength = byteToString(pkt.DataLength);
                HeaderCRC = byteToString(pkt.HeaderCRC);
                DataBytes = byteToString(pkt.DataBytes);
                DataCRC = byteToString(pkt.DataCRC);
            }
        }
        
        // Method to convert byte array to string
        private string byteToString(byte[] bytes) {
            string tmp = bytes == null ? "" : string.Concat(bytes.Select(b => b.ToString("x2")));
            if(tmp.Length > 2) {
                tmp = tmp.TrimStart('0');
            }
            if(tmp.Length % 2 != 0) {
                tmp = "0" + tmp;
            }
            return tmp;
        }

        // Overload for single byte
        private string byteToString(byte b) {
            return b.ToString("x2");
        }

        // Overload for uint
        private string byteToString(uint val) {
            byte[] bytes = BitConverter.GetBytes(val);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
            return byteToString(bytes);
        }

        // Overload for ushort
        private string byteToString(ushort val) {
            byte[] bytes = BitConverter.GetBytes(val);
            if(BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
            return byteToString(bytes);
        }
    }

    [TestClass]
    public class PacketViewTester {
        [TestMethod]
        public void testVariableGetters() {
            Packet testPacket = new Packet(1, 2, "09-09-2016 00:35:07.223");
            PacketView testPacketView = new PacketView(testPacket);

            bool isAccurate = false;

            if(testPacketView.CRCError == false && testPacketView.Valid == false && testPacketView.DataPacket == false) {
                isAccurate = true;
            }

            Assert.IsTrue(isAccurate);
        }
    }
}
