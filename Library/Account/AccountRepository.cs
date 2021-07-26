using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Account
{
    public interface IAccountRepository
    {
        Task<AccountUserEntity> GetUserAsync(int userId);
        Task<AccountUserEntity> GetUserAsync(int userTypeId, string externalUserId);
        Task<List<AccountUserEntity>> GetUsersAsync();
        Task<AccountUserSessionEntity> GetUserSessionAsync(int userSessionId);
        Task<List<AccountUserSessionEntity>> GetUserSessionsAsync();
        Task<List<AccountUserTypeEntity>> GetUserTypesAsync();
        Task<List<AccountUserGenderEntity>> GetUserGendersAsync();
    }

    class AccountRepository : BaseRepository<AccountContext>, IAccountRepository
    {
        public AccountRepository(AccountContext context) : base(context) { }

        public async Task<List<AccountUserEntity>> GetUsersAsync() =>
            await _context.Users
                .Include(user => user.Google)
                .Include(user => user.Stocktwits)
                .Include(user => user.Fitbit)
                .Include(user => user.UserSessions)
                .ToListAsync();

        public async Task<List<AccountUserTypeEntity>> GetUserTypesAsync() =>
            await _context.UserTypes
                .ToListAsync();

        public async Task<List<AccountUserGenderEntity>> GetUserGendersAsync() =>
            await _context.UserGenders
                .ToListAsync();

        public async Task<AccountUserSessionEntity> GetUserSessionAsync(int userSessionId) =>
            await _context.UserSessions
                .Include(userSession => userSession.User).ThenInclude(user => user.Google)
                .Include(userSession => userSession.User).ThenInclude(user => user.Stocktwits)
                .Include(userSession => userSession.User).ThenInclude(user => user.Fitbit)
                .SingleOrDefaultAsync(userSession => userSession.UserSessionId == userSessionId);

        public async Task<List<AccountUserSessionEntity>> GetUserSessionsAsync() =>
            await _context.UserSessions
                .Include(userSession => userSession.User).ThenInclude(user => user.Google)
                .Include(userSession => userSession.User).ThenInclude(user => user.Stocktwits)
                .Include(userSession => userSession.User).ThenInclude(user => user.Fitbit)
                .ToListAsync();

        public async Task<AccountUserEntity> GetUserAsync(int userId) =>
            await _context.Users
                .Include(user => user.Google)
                .Include(user => user.Stocktwits)
                .Include(user => user.Fitbit)
                .Include(user => user.UserSessions)
                .SingleOrDefaultAsync(user => user.UserId == userId);

        public async Task<AccountUserEntity> GetUserAsync(int userTypeId, string externalUserId) =>
            await _context.Users
                .Include(user => user.Google)
                .Include(user => user.Stocktwits)
                .Include(user => user.Fitbit)
                .Include(user => user.UserSessions)
                .SingleOrDefaultAsync(user =>
                    user.UserTypeId == userTypeId &&
                    user.ExternalUserId == externalUserId
                );
    }
}