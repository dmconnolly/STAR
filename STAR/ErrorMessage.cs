using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class ErrorMessage : Message {
        private string errorString;

        // Accessor for error string
        public string Text {
            get {
                return errorString;
            }
        }

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.fff
        // and a string containing the error message
        public ErrorMessage(string dateString, string errorString) : base(dateString) {
            this.errorString = errorString;
        }
    }
}
