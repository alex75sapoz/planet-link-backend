using Library.Error;
using Library.Error.Contract;
using Library.User;
using Library.User.Enum;
using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    public interface IStockMarketService
    {
        Task<StockMarketQuoteUserAlertContract> CompleteQuoteUserAlertAsync(int userId, int quoteUserAlertId);
        Task<StockMarketQuoteUserAlertContract> CreateQuoteUserAlertAsync(int userId, int quoteId, (decimal sell, decimal stopLoss) alert, DateTimeZone timezone);
        Task<StockMarketQuoteUserEmotionContract> CreateQuoteUserEmotionAsync(int userId, int quoteId, int emotionId, DateTimeZone timezone);
        StockMarketConfigurationContract GetConfiguration();
        StockMarketEmotionContract GetEmotion(int emotionId);
        Task<StockMarketGlobalContract> GetGlobalAsync();
        StockMarketQuoteContract GetQuote(int quoteId);
        Task<List<StockMarketQuoteCandleContract>> GetQuoteCandlesAsync(int quoteId, int timeframeId);
        Task<StockMarketQuoteCompanyContract> GetQuoteCompanyAsync(int quoteId);
        List<StockMarketQuoteEmotionCountContract> GetQuoteEmotionCounts(int quoteId, DateTimeZone timezone);
        Task<StockMarketQuotePriceContract> GetQuotePriceAsync(int quoteId);
        Task<List<StockMarketQuoteReverseSplitContract>> GetQuoteReverseSplitsAsync(int quoteId);
        StockMarketQuoteUserConfigurationContract GetQuoteUserConfiguration(int userId, int quoteId, DateTimeZone timezone);
        StockMarketTimeframeContract GetTimeframe(int timeframeId);
        StockMarketUserContract GetUser(int userId);
        List<StockMarketQuoteContract> SearchQuotesAsync(string keyword);
        List<StockMarketQuoteUserAlertContract> SearchQuoteUserAlerts(int alertTypeId, int? userId, int? quoteId);
    }

    class StockMarketService : BaseService<StockMarketConfiguration, StockMarketRepository>, IStockMarketService
    {
        public StockMarketService(StockMarketConfiguration configuration, StockMarketRepository repository, IUserService userService, IErrorService errorService, IMemoryCache memoryCache) : base(configuration, repository, memoryCache)
        {
            _financialModelingPrepApi = new RestClient(_configuration.FinancialModelingPrepApi.Server);
            _userService = userService;
            _errorService = errorService;
        }

        private readonly IRestClient _financialModelingPrepApi;
        private readonly IUserService _userService;
        private readonly IErrorService _errorService;

        #region Search

        public List<StockMarketQuoteContract> SearchQuotesAsync(string keyword) =>
            StockMarketMemoryCache.StockMarketQuotes
                .Where(quote => quote.Value.Symbol.StartsWith(keyword))
                .OrderBy(quote => quote.Value.Symbol.Length)
                .Take(_configuration.Limit.SearchQuotesLimit)
                .Select(quote => quote.Value)
                .ToList();

        public List<StockMarketQuoteUserAlertContract> SearchQuoteUserAlerts(int alertTypeId, int? userId, int? quoteId)
        {
            if (userId.HasValue && _userService.GetUser(userId.Value).UserTypeId != (int)UserType.Stocktwits)
                throw new BadRequestException($"{nameof(userId)} is not of {nameof(UserType.Stocktwits)} type");

            return StockMarketMemoryCache.StockMarketQuoteUserAlerts
                .Where(quoteUserAlert =>
                    (quoteUserAlert.Value.AlertTypeId == alertTypeId) &&
                    (!userId.HasValue || quoteUserAlert.Value.UserId == userId) &&
                    (!quoteId.HasValue || quoteUserAlert.Value.QuoteId == quoteId)
                )
                .Select(quoteUserAlert => quoteUserAlert.Value)
                .OrderByDescending(quoteUserAlert => quoteUserAlert.CreatedOn)
                .Take(_configuration.Limit.SearchQuoteUserAlertsLimit)
                .ToList();
        }

        #endregion

        #region Get

        public StockMarketQuoteContract GetQuote(int quoteId) =>
            StockMarketMemoryCache.StockMarketQuotes.TryGetValue(quoteId, out StockMarketQuoteContract? quote)
                ? quote
                : throw new BadRequestException($"{nameof(quoteId)} is invalid");

        public StockMarketEmotionContract GetEmotion(int emotionId) =>
            StockMarketMemoryCache.StockMarketEmotions.TryGetValue(emotionId, out StockMarketEmotionContract? emotion)
                ? emotion
                : throw new BadRequestException($"{nameof(emotionId)} is invalid");

        public StockMarketTimeframeContract GetTimeframe(int timeframeId) =>
            StockMarketMemoryCache.StockMarketTimeframes.TryGetValue(timeframeId, out StockMarketTimeframeContract? timeframe)
                ? timeframe
                : throw new BadRequestException($"{nameof(timeframeId)} is invalid");

        public async Task<StockMarketGlobalContract> GetGlobalAsync()
        {
            var memoryCacheKey = $"{{StockMarket}}.{{StockMarketGlobalContract}}";

            if (_memoryCache.TryGetValue(memoryCacheKey, out StockMarketGlobalContract global))
                return global;

            return _memoryCache.Set(
                memoryCacheKey,
                (await GetGlobalResponseAsync())
                    .MapToGlobalContract(),
                TimeSpan.FromSeconds(_configuration.Duration.GlobalCacheDurationInSeconds)
            );
        }

        public async Task<StockMarketQuotePriceContract> GetQuotePriceAsync(int quoteId)
        {
            var memoryCacheKey = $"{{StockMarket}}.{{StockMarketQuotePriceContract}}.{{QuoteId}}={quoteId}";

            if (_memoryCache.TryGetValue(memoryCacheKey, out StockMarketQuotePriceContract quotePrice))
                return quotePrice;

            var quote = GetQuote(quoteId);

            return _memoryCache.Set(
                memoryCacheKey,
                (await GetQuotePriceResponseAsync(quote.Symbol))
                    .MapToQuotePriceContract(),
                TimeSpan.FromSeconds(_configuration.Duration.QuotePriceCacheDurationIsSeconds)
            );
        }

        public async Task<StockMarketQuoteCompanyContract> GetQuoteCompanyAsync(int quoteId)
        {
            var memoryCacheKey = $"{{StockMarket}}.{{StockMarketQuoteCompanyContract}}.{{QuoteId}}={quoteId}";

            if (_memoryCache.TryGetValue(memoryCacheKey, out StockMarketQuoteCompanyContract quoteCompany))
                return quoteCompany;

            var quote = GetQuote(quoteId);

            return _memoryCache.Set(
                memoryCacheKey,
                (await GetQuoteCompanyResponseAsync(quote.Symbol))
                    .MapToQuoteCompanyContract(),
                TimeSpan.FromSeconds(_configuration.Duration.QuoteCompanyCacheDurationInSeconds)
            );
        }

        public async Task<List<StockMarketQuoteCandleContract>> GetQuoteCandlesAsync(int quoteId, int timeframeId)
        {
            var memoryCacheKey = $"{{StockMarket}}.{{List<StockMarketQuoteCandleContract>}}.{{QuoteId}}={quoteId}.{{TimeframeId}}={timeframeId}";

            if (_memoryCache.TryGetValue(memoryCacheKey, out List<StockMarketQuoteCandleContract> quoteCandles))
                return quoteCandles;

            var quote = GetQuote(quoteId);
            var timeframe = GetTimeframe(timeframeId);

            //How does this code work?
            //Every timeframe is 1 multiplier
            //A multiplier increases if the timeframe needs to be extended further
            //Example: OneDay, Multiplier = 3  <= This will return at most 3 days worth of 1 minute data because the timeframe is multiplied by 3

            quoteCandles = new List<StockMarketQuoteCandleContract>();
            var quoteCandlesResponse = new List<StockMarketQuoteCandleResponse>();
            Func<(int currentSet, DateTime setStartDate, DateTime setEndDate, DateTime timeframeMultiplierStartDate), bool> timeframeMultiplierCheck;

            //Setup goes here
            switch (timeframe.TimeframeId)
            {
                case (int)Timeframe.OneDay:
                    {
                        quoteCandlesResponse = await GetQuoteCandlesTimeframeResponseAsync(quote.Symbol, "1min");
                        timeframeMultiplierCheck = (data) => data.currentSet % 1 == 0;
                        break;
                    }
                case (int)Timeframe.FiveDay:
                    {
                        quoteCandlesResponse = await GetQuoteCandlesTimeframeResponseAsync(quote.Symbol, "5min");
                        timeframeMultiplierCheck = (data) => data.currentSet % 5 == 0;
                        break;
                    }
                case (int)Timeframe.OneMonth:
                    {
                        quoteCandlesResponse = await GetQuoteCandlesTimeframeResponseAsync(quote.Symbol, "30min");
                        timeframeMultiplierCheck = (data) => Period.Between(LocalDate.FromDateTime(data.setEndDate), LocalDate.FromDateTime(data.timeframeMultiplierStartDate)).Months >= 1;
                        break;
                    }
                case (int)Timeframe.OneYear:
                    {
                        var to = DateTimeOffset.Now.AtTimezone(BaseExtension.EasternTimezone).Date;
                        to = to.DayOfWeek switch
                        {
                            DayOfWeek.Saturday => to.AddDays(-1),
                            DayOfWeek.Sunday => to.AddDays(-2),
                            _ => to
                        };
                        var from = to.AddYears(-1 * timeframe.Multiplier);

                        quoteCandlesResponse = await GetQuoteCandlesDailyResponseAsync(quote.Symbol, from, to);
                        timeframeMultiplierCheck = (data) => Period.Between(LocalDate.FromDateTime(data.setEndDate), LocalDate.FromDateTime(data.timeframeMultiplierStartDate)).Years >= 1;
                        break;
                    }
                case (int)Timeframe.FiveYear:
                    {
                        var to = DateTimeOffset.Now.AtTimezone(BaseExtension.EasternTimezone).Date;
                        to = to.DayOfWeek switch
                        {
                            DayOfWeek.Saturday => to.AddDays(-1),
                            DayOfWeek.Sunday => to.AddDays(-2),
                            _ => to
                        };
                        var from = to.AddYears(-5 * timeframe.Multiplier);

                        quoteCandlesResponse = await GetQuoteCandlesDailyResponseAsync(quote.Symbol, from, to);
                        timeframeMultiplierCheck = (data) => Period.Between(LocalDate.FromDateTime(data.setEndDate), LocalDate.FromDateTime(data.timeframeMultiplierStartDate)).Years >= 5;
                        break;
                    }
                default: throw new BadRequestException($"{nameof(timeframeId)} is currently not supported");
            }

            //Everything under here is dynamic, no matter the timeframe
            var quoteUserAlerts = StockMarketMemoryCache.StockMarketQuoteUserAlerts
                                           .Where(quoteUserAlert =>
                                               quoteUserAlert.Value.QuoteId == quote.QuoteId &&
                                               quoteUserAlert.Value.CreatedOn >= quoteCandlesResponse[^1].CreatedOn &&
                                               quoteUserAlert.Value.CreatedOn <= quoteCandlesResponse[0].CreatedOn
                                           )
                                           .Select(quoteUserAlert => quoteUserAlert.Value)
                                           .ToList();

            var currentTimeframeMultiplier = 1;
            var currentSet = 1;

            DateTime? currentTimeframeMultiplierStartDate = null;
            StockMarketQuoteCandleResponse? lastCandleFromPreviousSet = null;

            while (quoteCandlesResponse.Any() && currentTimeframeMultiplier <= timeframe.Multiplier)
            {
                if (currentTimeframeMultiplierStartDate is null)
                    currentTimeframeMultiplierStartDate = quoteCandlesResponse[0].CreatedOn.Date;

                var quoteCandleResponseSet = quoteCandlesResponse.Where(quoteCandleResponse => quoteCandleResponse.CreatedOn.Date == quoteCandlesResponse[0].CreatedOn.Date)
                                                                 .ToList();
                var quoteUserAlertsSet = quoteUserAlerts.Where(quoteUserAlert => quoteUserAlert.CreatedOn.AtTimezone(BaseExtension.EasternTimezone).Date == quoteCandlesResponse[0].CreatedOn.Date)
                                                        .ToList();

                //Response candles are ordered in descending order
                for (int index = 0; index < quoteCandleResponseSet.Count - 1; index++)
                {
                    var currentQuoteCandleResponse = quoteCandleResponseSet[index];  //Example: 09:32 | 09:40 | 10:00
                    var nextQuoteCandleResponse = quoteCandleResponseSet[index + 1]; //Example: 09:31 | 09:35 | 09:30

                    quoteCandles.Insert(0, currentQuoteCandleResponse.MapToQuoteCandleContract(
                        quoteUserAlertsSet.Where(quoteUserAlert =>
                                              quoteUserAlert.CreatedOn <= currentQuoteCandleResponse.CreatedOn &&
                                              quoteUserAlert.CreatedOn > nextQuoteCandleResponse.CreatedOn
                                          )
                                          .ToList(),
                        currentTimeframeMultiplier
                    ));
                }

                //Add last candle
                quoteCandles.Insert(0, quoteCandleResponseSet[^1].MapToQuoteCandleContract(
                    lastCandleFromPreviousSet is not null
                        ? quoteUserAlertsSet.Where(quoteUserAlert =>
                                              quoteUserAlert.CreatedOn <= lastCandleFromPreviousSet.CreatedOn &&
                                              quoteUserAlert.CreatedOn > quoteCandleResponseSet[0].CreatedOn
                                            )
                                            .ToList()
                        : new List<StockMarketQuoteUserAlertContract>(),
                    currentTimeframeMultiplier
                ));

                if (timeframeMultiplierCheck((currentSet, quoteCandleResponseSet[0].CreatedOn.Date, quoteCandleResponseSet[^1].CreatedOn.Date, currentTimeframeMultiplierStartDate.Value)))
                {
                    currentTimeframeMultiplier++;
                    currentTimeframeMultiplierStartDate = null;
                }

                currentSet++;
                lastCandleFromPreviousSet = quoteCandleResponseSet[^1];
                quoteCandlesResponse.RemoveRange(0, quoteCandleResponseSet.Count);
            }

            return _memoryCache.Set(
                memoryCacheKey,
                quoteCandles,
                TimeSpan.FromSeconds(_configuration.Duration.QuoteCandlesCacheDurationInSeconds)
            );
        }

        public async Task<List<StockMarketQuoteReverseSplitContract>> GetQuoteReverseSplitsAsync(int quoteId)
        {
            var memoryCacheKey = $"{{StockMarket}}.{{List<StockMarketQuoteReverseSplitContract>}}.{{QuoteId}}={quoteId}";

            if (_memoryCache.TryGetValue(memoryCacheKey, out List<StockMarketQuoteReverseSplitContract> quoteReverseSplits))
                return quoteReverseSplits;

            var quote = GetQuote(quoteId);

            return _memoryCache.Set(
                memoryCacheKey,
                (await GetQuoteReverseSplitsResponseAsync(quote.Symbol))
                    .Select(quoteReverseSplit => quoteReverseSplit.MapToReverseSplitContract())
                    .OrderBy(quoteReverseSplit => quoteReverseSplit.CreatedOn)
                    .ToList(),
                TimeSpan.FromSeconds(_configuration.Duration.QuoteReverseSplitsCacheDurationInSeconds)
            );
        }

        public List<StockMarketQuoteEmotionCountContract> GetQuoteEmotionCounts(int quoteId, DateTimeZone timezone)
        {
            var quote = GetQuote(quoteId);

            return StockMarketMemoryCache.StockMarketQuoteUserEmotions.GetQuoteUserEmotionsAtTimezoneToday(timezone)
                .GroupBy(quoteUserEmotion => quoteUserEmotion.EmotionId)
                .Select(quoteUserEmotionGroup => new StockMarketQuoteEmotionCountContract()
                {
                    EmotionId = quoteUserEmotionGroup.Key,
                    QuoteCount = quoteUserEmotionGroup.Where(quoteUserEmotion => quoteUserEmotion.QuoteId == quote.QuoteId).Count(),
                    GlobalCount = quoteUserEmotionGroup.Count()
                })
                .ToList();
        }

        public StockMarketQuoteUserConfigurationContract GetQuoteUserConfiguration(int userId, int quoteId, DateTimeZone timezone)
        {
            var user = _userService.GetUser(userId);
            var quote = GetQuote(quoteId);

            var quoteUserEmotions = StockMarketMemoryCache.StockMarketQuoteUserEmotions.GetQuoteUserEmotionsAtTimezoneToday(timezone, userId);

            return new StockMarketQuoteUserConfigurationContract()
            {
                EmotionId = quoteUserEmotions.SingleOrDefault(quoteUserEmotion => quoteUserEmotion.QuoteId == quote.QuoteId)?.EmotionId,
                SelectionsToday = quoteUserEmotions.Count,
                LimitToday = _configuration.Limit.CreateQuoteUserEmotionLimit
            };
        }

        public StockMarketConfigurationContract GetConfiguration() => new()
        {
            AlertTypes = StockMarketMemoryCache.StockMarketAlertTypes.Select(alertType => alertType.Value).ToList(),
            AlertCompletedTypes = StockMarketMemoryCache.StockMarketAlertCompletedTypes.Select(alertCompletedType => alertCompletedType.Value).ToList(),
            Emotions = StockMarketMemoryCache.StockMarketEmotions.Select(emotion => emotion.Value).ToList(),
            Exchanges = StockMarketMemoryCache.StockMarketExchanges.Select(exchange => exchange.Value).ToList(),
            Timeframes = StockMarketMemoryCache.StockMarketTimeframes.Select(timeframe => timeframe.Value).ToList(),
            QuoteUserAlertRequirement = new StockMarketQuoteUserAlertRequirementConfigurationContract()
            {
                MinimumFollowersCount = _configuration.Requirement.CreateQuoteUserAlertMinimumFollowersCount,
                MinimumFollowingsCount = _configuration.Requirement.CreateQuoteUserAlertMinimumFollowingsCount,
                MinimumStocktwitsCreatedOnAgeInMonths = _configuration.Requirement.CreateQuoteUserAlertMinimumStocktwitsCreatedOnAgeInMonths,
                MinimumPostsCount = _configuration.Requirement.CreateQuoteUserAlertMinimumPostsCount,
                MinimumLikesCount = _configuration.Requirement.CreateQuoteUserAlertMinimumLikesCount,
                MinimumWatchlistQuotesCount = _configuration.Requirement.CreateQuoteUserAlertMinimumWatchlistQuotesCount,
                MaximumAlertsInProgressCount = _configuration.Limit.CreateQuoteUserAlertLimit
            }
        };

        public StockMarketUserContract GetUser(int userId)
        {
            var user = _userService.GetUser(userId);

            if (user.UserTypeId != (int)UserType.Stocktwits)
                throw new BadRequestException($"{nameof(userId)} is not of {nameof(UserType.Stocktwits)} type");

            return new StockMarketUserContract()
            {
                AlertTypeCounts = StockMarketMemoryCache.StockMarketQuoteUserAlerts
                    .Where(quoteUserAlert => quoteUserAlert.Value.UserId == user.UserId)
                    .GroupBy(quoteUserAlert => quoteUserAlert.Value.AlertTypeId)
                    .Select(alertTypeGroup => new StockMarketUserAlertTypeCountContract()
                    {
                        AlertTypeId = alertTypeGroup.Key,
                        Count = alertTypeGroup.Count(),
                        Points = alertTypeGroup.Key switch
                        {
                            (int)AlertType.InProgress => 0,
                            (int)AlertType.Successful => alertTypeGroup.Select(quoteUserAlert => quoteUserAlert.Value.CompletedSellPoints!.Value)
                                                                       .DefaultIfEmpty(0)
                                                                       .Sum(),
                            (int)AlertType.Unsuccessful => alertTypeGroup.Select(quoteUserAlert => quoteUserAlert.Value.CompletedSellPoints!.Value)
                                                                         .DefaultIfEmpty(0)
                                                                         .Sum(),
                            _ => 0
                        }
                    }).ToList(),
                UserId = user.UserId
            };
        }

        #endregion

        #region Create

        public async Task<StockMarketQuoteUserAlertContract> CreateQuoteUserAlertAsync(int userId, int quoteId, (decimal sell, decimal stopLoss) alert, DateTimeZone timezone)
        {
            var user = _userService.GetUser(userId);
            var quote = GetQuote(quoteId);

            if (user.Stocktwits!.CreatedOn.UtcDateTime.Date.AddMonths(_configuration.Requirement.CreateQuoteUserAlertMinimumStocktwitsCreatedOnAgeInMonths) > DateTimeOffset.UtcNow.Date)
                throw new BadRequestException($"Your account must be at least {_configuration.Requirement.CreateQuoteUserAlertMinimumStocktwitsCreatedOnAgeInMonths} months old");

            if (user.Stocktwits.FollowersCount < _configuration.Requirement.CreateQuoteUserAlertMinimumFollowersCount)
                throw new BadRequestException($"Your account must have at least {_configuration.Requirement.CreateQuoteUserAlertMinimumFollowersCount} followers");

            if (user.Stocktwits.FollowingsCount < _configuration.Requirement.CreateQuoteUserAlertMinimumFollowingsCount)
                throw new BadRequestException($"Your account must follow at least {_configuration.Requirement.CreateQuoteUserAlertMinimumFollowingsCount} other accounts");

            if (user.Stocktwits.PostsCount < _configuration.Requirement.CreateQuoteUserAlertMinimumPostsCount)
                throw new BadRequestException($"Your account must have at least {_configuration.Requirement.CreateQuoteUserAlertMinimumPostsCount} posts");

            if (user.Stocktwits.LikesCount < _configuration.Requirement.CreateQuoteUserAlertMinimumLikesCount)
                throw new BadRequestException($"Your account must have at least {_configuration.Requirement.CreateQuoteUserAlertMinimumLikesCount} likes");

            if (user.Stocktwits.WatchlistQuotesCount < _configuration.Requirement.CreateQuoteUserAlertMinimumWatchlistQuotesCount)
                throw new BadRequestException($"Your account must have at least {_configuration.Requirement.CreateQuoteUserAlertMinimumWatchlistQuotesCount} watchlist quotes");

            var quoteUserAlerts = StockMarketMemoryCache.StockMarketQuoteUserAlerts
                .Where(quoteUserAlert =>
                    quoteUserAlert.Value.UserId == user.UserId &&
                    !quoteUserAlert.Value.AlertCompletedTypeId.HasValue
                )
                .Select(quoteUserAlert => quoteUserAlert.Value)
                .ToList();

            if (quoteUserAlerts.Any(quoteUserAlert => quoteUserAlert.QuoteId == quote.QuoteId))
                throw new BadRequestException("You already created an alert for this quote");

            if (quoteUserAlerts.Count >= _configuration.Limit.CreateQuoteUserAlertLimit)
                throw new BadRequestException("You have reached your limit");

            var global = await GetGlobalAsync();

            var price = await GetQuotePriceAsync(quote.QuoteId);

            if (price.Current >= alert.sell)
                throw new BadRequestException($"{nameof(alert.sell)} must be greater than current price");

            if (price.Current <= alert.stopLoss)
                throw new BadRequestException($"{nameof(alert.stopLoss)} must be less than current price");

            if (StockMarketExtension.GetChangePercent(from: price.Current, to: alert.sell) > _configuration.Limit.CreateQuoteUserAlertSellPointsLimit)
                throw new BadRequestException($"{nameof(alert.sell)} points must be less than 300");

            if (StockMarketExtension.GetChangePercent(from: price.Current, to: alert.stopLoss) < _configuration.Limit.CreateQuoteUserAlertStopLossPointsLimit)
                throw new BadRequestException($"{nameof(alert.stopLoss)} points must be less than 50");

            var quoteUserAlert = (await _repository.AddAndSaveChangesAsync(new StockMarketQuoteUserAlertEntity()
            {
                QuoteId = quote.QuoteId,
                UserId = user.UserId,
                AlertTypeId = (int)AlertType.InProgress,
                Buy = price.Current,
                Sell = alert.sell,
                StopLoss = alert.stopLoss,
                CreatedOn = DateTimeOffset.Now.AtTimezone(timezone),
                LastCheckOn = DateTimeOffset.Now,
                LastReverseSplitCheckOn = DateTimeOffset.Now,
                ReverseSplitCount = 0
            })).MapToQuoteUserAlertContract();

            StockMarketMemoryCache.StockMarketQuoteUserAlerts.TryAdd(quoteUserAlert.QuoteUserAlertId, quoteUserAlert);

            return quoteUserAlert;
        }

        public async Task<StockMarketQuoteUserEmotionContract> CreateQuoteUserEmotionAsync(int userId, int quoteId, int emotionId, DateTimeZone timezone)
        {
            var user = _userService.GetUser(userId);
            var quote = GetQuote(quoteId);
            var emotion = GetEmotion(emotionId);

            var quoteUserEmotions = StockMarketMemoryCache.StockMarketQuoteUserEmotions.GetQuoteUserEmotionsAtTimezoneToday(timezone, userId);

            if (quoteUserEmotions.Any(userEmotion => userEmotion.QuoteId == quote.QuoteId))
                throw new BadRequestException("You already selected an emotion");

            if (quoteUserEmotions.Count >= _configuration.Limit.CreateQuoteUserEmotionLimit)
                throw new BadRequestException("You have reached your daily limit. Come back tomorrow!");

            var quoteUserEmotion = (await _repository.AddAndSaveChangesAsync(new StockMarketQuoteUserEmotionEntity()
            {
                QuoteId = quote.QuoteId,
                UserId = user.UserId,
                EmotionId = emotion.EmotionId,
                CreatedOn = DateTimeOffset.Now.AtTimezone(timezone)
            })).MapToQuoteUserEmotionContract();

            StockMarketMemoryCache.StockMarketQuoteUserEmotions.TryAdd(quoteUserEmotion.QuoteUserEmotionId, quoteUserEmotion);

            return quoteUserEmotion;
        }

        #endregion

        #region Update

        public async Task<StockMarketQuoteUserAlertContract> CompleteQuoteUserAlertAsync(int userId, int quoteUserAlertId)
        {
            var user = _userService.GetUser(userId);

            if (!StockMarketMemoryCache.StockMarketQuoteUserAlerts.TryGetValue(quoteUserAlertId, out StockMarketQuoteUserAlertContract? quoteUserAlert))
                throw new BadRequestException($"{nameof(quoteUserAlertId)} is invalid");

            if (quoteUserAlert.UserId != user.UserId)
                throw new BadRequestException($"{nameof(quoteUserAlertId)} is not owned by you");

            if (quoteUserAlert.AlertCompletedTypeId.HasValue)
                throw new BadRequestException($"{nameof(quoteUserAlertId)} is already completed");

            await ProcessQuoteUserAlertsReverseSplitsAsync(new List<int> { quoteUserAlert.QuoteUserAlertId });
            await ProcessQuoteUserAlertsInProgressAsync((int)AlertCompletedType.Manual, new List<int> { quoteUserAlert.QuoteUserAlertId });

            return quoteUserAlert;
        }

        #endregion

        #region Processing

        internal async Task ProcessQuotesAsync()
        {
            List<StockMarketQuoteResponse> quotesResponse;

            try
            {
                quotesResponse = await GetQuotesResponseAsync();
            }
            catch (Exception exception)
            {
                await _errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
                (
                    className: nameof(StockMarketService),
                    classMethodName: nameof(ProcessQuotesAsync),
                    exceptionType: exception.GetType().Name,
                    exceptionMessage: exception.GetFullMessage()
                ));
                return;
            }

            var financialModelingPrepExchanges = StockMarketMemoryCache.StockMarketExchanges
                .Select(exchange => exchange.Value)
                .ToDictionary(exchange => exchange.FinancialModelingPrepId);

            var quoteEntities = await _repository.GetQuotesAsync();

            var newQuoteEntities = await _repository.AddRangeAndSaveChangesAsync(
                quotesResponse.GroupJoin(
                    quoteEntities,
                    quoteResponse => quoteResponse.Symbol,
                    quoteEntity => quoteEntity.Symbol,
                    (quoteResponse, joinedQuotesEntity) => (quoteResponse, quoteEntity: joinedQuotesEntity.SingleOrDefault())
                )
                .Where(quoteData =>
                    quoteData.quoteEntity is null &&
                    !string.IsNullOrWhiteSpace(quoteData.quoteResponse.Name) &&
                    !string.IsNullOrWhiteSpace(quoteData.quoteResponse.Symbol) &&
                    financialModelingPrepExchanges.ContainsKey(quoteData.quoteResponse.Exchange))
                .Select(quoteData => new StockMarketQuoteEntity()
                {
                    ExchangeId = financialModelingPrepExchanges[quoteData.quoteResponse.Exchange].ExchangeId,
                    Symbol = quoteData.quoteResponse.Symbol,
                    Name = quoteData.quoteResponse.Name
                })
                .ToList()
            );

            foreach (var quote in newQuoteEntities.Select(quoteEntity => quoteEntity.MapToQuoteContract()))
                StockMarketMemoryCache.StockMarketQuotes.TryAdd(quote.QuoteId, quote);
        }

        internal async Task ProcessQuoteUserAlertsReverseSplitsAsync(List<int>? quoteUserAlertIds = null)
        {
            var quoteUserAlertEntities = await _repository.GetQuoteUserAlertsAsync(quoteUserAlertIds: quoteUserAlertIds);
            var quoteUserAlertCacheUpdates = new List<Action>();

            foreach (var quoteId in quoteUserAlertEntities.Select(quoteUserAlertEntity => quoteUserAlertEntity.QuoteId).Distinct())
            {
                List<StockMarketQuoteReverseSplitContract> quoteReverseSplits;

                try
                {
                    quoteReverseSplits = await GetQuoteReverseSplitsAsync(quoteId);
                }
                catch (Exception exception)
                {
                    await _errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
                    (
                        className: nameof(StockMarketService),
                        classMethodName: nameof(ProcessQuoteUserAlertsReverseSplitsAsync),
                        exceptionType: exception.GetType().Name,
                        exceptionMessage: exception.GetFullMessage()
                    )
                    {
                        Input = $"{nameof(quoteId)}:{quoteId}"
                    });
                    continue;
                }

                foreach (var quoteUserAlertEntity in quoteUserAlertEntities.Where(quoteUserAlertEntity => quoteUserAlertEntity.QuoteId == quoteId))
                {
                    var originalQuoteUserAlertEntityReverseSplitCount = quoteUserAlertEntity.ReverseSplitCount;

                    foreach (var quoteReverseSplit in quoteReverseSplits.Where(quoteReverseSplit => quoteReverseSplit.CreatedOn >= quoteUserAlertEntity.LastReverseSplitCheckOn))
                    {
                        var ratio = quoteReverseSplit.Denominator / quoteReverseSplit.Numerator;

                        quoteUserAlertEntity.Buy *= ratio;
                        quoteUserAlertEntity.Sell *= ratio;
                        quoteUserAlertEntity.StopLoss *= ratio;

                        if (quoteUserAlertEntity.CompletedSell.HasValue)
                            quoteUserAlertEntity.CompletedSell *= ratio;

                        quoteUserAlertEntity.ReverseSplitCount++;
                    }

                    quoteUserAlertEntity.LastReverseSplitCheckOn = DateTimeOffset.Now;

                    if (originalQuoteUserAlertEntityReverseSplitCount != quoteUserAlertEntity.ReverseSplitCount)
                        quoteUserAlertCacheUpdates.Add(() =>
                        {
                            if (!StockMarketMemoryCache.StockMarketQuoteUserAlerts.TryGetValue(quoteUserAlertEntity.QuoteUserAlertId, out StockMarketQuoteUserAlertContract? quoteUserAlert))
                                return;

                            quoteUserAlert.Buy = quoteUserAlertEntity.Buy;
                            quoteUserAlert.Sell = quoteUserAlertEntity.Sell;
                            quoteUserAlert.StopLoss = quoteUserAlertEntity.StopLoss;
                            quoteUserAlert.CompletedSell = quoteUserAlertEntity.CompletedSell;
                        });
                }
            }

            await _repository.SaveChangesAsync();

            foreach (var quoteUserAlertCacheUpdate in quoteUserAlertCacheUpdates)
                quoteUserAlertCacheUpdate();
        }

        internal async Task ProcessQuoteUserAlertsInProgressAsync(int alertCompletionTypeId, List<int>? quoteUserAlertIds = null)
        {
            var quoteUserAlertEntities = (await _repository.GetQuoteUserAlertsAsync(alertTypeId: (int)AlertType.InProgress, quoteUserAlertIds))
                .Where(quoteUserAlertEntity => quoteUserAlertEntity.LastReverseSplitCheckOn >= quoteUserAlertEntity.LastCheckOn)
                .ToList();
            var quoteUserAlertCacheUpdates = new List<Action>();

            foreach (var quoteId in quoteUserAlertEntities.Select(quoteUserAlertEntity => quoteUserAlertEntity.QuoteId).Distinct())
            {
                List<StockMarketQuoteCandleContract> quoteCandles;
                StockMarketQuotePriceContract? price;

                try
                {
                    quoteCandles = await GetQuoteCandlesAsync(quoteId, (int)Timeframe.OneDay);
                    price = alertCompletionTypeId == (int)AlertCompletedType.Manual
                        ? await GetQuotePriceAsync(quoteId)
                        : null;
                }
                catch (Exception exception)
                {
                    await _errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
                    (
                        className: nameof(StockMarketService),
                        classMethodName: nameof(ProcessQuoteUserAlertsReverseSplitsAsync),
                        exceptionType: exception.GetType().Name,
                        exceptionMessage: exception.GetFullMessage()
                    )
                    {
                        Input = $"{nameof(quoteId)}:{quoteId}"
                    });
                    continue;
                }

                foreach (var quoteUserAlertEntity in quoteUserAlertEntities.Where(quoteUserAlertEntity => quoteUserAlertEntity.QuoteId == quoteId))
                {
                    foreach (var quoteCandle in quoteCandles.Where(quoteCandle => quoteCandle.CreatedOn > quoteUserAlertEntity.CreatedOn))
                    {
                        if (quoteCandle.High >= quoteUserAlertEntity.Sell)
                            quoteUserAlertEntity.AlertTypeId = (int)AlertType.Successful;
                        else if (quoteCandle.Low <= quoteUserAlertEntity.StopLoss)
                            quoteUserAlertEntity.AlertTypeId = (int)AlertType.Unsuccessful;
                        else
                            continue;

                        quoteUserAlertEntity.AlertCompletedTypeId = alertCompletionTypeId;
                        quoteUserAlertEntity.CompletedSell = quoteUserAlertEntity.AlertTypeId == (int)AlertType.Successful
                                                                 ? quoteUserAlertEntity.Sell
                                                                 : quoteUserAlertEntity.StopLoss;
                        quoteUserAlertEntity.CompletedOn = quoteCandle.CreatedOn.ToOffset(quoteUserAlertEntity.CreatedOn.Offset);
                        break;
                    }

                    if (price is not null && quoteUserAlertEntity.AlertTypeId == (int)AlertType.InProgress)
                    {
                        quoteUserAlertEntity.AlertTypeId = price.Current >= quoteUserAlertEntity.Buy
                                                               ? (int)AlertType.Successful
                                                               : (int)AlertType.Unsuccessful;
                        quoteUserAlertEntity.AlertCompletedTypeId = alertCompletionTypeId;
                        quoteUserAlertEntity.CompletedSell = price.Current;
                        quoteUserAlertEntity.CompletedOn = price.CreatedOn.ToOffset(quoteUserAlertEntity.CreatedOn.Offset);
                    }

                    quoteUserAlertEntity.LastCheckOn = DateTimeOffset.Now;

                    if (quoteUserAlertEntity.AlertTypeId != (int)AlertType.InProgress)
                        quoteUserAlertCacheUpdates.Add(() =>
                        {
                            if (!StockMarketMemoryCache.StockMarketQuoteUserAlerts.TryGetValue(quoteUserAlertEntity.QuoteUserAlertId, out StockMarketQuoteUserAlertContract? quoteUserAlert))
                                return;

                            quoteUserAlert.AlertTypeId = quoteUserAlertEntity.AlertTypeId;
                            quoteUserAlert.AlertCompletedTypeId = quoteUserAlertEntity.AlertCompletedTypeId;
                            quoteUserAlert.CompletedSell = quoteUserAlertEntity.CompletedSell;
                            quoteUserAlert.CompletedOn = quoteUserAlertEntity.CompletedOn;
                        });
                }
            }

            await _repository.SaveChangesAsync();

            foreach (var quoteUserAlertCacheUpdate in quoteUserAlertCacheUpdates)
                quoteUserAlertCacheUpdate();
        }

        #endregion

        #region Financial Modeling Prep Api

        private async Task<StockMarketGlobalResponse> GetGlobalResponseAsync() =>
            (await _financialModelingPrepApi.ExecuteGetAsync<StockMarketGlobalResponse>(
                new RestRequest("is-the-market-open")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
            )).GetData(isSuccess: (response) => response is not null);

        private async Task<List<StockMarketQuoteResponse>> GetQuotesResponseAsync() =>
            (await _financialModelingPrepApi.ExecuteGetAsync<List<StockMarketQuoteResponse>>(
                new RestRequest("search-ticker")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
                    .AddQueryParameter("query", "")
                    .AddQueryParameter("limit", "1000000")
            )).GetData(isSuccess: (response) => response is not null && response.Any());

        private async Task<StockMarketQuotePriceResponse> GetQuotePriceResponseAsync(string symbol) =>
            (await _financialModelingPrepApi.ExecuteGetAsync<List<StockMarketQuotePriceResponse>>(
                new RestRequest($"quote/{symbol}")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
            )).GetData(isSuccess: (response) => response is not null && response.SingleOrDefault() is not null)
              .First();

        private async Task<List<StockMarketQuotePriceResponse>> GetQuotesPriceResponseAsync(List<string> symbols) =>
            (await _financialModelingPrepApi.ExecuteGetAsync<List<StockMarketQuotePriceResponse>>(
                new RestRequest($"quote/{string.Join(',', symbols)}")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
            )).GetData(isSuccess: (response) => response is not null && response.Any());

        private async Task<StockMarketQuoteCompanyResponse> GetQuoteCompanyResponseAsync(string symbol) =>
            (await _financialModelingPrepApi.ExecuteGetAsync<List<StockMarketQuoteCompanyResponse>>(
                new RestRequest($"profile/{symbol}")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
            )).GetData(isSuccess: (response) => response is not null && response.SingleOrDefault() is not null)
              .First();

        private async Task<List<StockMarketQuoteCompanyResponse>> GetQuotesCompanyResponseAsync(List<string> symbols) =>
            (await _financialModelingPrepApi.ExecuteGetAsync<List<StockMarketQuoteCompanyResponse>>(
                new RestRequest($"profile/{string.Join(',', symbols)}")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
            )).GetData(isSuccess: (response) => response is not null && response.Any());

        private async Task<List<StockMarketQuoteCandleResponse>> GetQuoteCandlesTimeframeResponseAsync(string symbol, string timeframeId) =>
            (await _financialModelingPrepApi.ExecuteGetAsync<List<StockMarketQuoteCandleResponse>>(
                new RestRequest($"historical-chart/{timeframeId}/{symbol}")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
            )).GetData(isSuccess: (response) => response is not null && response.Any());

        private async Task<List<StockMarketQuoteCandleResponse>> GetQuoteCandlesDailyResponseAsync(string symbol, DateTimeOffset from, DateTimeOffset to) =>
            (await _financialModelingPrepApi.ExecuteGetAsync<StockMarketQuoteCandleFullResponse>(
                new RestRequest($"historical-price-full/{symbol}")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
                    .AddQueryParameter("from", $"{from:yyyy-MM-dd}")
                    .AddQueryParameter("to", $"{to:yyyy-MM-dd}")
            )).GetData(isSuccess: (response) => response?.Candles is not null && response.Candles.Any()).Candles;

        private async Task<List<StockMarketQuoteReverseSplitResponse>> GetQuoteReverseSplitsResponseAsync(string symbol) =>
            (await _financialModelingPrepApi.ExecuteGetAsync<StockMarketQuoteReverseSplitFullResponse>(
                new RestRequest($"historical-price-full/stock_split/{symbol}")
                    .AddQueryParameter("apikey", _configuration.FinancialModelingPrepApi.AuthenticationKey)
            )).GetData(isSuccess: (response) => response is not null).ReverseSplits ?? new List<StockMarketQuoteReverseSplitResponse>();

        #endregion
    }
}