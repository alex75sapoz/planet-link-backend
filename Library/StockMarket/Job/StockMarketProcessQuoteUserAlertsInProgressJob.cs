﻿using Library.Base;
using Library.Error;
using Library.Error.Contract;
using Library.StockMarket.Enum;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.StockMarket.Job
{
    internal class StockMarketProcessQuoteUserAlertsInProgressJob : BaseJob
    {
        public StockMarketProcessQuoteUserAlertsInProgressJob(IServiceProvider serviceProvider) : base(serviceProvider, (
            delay: TimeSpan.FromMinutes(30),
            interval: TimeSpan.FromDays(1),
            isDependentOnCache: true
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<StockMarketService>();

            await service.ProcessQuoteUserAlertsReverseSplitsAsync();
            await service.ProcessQuoteUserAlertsInProgressAsync((int)AlertCompletedType.Automatic);
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingContract()
            {
                ClassName = nameof(StockMarketProcessQuotesJob),
                ClassMethodName = nameof(StockMarketProcessQuotesJob.StartAsync),
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.GetFullMessage()
            });
        }

    }
}