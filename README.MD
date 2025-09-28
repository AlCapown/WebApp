
## Install .NET WebAssembly build tools
```ps
dotnet workload install wasm-tools
```

## Creating A Migration

1. Build -> Rebuild Solution.
2. Open Tools -> NuGet Package Manager -> Package Manager Console.
3. Change default project to Database\WebApp.Database in the NuGet Package Manager drop down.
4. Execute the following command after changing the migration name to something descriptive.

```ps
Add-Migration "EnterDescriptiveName"
```

## Dev Certs
1. Open Tools -> NuGet Package Manager -> Package Manager Console.
2. Run the folling 
```ps
dotnet dev-certs https --clean
dotnet dev-certs https --trust
dotnet dev-certs https --check
```

## Docker Images

1. Save Image
```ps
$desktopPath = [Environment]::GetFolderPath("Desktop")
docker save -o "$desktopPath\image.tar" webappserver:latest
```

2. Copy Image to server
3. Load Image on server
```bash
docker load -i "$HOME/Desktop/image.tar"
```

## Creating and restoring a MSSQL backup file from inside a container
1. Create backup file .bak from SSMS
2. Copy to desktop
3. Run in home dir
```bash
sudo docker cp Desktop/WebApp.bak  mssql:/var/opt/mssql/backup
```
4. Restore backup from SSMS. Look in `/mssql/backup` folder for .bak. Make sure to check the box to overwrite current database.