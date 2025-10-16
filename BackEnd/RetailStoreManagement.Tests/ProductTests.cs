using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RetailStoreManagement.Models;
using RetailStoreManagement.Tests;
using System.Net;
using System.Net.Http.Json;

namespace RetailStoreManagement.IntegrationTests
{
    public class ProductTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public ProductTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        //CreateProduct_ReturnsCreatedProduct
        //Prepare: create mock product data
        //Assert: response contains created product
        [Fact]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureCreated();
            }

            var newProduct = new Product
            {
                Title = "Wireless Mouse",
                Price = 19.99M
            };

            var response = await _httpClient.PostAsJsonAsync("/api/product", newProduct);
            response.EnsureSuccessStatusCode();

            var createdProduct = await response.Content.ReadFromJsonAsync<Product>();

            Assert.NotNull(createdProduct);
            Assert.NotEmpty(createdProduct.SKU);
            Assert.Equal("Wireless Mouse", createdProduct.Title);
            Assert.Equal(19.99M, createdProduct.Price);
        }

        //GetProduct_ReturnsProduct
        //Prepare: create mock product
        //Assert: GET returns the mock product
        [Fact]
        public async Task GetProduct_ReturnsProduct()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureCreated();
            }

            var newProduct = new Product
            {
                Title = "Wireless Keyboard",
                Price = 49.99M
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/api/product", newProduct);
            var createdProduct = await postResponse.Content.ReadFromJsonAsync<Product>();

            var getResponse = await _httpClient.GetAsync($"/api/product/{createdProduct!.SKU}");
            getResponse.EnsureSuccessStatusCode();

            var fetchedProduct = await getResponse.Content.ReadFromJsonAsync<Product>();

            Assert.Equal(createdProduct.SKU, fetchedProduct!.SKU);
            Assert.Equal("Wireless Keyboard", fetchedProduct.Title);
            Assert.Equal(49.99M, fetchedProduct.Price);
        }

        //CreateMultipleProducts_GeneratesUniqueSKUs
        //Prepare: create multiple mock products
        //Assert: product SKUs are unique
        [Fact]
        public async Task CreateMultipleProducts_GeneratesUniqueSKUs()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureCreated();
            }

            var product1 = new Product { Title = "Product1", Price = 10.0M };
            var product2 = new Product { Title = "Product2", Price = 20.0M };

            var response1 = await _httpClient.PostAsJsonAsync("/api/product", product1);
            var response2 = await _httpClient.PostAsJsonAsync("/api/product", product2);

            var created1 = await response1.Content.ReadFromJsonAsync<Product>();
            var created2 = await response2.Content.ReadFromJsonAsync<Product>();

            created1!.SKU.Should().NotBe(created2!.SKU);
        }

        //DeleteProduct_ReturnNoContentSuccess
        //Prepare: seed DB with test data
        //Assert: DELETE returns 204 NoContent
        [Fact]
        public async Task DeleteProduct_ReturnNoContentSuccess()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            string sku = "1234567B";

            var deleteResponse = await _httpClient.DeleteAsync($"/api/product/{sku}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        //UpdateProduct_ReturnNoContentSuccess
        //Prepare: seed DB and prepare updated product
        //Assert: PUT returns NoContent and product updated
        [Fact]
        public async Task UpdateProduct_ReturnNoContentSuccess()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RetailStoreContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            var product = new Product { SKU = "1234567B", Title = "Gaming Keyboard", Price = 99.99M };

            var updateResponse = await _httpClient.PutAsJsonAsync("/api/product", product);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updatedProductResponse = await _httpClient.GetAsync($"/api/product/{product.SKU}");
            var updatedProduct = await updatedProductResponse.Content.ReadFromJsonAsync<Product>();

            updatedProduct!.Title.Should().Be("Gaming Keyboard");
            updatedProduct!.Price.Should().Be(99.99M);
        }
    }
}