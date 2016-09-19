using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class PacketView {
        private const string timeFormat = "dd-MM-yyyy HH:mm:ss.fff";

        private long m_timeTicks;
        private string m_timeString;
        private byte m_sourcePort;
        private byte m_destPort;
        private string m_type;
        private string m_message;
        private string m_endCode;
        private bool m_valid;

        public long TimeTicks    { get { return m_timeTicks;  }}
        public string TimeString { get { return m_timeString; }}
        public byte Source       { get { return m_sourcePort; }}
        public byte Destination  { get { return m_destPort;   }}
        public string Type       { get { return m_type;       }}
        public string Message    { get { return m_message;    }}
        public string EndCode    { get { return m_endCode;    }}
        public bool Valid        { get { return m_valid;      }}

        public PacketView(Packet packet) {
            m_timeTicks = packet.Time;
            m_timeString = packet.Timestamp;
            m_sourcePort = packet.Port;

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
