#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ComicApi/ComicApi.csproj", "ComicApi/"]
RUN dotnet restore "ComicApi/ComicApi.csproj"
COPY . .
WORKDIR "/src/ComicApi"
RUN dotnet build "ComicApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ComicApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ComicApi.dll"]