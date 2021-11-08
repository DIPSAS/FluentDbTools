@echo off
set pack_dir=%USERPROFILE%\.nuget\packages
set VERSION=1.2.26

dotnet build src\FluentDbTools\FluentDbTools.sln -c Debug

:: Implementations
for %%i in (FluentDbTools.DbProviders,FluentDbTools.Migration,FluentDbTools.Migration.Common,FluentDbTools.Migration.Oracle,FluentDbTools.Migration.Postgres,FluentDbTools.SqlBuilder,FluentDbTools.SqlBuilder.Dapper) do (
call :handlePackage %%i Implementations
)

:: Extensions
for %%i in (FluentDbTools.Extensions.DbProvider,FluentDbTools.Extensions.Migration,FluentDbTools.Extensions.MSDependencyInjection,FluentDbTools.Extensions.MSDependencyInjection.Oracle,FluentDbTools.Extensions.MSDependencyInjection.Postgres,FluentDbTools.Extensions.SqlBuilder) do (
call :handlePackage %%i Extensions
)
:: Abstractions
for %%i IN (FFluentDbTools.Abstractions,FluentDbTools.Common.Abstractions,FluentDbTools.Migration.Abstractions,FluentDbTools.SqlBuilder.Abstractions) do (
call :handlePackage %%i Abstractions
)

:: Contracts
for %%i IN (FluentDbTools.Contracts,FluentDbTools.Migration.Contracts) do (
call :handlePackage %%i Contracts
)


exit /b

:handlePackage
if exist src\FluentDbTools\%2\%1\bin\Debug\netstandard2.0\%1.dll (
	echo copy src\FluentDbTools\%2\%1\bin\Debug\netstandard2.0\%1.??? %pack_dir%\%1\%VERSION%\lib\netstandard2.0
	xcopy src\FluentDbTools\%2\%1\bin\Debug\netstandard2.0\%1.??? %pack_dir%\%1\%VERSION%\lib\netstandard2.0 /Y /F /C /I
)

exit /b
