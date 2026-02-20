using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Folio.Core.Application.DTOs.Auth;
using Folio.Core.Application.DTOs.User;
using Folio.Infrastructure.Identity;
using Folio.Infrastructure.Persistence;
using Folio_Backend_Integration_Tests.Utils;
using FolioWebAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Folio_Backend_Integration_Tests.API.Controllers;

public class UserAccountControllerTests : TestsBase
{
  private readonly string baseUrl = "/api/account";
  private readonly string userProfileUrl = "/api/account/profile";
  private readonly string deleteAccountUrl = "/api/account/delete";
  private readonly string dbName = Guid.NewGuid().ToString();

  private readonly JsonSerializerOptions _jsonSerializerOptions;
  private readonly WebApplicationFactory<Program> _webApplicationFactory;
  private readonly HttpClient _client;


  public UserAccountControllerTests()
  {
    _jsonSerializerOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    _webApplicationFactory = BuildWebApplicationFactory(dbName);
    _client = _webApplicationFactory.CreateClient();
  }

  [Fact]
  public async Task GetProfileDetails_ReturnsPersistedDetailsInDatabase()
  {
    // Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    using (var scope = _webApplicationFactory.Services.CreateScope())
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      var userInDb = await dbContext.Users.FirstAsync(u => u.Id == user.Id, TestContext.Current.CancellationToken);

      await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    // Act
    var response = await _client.GetAsync(userProfileUrl, TestContext.Current.CancellationToken);
    response.EnsureSuccessStatusCode();

    // Assert
    var profile = await response.Content.ReadFromJsonAsync<UserProfileDetailsDTO>(TestContext.Current.CancellationToken);

    profile.Should().NotBeNull();
    profile!.Name.Should().Be(user.Name);
    profile.Email.Should().Be(user.Email);
    profile.PhoneNumber.Should().Be(user.PhoneNumber);
  }

  [Fact]
  public async Task GetProfileDetails_ReturnsStatusCode401_WhenUserIsNotAuthenticated()
  {
    // Act
    var response = await _client.GetAsync(userProfileUrl, TestContext.Current.CancellationToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task Update_UpdatesUserInformation()
  {
    // Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    var userUpdateDTO = new UserUpdateDTO
    {
      UserId = user.Id,
      Name = "UpdateUserName",
      Email = "Updated@email.com",
    };

    // Act
    var response = await _client.PutAsJsonAsync(userProfileUrl, userUpdateDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
    response.EnsureSuccessStatusCode();

    // Assert
    using var scope = _webApplicationFactory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var updatedUserInDb = await dbContext.Users.FirstAsync(u => u.Id == user.Id, TestContext.Current.CancellationToken);

    updatedUserInDb.Name.Should().Be(userUpdateDTO.Name);
    updatedUserInDb.Email.Should().Be(userUpdateDTO.Email);
  }

  [Fact]
  public async Task Update_ReturnsStatusCode204()
  {
    // Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    var userUpdateDTO = new UserUpdateDTO
    {
      UserId = user.Id,
      Name = "UpdateUserName",
      Email = "Updated@email.com",
    };

    // Act
    var response = await _client.PutAsJsonAsync(userProfileUrl, userUpdateDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

    //Assert
    response.StatusCode.Should().Be(HttpStatusCode.NoContent);
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