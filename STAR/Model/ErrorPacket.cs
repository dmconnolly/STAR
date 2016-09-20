namespace STAR.Model {
    /*
     * Packet which has a timestamp and error message
     */
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
        public ErrorPacket(byte entryPort, byte exitPort, string dateString, string errorMessage)
                : base(entryPort, exitPort, dateString) {
            this.m_message = errorMessage;
        }
    }
}
