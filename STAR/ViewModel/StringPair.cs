namespace STAR.ViewModel {
    public class StringPair {

        public StringPair(string property, string value) {
            Property = property;
            Value = value;
        }

        public string Property { get; set; }
        public string Value { get; set; }
    };
}
