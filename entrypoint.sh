#!/bin/bash

dotnet restore
dotnet build -c ${BUILD_CONFIGURATION} --no-restore

dotnet run -c ${BUILD_CONFIGURATION} --no-build