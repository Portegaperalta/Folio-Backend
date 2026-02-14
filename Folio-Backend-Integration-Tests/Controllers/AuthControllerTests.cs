using Folio.Core.Application.DTOs.Auth;
using Folio_Backend_Integration_Tests.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Folio_Backend_Integration_Tests.Controllers
{
    [TestClass]
    public class AuthControllerTests : TestsBase
    {
        private static readonly string registerUrl = "/api/auth/register";
        private static readonly string loginUrl = "/api/auth/login";
        private static readonly string renewTokenUrl = "/api/auth/renew-token";
        private readonly string dbName = Guid.NewGuid().ToString();
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    [TestMethod]
        public async Task Register_ReturnsStatusCode400_WhenRegistrationFieldsAreInvalid()
        {
            //Arrange
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var invalidRegistrationCredentials = new RegisterCredentialsDTO
            {
                Name = "",
                Email = "invalidEmail.com",
                Password = "invalidPassword"
            };

            //Act
            var response = await client.PostAsJsonAsync(registerUrl, invalidRegistrationCredentials);

            //Assert
            var statusCode = response.StatusCode;
            Assert.AreEqual(expected: HttpStatusCode.BadRequest, actual: statusCode);
        }

        [TestMethod]
        public async Task Register_ReturnsValidationErrors_WhenRegistrationFieldsAreInvalid()
        {
            //Arrange
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var invalidRegistrationCredentials = new RegisterCredentialsDTO
            {
                Name = "",
                Email = "invalidEmail.com",
                Password = "invalidPassword"
            };

            string[] expectedErrors = 
                [
                "The field Name is required",
                "The field Email must be a valid email address",
                "Password must contain at least one special character"
                ];

            //Act
            var response = await client.PostAsJsonAsync(registerUrl, invalidRegistrationCredentials, jsonSerializerOptions);

            //Assert
            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            var errorMessages = problemDetails!.Errors.Values.SelectMany(x => x).ToList();

            CollectionAssert.AreEquivalent(expectedErrors, errorMessages);
        }
    }
}
