namespace Library.Error
{
    public interface IErrorRepository
    {

    }

    class ErrorRepository : BaseRepository<ErrorContext>, IErrorRepository
    {
        public ErrorRepository(ErrorContext context) : base(context) { }
    }
}