#!/bin/bash
dotnet --info

echo disabling Fody because it is not needed for CI

cd src
dotnet remove Stratis.Bitcoin/Stratis.Bitcoin.csproj package Tracer.Fody

echo STARTED dotnet build
dotnet build -c Release ${path} -v m

