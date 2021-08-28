namespace Library.StockMarket.Enum
{
    public enum StockMarketDictionary : int
    {
        Exchanges = 1,
        Timeframes = 2,
        AlertTypes = 3,
        AlertCompletedTypes = 4,
        Emotions = 5,
        Quotes = 6,
        QuoteUserAlerts = 7,
        QuoteUserEmotions = 8
    }

    public enum Exchange : int
    {
        Nyse = 1,
        Nasdaq = 2,
        Amex = 3,
        Crypto = 4,
        Forex = 5,
        Euronext = 6,
        Index = 7,
        Tsx = 8,
        Commodity = 9,
        MutualFund = 10,
        Etf = 11
    }

    public enum Timeframe : int
    {
        OneDay = 1,
        FiveDay = 2,
        OneMonth = 3,
        OneYear = 4,
        FiveYear = 5
    }

    public enum AlertType : int
    {
        InProgress = 1,
        Successful = 2,
        Unsuccessful = 3
    }

    public enum AlertCompletedType : int
    {
        Automatic = 1,
        Manual = 2
    }

    public enum Emotion : int
    {
        Moon = 1,
        Rocket = 2,
        Thinking = 3,
        Bear = 4,
        Skull = 5
    }
}