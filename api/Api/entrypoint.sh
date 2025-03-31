#!/bin/bash

dotnet ef migrations add InitialCreate
dotnet ef database update

cd /app/publish || exit
dotnet Api.dll