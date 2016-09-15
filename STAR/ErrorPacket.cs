namespace STAR {
    class ErrorPacket : Packet {
        private string errorString;

        // Accessor for error string
        public string Text {
            get {
                return errorString;
            }
        }

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        // and a string containing the error packet
        public ErrorPacket(string dateString, string errorString) : base(dateString) {
            this.errorString = errorString;
        }
    }
}
