# Installation of Azure Function


## SQL Server Preparation

```
ALTER DATABASE poc_database
SET CHANGE_TRACKING = ON  
(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);

ALTER TABLE poc_table  
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON);
```


## Required Packages


```
dotnet add package System.Text.Json
dotnet add package System.Data.SqlClient
```

## Required Additional Files

- add local.settings.json : to store your azure blob storage and SQL Server

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "WEBSITE_SITE_NAME": "pocfunction-local",
    "SqlConnectionString": "Server=Server;Database=Database;Integrated Security=true;Encrypt=false;TrustServerCertificate=true;",
    "CPI_ENDPOINT": "CPI_ENDPOINT",
    "CPI_USERNAME": "CPI_CLIENT_ID",
    "CPI_PASSWORD": "CPI_SECRET"
  }
}

```


## Start the Azure Function

```
dotnet build
func start
```




## Deploy to Azure Cloud

### Deploy New Azure Function
```
az login
az functionapp create --resource-group MyResourceGroup --consumption-plan-location eastus --runtime python --name MyFunctionApp --storage-account mystorageaccount
func azure functionapp publish MyFunctionApp
```

### Deploy to Existing Azure Function

```
az functionapp deployment source config-zip -g <resource-group> -n <function-app-name> --src <zip-path>

```
