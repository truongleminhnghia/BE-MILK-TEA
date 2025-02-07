# Sử dụng .NET SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy tất cả project vào container
COPY *.sln ./
COPY Repositories/*.csproj ./Repositories/
COPY Services/*.csproj ./Services/
COPY WebApi/*.csproj ./WebApi/

# Restore dependencies
RUN dotnet restore

# Copy toàn bộ source code
COPY . .

# Build ứng dụng
RUN dotnet publish WebApi/WebApi.csproj -c Release -o /out

# Sử dụng runtime để chạy app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Chạy ứng dụng WebApi
ENTRYPOINT ["dotnet", "WebApi.dll"]
