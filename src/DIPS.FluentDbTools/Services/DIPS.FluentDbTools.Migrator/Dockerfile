FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY . .
WORKDIR /src/DIPS.DbTools/Services/DIPS.FluentDbTools.Migrator
RUN dotnet publish DIPS.FluentDbTools.Migrator.csproj -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DIPS.FluentDbTools.Migrator.dll"]
