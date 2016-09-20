using STAR.Model;

namespace STAR.ViewModel {
    /*
     * Used as an interface to the list of packets
     * in order to display information on the UI
     */
    class PacketView {
        private const string timeFormat = "dd-MM-yyyy HH:mm:ss.fff";

        private long m_timeTicks;
        private string m_timeString;
        private byte m_entryPort;
        private byte m_exitPort;
        private string m_type;
        private string m_message;
        private string m_endCode;
        private bool m_valid;

        // Accessors for class member variables
        public long TimeTicks    { get { return m_timeTicks;  }}
        public string TimeString { get { return m_timeString; }}
        public byte EntryPort    { get { return m_entryPort;  }}
        public byte ExitPort     { get { return m_exitPort;   }}
        public string PacketType { get { return m_type;       }}
        public string Message    { get { return m_message;    }}
        public string EndCode    { get { return m_endCode;    }}
        public bool Valid        { get { return m_valid;      }}

        // Constructor for PacketView
        // Takes a Packet of any time as a parameter
        // initialises the elements which will be displayed
        // on the GUI
        public PacketView(Packet packet) {
            m_timeTicks = packet.Time;
            m_timeString = packet.Timestamp;
            m_entryPort = packet.EntryPort;
            m_exitPort = packet.ExitPort;

            if(packet is DataPacket) {
                m_type = "Data";
                m_endCode = (packet as DataPacket).EndCode;
                m_valid = (packet as DataPacket).Valid;
            } else {
                m_type = "Error";
                m_message = (packet as ErrorPacket).Message;
            }
        }
    }
}
