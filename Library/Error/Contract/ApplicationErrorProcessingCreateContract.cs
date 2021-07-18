namespace Library.Application.Contract
{
    public class ApplicationErrorProcessingCreateContract
    {
        public ApplicationErrorProcessingCreateContract(string className, string classMethodName, string exceptionType, string exceptionMessage)
        {
            ClassName = className;
            ClassMethodName = classMethodName;
            ExceptionType = exceptionType;
            ExceptionMessage = exceptionMessage;
        }

        public string ClassName { get; set; }
        public string ClassMethodName { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string? Input { get; set; }
    }
}