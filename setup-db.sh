# docker pull microsoft/mssql-server-linux:2017-latest
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Cobra1234" -p 1401:1433 --name sql2 microsoft/mssql-server-linux:2017-latest