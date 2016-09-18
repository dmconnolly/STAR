namespace STAR {
    class ErrorPacket : Packet {
        private string m_message;

        // Accessor for error string
        public string Message {
            get {
                return m_message;
            }
        }

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        // and a string containing the error packet
        public ErrorPacket(byte port, string dateString, string errorMessage) : base(port, dateString) {
            this.m_message = errorMessage;
        }
    }
}
