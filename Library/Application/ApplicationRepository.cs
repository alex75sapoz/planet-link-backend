namespace Library.Application
{
    public interface IApplicationRepository
    {

    }

    class ApplicationRepository : BaseRepository<ApplicationContext>, IApplicationRepository
    {
        public ApplicationRepository(ApplicationContext context) : base(context) { }
    }
}