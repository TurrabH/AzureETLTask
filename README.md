### Schema:

![schema](https://github.com/TurrabH/AzureETLTask/assets/151545901/93309614-693c-45e1-987a-737b0a145614)

### Dependencies
Make sure you have the following dependencies on your computer, in order to successfully run the project locally:
1. Visual Studio 2022
1. .NET 8 Runtime
1. Azure Cloud portal credentials that have an access to a tenant with an active subscription.

### Setup
Firstly, login to Visual Studio with the right cloud portal credentials that have an access to a tenant with an active subscription.

![image (1)](https://github.com/TurrabH/AzureETLTask/assets/151545901/444c61ac-7612-4561-83a0-9b71315a224f)

Since, we were in a time-constraint and resource-bound environment, configuring Pulumi or Terraform to set up the infrastructure on Azure through code was not an option. This requires the step of configuring resources of Azure on the portal manually.
We need to make sure we have the following resources on the portal configured with the same names as mentioned followed:

![Screenshot 2024-01-18 152629](https://github.com/TurrabH/AzureETLTask/assets/151545901/431df30a-e5a1-4807-9ae5-81367bbee855)

After that, in the Visual Studio, make sure the Turrab.ETLTask.Core is set up as the startup project and start the application. If everything works fine, the user should be prompted with the loading screen.

![image (21)](https://github.com/TurrabH/AzureETLTask/assets/151545901/cfa46520-f983-420b-a290-0410987f2559)


![image (2)](https://github.com/TurrabH/AzureETLTask/assets/151545901/e565adc3-cd26-4955-881b-10b8268d0163)

Once the KeyVault is configured by the code, the user is prompted with a menu with a set of options.

![image (19)](https://github.com/TurrabH/AzureETLTask/assets/151545901/ebcb50b8-d020-4db8-a8ff-6b9d90fcec7e)

Running the project for the first time requires the user to run the option no. 3 (Apply Migrations), in order to set up all the tables and other dependencies on the cloud correctly.
Afterwards, the user can go through other options like datafactory options or search products as per his/her convenience.

![image (20)](https://github.com/TurrabH/AzureETLTask/assets/151545901/70586c30-44e3-4c20-b275-a4b278b8723f)


### Technical Documentation
Following is the documentation that contains the technical decisions and trade-offs made during the development process, as well as any challenges encountered and solutions implemented.
https://docs.google.com/document/d/1RYA97nbnN5w8mv7vGLvOuPOR8XCjqGzqsRrm44eRDwY/edit?usp=sharing



