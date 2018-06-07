#!/bin/bash
dotnet --info
echo STARTED dotnet build
cd src
dotnet build -c Release ${path} -v m

