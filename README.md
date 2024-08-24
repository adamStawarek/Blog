##	Database changes
###	Managing migrations
Migrations are managed (added, removed, applied, reverted) through EF Core command-line tools.  
Navigate to the migrations project (Blog.Infrastructure.DatabaseMigrations) in a command prompt to work with project migrations.  

Below is the list of popular commands:  
1.	Add new migration:  
`dotnet ef migrations add {MIGRATION_NAME} --startup-project "./../Blog.Server/Blog.Server.csproj"`  
2.	Remove pending migration (possible only if database was not updated yet):   
`dotnet ef migrations remove --startup-project "./../Blog.Server/Blog.Server.csproj"`  
3.	Update database:  
`dotnet ef database update --startup-project "./../Blog.Server/Blog.Server.csproj"`  
