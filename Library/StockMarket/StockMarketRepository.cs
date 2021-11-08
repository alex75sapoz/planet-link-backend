using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    class StockMarketRepository : BaseRepository<StockMarketContext>, IStockMarketRepository
    {
        public StockMarketRepository(StockMarketContext context) : base(context) { }

        public async Task<List<StockMarketQuoteEntity>> GetQuotesAsync() =>
            await _context.Quotes
                .ToListAsync();

        public async Task<StockMarketQuoteEntity?> GetQuoteAsync(int quoteId) =>
            await _context.Quotes
                .FindAsync(quoteId);

        public async Task<List<StockMarketExchangeEntity>> GetExchangesAsync() =>
            await _context.Exchanges
                .ToListAsync();

        public async Task<StockMarketExchangeEntity?> GetExchangeAsync(int exchangeId) =>
            await _context.Exchanges
                .FindAsync(exchangeId);

        public async Task<List<StockMarketTimeframeEntity>> GetTimeframesAsync() =>
            await _context.Timeframes
                .ToListAsync();

        public async Task<StockMarketTimeframeEntity?> GetTimeframeAsync(int timeframeId) =>
            await _context.Timeframes
                .FindAsync(timeframeId);

        public async Task<List<StockMarketQuoteUserAlertEntity>> GetQuoteUserAlertsAsync(int? alertTypeId = null, List<int>? quoteUserAlertIds = null) =>
            await _context.QuoteUserAlerts
                .Include(quoteUserAlert => quoteUserAlert.Quote)
                .Where(quoteUserAlert =>
                    (!alertTypeId.HasValue || quoteUserAlert.AlertTypeId == alertTypeId) &&
                    (quoteUserAlertIds == null || quoteUserAlertIds.Contains(quoteUserAlert.QuoteUserAlertId))
                )
                .ToListAsync();

        public async Task<List<StockMarketAlertTypeEntity>> GetAlertTypesAsync() =>
            await _context.AlertTypes
                .ToListAsync();

        public async Task<StockMarketAlertTypeEntity?> GetAlertTypeAsync(int alertTypeId) =>
            await _context.AlertTypes
                .FindAsync(alertTypeId);

        public async Task<List<StockMarketAlertCompletedTypeEntity>> GetAlertCompletedTypesAsync() =>
            await _context.AlertCompletedTypes
                .ToListAsync();

        public async Task<StockMarketAlertCompletedTypeEntity?> GetAlertCompletedTypeAsync(int alertCompletedTypeId) =>
            await _context.AlertCompletedTypes
                .FindAsync(alertCompletedTypeId);

        public async Task<StockMarketQuoteUserAlertEntity?> GetQuoteUserAlertAsync(int quoteUserAlertId) =>
            await _context.QuoteUserAlerts
                .Include(quoteUserAlert => quoteUserAlert.Quote)
                .SingleOrDefaultAsync(quoteUserAlert => quoteUserAlert.QuoteUserAlertId == quoteUserAlertId);

        public async Task<List<StockMarketQuoteUserEmotionEntity>> GetQuoteUserEmotionsAsync(DateTimeOffset from) =>
            await _context.QuoteUserEmotions
                .Where(quoteUserEmotion => quoteUserEmotion.CreatedOn >= from)
                .ToListAsync();

        public async Task<StockMarketQuoteUserEmotionEntity?> GetQuoteUserEmotionAsync(int quoteUserEmotionId) =>
            await _context.QuoteUserEmotions
                .FindAsync(quoteUserEmotionId);

        public async Task<List<StockMarketEmotionEntity>> GetEmotionsAsync() =>
            await _context.Emotions
                .ToListAsync();

        public async Task<StockMarketEmotionEntity?> GetEmotionAsync(int emotionId) =>
            await _context.Emotions
                .FindAsync(emotionId);
    }

    public interface IStockMarketRepository
    {
        Task<List<StockMarketAlertCompletedTypeEntity>> GetAlertCompletedTypesAsync();
        Task<StockMarketAlertCompletedTypeEntity?> GetAlertCompletedTypeAsync(int alertCompletedTypeId);
        Task<List<StockMarketAlertTypeEntity>> GetAlertTypesAsync();
        Task<StockMarketAlertTypeEntity?> GetAlertTypeAsync(int alertTypeId);
        Task<List<StockMarketEmotionEntity>> GetEmotionsAsync();
        Task<StockMarketEmotionEntity?> GetEmotionAsync(int emotionId);
        Task<List<StockMarketExchangeEntity>> GetExchangesAsync();
        Task<StockMarketExchangeEntity?> GetExchangeAsync(int exchangeId);
        Task<List<StockMarketQuoteEntity>> GetQuotesAsync();
        Task<StockMarketQuoteEntity?> GetQuoteAsync(int quoteId);
        Task<StockMarketQuoteUserAlertEntity?> GetQuoteUserAlertAsync(int quoteUserAlertId);
        Task<List<StockMarketQuoteUserAlertEntity>> GetQuoteUserAlertsAsync(int? alertTypeId = null, List<int>? quoteUserAlertIds = null);
        Task<List<StockMarketQuoteUserEmotionEntity>> GetQuoteUserEmotionsAsync(DateTimeOffset from);
        Task<StockMarketQuoteUserEmotionEntity?> GetQuoteUserEmotionAsync(int quoteUserEmotionId);
        Task<List<StockMarketTimeframeEntity>> GetTimeframesAsync();
        Task<StockMarketTimeframeEntity?> GetTimeframeAsync(int timeframeId);
    }
}