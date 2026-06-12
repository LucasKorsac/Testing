@echo off
cd /d "C:\Users\ПК\source\repos\LucasKorsac\Testing"
echo Copying ABLibrary.dll to wwwroot/download...

set source=ABLibrary\bin\Release\netstandard2.0\ABLibrary.dll
set target=WebAppTest\wwwroot\download\ABLibrary.dll

if not exist "%source%" (
    echo Building ABLibrary...
    dotnet build ABLibrary\ABLibrary.csproj -c Release -f netstandard2.0
)

if not exist "%source%" (
    echo Build failed or DLL not found!
    pause
    exit /b 1
)

if not exist "WebAppTest\wwwroot\download" (
    mkdir WebAppTest\wwwroot\download
)

copy /Y "%source%" "%target%"

if exist "%target%" (
    echo Success! DLL copied to WebAppTest\wwwroot\download\ABLibrary.dll
) else (
    echo Failed to copy DLL
)

pause