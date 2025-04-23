FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RunMate/RunMate.UserService.csproj", "RunMate/"]
COPY ["RunMate.Shared.Auth/RunMate.Shared.Auth.csproj", "RunMate.Shared.Auth/"]
RUN dotnet restore "RunMate/RunMate.UserService.csproj"
COPY . .
WORKDIR "/src/RunMate"
RUN dotnet build "RunMate.UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RunMate.UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RunMate.UserService.dll"]