using STAR.Model;
using System;

namespace STAR.ViewModel {
    /*
     * Used as an interface to the list of packets
     * in order to display information on the UI
     */
    class PacketView {
        private const string timeFormat = "dd-MM-yyyy HH:mm:ss.fff";

        public long   TimeTicks        { get; private set; }
        public string TimeString       { get; private set; }
        public byte   EntryPort        { get; private set; }
        public byte   ExitPort         { get; private set; }
        public Type   PacketType       { get; private set; }
        public bool   DataPacket       { get; private set; }
        public string PacketTypeString { get; private set; }
        public string Message          { get; private set; }
        public string EndCode          { get; private set; }
        public bool   Valid            { get; private set; }

        public byte   DestinationKey       { get; private set; }
        public byte[] SourcePathAddress    { get; private set; }
        public byte   SourceLogicalAddress { get; private set; }
        public ushort TransactionId        { get; private set; }
        public byte   ExtendedWriteAddress { get; private set; }
        public uint   WriteAddress         { get; private set; }
        public uint   ReadAddress          { get; private set; }
        public uint   DataLength           { get; private set; }
        public byte   HeaderCRC            { get; private set; }
        public byte[] DataBytes            { get; private set; }
        public byte   DataCRC              { get; private set; }
        public byte   Status               { get; private set; }
        public byte   ReplyCRC             { get; private set; }
        public byte   DestinationLogicalAddress { get; private set; }

        // Constructor for PacketView
        // Takes a Packet of any time as a parameter
        // initialises the elements which will be displayed
        // on the GUI
        public PacketView(Packet packet) {
            TimeTicks = packet.Time;
            TimeString = packet.Timestamp;
            EntryPort = packet.EntryPort;
            ExitPort = packet.ExitPort;

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
            EndCode = (packet as DataPacket).EndCode;
            Valid = (packet as DataPacket).Valid;

            if(PacketType == typeof(WriteCommandPacket)) {
                WriteCommandPacket pkt = packet as WriteCommandPacket;
                PacketTypeString = "Write command";
                DestinationKey = pkt.DestinationKey;
                SourcePathAddress = pkt.SourcePathAddress;
                SourceLogicalAddress = pkt.SourceLogicalAddress;
                TransactionId = pkt.TransactionId;
                ExtendedWriteAddress = pkt.ExtendedWriteAddress;
                WriteAddress = pkt.WriteAddress;
                DataLength = pkt.DataLength;
                HeaderCRC = pkt.HeaderCRC;
                DataBytes = pkt.DataBytes;
                DataCRC = pkt.DataCRC;
            } else if(PacketType == typeof(WriteResponsePacket)) {
                WriteResponsePacket pkt = packet as WriteResponsePacket;
                PacketTypeString = "Write response";
                Status = pkt.Status;
                DestinationLogicalAddress = pkt.DestinationLogicalAddress;
                TransactionId = pkt.TransactionId;
                ReplyCRC = pkt.ReplyCRC;
            } else if(PacketType == typeof(ReadCommandPacket)) {
                ReadCommandPacket pkt = packet as ReadCommandPacket;
                PacketTypeString = "Read command";
                DestinationKey = pkt.DestinationKey;
                SourcePathAddress = pkt.SourcePathAddress;
                SourceLogicalAddress = pkt.SourceLogicalAddress;
                TransactionId = pkt.TransactionId;
                ExtendedWriteAddress = pkt.ExtendedWriteAddress;
                ReadAddress = pkt.ReadAddress;
                DataLength = pkt.DataLength;
                HeaderCRC = pkt.HeaderCRC;
            } else {
                // Read response
                ReadResponsePacket pkt = packet as ReadResponsePacket;
                PacketTypeString = "Read response";
                Status = pkt.Status;
                DestinationLogicalAddress = pkt.DestinationLogicalAddress;
                TransactionId = pkt.TransactionId;
                DataLength = pkt.DataLength;
                HeaderCRC = pkt.HeaderCRC;
                DataBytes = pkt.DataBytes;
                DataCRC = pkt.DataCRC;
            }
        }
    }
}
