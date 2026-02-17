using FluentAssertions;
using Folio.Core.Application.DTOs.Auth;
using Folio.Infrastructure.Identity;
using Folio.Infrastructure.Persistence;
using Folio_Backend_Integration_Tests.Utils;
using FolioWebAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Folio_Backend_Integration_Tests.Controllers
{
    public class AuthControllerTests : TestsBase
    {
        private static readonly string registerUrl = "/api/auth/register";
        private static readonly string loginUrl = "/api/auth/login";
        private static readonly string renewTokenUrl = "/api/auth/renew-token";

        private readonly string dbName = Guid.NewGuid().ToString();

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _client;

        public AuthControllerTests()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _webApplicationFactory = BuildWebApplicationFactory(dbName);
            _client = _webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task Register_ReturnsStatusCode400_WhenRegistrationFieldsAreInvalid()
        {
            //Arrange
            var invalidRegistrationCredentials = new RegisterCredentialsDTO
            {
                Name = "",
                Email = "invalidEmail.com",
                Password = "invalidPassword"
            };

            //Act
            var response = await _client.PostAsJsonAsync(registerUrl, invalidRegistrationCredentials, TestContext.Current.CancellationToken);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_ReturnsValidationErrors_WhenRegistrationFieldsAreInvalid()
        {
            //Arrange
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
            var response = await _client.PostAsJsonAsync(registerUrl, invalidRegistrationCredentials, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
            
            var errorMessages = problemDetails!.Errors.Values.SelectMany(x => x).ToList();

            errorMessages.Should().AllBeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task Register_PeristsDataInDatabase()
        {
            //Arrage
            var registrationCredentials = new RegisterCredentialsDTO
            {
                Name = "fakeUser",
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"
            };

            //Act
            var response = await _client.PostAsJsonAsync(registerUrl, registrationCredentials, _jsonSerializerOptions, TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();

            //Assert
            using var scope = _webApplicationFactory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userInDb = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == registrationCredentials.Email, TestContext.Current.CancellationToken);

            userInDb.Should().NotBeNull();
            userInDb.Name.Should().BeSameAs(registrationCredentials.Name);
        }

        [Fact]
        public async Task Login_ReturnsStatusCode401_WhenLoginCredentialsAreInvalid()
        {
            //Arrange
            var invalidLoginCredentials = new LoginCredentialsDTO
            {
                Email = "testUser@test.com",
                Password = "@InvalidPassword123"
            };

            //Act
            var response = await _client.PostAsJsonAsync(loginUrl, invalidLoginCredentials, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            response.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
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
                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            var validLoginCredentials = new LoginCredentialsDTO
            {
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"!,
            };

            //Act
            var response = await client.PostAsJsonAsync(loginUrl, validLoginCredentials, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            response.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_ReturnsValidJWT_WhenCredentialsAreValid()
        {
            //Arrange
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
                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            var validLoginCredentials = new LoginCredentialsDTO
            {
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"!,
            };

            //Act
            var response = await _client.PostAsJsonAsync(loginUrl, validLoginCredentials, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponseDTO>(TestContext.Current.CancellationToken);

            authResponse!.Token.Should().NotBeNullOrEmpty();

            var tokenParts = authResponse.Token.Split('.');

            tokenParts.Should().HaveCount(3);
        }

        [Fact]
        public async Task RenewToken_ReturnsNewValidToken()
        {
            //Arrange
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
                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            var validLoginCredentials = new LoginCredentialsDTO
            {
                Email = "fakeUser@test.com",
                Password = "#fakeUserpassword123"!,
            };

            //Act

            var loginResponse = await _client.PostAsJsonAsync(loginUrl, validLoginCredentials, _jsonSerializerOptions, TestContext.Current.CancellationToken);
            loginResponse.EnsureSuccessStatusCode();

             // extracts token from login response
            var loginContent = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponseDTO>(TestContext.Current.CancellationToken);
            var oldToken = loginContent?.Token;

            // waits for the clock to tick to the next second
            await Task.Delay(1001, TestContext.Current.CancellationToken);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oldToken);
            var tokenRenewResponse = await _client.GetAsync(renewTokenUrl, TestContext.Current.CancellationToken);

            //Assert
            tokenRenewResponse.EnsureSuccessStatusCode();

             // extracts new token from renew-token response
            var renewContent = await tokenRenewResponse.Content.ReadFromJsonAsync<AuthenticationResponseDTO>(TestContext.Current.CancellationToken);
            var newToken = renewContent?.Token;

            newToken.Should().NotBeNull();

            var newTokenParts = newToken.Split('.');

            newTokenParts.Should().HaveCount(3);
            oldToken.Should().NotBeSameAs(newToken);
        }
    }
}
