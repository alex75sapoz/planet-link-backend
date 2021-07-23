using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Library.Application
{
    public interface IApplicationService
    {
        Task CreateErrorProcessingAsync(ApplicationErrorProcessingCreateContract processing);
        Task CreateErrorRequestAsync(ApplicationErrorRequestCreateContract request);
    }

    class ApplicationService : BaseService<ApplicationConfiguration, ApplicationRepository>, IApplicationService
    {
        public ApplicationService(ApplicationConfiguration configuration, ApplicationRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Create

        public async Task CreateErrorProcessingAsync(ApplicationErrorProcessingCreateContract errorProcessing)
        {
            try
            {
                await _repository.AddAndSaveChangesAsync(new ApplicationErrorEntity()
                {
                    ErrorTypeId = (int)ErrorType.Processing,
                    CreatedOn = DateTimeOffset.Now,
                    Processing = new ApplicationErrorProcessingEntity()
                    {
                        ClassName = errorProcessing.ClassName,
                        ClassMethodName = errorProcessing.ClassMethodName,
                        ExceptionType = errorProcessing.ExceptionType,
                        ExceptionMessage = errorProcessing.ExceptionMessage,
                        Input = errorProcessing.Input
                    }
                });
            }
            catch { }
        }

        public async Task CreateErrorRequestAsync(ApplicationErrorRequestCreateContract errorRequest)
        {
            try
            {
                await _repository.AddAndSaveChangesAsync(new ApplicationErrorEntity()
                {
                    ErrorTypeId = (int)ErrorType.Request,
                    CreatedOn = DateTimeOffset.Now,
                    Request = new ApplicationErrorRequestEntity()
                    {
                        Method = errorRequest.Method,
                        Path = errorRequest.Path,
                        Query = errorRequest.Query,
                        StatusCodeId = errorRequest.StatusCodeId,
                        ExceptionType = errorRequest.ExceptionType,
                        ExceptionMessage = errorRequest.ExceptionMessage,
                        TimezoneId = errorRequest.TimezoneId,
                        UserSessionId = errorRequest.UserSessionId,
                        UserId = errorRequest.UserId
                    }
                });
            }
            catch { }
        }

        #endregion
    }
}