namespace Library.Application
{
    class ApplicationRepository : BaseRepository<ApplicationContext>, IApplicationRepository
    {
        public ApplicationRepository(ApplicationContext context) : base(context) { }
    }

    public interface IApplicationRepository
    {

    }
}