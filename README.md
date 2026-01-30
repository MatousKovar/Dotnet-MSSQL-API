# Simple dotnet api
- Api je základní demonstrace dotnet api vyhužívající MVC architektury
- Pro komunikaci s databází je použito EF (entity framework).
- Databáze běží v jednoduchém Docker containeru a dá se vytvořit pomocí příkazů v database_scripts. Více o databázi je v database.md
- Co se týče api, tak to obsahuje Controller, který běží nad touto databází. 
- Lokálně pravděpodobně nebude fungovat https port, to se dá vyřešit pomocí `dotnet dev-certs https --trust`

--- 
## Docker

- Bezi v nem MSSQL server 2025
- Užitečné příkazy
  - `docker up -d` - spustí docker
  - `docker ps` - bezici containery
  - `docker exec -it simple_api_database_mock_container bash` - pro přístup do shellu databáze
---
## Databáze

- Diagram v [dbdiagram.io](https://dbdiagram.io/d/69735941bd82f5fce2664f79)
- Pro práci s ní v C# se používá Entity Framework(EF), balíčky NuGet SqlServer a Design (viz. `dotnet_simple_api.csproj`)
- Credentials do databáze jsou v appsettings.json - connection string.
- Pro EF je potřeba nainstalovat tool pomocí `dotnet tool install --global dotnet-ef` - spouštění EF [příkazů](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) 
- Entity Framework je zde pomocí [database first]( https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli) přístupu
- Tvorba modelů je jednoduchá pomocí: `dotnet ef dbcontext scaffold "Server=localhost,1433;Database=machine_db;User Id=sa;Password=Dochazka123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --force --namespace SimpleAPI.Models --context-dir Data` viz: dotnet ef dbcontext scaffold --help 
  - Důležité je v aby namespace sedělo se zbytkem projektu (automaticky se tvoří podle jména db tuším), 
  - context-dir automaticky nastaví kam se uloží DbContext class
- Pro ukladani citlivych informaci mimo zdroják: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
- Idealne na produkci budu mit appsettings.json DefaultConnection jako enviromantal secret na gitu, nebudu to mit v plaintextu a nastavim, ze zmeny v appsettings.json se nepushujou, aby mi to vyvojari neprepisovali, pro tohle lokalni repo je to overkill