using Api.Library.Error.Contract;
using Api.Library.Error.Entity;
using Api.Library.Error.Enum;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Api.Library.Error
{
    public interface IErrorService
    {
        Task CreateErrorProcessingAsync(ErrorProcessingContract processing);
        Task CreateErrorRequestAsync(ErrorRequestContract request);
    }

    internal class ErrorService : LibraryService<ErrorConfiguration, ErrorRepository>, IErrorService
    {
        public ErrorService(ErrorConfiguration configuration, ErrorRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Create

        public async Task CreateErrorProcessingAsync(ErrorProcessingContract processing)
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

        public async Task CreateErrorRequestAsync(ErrorRequestContract request)
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