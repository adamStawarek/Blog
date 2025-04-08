# Maintenance guide

## Infrastructure 

Before one can start the application, additional tools needs to be up and running like 
sql server and seq.

Prerquisute: 
1. Install [podman](https://podman-desktop.io/docs/installation) (or docker).  
2. From the root directory navigate to `infrastructure/development`.  
2. Open terminal and ensure podman is running (`podman machine start`).  

Below are serveral most common commands: 
* Run services: `podman compose --env-file dev.env`  
* Stop services: `podman compose stop`  
* Remove containers: `podman compose down && podman container prune -f`
* Remove containers with volumes: `podman compose down -v && podman container prune -f`  

Seq:  
>	**Url**: [http://localhost:5555](http://localhost:5341)  

Database:  
>	**Server**: 127.0.0.1,1533  
>	**User**: sa  
>	**Password**: Password#123  

To use sql server already that is already installed on the machine instead of podman container replace connection 
string from appsettings.development.json with following value:   
`Data Source=.;Initial Catalog=BlogDb;Integrated Security=true;TrustServerCertificate=True`.

### Running web application with compose

Development (no build):
```docker
podman compose --env-file dev.env -f docker-compose.yml -f docker-compose.dev.yml up
```

Development (with build):
```docker
podman compose --env-file dev.env -f docker-compose.yml -f docker-compose.dev.yml up --build
```

Production:
```docker
podman compose --env-file prod.env -f docker-compose.yml -f docker-compose.prod.yml up
```

##	Database changes

### Install dotnet ef tools:  
`dotnet tool install --global dotnet-ef`

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

### Frontend project organization

The organization of angular modules is based on this [website](https://medium.com/@marketing_26756/angular-best-practices-tips-for-project-structure-and-organization-490ca7950829).  
In general, the aim is to have flat folder structure with compnents that can be easly located,
even by person who is new to the project.

> **app**

>> **core**
   
>> **shared**   

>> **features**  

>>> appointments  

>>>> components 

>>>> dialogs 

>>>> directives  

>>>> pipes  

>>>> pages  

>>>> services   

>>>> appointments.module.ts  
 

**Core** module: core functions, global services and models that don't have any
relations to the feature module. One must import the core module **only** in the app root module.

**Shared** module: components, directives and pipes shared accross various modules. 
The shared module should **not** depend on any other module in the application.


## Google Cloud Platform
Google provides multiple credential types, but for server-to-server authentication (Client Credentials Flow), you must use a Service Account Key.

1. Go to Google Cloud Console:  
	- Navigate to Google Cloud Console.
	- Open your project where the Google Drive API is enabled.

2. Create a Service Account:
	- Go to IAM & Admin → Service Accounts.
	- Click Create Service Account.
	- Assign the role: "Editor" or "Drive File Access".
	- Click Create and Continue → Done.
	
3. Download the Correct JSON Key:
	- Select the Service Account you created.
	- Go to the "Keys" tab → Click "Add Key" → "Create new key".
	- Choose JSON format and download the file.
	- Place the file in your project (path/to/service_account.json).

Even if the authentication works, your service account must have permission to upload files.

4. Go to Google Drive.
	- Create a folder for uploads.
	- Right-click the folder → Share.
	- Add the service account email (found in your JSON) as an editor.