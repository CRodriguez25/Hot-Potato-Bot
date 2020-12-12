FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .
WORKDIR /source/PotatoBot.Discord

# Disable Parallel because apparently there's some issue with restoring...
RUN dotnet restore --disable-parallel
RUN dotnet publish --no-restore -c release -o /app 

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS api
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "PotatoBot.Discord.dll"]