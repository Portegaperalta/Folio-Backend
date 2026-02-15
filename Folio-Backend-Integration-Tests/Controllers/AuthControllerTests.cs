using Folio.Core.Application.DTOs.Auth;
using Folio.Infrastructure.Identity;
using Folio.Infrastructure.Persistence;
using Folio_Backend_Integration_Tests.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

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

        [TestMethod]
        public async Task Register_PeristsDataInDatabase()
        {
            //Arrage
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var registrationCredentials = new RegisterCredentialsDTO
            {
                Name = "fakeUser",
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"
            };

            //Act
            var response = await client.PostAsJsonAsync(registerUrl, registrationCredentials, jsonSerializerOptions);

            //Assert
            response.EnsureSuccessStatusCode();

            using var scope = factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userInDb = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == registrationCredentials.Email);

            Assert.IsNotNull(userInDb);
            Assert.AreEqual(registrationCredentials.Name, userInDb.Name);
        }

        [TestMethod]
        public async Task Login_ReturnsStatusCode401_WhenLoginCredentialsAreInvalid()
        {
            //Arrange
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            var invalidLoginCredentials = new LoginCredentialsDTO
            {
                Email = "testUser@test.com",
                Password = "@InvalidPassword123"
            };

            //Act
            var response = await client.PostAsJsonAsync(loginUrl, invalidLoginCredentials, jsonSerializerOptions);

            //Assert
            var statusCode = response.StatusCode;
            Assert.AreEqual(expected: HttpStatusCode.Unauthorized, actual: statusCode);
        }

        [TestMethod]
        public async Task Login_ReturnsStatusCode200_WhenCredentialsAreValid()
        {
            //Arrage
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            using (var context = BuildContext(dbName))
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                var user = new ApplicationUser
                {
                    Name = "fakeUser",
                    Email = "fakeUser@test.com",
                    UserName = "fakeUser@test.com",
                    NormalizedEmail = "FAKEUSER@TEST.COM",
                    NormalizedUserName = "FAKEUSER@TEST.COM",
                    SecurityStamp = Guid.NewGuid().ToString() 
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "#fakeUserpassword123");

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var validLoginCredentials = new LoginCredentialsDTO
            {
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"!,
            };

            //Act
            var response = await client.PostAsJsonAsync(loginUrl, validLoginCredentials, jsonSerializerOptions);

            //Assert
            Assert.AreEqual(expected: HttpStatusCode.OK, actual: response.StatusCode);
        }

        [TestMethod]
        public async Task Login_ReturnsValidJWT_WhenCredentialsAreValid()
        {
            //Arrange
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            using (var context = BuildContext(dbName))
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                var user = new ApplicationUser
                {
                    Name = "fakeUser",
                    Email = "fakeUser@test.com",
                    UserName = "fakeUser@test.com",
                    NormalizedEmail = "FAKEUSER@TEST.COM",
                    NormalizedUserName = "FAKEUSER@TEST.COM",
                    SecurityStamp = Guid.NewGuid().ToString() 
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "#fakeUserpassword123");

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var validLoginCredentials = new LoginCredentialsDTO
            {
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"!,
            };

            //Act
            var response = await client.PostAsJsonAsync(loginUrl, validLoginCredentials, jsonSerializerOptions);

            //Assert
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponseDTO>();

            Assert.IsFalse(string.IsNullOrEmpty(authResponse!.Token));

            var tokenParts = authResponse.Token.Split('.');

            Assert.HasCount(expected: 3, tokenParts);
        }

        [TestMethod]
        public async Task RenewToken_ReturnsNewValidToken()
        {
            var factory = BuildWebApplicationFactory(dbName);
            var client = factory.CreateClient();

            using (var context = BuildContext(dbName))
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                var user = new ApplicationUser
                {
                    Name = "fakeUser",
                    Email = "fakeUser@test.com",
                    UserName = "fakeUser@test.com",
                    NormalizedEmail = "FAKEUSER@TEST.COM",
                    NormalizedUserName = "FAKEUSER@TEST.COM",
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "#fakeUserpassword123");

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var validLoginCredentials = new LoginCredentialsDTO
            {
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"!,
            };

            //Act

            var loginResponse = await client.PostAsJsonAsync(loginUrl, validLoginCredentials, jsonSerializerOptions);
            loginResponse.EnsureSuccessStatusCode();

             // extracts token from login response
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponseDTO>();
            var oldToken = loginContent?.Token;

            // waits for the clock to tick to the next second
            await Task.Delay(1001);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oldToken);
            var tokenRenewResponse = await client.GetAsync(renewTokenUrl);

            //Assert
            tokenRenewResponse.EnsureSuccessStatusCode();

             // extracts new token from renew-token response
            var renewContent = await tokenRenewResponse.Content.ReadFromJsonAsync<AuthenticationResponseDTO>();
            var newToken = renewContent?.Token;

            Assert.IsNotNull(newToken);

            var newTokenParts = newToken.Split('.');

            Assert.HasCount(expected: 3, newTokenParts);
            Assert.AreNotEqual(oldToken, newToken);
        }
    }
}
