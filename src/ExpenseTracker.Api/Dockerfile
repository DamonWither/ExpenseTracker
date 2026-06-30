FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ExpenseTracker.Api/ExpenseTracker.Api.csproj", "src/ExpenseTracker.Api/"]
RUN dotnet restore "src/ExpenseTracker.Api/ExpenseTracker.Api.csproj"
COPY . .
RUN dotnet publish "src/ExpenseTracker.Api/ExpenseTracker.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ExpenseTracker.Api.dll"]
