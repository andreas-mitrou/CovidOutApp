@echo off
echo 'Create controller utility'

set /p controller_name="Set Controller Name:"
set /p model_name="Set Model Name:"

echo "%controller_name%"
echo "%model_name%"
dotnet aspnet-codegenerator controller -name %controller_name% -actions -m "%model_name%" -dc "CovidOutApp.Web.Data.ApplicationDbContext" -outDir Controllers