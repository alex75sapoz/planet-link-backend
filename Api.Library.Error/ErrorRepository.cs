namespace Api.Library.Error
{
    public interface IErrorRepository
    {

    }

    internal class ErrorRepository : LibraryRepository<ErrorContext>, IErrorRepository
    {
        public ErrorRepository(ErrorContext context) : base(context) { }
    }
}