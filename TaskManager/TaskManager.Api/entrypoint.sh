#!/bin/bash

cd /src/TaskManager.Api || exit

dotnet ef migrations add InitialCreate
dotnet ef database update

cd /app/api || exit
dotnet TaskManager.Api.dll