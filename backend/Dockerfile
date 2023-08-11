FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ./Refugio/Refugio.csproj ./Refugio/
COPY ./Refugio.DataBase/Refugio.DataBase.csproj ./Refugio.DataBase/
COPY ./Refugio.DataBase.Models/Refugio.DataBase.Models.csproj ./Refugio.DataBase.Models/
COPY ./Refugio.Adapter/Refugio.Adapter.csproj ./Refugio.Adapter/
COPY ./Refugio.Interfaces/Refugio.Interfaces.csproj ./Refugio.Interfaces/
COPY ./Refugio.Models/Refugio.Models.csproj ./Refugio.Models/
COPY ./VKAPI/VKAPI.csproj ./VKAPI/
COPY ./Refugio.Common/Refugio.Common.csproj ./Refugio.Common/
RUN dotnet restore Refugio/Refugio.csproj

COPY . .

WORKDIR /app/Refugio
RUN dotnet build -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "Refugio.dll"]