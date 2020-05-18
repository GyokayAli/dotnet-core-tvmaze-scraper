# Scraper API
An ASP.NET Core Web API to serve shows/cast data. Includes a background scraper to retrieve the necessary data from [TVmaze API](http://www.tvmaze.com/api) and persists in own data storage.

* Live at: https://scraperapiappservice.azurewebsites.net/swagger

### Technical stuff
* .NET Core 2.2
* MS SQL Server
* Entity Framework Core
* AutoMapper
* NSwag

### API
* Check it out using [Swagger](http://www.tvmaze.com/api) - /swagger
* GET paginated result of shows including the cast - [Test](https://scraperapiappservice.azurewebsites.net/api/shows) - /api/shows
* Pagination default values are as follows: `page = 0`, `pageSize = 25` and maximum `pageSize = 100` - /api/shows?page=1&pageSize=50

### Background service
* Executed immediately on startup.
* After the first run, scheduled to be executed every day at midnight.
