FROM mcr.microsoft.com/dotnet/sdk:5.0.300 AS build
WORKDIR /app
COPY . .
RUN dotnet publish "src/ImageCreatorWebAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/publish .
RUN apt-get update -y && apt-get install -y curl
HEALTHCHECK --interval=5s --timeout=3s CMD curl --fail http://localhost:80/api/health || exit 1
EXPOSE 80
ENTRYPOINT ["dotnet", "ImageCreatorWebAPI.dll"]
