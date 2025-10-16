using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RetailStoreManagement.Models;
using RetailStoreManagement.Tests;
using System.Net.Http.Json;

namespace RetailStoreManagement.IntegrationTests
{
    public class PurchaseTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public PurchaseTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        //CreatePurchase_ReturnsCreatedPurchase
        //Prepare: seed DB and create mock purchase
        //Assert: response contains created purchase with purchased products
        [Fact]
        public async Task CreatePurchase_ReturnsCreatedPurchase()
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
                PurchaseDate = DateTime.UtcNow,
                CustomerId = 3,
                PurchaseProducts = new[]
                {
                    new { ProductId = "1234567C", Quantity = 2 },
                    new { ProductId = "1234567A", Quantity = 8 }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/api/purchase", newPurchase);
            response.EnsureSuccessStatusCode();

            var createdPurchase = await response.Content.ReadFromJsonAsync<Purchase>();

            Assert.NotNull(createdPurchase);
            Assert.Equal(3, createdPurchase.CustomerId);
            Assert.Equal(2, createdPurchase.PurchaseProducts?.Count);
        }

        //GetPurchase_ReturnsPurchase
        //Prepare: seed DB and create mock purchase
        //Assert: GET returns the mock purchase
        [Fact]
        public async Task GetPurchase_ReturnsPurchase()
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
            var createdPurchase = await postResponse.Content.ReadFromJsonAsync<Purchase>();

            var getResponse = await _httpClient.GetAsync($"/api/purchase/{createdPurchase!.Id}");
            getResponse.EnsureSuccessStatusCode();

            var fetchedPurchase = await getResponse.Content.ReadFromJsonAsync<Purchase>();

            Assert.Equal(createdPurchase.Id, fetchedPurchase!.Id);
            Assert.Equal(createdPurchase.PurchaseDate, fetchedPurchase.PurchaseDate);
            Assert.Equal(2, fetchedPurchase.PurchaseProducts?.Count);
        }

        //CreatePurchaseWithNonExistingCustomer_ReturnsNotFound
        //Prepare: seed DB and create mock purchase with invalid customer id
        //Assert: POST returns 404 Not Found
        [Fact]
        public async Task CreatePurchaseWithNonExistingCustomer_ReturnsNotFound()
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
                CustomerId = 88,
                PurchaseProducts = new[]
                {
                    new { ProductId = "1234567C", Quantity = 2 },
                    new { ProductId = "1234567A", Quantity = 8 }
                }
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/purchase", newPurchase);
            postResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        //CreatePurchaseWithNonExistingProduct_ReturnsNotFound
        //Prepare: seed DB and create mock purchase with invalid product sku
        //Assert: POST returns 400 Bad Request
        [Fact]
        public async Task CreatePurchaseWithNonExistingProduct_ReturnsNotFound()
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
                CustomerId = 1,
                PurchaseProducts = new[]
                {
                    new { ProductId = "invalid", Quantity = 2 },
                    new { ProductId = "invalid2", Quantity = 8 }
                }
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/purchase", newPurchase);
            postResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}