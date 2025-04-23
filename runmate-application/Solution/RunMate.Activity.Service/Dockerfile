FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RunMate.Activity.Service/RunMate.EngagementService.csproj", "RunMate.Activity.Service/"]
COPY ["RunMate.Shared.Auth/RunMate.Shared.Auth.csproj", "RunMate.Shared.Auth/"]
RUN dotnet restore "RunMate.Activity.Service/RunMate.EngagementService.csproj"
COPY . .
WORKDIR "/src/RunMate.Activity.Service"
RUN dotnet build "RunMate.EngagementService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RunMate.EngagementService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RunMate.EngagementService.dll"]