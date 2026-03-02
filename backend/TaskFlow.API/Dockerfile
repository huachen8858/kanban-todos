FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY TaskFlow.sln ./
COPY TaskFlow.API/TaskFlow.API.csproj TaskFlow.API/
COPY TaskFlow.Application/TaskFlow.Application.csproj TaskFlow.Application/
COPY TaskFlow.Domain/TaskFlow.Domain.csproj TaskFlow.Domain/
COPY TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj TaskFlow.Infrastructure/
COPY TaskFlow.Tests/TaskFlow.Tests.csproj TaskFlow.Tests/

RUN dotnet restore

COPY . .
RUN dotnet publish TaskFlow.API/TaskFlow.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
CMD ["dotnet", "TaskFlow.API.dll"]
