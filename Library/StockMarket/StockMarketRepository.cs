using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    public interface IStockMarketRepository
    {
        Task<List<StockMarketAlertCompletedTypeEntity>> GetAlertCompletedTypesAsync();
        Task<List<StockMarketAlertTypeEntity>> GetAlertTypesAsync();
        Task<List<StockMarketEmotionEntity>> GetEmotionsAsync();
        Task<List<StockMarketExchangeEntity>> GetExchangesAsync();
        Task<List<StockMarketQuoteEntity>> GetQuotesAsync();
        Task<StockMarketQuoteUserAlertEntity> GetQuoteUserAlertAsync(int quoteUserAlertId);
        Task<List<StockMarketQuoteUserAlertEntity>> GetQuoteUserAlertsAsync(int? alertTypeId = null, List<int>? quoteUserAlertIds = null);
        Task<List<StockMarketQuoteUserEmotionEntity>> GetQuoteUserEmotionsAsync(DateTimeOffset from);
        Task<List<StockMarketTimeframeEntity>> GetTimeframesAsync();
    }

    class StockMarketRepository : BaseRepository<StockMarketContext>, IStockMarketRepository
    {
        public StockMarketRepository(StockMarketContext context) : base(context) { }

        public async Task<List<StockMarketQuoteEntity>> GetQuotesAsync() =>
            await _context.StockMarketQuotes
                .ToListAsync();

        public async Task<List<StockMarketExchangeEntity>> GetExchangesAsync() =>
            await _context.StockMarketExchanges
                .ToListAsync();

        public async Task<List<StockMarketTimeframeEntity>> GetTimeframesAsync() =>
            await _context.StockMarketTimeframes
                .ToListAsync();

        public async Task<List<StockMarketQuoteUserAlertEntity>> GetQuoteUserAlertsAsync(int? alertTypeId = null, List<int>? quoteUserAlertIds = null) =>
            await _context.StockMarketQuoteUserAlerts
                .Include(quoteUserAlert => quoteUserAlert.Quote)
                .Where(quoteUserAlert =>
                    (!alertTypeId.HasValue || quoteUserAlert.AlertTypeId == alertTypeId) &&
                    (quoteUserAlertIds == null || quoteUserAlertIds.Contains(quoteUserAlert.QuoteUserAlertId))
                )
                .ToListAsync();

        public async Task<List<StockMarketAlertTypeEntity>> GetAlertTypesAsync() =>
            await _context.StockMarketAlertTypes
                .ToListAsync();

        public async Task<List<StockMarketAlertCompletedTypeEntity>> GetAlertCompletedTypesAsync() =>
            await _context.StockMarketAlertCompletedTypes
                .ToListAsync();

        public async Task<StockMarketQuoteUserAlertEntity> GetQuoteUserAlertAsync(int quoteUserAlertId) =>
            await _context.StockMarketQuoteUserAlerts
                .Include(quoteUserAlert => quoteUserAlert.Quote)
                .SingleOrDefaultAsync(quoteUserAlert => quoteUserAlert.QuoteUserAlertId == quoteUserAlertId);

        public async Task<List<StockMarketQuoteUserEmotionEntity>> GetQuoteUserEmotionsAsync(DateTimeOffset from) =>
            await _context.StockMarketQuoteUserEmotions
                .Where(quoteUserEmotion => quoteUserEmotion.CreatedOn >= from)
                .ToListAsync();

        public async Task<List<StockMarketEmotionEntity>> GetEmotionsAsync() =>
            await _context.StockMarketEmotions
                .ToListAsync();
    }
}