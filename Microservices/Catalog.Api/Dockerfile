FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY . .
# COPY ../Common ./Common
# WORKDIR /Microservices/Microservices/Catalog/Catalog.Api
# -nowarn:msb3202,nu1503
RUN dotnet restore
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]

# COPY entrypoint.sh /usr/local/bin/
# RUN ln -s usr/local/bin/entrypoint.sh / # backwards compat
# ENTRYPOINT [ "entrypoint.sh" ]

# RUN chmod +x ./src/entrypoint.sh
# CMD /bin/bash ./src/entrypoint.sh
