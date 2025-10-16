using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RetailStoreManagement.Models;
using RetailStoreManagement.Tests;
using System.Net;
using System.Net.Http.Json;

namespace RetailStoreManagement.IntegrationTests
{
    public class CustomerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public CustomerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        //CreateCustomer_ReturnsCreatedCustomer
        //Prepare: mock customer data
        //Assert: response contains created customer
        [Fact]
        public async Task CreateCustomer_ReturnsCreatedCustomer()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureCreated();
            }

            var newCustomer = new Customer
            {
                FullName = "Roza Mpoukouvala",
                Email = "rozaboukouvala@gmail.com"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/customer", newCustomer);
            response.EnsureSuccessStatusCode();

            var createdCustomer = await response.Content.ReadFromJsonAsync<Customer>();

            Assert.NotNull(createdCustomer);
            Assert.Equal("Roza Mpoukouvala", createdCustomer.FullName);
            Assert.Equal("rozaboukouvala@gmail.com", createdCustomer.Email);
        }

        //GetCustomer_ReturnsCustomer
        //Prepare: create mock customer
        //Assert: GET returns the mock customer
        [Fact]
        public async Task GetCustomer_ReturnsCustomer()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureCreated();
            }

            var newCustomer = new Customer
            {
                FullName = "Roza Mpoukouvala",
                Email = "rozaboukouvala@gmail.com"
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/customer", newCustomer);
            var createdCustomer = await postResponse.Content.ReadFromJsonAsync<Customer>();

            var getResponse = await _httpClient.GetAsync($"/api/customer/{createdCustomer!.Id}");
            getResponse.EnsureSuccessStatusCode();

            var fetchedCustomer = await getResponse.Content.ReadFromJsonAsync<Customer>();

            Assert.Equal(createdCustomer.Id, fetchedCustomer!.Id);
            Assert.Equal("Roza Mpoukouvala", fetchedCustomer.FullName);
            Assert.Equal("rozaboukouvala@gmail.com", fetchedCustomer.Email);
        }

        //DeleteCustomer_ReturnNoContentSuccess
        //Prepare: seed DB with test data
        //Assert: DELETE returns 204 NoContent
        [Fact]
        public async Task DeleteCustomer_ReturnNoContentSuccess()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            string id = "2";

            var deleteResponse = await _httpClient.DeleteAsync($"/api/customer/{id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        //UpdateCustomer_ReturnNoContentSuccess
        //Prepare: seed DB and prepare updated customer
        //Assert: PUT returns NoContent and data updated
        [Fact]
        public async Task UpdateCustomer_ReturnNoContentSuccess()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            var customer = new Customer { Id = 2, FullName = "Roza Mpoukouvala", Email = "rozaboukovala@gmail.com" };

            var updateResponse = await _httpClient.PutAsJsonAsync("/api/customer", customer);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updatedCustomerResponse = await _httpClient.GetAsync($"/api/customer/{customer.Id}");
            var updatedCustomer = await updatedCustomerResponse.Content.ReadFromJsonAsync<Customer>();

            updatedCustomer!.FullName.Should().Be("Roza Mpoukouvala");
            updatedCustomer!.Email.Should().Be("rozaboukovala@gmail.com");
        }

        //DeleteCustomerWithPurchases_ReturnsConflict
        //Prepare: seed DB, create a purchase and then delete the related customer 
        //Assert: POST returns Conflict
        [Fact]
        public async Task DeleteCustomerWithPurchases_ReturnsConflict()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            var newPurchase = new
            {
                CustomerId = 3,
                PurchaseProducts = new[]
                {
                    new { ProductId = "1234567C", Quantity = 2 },
                    new { ProductId = "1234567A", Quantity = 8 }
                }
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/purchase", newPurchase);

            string id = "3";

            var deleteResponse = await _httpClient.DeleteAsync($"/api/customer/{id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}