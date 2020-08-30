@echo off
echo 'Create controller utility'
echo 'Create an empty Controller'

set /p controller_name="Set Controller Name:"
echo "%controller_name%"

dotnet aspnet-codegenerator controller -name %controller_name% -outDir Controllers