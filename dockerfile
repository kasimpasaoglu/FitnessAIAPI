# 1. Base image: .NET 9 ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# 2. SDK image: .NET 9 SDK (build & publish aşaması için)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et
COPY ["API.csproj", "./"]
RUN dotnet restore "API.csproj"

# Tüm kaynakları kopyala ve publish et
COPY . .
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# 3. Final image: runtime + publish edilmiş dosyalar
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "API.dll"]
