using Library.Base;
using Library.User.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.User
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserAsync(int userId);
        Task<UserEntity> GetUserAsync(int userTypeId, string externalUserId);
        Task<List<UserEntity>> GetUsersAsync();
        Task<UserSessionEntity> GetUserSessionAsync(int userSessionId);
        Task<List<UserSessionEntity>> GetUserSessionsAsync();
        Task<List<UserTypeEntity>> GetUserTypesAsync();
    }

    internal class UserRepository : BaseRepository<UserContext>, IUserRepository
    {
        public UserRepository(UserContext context) : base(context) { }

        public async Task<List<UserEntity>> GetUsersAsync() =>
            await _context.Users
                .Include(user => user.Google)
                .Include(user => user.Stocktwits)
                .Include(user => user.Sessions)
                .ToListAsync();

        public async Task<List<UserTypeEntity>> GetUserTypesAsync() =>
            await _context.UserTypes
                .ToListAsync();

        public async Task<UserSessionEntity> GetUserSessionAsync(int userSessionId) =>
            await _context.UserSessions
                .Include(userSession => userSession.User).ThenInclude(user => user.Google)
                .Include(userSession => userSession.User).ThenInclude(user => user.Stocktwits)
                .SingleOrDefaultAsync(userSession => userSession.UserSessionId == userSessionId);

        public async Task<List<UserSessionEntity>> GetUserSessionsAsync() =>
            await _context.UserSessions
                .Include(userSession => userSession.User).ThenInclude(user => user.Google)
                .Include(userSession => userSession.User).ThenInclude(user => user.Stocktwits)
                .ToListAsync();

        public async Task<UserEntity> GetUserAsync(int userId) =>
            await _context.Users
                .Include(user => user.Google)
                .Include(user => user.Stocktwits)
                .Include(user => user.Sessions)
                .SingleOrDefaultAsync(user => user.UserId == userId);

        public async Task<UserEntity> GetUserAsync(int userTypeId, string externalUserId) =>
            await _context.Users
                .Include(user => user.Google)
                .Include(user => user.Stocktwits)
                .Include(user => user.Sessions)
                .SingleOrDefaultAsync(user =>
                    user.UserTypeId == userTypeId &&
                    user.ExternalUserId == externalUserId
                );
    }
}