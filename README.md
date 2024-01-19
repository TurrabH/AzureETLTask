### Schema:
![schema](https://github.com/TurrabH/csharp-sql-architect-task/assets/151545901/3ed9f457-64d3-41a1-a488-bc3173cd26b9)

### Dependencies
Make sure you have the following dependencies on your computer, in order to successfully run the project locally:
1. Visual Studio 2022
1. .NET 8 Runtime
1. Azure Cloud portal credentials that have an access to a tenant with an active subscription.

### Setup
Firstly, login to Visual Studio with the right cloud portal credentials that have an access to a tenant with an active subscription.

![image (17)](https://github.com/TurrabH/csharp-sql-architect-task/assets/151545901/f460e50a-8ae6-4534-aed5-44338088bb13)

Since, we were in a time-constraint and resource-bound environment, configuring Pulumi or Terraform to set up the infrastructure on Azure through code was not an option. This requires the step of configuring resources of Azure on the portal manually.
We need to make sure we have the following resources on the portal configured with the same names as mentioned followed:
![Screenshot 2024-01-18 152629](https://github.com/TurrabH/csharp-sql-architect-task/assets/151545901/036ff60c-9b99-4cfc-b08f-1ad198354c1a)

After that, make sure the Turrab.ETLTask.Core is set up as the startup project and start the application. If everything works fine, the user should be prompted with the loading screen.

![image (18)](https://github.com/TurrabH/csharp-sql-architect-task/assets/151545901/5772fb96-ede1-4525-9add-02ba1f7d38bc)


Once the KeyVault is configured by the code, the user is prompted with a menu with a set of options.

![image (19)](https://github.com/TurrabH/csharp-sql-architect-task/assets/151545901/eb9e5f5d-42f4-4726-ab59-cdf4f90cc852)

Running the project for the first time requires the user to run the option 3 (Apply Migrations), in order to set up all the tables and other dependencies on the cloud correctly.
Afterwards, the user can go through other options like datafactory options or search products as per his/her convenience.

![image (20)](https://github.com/TurrabH/csharp-sql-architect-task/assets/151545901/a0b5fad4-02c2-4afe-adb7-fd537d80a05b)

### Technical Documentation
Following is the documentation that contains the technical decisions and trade-offs made during the development process, as well as any challenges encountered and solutions implemented.
https://docs.google.com/document/d/1RYA97nbnN5w8mv7vGLvOuPOR8XCjqGzqsRrm44eRDwY/edit?usp=sharing



