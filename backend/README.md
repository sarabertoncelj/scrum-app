
# SMRPO BACK-END

  

## Development environment

Developed in Visual Studio 2019 using Microsoft .NET technologies.

  

## Database

**MySQL** is used for persistent data storage. Development instance of which can be started as a docker container.

```bash

$ docker run --name mysql -e MYSQL_ALLOW_EMPTY_PASSWORD=yes -e MYSQL_DATABASE=smrpo -d -p 3306:3306 mysql:latest

```

or can be set up locally on the testing machine.

Database connection settings are defined in `appsettings.json`.

  

**Update/setup** MySQL after an instance is active and accessible.

To create or change the database tables trigger the following command:

```bash

$ Update-Database

```

in PackageManager or

```bash

$ dotnet ef database update

```

in Dotnet CLI.

This executes the last migration file created by the migration action and applies changes to the database schema.

  

If you change data transfer object (models) you need to perform a migration by triggering the following command:

```bash

$ Add-Migration <name>

```

in PackageManager or

```bash

$ dotnet ef migrations add <name>

```

in Dotnet CLI.

This creates a new migration class as per specified `<name>`

  

In Visual Studio 2019 PackageManager is located in `Tools > NuGet Package Manager > Package Manager console`.

  

## Run

In Visual Studio 2019 select the `smrpo-be` configuration when running the application. For testing purposes a generated Swagger will open in default browser.

  

## Initial Data

Data will seed automatically.

**Users**
2 users will be created:

- Administrator: username&password: admin

- User: username&password: user

**Project**
1 Project with following properties will be created:

	Name: Seed Project
	ProjectUsers: 
	 - seedPO (ProjectOwner) with password: seed
	 - seedSM (ScrumMaster) with password: seed
	 - seedM (Member) with passwords: seed
	UserStories:
	 - #1 Test user story (inside sprint)
	 - #2 Test user story
	Sprints:
	 - Sprint 1 (#1 Test user story)
	 - Sprint 2 (empty)

Only administrator can create a project. Then he can manage projects users. If project has a Scrum Master, he can manege users aswell.

Scrum master can create sprints and add user stories to it.


## Docker

**1. Build/Run backend**
```bash
$ docker build -t smrpo-be .
$ docker run -p 8000:80 smrpo-be
```
**2. Pull/Run Database**
```bash
$ docker run --name smrpo-db -e MYSQL_ROOT_PASSWORD=smrporoot -d mysql
```

**3. Create network and connect**
```bash
$ docker network create smrpo-network
$ docker network connect smrpo-network smrpo-db
$ docker network connect smrpo-network smrpo-be
```

**4. Restart backend to apply database migrations**
```bash
$ docker restart smrpo-be
```

**5. Try Swagger** http://{YOUR DOCKER IP}:8000/swagger/index.html

## Docker Hub
```bash
$ docker build -t {repository}/smrpo-be:{tag} .
$ docker push {repository}/smrpo-be:{tag}
```

## AWS
1. Create EC2 instance
2. Add TCP, HTTP, HTTPS, SSH inbound rules
3. install packages
```bash
$ sudo yum update -y
$ sudo yum install docker -y
$ sudo usermod -a -G docker ec2-user
```
Run commands in "Docker" section


