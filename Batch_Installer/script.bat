@ echo off

xcopy features\* "C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\TEMPLATE\FEATURES" /s /c /q /h /r /y

gacutil.exe /i /f gac\ITXWorkflowAppDeployment.dll 

gacutil.exe /i /f gac\Ionic.Zip.dll 

gacutil.exe /i gac\ITXProjectsLibrary.dll 

stsadm.exe -o installfeature -filename "ITSWorkflowAppDeployment\feature.xml"

iisreset

pause