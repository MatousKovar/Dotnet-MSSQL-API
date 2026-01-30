## Databáze
- diagram v dbmd https://dbdiagram.io/d/69735941bd82f5fce2664f79
- Běží v docker containeru
- Pro práci s ní v C# se používá Entity Framework(EF), balíčky NuGet SqlServer a Design 
- Pro EF je potřeba nainstalovat tool pomocí `dotnet tool install --global dotnet-ef` - spouštění EF (příkazů)[https://learn.microsoft.com/en-us/ef/core/cli/dotnet] 
- EF vede k tvorbě Entity frst, pro případ že máme databázi už existující se vyplatí Database First přístup - https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli
- Pro vytvoreni modelu: `dotnet ef dbcontext scaffold "Server=localhost,1433;Database=machine_db;User Id=sa;Password=Dochazka123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --force --namespace SimpleAPI.Models` viz: dotnet ef dbcontext scaffold --help Dulezite je namespace aby sedelo se zbytkem projektu
- 
