namespace Api.Configuration.Authorization
{
    internal enum Requirement
    {
        //Location
        LocationMemoryCache,

        //Programming
        ProgrammingMemoryCache,

        //StockMarket
        StockMarketMemoryCache,

        //User
        UserTypeAny,
        UserTypeGoogle,
        UserTypeStocktwits,
        UserMemoryCache,

        //Weather
        WeatherMemoryCache
    }
}