using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Library.Error
{
    public interface IErrorService
    {
        Task CreateErrorProcessingAsync(ErrorProcessingCreateContract processing);
        Task CreateErrorRequestAsync(ErrorRequestCreateContract request);
    }

    internal class ErrorService : BaseService<ErrorConfiguration, ErrorRepository>, IErrorService
    {
        public ErrorService(ErrorConfiguration configuration, ErrorRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Create

        public async Task CreateErrorProcessingAsync(ErrorProcessingCreateContract processing)
        {
            try
            {
                await _repository.AddAndSaveChangesAsync(new ErrorEntity()
                {
                    ErrorTypeId = (int)ErrorType.Processing,
                    CreatedOn = DateTimeOffset.Now,
                    Processing = new ErrorProcessingEntity()
                    {
                        ClassName = processing.ClassName,
                        ClassMethodName = processing.ClassMethodName,
                        ExceptionType = processing.ExceptionType,
                        ExceptionMessage = processing.ExceptionMessage,
                        Input = processing.Input
                    }
                });
            }
            catch { }
        }

        public async Task CreateErrorRequestAsync(ErrorRequestCreateContract request)
        {
            try
            {
                await _repository.AddAndSaveChangesAsync(new ErrorEntity()
                {
                    ErrorTypeId = (int)ErrorType.Request,
                    CreatedOn = DateTimeOffset.Now,
                    Request = new ErrorRequestEntity()
                    {
                        Method = request.Method,
                        Path = request.Path,
                        Query = request.Query,
                        StatusCodeId = request.StatusCodeId,
                        ExceptionType = request.ExceptionType,
                        ExceptionMessage = request.ExceptionMessage,
                        TimezoneId = request.TimezoneId,
                        UserSessionId = request.UserSessionId,
                        UserId = request.UserId
                    }
                });
            }
            catch { }
        }

        #endregion
    }
}