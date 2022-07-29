# DCT Project

DCT Project is a windows application which design for collecting user's hardware information and feedback it to server.

This application will inject the background application to monitor your hardware usage and have uninstaller to help you uninstall it.

## How it works ##
- First
-> We build in the necessary binary execution file into DCTClient.exe and when user run it, it will copy all files above into user's device and registry task schedule to start up the background application every system boot.
- Second
-> We simulate the uninstall behavior to registry unstall key in system, so that user can find the entry in add-remove program page and uninstall it.

## Project Environment ##
- Visual Studio 2019 with dotnet framework 4.5