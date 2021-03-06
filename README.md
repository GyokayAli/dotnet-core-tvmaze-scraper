# Scraper API
ASP.NET Core Web API to serve shows/cast data. Includes a background scraper to retrieve the necessary data from [TVmaze API](http://www.tvmaze.com/api) and persists in own data storage.

[![Build Status](https://dev.azure.com/gyokaymetinali/ScraperAPI/_apis/build/status/GyokayAli.dotnet-core-tvmaze-scraper?branchName=master)](https://dev.azure.com/gyokaymetinali/ScraperAPI/_build/latest?definitionId=2&branchName=master)

### Technical stuff
* .NET Core 2.2
* MS SQL Server
* Entity Framework Core
* [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions)
* AutoMapper
* NSwag

### API
* GET paginated result of shows including the cast => `/api/shows`
* Pagination default values: `page = 0`, `pageSize = 25` and maximum `pageSize = 100` => `/api/shows?page=1&pageSize=50`

### Background service
* Executed immediately on startup.
* After the first run, scheduled to be executed every day at midnight.
* Continues running from the last page where left off using the last available show id in the storage.

#### Start using
* Update `DefaultConnection` in `appsettings.json`
* Apply migrations => `dotnet ef database update`
