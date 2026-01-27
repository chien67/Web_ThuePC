# Graduation Thesis (Project Thesis) - Hotel Management System
+ http://localhost:2001/ (U/P: admin/123456)
+ http://103.226.250.247:6868/ (U/P: admin/123456)

## How to publish
+ Stop Website "QL_Laptop"
+ Clean build ASP.NET MVC 5
+ Publish trực tiếp bằng Visual Studio 2026 Insiders
+ Start Websitee "QL_Laptop"
+ [IISProfile.pubxml](https://github.com/chien67/Web_ThuePC/blob/feature/publish-to-iis/DATN_Web/Properties/PublishProfiles/IISProfile.pubxml)

```
Solution name: D:\Chien67\Web_ThuePC\DATN_Web.sln

Project name: D:\Chien67\Web_ThuePC\DATN_Web\DATN_Web.csproj

Web.config: D:\Chien67\Web_ThuePC\DATN_Web\Web.config

packages.config: D:\Chien67\Web_ThuePC\DATN_Web\packages.config

Profile name: IISProfile

Folder Published: D:\Chien67\Web_ThuePC\Published

Database name: QL_Laptop

Website name: QL_Laptop

Visual Studio 2026 Insiders: "C:\Program Files\Microsoft Visual Studio\18\Insiders\Common7\IDE\devenv.exe"

MSBuild 2026 Insiders: "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe"

"C:\Windows\System32\inetsrv\appcmd.exe" stop site /site.name:"QL_Laptop"

"C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" "D:\Chien67\Web_ThuePC\DATN_Web\DATN_Web.csproj" /p:DeployOnBuild=true /p:PublishProfile=IISProfile /p:Configuration=Release

"C:\Windows\System32\inetsrv\appcmd.exe" start site /site.name:"QL_Laptop"
```

## Hotel Management System Project Thesis

## Technology

+ .NET Framework 4.7.2
+ ASP.NET MVC 5
+ [Razor Pages or Razor Syntax](https://www.tutorialsteacher.com/mvc/razor-syntax)
+ SQL Server
+ ADO.NET, Stored Procedure
+ Dapper.Contrib
+ Log4Net
+ HTML, CSS, JavaScript
+ Boostrap, jQuery
+ AdminLTE 2

## Plan: From 1-Oct-2024 To 7-Jan-2025
+ https://docs.google.com/spreadsheets/d/1mtyRUXz3GQ5hWTfodZnOoKC7-sOjZa8nqzoa3Vqx9EM

## Doc
+ https://docs.google.com/document/d/1vGgCN9j4ByMrY8ZzPAOm2Cwz7aApTxCpOaYFNR7-p7k/
+ https://docs.google.com/document/d/159pprqlE1FEtENNlpcf5qBKgv2CdKpHzp5HsHvbbxBE/

## Security
+ https://www.codeproject.com/Articles/408306/Understanding-and-Implementing-ASP-NET-Custom-Form
+ https://www.codeproject.com/Articles/682113/Extending-Identity-Accounts-and-Implementing-Rol

## Dapper + Stored Procedure
+ https://code-maze.com/csharp-pass-output-parameters-to-stored-procedures-dapper/

## JavaScript Revealing Module Pattern
+ https://weblogs.asp.net/dwahlin/techniques-strategies-and-patterns-for-structuring-javascript-code-revealing-module-pattern
+ https://github.com/dejancaric/asp.net-mvc-and-revealing-module-pattern/blob/master/MvcDemo/assets/js/homepage.js
+ https://djaytechdiary.com/javascript-revealing-module-pattern

## FE: HTML + AdminLTE
+ https://github.com/gtechsltn/HotelManagementSystem-ASPNETMVC

## BE: Data Model for Hotel Management System
+ https://github.com/gtechsltn/hotel-management-system-hms/blob/master/SQLHotelManagement.sql

# References
+ https://github.com/gtechsltn/MVC.RMS
