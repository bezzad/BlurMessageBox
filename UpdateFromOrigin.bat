@echo off
Echo Start Updating ...

git checkout master
git fetch
git status
git pull

Echo Update Completed.

Echo ---------------------------------------------

Echo Start Building Project ...

IF NOT "%VS110COMNTOOLS%" == "" (call "%VS110COMNTOOLS%vsvars32.bat")
IF NOT "%VS120COMNTOOLS%" == "" (call "%VS120COMNTOOLS%vsvars32.bat")
IF NOT "%VS130COMNTOOLS%" == "" (call "%VS130COMNTOOLS%vsvars32.bat")
IF NOT "%VS140COMNTOOLS%" == "" (call "%VS140COMNTOOLS%vsvars32.bat")

for /F %%A in ('dir /b src\*.sln') do call devenv src\%%A /Rebuild "Release" 

Echo ---------------------------------------------
pause