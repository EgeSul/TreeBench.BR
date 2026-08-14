FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "TreeBench.API/TreeBench.API.csproj"
RUN dotnet publish "TreeBench.API/TreeBench.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

EXPOSE 5174
ENV ASPNETCORE_URLS=http://+:5174

COPY --from=build /app/publish .

# Sistemi ateşle
ENTRYPOINT ["dotnet", "TreeBench.API.dll"]