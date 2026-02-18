using FluentAssertions;
using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Application.DTOs.Folder;
using Folio.Core.Domain.Entities;
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
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Folio_Backend_Integration_Tests.API.Controllers
{
    public class FoldersControllerTests : TestsBase
    {

        private readonly string baseUrl = "/api/folders";
        private readonly string dbName = Guid.NewGuid().ToString();

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        
        public FoldersControllerTests(ITestOutputHelper output)
        {
            _jsonSerializerOptions = _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _webApplicationFactory = BuildWebApplicationFactory(dbName);
            _client = _webApplicationFactory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenUserHasNoFolders()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            //Act
            var response = await _client.GetAsync(baseUrl, TestContext.Current.CancellationToken);

            //Assert
            var result = await response.Content.ReadFromJsonAsync<List<FolderDTO>>(TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.Should().BeOfType<List<FolderDTO>>();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfFolderDTOsWhenUserHasFolders()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            using(var context = BuildContext(dbName))
            {
                context.Folders.AddRange( new Folder("funny",user.Id), new Folder("movies",user.Id));

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            //Act
            var response = await _client.GetAsync(baseUrl, TestContext.Current.CancellationToken);

            //Assert
            var result = await response.Content.ReadFromJsonAsync<List<FolderDTO>>(TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.Should().BeOfType<List<FolderDTO>>();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Count_ReturnsZero_WhenUserHasNoFolders()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            //Act
            var response = await _client.GetAsync($"{baseUrl}/count", TestContext.Current.CancellationToken);

            //Arrange
            var result = await response.Content.ReadFromJsonAsync<int>(TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().Be(0);
        }

        [Fact]
        public async Task Count_ReturnsTotalNumberOfFoldersInDatabase_WhenUserHasFolders()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            using (var context = BuildContext(dbName))
            {
                context.Folders.AddRange(new Folder("funny", user.Id));

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            //Act
            var response = await _client.GetAsync($"{baseUrl}/count", TestContext.Current.CancellationToken);

            //Assert
            var result = await response.Content.ReadFromJsonAsync<int>(TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Create_ReturnsStatusCode400_WhenCreationFieldsAreInvalid()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            var invalidFolderCreationDTO = new FolderCreationDTO { Name = "" };

            //Act
            var response = await _client.PostAsJsonAsync(baseUrl, invalidFolderCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ReturnValidationErrors_WhenCreationFieldsAreInvalid()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            var invalidFolderCreationDTO = new FolderCreationDTO { Name = "" };

            string[] expectedErrorMessages = ["The field Name is required"];

            //Act
            var response = await _client.PostAsJsonAsync(baseUrl, invalidFolderCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);

            var errorMessages = problemDetails!.Errors.Values.SelectMany(x => x).ToList();

            errorMessages.Should().BeEquivalentTo(expectedErrorMessages);
        }

        [Fact]
        public async Task Create_ReturnsStatusCode200_WhenFolderIsCreated()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            var folderCreationDTO = new FolderCreationDTO { Name = "test folder" };

            //Act
            var response = await _client.PostAsJsonAsync(baseUrl, folderCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_PersistsDataInDatabase()
        {
            //Arrange
            var (user, token) = await CreateAndLoginUserAsync();

            SetAuthToken(token);

            var folderCreationDTO = new FolderCreationDTO { Name = "test folder" };

            //Act
            var response = await _client.PostAsJsonAsync(baseUrl, folderCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();

            //Assert
            using var scope = _webApplicationFactory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var folderInDb = await dbContext.Folders.Where(f => f.UserId == user.Id).FirstOrDefaultAsync(TestContext.Current.CancellationToken);

            folderInDb.Should().NotBeNull();
            folderInDb.Name.Should().Be(folderCreationDTO.Name);
        }

        [Fact]
        public async Task Create_ReturnsStatusCode401_WhenUserIsUnauthorized()
        {
            //Arrange
            var folderCreationDTO = new FolderCreationDTO { Name = "test folder" };

            //Act
            var response = await _client.PostAsJsonAsync(baseUrl, folderCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        //Helper methods

        private async Task<(ApplicationUser user, string Token)> 
            CreateAndLoginUserAsync(string email = "fakeUser@test.com", string password = "#fakeUserpassword123")
        {
            //User creation
            var scope = _webApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            var user = new ApplicationUser
            {
                Name = "fakeUser",
                Email = email,
                UserName = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            user.PasswordHash = passwordHasher.HashPassword(user, password);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            //User login
            var loginDto = new LoginCredentialsDTO { Email = email, Password = password };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto, _jsonSerializerOptions);
            response.EnsureSuccessStatusCode();

            var loginResult = await response.Content.ReadFromJsonAsync<AuthenticationResponseDTO>();

            return (user, loginResult!.Token);
        }

        private void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
