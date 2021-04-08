namespace Library.Error.Contract
{
    public class ErrorProcessingContract
    {
        public string ClassName { get; set; }
        public string ClassMethodName { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string Input { get; set; }
    }
}