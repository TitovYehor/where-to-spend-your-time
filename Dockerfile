FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.sln ./
COPY ./WhereToSpendYourTime.Api/*.csproj ./WhereToSpendYourTime.Api/
COPY ./WhereToSpendYourTime.Data/*.csproj ./WhereToSpendYourTime.Data/
COPY ./WhereToSpendYourTime.ShareLib/*.csproj ./WhereToSpendYourTime.ShareLib/

RUN dotnet restore

COPY . ./

WORKDIR /app/WhereToSpendYourTime.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 10000
ENTRYPOINT ["dotnet", "WhereToSpendYourTime.Api.dll"]