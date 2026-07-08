FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HermeSoft_Fusion.csproj .
RUN dotnet restore "HermeSoft_Fusion.csproj"

COPY . .
RUN dotnet publish "HermeSoft_Fusion.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:3000
EXPOSE 3000

ENTRYPOINT ["dotnet", "HermeSoft_Fusion.dll"]
