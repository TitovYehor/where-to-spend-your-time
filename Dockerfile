FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY ./backend/WhereToSpendYourTime.Api/*.csproj ./backend/WhereToSpendYourTime.Api/
COPY ./backend/WhereToSpendYourTime.Data/*.csproj ./backend/WhereToSpendYourTime.Data/
COPY ./backend/WhereToSpendYourTime.ShareLib/*.csproj ./backend/WhereToSpendYourTime.ShareLib/

RUN dotnet restore

COPY . ./

WORKDIR /app/backend/WhereToSpendYourTime.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 10000
ENTRYPOINT ["dotnet", "WhereToSpendYourTime.Api.dll"]