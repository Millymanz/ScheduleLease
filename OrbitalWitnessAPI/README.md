# Schedule Lease Notice Microservice

# Description 
This project consists of a microservice project called the Schedule Lease Notice Service.
It's job is to provide easy access and organised access to schedule lease notice data, which it consumes from another serice in form of raw schedule lease data.

The solution contains the following projects (excluding unit test projects):
* OrbitalWitnessAPI, containing the microservice api controller, interaces and concrete implementation of these interface classes
* OrbitalWitness.Tests, containing the unit tests

# Run Instructions

## Requirements
The project was implemented with .NET 6. 
Please ensure you .NET 6 SDK installed on your machine.  

## To run with VSStudio
1. Open the OrbitalWitnessAPI.sln, this should load the required projects.
1. If not already set, you will be required to set the startup projects, this is done by right clicking on the solution name in the solution explorer. You in the context dialog, select "Set Startup Projects...". Under startup project, select Multiple startup projects, then select OrbitalWitnessAPI and set the Action to Start and do the same for EyeExamApi, hit ok. It is essential that the EyeExamApi is running alongside the OrbitalWitnessAPI.
1. Hit the Debug. 
1. When the services start they will load in your default browser and you should see swagger pages for both EyeExamApi and OrbitalWitnessAPI. 
1. You can test the OrbitalWitnessAPI by clicking on the Lease API Get button, this will load the data for schedule lease notices.

## Auth
The API uses Basic auth and the credentials can be find the in the appSettings json.

## Unit tests
All unit tests were implemented with the help of the xUnit testing framework and Fluent Assertions.


# Things to observe
## Consistent standards
This code uses consistent standards, this can be observed in things such as class private memeber variables are all denoted with underscore for example.
Most classes have an interace, in order to accomodate dependency injection.

## SOLID Principles
All throughout the application SOLID principles are followed, for example the ScheduleParser - has a single responsibility.
All the interfaces implemented do not enforce unwanted methods to a class.
The code is kept clean and testable through the use of dependency injection.

The SingletonOperationCache is a loss form of a singleton responsible for handling the caching of data within the service, this is done by adding/registering it as a singleton service.

## Accurate Result Set and Test Coverage
The unit testing demonstrates the accuracy of the parser and the test coverage.


## Future
Things that can be considered for future development include the following:-
1. Authentication - this should be implemented in order to access the schedule lease notice data
2. Secrets - both the EyeExamApi and OrbitalWitnessAPI should adopt secrets in order to manage and hide sensitive login details within the appsettings.json
3. Extension of API - the current implementation only considers providing all the schedule lease notice, there is scope to cap the amount provided and using pagination in the data. There is room to implement fetching the data by entry number or fetching the data by LesseesTitle.
Other things that can also be considered is better management of the caching of the data, in the event that there are new entries on the EyeExamApi, the current implementation will not accommodate these new entries unless there is a complete restart of the service freeing up the singleton/cache. So this can be extended to always check for new data, and updating the cache, and the same for removing outdated entries from the cache.
4. There is room for better logging using things like log4net, and logging errors in database or in file for error tracking.


## Production
It will be good idea to wrap the services into docker containers, and utilise the publish feature in visual studio to deploy the services for use.
For major production senarios we can host the services in kubernates in the cloud using AWS or Azure, and with the use of Helm.


