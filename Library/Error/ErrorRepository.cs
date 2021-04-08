using Library.Base;

namespace Library.Error
{
    public interface IErrorRepository
    {

    }

    internal class ErrorRepository : BaseRepository<ErrorContext>, IErrorRepository
    {
        public ErrorRepository(ErrorContext context) : base(context) { }
    }
}