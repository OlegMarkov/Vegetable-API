using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Vegetable.Core.Extensions;
using Vegetable.Entities;

namespace Vegetable.UnitTests.API
{
    [TestClass]
    public class VegetableWebApi
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        public VegetableWebApi()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseConfiguration(config).UseStartup<TestStartup>();
            });

            // Build and start the IHost
            _host = hostBuilder.Start();

            // Create an HttpClient to send requests to the TestServer
            _client = _host.GetTestClient();
            
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            TestStartup.CurrentOwner = Guid.Parse("dc56f0d9-d152-4b6e-8044-c7c62a1a4216");
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCRUDOwner()
        {

            var service = new Service()
            {
                Title = "Service",
                Description = "Description",
                Cost = 100M,
                // Duration = 15
            };

            // Act
            var owner = new Owner
            {
                Title = "Title",
                Description = "Description",
                Email = "email@email.com",
                Alias = "TestAlias",
                Services = new Service[] { service }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");            
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;

            TestStartup.CurrentOwner = createdOwner.Id;
            // Assert
            Assert.AreEqual(createdOwner.Title, owner.Title);
            Assert.AreEqual(createdOwner.Description, owner.Description);
            Assert.AreEqual(createdOwner.Email, owner.Email);
            Assert.AreEqual(createdOwner.Alias, owner.Alias);
            Assert.IsNotNull(createdOwner.Id);

            owner.Title = "Updated String";
            owner.Description = "Updated String";
            owner.Email = "Updated String";
            owner.Alias = "Updated String";

            var updateOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");           
            var updateResp = await _client.PutAsync("/owner/", updateOwner);
            updateResp.EnsureSuccessStatusCode();
            var updatedResult = GetObjectFromUrl<Owner>("/owner").Result;
            Assert.AreEqual(updatedResult.Description, owner.Description);

            //var deleteResp = await _client.DeleteAsync("/owner?Id=" + createdOwner.ID.ToString());
            //deleteResp.EnsureSuccessStatusCode();

            //var getNull = GetObjectFromUrl<Owner>("/owner/" + createdOwner.ID.ToString()).Result;
            //Assert.IsNull(getNull.Description);
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateService()
        {

            TestStartup.CurrentOwner = Guid.Parse("e6577ec5-c4ca-4637-86a7-3f7a8ad7d063");

            var newService = new Service()
            {
                Title = "Стрижка головы",
                Description = "Haircut from the best professionals",
                Cost = 156M,
                UsersCount = 10
            };

            //Act

            var jsonService = new StringContent(JsonConvert.SerializeObject(newService), Encoding.UTF8, "application/json");            
            var response = await _client.PostAsync("/owner/service", jsonService);
            var createdService = GetObjectFromResponse<Service>(response).Result;

            newService = new Service()
            {
                Title = "Стрижка бороды",
                Description = "Haircut from the best professionals",
                Cost = 100M,
                UsersCount = 10
            };

            //Act

            jsonService = new StringContent(JsonConvert.SerializeObject(newService), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("/owner/service", jsonService);
            createdService = GetObjectFromResponse<Service>(response).Result;

            newService = new Service()
            {
                Title = "Бритьё",
                Description = "Haircut from the best professionals",
                Cost = 100M,
                UsersCount = 10
            };

            //Act

            jsonService = new StringContent(JsonConvert.SerializeObject(newService), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("/owner/service", jsonService);
            createdService = GetObjectFromResponse<Service>(response).Result;

            // Assert
            Assert.AreEqual(newService.Title, createdService.Title);
            Assert.AreEqual(newService.Description, createdService.Description);
            Assert.AreEqual(newService.Duration, createdService.Duration);
            Assert.AreEqual(newService.Cost, createdService.Cost);
            Assert.AreEqual(newService.UsersCount, createdService.UsersCount);
            Assert.IsNotNull(createdService.Id);

            var ownerWithNewService = GetObjectFromUrl<Owner>("/owner").Result;
            Assert.IsNotNull(ownerWithNewService.Services.FirstOrDefault(s => s.Id == createdService.Id));
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanUpdateService()
        {
            // Arrange

            var service = new Service()
            {
                Title = "Haircut",
                Description = "Haircut from the best professionals",
                Cost = 156M,
                //Duration = 30,
                UsersCount = 10
            };

            // Create Owner
            var owner = new Owner
            {
                Title = "Homer Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "hsimpson",
                Services = new Service[] { service }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);

            //Act

            var newService = new Service()
            {
                Title = "Haircut New",
                Description = "Haircut new Description",
                Cost = 101M,
                UsersCount = 32
            };

            var jsonService = new StringContent(JsonConvert.SerializeObject(newService), Encoding.UTF8, "application/json");            
            response = await _client.PutAsync(String.Format("/owner/service/{0}", createdOwner.Services.First().Id.ToString()), jsonService);  
            var updatedServiceByDirectLink = GetObjectFromUrl<Service>(String.Format("/owner/service/{0}", createdOwner.Services.First().Id)).Result;
            var ownerWithUpdatedService = GetObjectFromUrl<Owner>("/owner").Result;
            var serviceFromOwnerObject = ownerWithUpdatedService.Services.First();

            // Assert
            Assert.AreEqual(newService.Title, updatedServiceByDirectLink.Title);
            Assert.AreEqual(newService.Description, updatedServiceByDirectLink.Description);
            Assert.AreEqual(newService.Duration, updatedServiceByDirectLink.Duration);
            Assert.AreEqual(newService.Cost, updatedServiceByDirectLink.Cost);
            Assert.AreEqual(newService.UsersCount, updatedServiceByDirectLink.UsersCount);

            Assert.AreEqual(newService.Title, serviceFromOwnerObject.Title);
            Assert.AreEqual(newService.Description, serviceFromOwnerObject.Description);
            Assert.AreEqual(newService.Duration, serviceFromOwnerObject.Duration);
            Assert.AreEqual(newService.Cost, serviceFromOwnerObject.Cost);
            Assert.AreEqual(newService.UsersCount, serviceFromOwnerObject.UsersCount);
            Assert.IsNotNull(serviceFromOwnerObject.Id);

        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateEmployee()
        {
            // Arrange
            // Create Owner

            var newService = new Service()
            {
                Title = "Haircut",
                Description = "Haircut from the best professionals",
                Cost = 156M,
                UsersCount = 10
            };

            var owner = new Owner
            {
                Title = "Homer Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "hsimpson",
                Services = new Service[] { newService }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);



            var reservation = new Reservation()
            {
                StartTime = new DateTime(2020, 1, 22, 10, 15, 0),
                EndTime = new DateTime(2020, 1, 22, 10, 45, 0),
                Cost = 156M,
                ReservationServices = { new ReservationService() { Service = createdOwner.Services.First(), ServiceId = createdOwner.Services.First().Id } }
            };

            var newEmployee = new Employee()
            {
                FirstName = "Victor",
                LastName = "Cherevkov",
                StartOfWorkDate = DateTime.Now.AddDays(-4),
                EndOfWorkDate = DateTime.Now.AddDays(4),
                WorkingDays = Days.Friday | Days.Monday,
                Reservations = new Reservation[] { reservation }
            };


            //Act I. Create Employee

            var jsonEmployee = new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json");            
            response = await _client.PostAsync("/owner/employee", jsonEmployee);

            var createdEmployee = GetObjectFromUrl<Employee[]>("/owner/employee").Result.FirstOrDefault();

            //Act II. Delete Employee

            //await _client.DeleteAsync(String.Format("/owner/{0}/employee/{1}", createdOwner.ID.ToString(), createdEmployee.ID.ToString()));
            //var ownerWithDeletedEmployee = GetObjectFromUrl<Owner>("/owner/" + createdOwner.ID.ToString()).Result;

            // Assert
            Assert.AreEqual(createdEmployee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(createdEmployee.LastName, newEmployee.LastName);

            Assert.AreEqual(createdEmployee.StartOfWorkDate.Value.Date, newEmployee.StartOfWorkDate.Value.Date);
            Assert.AreEqual(createdEmployee.EndOfWorkDate.Value.Date, newEmployee.EndOfWorkDate.Value.Date);
            Assert.IsNotNull(createdEmployee.Id);
            Assert.AreEqual(createdEmployee.WorkingDays, newEmployee.WorkingDays);
            Assert.IsNotNull(createdEmployee.Reservations.FirstOrDefault());

            //Assert.IsNull(ownerWithDeletedEmployee.Employees.FirstOrDefault(s => s.ID == createdEmployee.ID));
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanUpdateEmployee()
        {
            // Arrange
            // Create Owner

            var newService = new Service()
            {
                Title = "Haircut",
                Description = "Haircut from the best professionals",
                Cost = 156M,
                //Duration = 30,
                UsersCount = 10
            };

            var owner = new Owner
            {
                Title = "Homer Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "hsimpson",
                Services = new Service[] { newService }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);



            var reservation = new Reservation()
            {
                StartTime = new DateTime(2020, 1, 22, 10, 15, 0),
                EndTime = new DateTime(2020, 1, 22, 10, 45, 0),
                Cost = 156M,
                ReservationServices = { new ReservationService() { Service = createdOwner.Services.First(), ServiceId = createdOwner.Services.First().Id} }
            };

            var newEmployee = new Employee()
            {
                FirstName = "Victor",
                LastName = "Cherevkov",
                StartOfWorkDate = DateTime.Now.AddDays(-4),
                EndOfWorkDate = DateTime.Now.AddDays(4),
                WorkingDays = Days.Friday | Days.Monday,
                Reservations = new Reservation[] { reservation }
            };


            //Act I. Create Employee

            var jsonEmployee = new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json");            
            response = await _client.PostAsync("/owner/employee", jsonEmployee);
            var createdEmployee = GetObjectFromResponse<Employee>(response).Result;

            //Act II. Update Employee

            createdEmployee.FirstName = "Joe";
            createdEmployee.LastName = "Gomees";
            createdEmployee.StartOfWorkDate = DateTime.Now.AddDays(-2);
            createdEmployee.EndOfWorkDate = DateTime.Now.AddDays(2);
            createdEmployee.WorkingDays = Days.Saturday | Days.Tuesday;


            jsonEmployee = new StringContent(JsonConvert.SerializeObject(createdEmployee), Encoding.UTF8, "application/json");            
            var updateResp = await _client.PutAsync(String.Format("/owner/employee/{0}", createdEmployee.Id.ToString()), jsonEmployee);
            updateResp.EnsureSuccessStatusCode();

            var ownerWithNewEmployee = GetObjectFromUrl<Owner>("owner/").Result;

            var employeeFromOwner = GetObjectFromUrl<Employee>(String.Format("/owner/employee/{0}", createdEmployee.Id.ToString())).Result;

            //Act III. Delete Employee
            
            await _client.DeleteAsync(String.Format("/owner/employee/{0}", createdEmployee.Id.ToString()));
            var ownerWithDeletedEmployee = GetObjectFromUrl<Owner>("/owner").Result;

            // Assert
            Assert.AreEqual(createdEmployee.FirstName, employeeFromOwner.FirstName);
            Assert.AreEqual(createdEmployee.LastName, employeeFromOwner.LastName);
            Assert.AreEqual(createdEmployee.StartOfWorkDate.Value.Date, employeeFromOwner.StartOfWorkDate.Value.Date);
            Assert.AreEqual(createdEmployee.EndOfWorkDate.Value.Date, employeeFromOwner.EndOfWorkDate.Value.Date);
            Assert.AreEqual(createdEmployee.WorkingDays, employeeFromOwner.WorkingDays);

            Assert.IsNotNull(ownerWithNewEmployee.Employees.FirstOrDefault(s => s.Id == createdEmployee.Id));
            Assert.IsNull(ownerWithDeletedEmployee.Employees.FirstOrDefault(s => s.Id == createdEmployee.Id));
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateOwnerWithAddress()
        {
            // Arrange
            // Create Owner

            var address = new Address()
            {
                City = "Springfield",
                State = "Texas",
                PostalCode = "92548",
                Description = "My main Address",
                Points = "43.098892, 132.538172",
                Street = "Evergreen terras 12",
                Unit = "Test unit"
            };

            var owner = new Owner
            {
                Title = "Bart Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "bsimpson",
                Addresses = new Address[] { address }
            };

            // Act

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            var ownerWithNewAddress = GetObjectFromUrl<Owner>("owner/").Result;

            var createdAddress = ownerWithNewAddress.Addresses.First();

            // Assert
            Assert.AreEqual(address.City, createdAddress.City);
            Assert.AreEqual(address.State, createdAddress.State);
            Assert.AreEqual(address.PostalCode, createdAddress.PostalCode);
            Assert.AreEqual(address.Description, createdAddress.Description);
            Assert.AreEqual(address.Points, createdAddress.Points);
            Assert.AreEqual(address.Street, createdAddress.Street);
            Assert.AreEqual(address.Unit, createdAddress.Unit);
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateAddress()
        {
            // Arrange
            // Create Owner
            var owner = new Owner
            {
                Title = "Lisa Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "lsimpson"
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);

            var address = new Address()
            {
                City = "Springfield",
                State = "Texas",
                PostalCode = "92548",
                Description = "My main Address",
                Points = "43.098892, 132.538172",
                Street = "Evergreen terras 12",
                Unit = "Test unit"
            };

            //Act

            var jsonService = new StringContent(JsonConvert.SerializeObject(address), Encoding.UTF8, "application/json");            
            response = await _client.PostAsync("owner/address", jsonService);
            address = GetObjectFromResponse<Address>(response).Result;

            var createdAddress = GetObjectFromUrl<Address>(String.Format("/owner/address/{0}", address.Id.ToString())).Result;

            // Assert
            Assert.AreEqual(address.City, createdAddress.City);
            Assert.AreEqual(address.State, createdAddress.State);
            Assert.AreEqual(address.PostalCode, createdAddress.PostalCode);
            Assert.AreEqual(address.Description, createdAddress.Description);
            Assert.AreEqual(address.Points, createdAddress.Points);
            Assert.AreEqual(address.Street, createdAddress.Street);
            Assert.AreEqual(address.Unit, createdAddress.Unit);

        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanUpdateAddress()
        {
            // Arrange
            // Create Owner
            var address = new Address()
            {
                City = "Springfield",
                State = "Texas",
                PostalCode = "92548",
                Description = "My main Address",
                Points = "43.098892, 132.538172",
                Street = "Evergreen terras 12",
                Unit = "Test unit"
            };

            var owner = new Owner
            {
                Title = "Bart Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "bsimpson",
                Addresses = new Address[] { address }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            address.Id = createdOwner.Addresses.FirstOrDefault().Id;
            address.City = "Moskow";
            address.Description = "Updated address";
            address.Street = "Arbat 112";
            address.State = "Moskow oblast";
            address.Unit = "Updated Unit";
            address.Points = "43.094492, 132.538172";

            //Act

            var jsonService = new StringContent(JsonConvert.SerializeObject(address), Encoding.UTF8, "application/json");            
            response = await _client.PutAsync(String.Format("owner/address/{0}", address.Id), jsonService);

            var updatedAddress = GetObjectFromUrl<Address>(String.Format("/owner/address/{0}", address.Id.ToString())).Result;
            
            //await _client.DeleteAsync(String.Format("owner/address/{0}", address.Id));

            // Assert
            Assert.AreEqual(address.City, updatedAddress.City);
            Assert.AreEqual(address.State, updatedAddress.State);
            Assert.AreEqual(address.PostalCode, updatedAddress.PostalCode);
            Assert.AreEqual(address.Description, updatedAddress.Description);
            Assert.AreEqual(address.Points, updatedAddress.Points);
            Assert.AreEqual(address.Street, updatedAddress.Street);
            Assert.AreEqual(address.Unit, updatedAddress.Unit);


        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreatePhone()
        {
            // Arrange
            // Create Owner
            var owner = new Owner
            {
                Title = "Lisa Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "lsimpson"
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);

            var phone = new PhoneNumber()
            {
                Number = "1234567890",
                Type = PhoneNumberType.Mobile
            };

            //Act

            var jsonService = new StringContent(JsonConvert.SerializeObject(phone), Encoding.UTF8, "application/json");            
            response = await _client.PostAsync("/owner/phonenumber", jsonService);
            phone = GetObjectFromResponse<PhoneNumber>(response).Result;

            var createdPhone = GetObjectFromUrl<PhoneNumber>(String.Format("/owner/phonenumber/{0}", phone.Id.ToString())).Result;

            // Assert
            Assert.AreEqual(phone.Number, createdPhone.Number);
            Assert.AreEqual(phone.Type, createdPhone.Type);

        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanUpdatePhone()
        {
            // Arrange
            // Create Owner
            var phone = new PhoneNumber()
            {
                Number = "1234567890",
                Type = PhoneNumberType.Mobile
            };

            var owner = new Owner
            {
                Title = "Bart Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "bsimpson",
                PhoneNumbers = new PhoneNumber[] { phone }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            phone.Id = createdOwner.PhoneNumbers.FirstOrDefault().Id;
            phone.Number = "0987654321";
            phone.Type = PhoneNumberType.Classic;

            //Act

            var jsonService = new StringContent(JsonConvert.SerializeObject(phone), Encoding.UTF8, "application/json");            
            response = await _client.PutAsync(String.Format("/owner/phonenumber/{0}", phone.Id), jsonService);

            var updatedPhone = GetObjectFromUrl<PhoneNumber>(String.Format("/owner/phonenumber/{0}", phone.Id.ToString())).Result;
            
            await _client.DeleteAsync(String.Format("/owner/phonenumber/{0}", phone.Id));

            // Assert
            Assert.AreEqual(phone.Number, updatedPhone.Number);
            Assert.AreEqual(phone.Type, updatedPhone.Type);


        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateSocialNetwork()
        {
            // Arrange
            // Create Owner
            var owner = new Owner
            {
                Title = "Lisa Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "lsimpson"
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);

            var social = new SocialNetwork()
            {
                Url = "facebooc.com/senio",
                Type = SocialNetworkTypes.Facebook
            };

            //Act

            var json = new StringContent(JsonConvert.SerializeObject(social), Encoding.UTF8, "application/json");            
            response = await _client.PostAsync("/owner/socialnetwork", json);

            social = GetObjectFromResponse<SocialNetwork>(response).Result;

            var createdSocial = GetObjectFromUrl<SocialNetwork>(String.Format("/owner/socialnetwork/{0}", social.Id.ToString())).Result;

            // Assert
            Assert.AreEqual(social.Url, createdSocial.Url);
            Assert.AreEqual(social.Type, createdSocial.Type);

        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanUpdateSocial()
        {
            // Arrange
            // Create Owner
            var social = new SocialNetwork()
            {
                Url = "facebooc.com/senio",
                Type = SocialNetworkTypes.Facebook
            };


            var owner = new Owner
            {
                Title = "Bart Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "bsimpson",
                SocialNetworks = new SocialNetwork[] { social }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            social.Id = createdOwner.SocialNetworks.FirstOrDefault().Id;
            social.Url = "vk.com/test";
            social.Type = SocialNetworkTypes.VK;

            //Act

            var json = new StringContent(JsonConvert.SerializeObject(social), Encoding.UTF8, "application/json");            
            response = await _client.PutAsync(String.Format("/owner/socialnetwork/{0}", social.Id), json);

            var updatedSocial = GetObjectFromUrl<SocialNetwork>(String.Format("/owner/socialnetwork/{0}", social.Id.ToString())).Result;
                        
            await _client.DeleteAsync(String.Format("/owner/socialnetwork/{0}", social.Id));

            // Assert
            Assert.AreEqual(social.Url, updatedSocial.Url);
            Assert.AreEqual(social.Type, updatedSocial.Type);


        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Lev",
                LastName = "Tolstoy",
                Email = "lev.tolstoy@gmail.cpc",
                Phone = "1234567890"
            };

            var newCustomer = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            var newCustomerResponse = await _client.PostAsync("customer", newCustomer);
            var createdCustomer = GetObjectFromResponse<Customer>(newCustomerResponse).Result;

            var foundCustomer = GetObjectFromUrl<Customer>("/customer/" + createdCustomer.Id.ToString()).Result;

            await _client.DeleteAsync(String.Format("/customer/{0}", createdCustomer.Id.ToString()));

            // Assert
            Assert.AreEqual(customer.FirstName, foundCustomer.FirstName);
            Assert.AreEqual(customer.LastName, foundCustomer.LastName);
            Assert.AreEqual(customer.Phone, foundCustomer.Phone);
            Assert.AreEqual(customer.Email, foundCustomer.Email);

        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateReservation()
        {
            var newService = new Service()
            {
                Title = "Haircut",
                Description = "Haircut from the best professionals",
                Cost = 156M,
                UsersCount = 10,
                DurationInMinutes = 20
            };

            var owner = new Owner
            {
                Title = "Homer Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "hsimpson",
                Services = new Service[] { newService }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);

            var newSchedule = new Schedule
            {               
                ScheduleStartDate = DateTime.Now.AddMonths(-1),
                ScheduleEndDate = DateTime.Now.AddMonths(1)                
            };

            var newEmployee = new Employee()
            {
                FirstName = "Victor",
                LastName = "Cherevkov",
                StartOfWorkDate = DateTime.Now.AddDays(-4),
                EndOfWorkDate = DateTime.Now.AddDays(4),
                WorkingDays = Days.Friday | Days.Monday,
                Schedules = new Schedule[] { newSchedule }
            };

            var customer = new Customer()
            {
                FirstName = "Dan",
                LastName = "Brown",
                Email = "danbrown@test.ru"
            };

            var reservation = new Reservation()
            {
                StartTime = new DateTime(2020, 1, 22, 10, 15, 0),
                EndTime = new DateTime(2020, 1, 22, 10, 45, 0),
                Cost = 156M,
                ReservationServices = { new ReservationService() { Service = createdOwner.Services.First(), ServiceId = createdOwner.Services.First().Id } },
                Customer = customer
            };


            //Act I. Create Employee

            var jsonEmployee = new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("/owner/employee", jsonEmployee);

            var createdEmployee = GetObjectFromUrl<Employee[]>("/owner/employee").Result.FirstOrDefault();

            //Act II. Create Reservation

            reservation.EmployeeId = createdEmployee.Id;
            var jsonReservation = new StringContent(JsonConvert.SerializeObject(reservation), Encoding.UTF8, "application/json");
            response = await _client.PostAsync(String.Format("/owner/reservation"), jsonReservation);

            createdEmployee = GetObjectFromUrl<Employee[]>("owner/employee").Result.FirstOrDefault();

            var reservationFromEmployee = createdEmployee.Reservations.Last();
            //Act II. Delete Employee

            //await _client.DeleteAsync(String.Format("/owner/{0}/employee/{1}", createdOwner.ID.ToString(), createdEmployee.ID.ToString()));
            //var ownerWithDeletedEmployee = GetObjectFromUrl<Owner>("/owner/" + createdOwner.ID.ToString()).Result;

            // Assert
            Assert.AreEqual(createdEmployee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(createdEmployee.LastName, newEmployee.LastName);

            Assert.AreEqual(createdEmployee.StartOfWorkDate.Value.Date, newEmployee.StartOfWorkDate.Value.Date);
            Assert.AreEqual(createdEmployee.EndOfWorkDate.Value.Date, newEmployee.EndOfWorkDate.Value.Date);
            Assert.IsNotNull(createdEmployee.Id);
            Assert.AreEqual(createdEmployee.WorkingDays, newEmployee.WorkingDays);
            Assert.IsNotNull(createdEmployee.Reservations.FirstOrDefault());
            //Assert.AreEqual(reservationFromEmployee.DateTimeRange.StartTime, reservation.DateTimeRange.StartTime);
            //Assert.AreEqual(reservationFromEmployee.DateTimeRange.EndTime, reservation.DateTimeRange.EndTime);
            Assert.AreEqual(reservationFromEmployee.Cost, reservation.Cost);

            //Assert.IsNull(ownerWithDeletedEmployee.Employees.FirstOrDefault(s => s.ID == createdEmployee.ID));
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanChangeReservationEmployee()
        {
            var newService = new Service()
            {
                Title = "Haircut",
                Description = "Haircut from the best professionals",
                Cost = 156M,
                UsersCount = 10,
                DurationInMinutes = 20
            };

            var owner = new Owner
            {
                Title = "Homer Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "hsimpson",
                Services = new Service[] { newService }
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);

            var newSchedule = new Schedule
            {
                ScheduleStartDate = DateTime.Now.AddMonths(-1),
                ScheduleEndDate = DateTime.Now.AddMonths(1)               
            };

            var newEmployee = new Employee()
            {
                FirstName = "Victor",
                LastName = "Cherevkov",
                StartOfWorkDate = DateTime.Now.AddDays(-4),
                EndOfWorkDate = DateTime.Now.AddDays(4),
                WorkingDays = Days.Friday | Days.Monday,
                Schedules = new Schedule[] { newSchedule }
            };

            var newEmployee2 = new Employee()
            {
                FirstName = "Second",
                LastName = "One",
                StartOfWorkDate = DateTime.Now.AddDays(-4),
                EndOfWorkDate = DateTime.Now.AddDays(4),
                WorkingDays = Days.Friday | Days.Monday,
                Schedules = new Schedule[] { newSchedule }
            };

            var customer = new Customer()
            {
                FirstName = "Dan",
                LastName = "Brown",
                Email = "danbrown@test.ru"
            };

            var reservation = new Reservation()
            {
                StartTime = new DateTime(2020, 1, 22, 10, 15, 0),
                EndTime = new DateTime(2020, 1, 22, 10, 45, 0),
                Cost = 156M,
                ReservationServices = { new ReservationService() { Service = createdOwner.Services.First(), ServiceId = createdOwner.Services.First().Id } },
                Customer = customer
            };


            //Act I. Create Employees
            var jsonEmployee = new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("/owner/employee", jsonEmployee);
            var createdEmployee = GetObjectFromResponse<Employee>(response).Result;

            var jsonEmployee2 = new StringContent(JsonConvert.SerializeObject(newEmployee2), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("/owner/employee", jsonEmployee);
            var createdEmployee2 = GetObjectFromResponse<Employee>(response).Result;

            //Act II. Create Reservation
            reservation.EmployeeId = createdEmployee.Id;
            var jsonReservation = new StringContent(JsonConvert.SerializeObject(reservation), Encoding.UTF8, "application/json");
            response = await _client.PostAsync(String.Format("/owner/reservation"), jsonReservation);
            var createdReservation = GetObjectFromResponse<Reservation>(response).Result;

            //Act III. Update Reservation
            createdReservation.EmployeeId = createdEmployee2.Id;
            jsonReservation = new StringContent(JsonConvert.SerializeObject(createdReservation), Encoding.UTF8, "application/json");
            response = await _client.PutAsync(String.Format("/owner/reservation/"+ createdReservation.Id), jsonReservation);

            createdEmployee = GetObjectFromUrl<Employee>("owner/employee/"+ createdEmployee.Id).Result;
            createdEmployee2 = GetObjectFromUrl<Employee>("owner/employee/" + createdEmployee2.Id).Result;

            // Assert
            Assert.IsTrue(createdEmployee.Reservations.Count == 0);
            Assert.IsTrue(createdEmployee2.Reservations.Count == 1);
            Assert.AreEqual(createdEmployee2.Reservations.FirstOrDefault().EmployeeId, createdEmployee2.Id);
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateSchedule()
        {
            //Assert

            var owner = new Owner
            {
                Title = "Homer Simpson",
                Description = "Barber",
                Email = "email@email.com",
                Alias = "hsimpson"
            };

            var newOwner = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/owner", newOwner);
            var createdOwner = GetObjectFromResponse<Owner>(response).Result;
            TestStartup.CurrentOwner = createdOwner.Id;

            Assert.IsNotNull(createdOwner);

            var newScheduleOne = new Schedule
            {                
                ScheduleStartDate = DateTime.Now.AddMonths(-1),
                ScheduleEndDate = DateTime.Now.AddMonths(1),
                OnDays = 5,
                OffDays = 2,
                ScheduleType = ScheduleType.Week,
                ScheduleOnDays = new List<ScheduleOnDay>() {
                    new ScheduleOnDay{ 
                        Sequence = 1,
                        WorkStartTime = new TimeSpan(8, 0, 0),
                        WorkEndTime = new TimeSpan(17, 0, 0),
                        BreakStartTime = new TimeSpan(12, 0, 0),
                        BreakEndTime = new TimeSpan(13, 0, 0)
                    },
                    new ScheduleOnDay{
                        Sequence = 2,
                        WorkStartTime = new TimeSpan(8, 0, 0),
                        WorkEndTime = new TimeSpan(17, 0, 0),
                        BreakStartTime = new TimeSpan(12, 0, 0),
                        BreakEndTime = new TimeSpan(13, 0, 0)
                    },
                    new ScheduleOnDay{
                        Sequence = 3,
                        WorkStartTime = new TimeSpan(8, 0, 0),
                        WorkEndTime = new TimeSpan(17, 0, 0),
                        BreakStartTime = new TimeSpan(12, 0, 0),
                        BreakEndTime = new TimeSpan(13, 0, 0)
                    },
                    new ScheduleOnDay{
                        Sequence = 4,
                        WorkStartTime = new TimeSpan(8, 0, 0),
                        WorkEndTime = new TimeSpan(20, 0, 0),
                        BreakStartTime = new TimeSpan(12, 0, 0),
                        BreakEndTime = new TimeSpan(13, 0, 0)
                    },
                    new ScheduleOnDay{
                        Sequence = 5,
                        WorkStartTime = new TimeSpan(8, 0, 0),
                        WorkEndTime = new TimeSpan(20, 0, 0),
                        BreakStartTime = new TimeSpan(12, 0, 0),
                        BreakEndTime = new TimeSpan(13, 0, 0)
                    }
                }
            };            

            var newEmployee = new Employee()
            {
                FirstName = "Victor",
                LastName = "Cherevkov",
                StartOfWorkDate = DateTime.Now.AddDays(-4),
                EndOfWorkDate = DateTime.Now.AddDays(4),
                WorkingDays = Days.Friday | Days.Monday              
            };

            //Act I. Create Employee

            var jsonEmployee = new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("/owner/employee", jsonEmployee);
            var createdEmployee = GetObjectFromResponse<Employee>(response).Result;
            newScheduleOne.EmployeeId = createdEmployee.Id;

            //Act II. Create Scedule

            var jsonSchedule = new StringContent(JsonConvert.SerializeObject(newScheduleOne), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("owner/schedule", jsonSchedule);
            newScheduleOne = GetObjectFromResponse<Schedule>(response).Result;
            createdEmployee = GetObjectFromUrl<Employee>(String.Format("/owner/employee/{0}", createdEmployee.Id)).Result;
            var createdSchedule = GetObjectFromUrl<Schedule>(String.Format("/owner/schedule/{0}", newScheduleOne.Id)).Result;
            var scheduleOneFromEmployee = createdEmployee.Schedules.FirstOrDefault(x => x.Id == newScheduleOne.Id);           
            
            //Act II. Delete Employee

            await _client.DeleteAsync(String.Format("/owner/{0}/employee/{1}", createdOwner.Id.ToString(), createdEmployee.Id.ToString()));
            var ownerWithDeletedEmployee = GetObjectFromUrl<Owner>("/owner/" + createdOwner.Id.ToString()).Result;

            // Assert
            Assert.AreEqual(createdEmployee.Schedules.Count, 1);
            Assert.AreEqual(createdEmployee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(createdEmployee.LastName, newEmployee.LastName);

            Assert.AreEqual(createdEmployee.StartOfWorkDate.Value.Date, newEmployee.StartOfWorkDate.Value.Date);
            Assert.AreEqual(createdEmployee.EndOfWorkDate.Value.Date, newEmployee.EndOfWorkDate.Value.Date);
            Assert.IsNotNull(createdEmployee.Id);
            Assert.AreEqual(createdEmployee.WorkingDays, newEmployee.WorkingDays);
            
            Assert.AreEqual(scheduleOneFromEmployee.OnDays, newScheduleOne.OnDays);
            Assert.AreEqual(scheduleOneFromEmployee.OffDays, newScheduleOne.OffDays);
            Assert.AreEqual(scheduleOneFromEmployee.ScheduleStartDate, newScheduleOne.ScheduleStartDate);
            Assert.AreEqual(scheduleOneFromEmployee.ScheduleEndDate, newScheduleOne.ScheduleEndDate);
            Assert.AreEqual(scheduleOneFromEmployee.ScheduleType, newScheduleOne.ScheduleType);
            Assert.AreEqual(scheduleOneFromEmployee.ScheduleOnDays.Count, newScheduleOne.ScheduleOnDays.Count);
            
            Assert.IsNull(ownerWithDeletedEmployee.Employees.FirstOrDefault(s => s.Id == createdEmployee.Id));
        }


        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanGetCurrencies()
        {
            var currencies = await GetObjectFromUrl<Currency[]>("/settings/currency");
            Assert.IsTrue(currencies.Any());
        }

        [TestCategory("Vegetable.API"), TestMethod]
        public async Task CanCreateNotification()
        {
          
            TestStartup.CurrentOwner = Guid.Parse("84fdb0ce-5e7b-49bf-b2cc-ffa41c4f9f13");

            
            var response = await _client.GetAsync("/owner/service");
            var services = GetObjectFromResponse<Service[]>(response).Result;

            var responseCustomer = await _client.GetAsync("/owner/customer/all");
            var customers = GetObjectFromResponse<Customer[]>(responseCustomer).Result;

            var responseEmployee = await _client.GetAsync("/owner/employee");
            var employees = GetObjectFromResponse<Employee[]>(responseEmployee).Result;

            var rs = new List<ReservationService>();
            rs.Add(new ReservationService() { Service = services[0], ServiceId = services[0].Id });

            var reservation = new Reservation()
            {
                StartTime = new DateTime(2021, 12, 15, 10, 15, 0),
                EndTime = new DateTime(2021, 12, 15, 10, 45, 0),
                Cost = 156M,
                ReservationServices = rs,
                Customer = customers[0]
            };

          

            reservation.EmployeeId = employees[0].Id;
            var jsonReservation = new StringContent(JsonConvert.SerializeObject(reservation), Encoding.UTF8, "application/json");
            var responseReservation = await _client.PostAsync(String.Format("/owner/reservation"), jsonReservation);

            reservation = GetObjectFromResponse<Reservation>(responseReservation).Result;


            var notification = new Notification()
            {
               NotificationDateUTC = DateTime.UtcNow.AddDays(-1),               
               ReservationId = reservation.Id,
               NotificationType = NotificationType.CancelReservationClient
            };


            var jsonNotification = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json");
            var responseNotification = await _client.PostAsync("/owner/notification", jsonNotification);
            var newNotification = GetObjectFromResponse<Notification>(responseNotification).Result;


            Assert.IsNotNull(newNotification.Id);
        }

        private async Task<T> GetObjectFromUrl<T>(string url)
        {            
            var response = await _client.GetAsync(url);
            return await GetObjectFromResponse<T>(response);
        }

        private async Task<T> GetObjectFromResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsAsync<string>();
            return JsonHelper.ToObject<T>(content);
        }
    }
}
