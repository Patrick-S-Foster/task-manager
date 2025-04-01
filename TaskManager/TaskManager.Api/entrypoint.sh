#!/bin/bash

dotnet ef migrations add InitialCreate
dotnet ef database update

cd /app/api/publish || exit
dotnet TaskManager.Api.dll