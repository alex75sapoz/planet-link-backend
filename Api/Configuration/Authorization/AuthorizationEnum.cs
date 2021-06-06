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
        UserTypeGoogle,
        UserTypeStocktwits,
        UserMemoryCache,

        //Weather
        WeatherMemoryCache
    }
}