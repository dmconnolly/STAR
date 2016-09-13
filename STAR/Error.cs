using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAR {
    class Error : Message {
        private string errorString;

        // Takes date string in the form dd-MM-yyyy HH:mm:ss.FFFF
        // and a string containing the error message
        public Error(string dateString, string errorString) : base(dateString) {
            this.errorString = errorString;
        }

        // Accessor for error string
        public override string ToString() {
            return errorString;
        }
    }
}
