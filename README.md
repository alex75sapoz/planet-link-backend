## Api Explorer
https://api.planet-link.com/swagger

## Project Structure

> Api

**Configuration**
Provides access to configuration methods that the startup class calls

**Controller**
Provides access to routes

> Library

**Base**

All class libraries must reference this project. It provides access to base classes for services, repositories, context, etc. All nuget packages are maintained here. If your class library needs a nuget package then you would add it to this project

**Application**

Provides access to log errors based on different types such as RequestError, ProcessingError, AuthenticationError

**Location**

Provides access to cities, countries, etc
 
**StockMarket**

Provides access to quotes, candles, etc

**Account**

Provides access to users, sessions, etc

**Weather**

Provides access to observations, forecasts, etc

**Programming**

Provides access to projects
