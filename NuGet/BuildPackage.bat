@echo off

echo Building NuGet package...
nuget pack MicroTweet.nuspec

rem If an error occurred, pause so the user can see it
if %errorlevel% neq 0 (pause) else (echo Done.)
