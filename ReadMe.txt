C:\Windows\System32>
C:\Windows\System32>"C:\Windows\System32\inetsrv\appcmd.exe" stop site /site.name:"QL_Laptop"
"QL_Laptop" successfully stopped

C:\Windows\System32>"C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" "D:\Chien67\Web_ThuePC\DATN_Web\DATN_Web.csproj" /p:DeployOnBuild=true /p:PublishProfile=IISProfile /p:Configuration=Release

C:\Windows\System32>"C:\Windows\System32\inetsrv\appcmd.exe" start site /site.name:"QL_Laptop"
"QL_Laptop" successfully started.