#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV DOCKER="true"

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OTAWebApp/OTAWebApp.csproj", "OTAWebApp/"]
RUN dotnet restore "OTAWebApp/OTAWebApp.csproj"
COPY . .
WORKDIR "/src/OTAWebApp"
RUN dotnet build "OTAWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OTAWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OTAWebApp.dll"]