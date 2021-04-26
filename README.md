## Project Structure

> Api

**Configuration**
Provides access to configuration methods that the startup class calls
  - Uses scheduled background tasks

**Controller**
Provides access to routes

> Library

**Base**
All class libraries must reference this project. It provides access to base classes for services, repositories, context, etc. All nuget packages are maintained here. If your class library needs a nuget package then you would add it to this project

**Error**
Provides access to log errors based on different types such as RequestError or ProcessingError

**Location**
Provides access to cities, countries, etc

 - Uses a long term in memory cache
 

**StockMarket**
Provides access to quotes, candles, etc

 - Uses a long term in memory cache
 - Uses a short term in memory cache
 - Uses an external stock market api
 - Uses scheduled background tasks

**User**
Provides access to users, sessions, etc

 - Uses a long term memory cache
 - Uses multiple external user apis

**Weather**
Provides access to observations, forecasts, etc

 - Uses a long term in memory cache
 - Uses a short term in memory cache
