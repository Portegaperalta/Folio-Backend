using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Folio.Core.Application.DTOs.Bookmark;
using Folio.Core.Application.DTOs.Auth;
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
using Xunit;

namespace Folio_Backend_Integration_Tests.API.Controllers;

public class BookmarksControllerTests : TestsBase
{
  // Bookmark Controller uses nested route:
  // /api/{folderId}/bookmarks

  private readonly string baseUrl = "/api";

  private readonly string dbName = Guid.NewGuid().ToString();
  private readonly JsonSerializerOptions _jsonSerializerOptions;
  private readonly WebApplicationFactory<Program> _webApplicationFactory;
  private readonly HttpClient _client;
  private readonly ITestOutputHelper _output;
  public BookmarksControllerTests(ITestOutputHelper output)
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
  public async Task GetAll_ReturnsEmptyList_WhenUserHasNoBookmarks()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    //Act
    var response = await _client.GetAsync($"/api/{folderId}/bookmarks", TestContext.Current.CancellationToken);
    response.EnsureSuccessStatusCode();

    //Assert
    var result = await response.Content.ReadFromJsonAsync<List<BookmarkDTO>>(TestContext.Current.CancellationToken);

    result.Should().NotBeNull();
    result.Should().BeOfType<List<BookmarkDTO>>();
    result.Should().HaveCount(0);
  }

  [Fact]
  public async Task GetAll_ReturnsStatusCode200_WhenRequestIsSuccessful()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    //Act
    var response = await _client.GetAsync($"/api/{folderId}/bookmarks", TestContext.Current.CancellationToken);

    //Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task GetAll_ReturnsListOfBookmarkDTOs_WhenUserHasBookmarks()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;

      context.Bookmarks.Add(new Bookmark("https://example.com", "Example", folder.Id, user.Id));
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    //Act
    var response = await _client.GetAsync($"/api/{folderId}/bookmarks", TestContext.Current.CancellationToken);
    response.EnsureSuccessStatusCode();

    //Assert
    var result = await response.Content.ReadFromJsonAsync<List<BookmarkDTO>>(TestContext.Current.CancellationToken);

    result.Should().NotBeNull();
    result.Should().BeOfType<List<BookmarkDTO>>();
    result.Should().HaveCount(1);
  }

  [Fact]
  public async Task GetById_ReturnsStatusCode404_WhenBookmarkDoesNotExist()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;

      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    //Act
    Guid nonExistentBookmarkId = new();
    var response = await _client.GetAsync($"/api/{folderId}/bookmarks/{nonExistentBookmarkId}", TestContext.Current.CancellationToken);

    //Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task Count_ReturnsZero_WhenUserHasNoBookmarks()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    //Act
    var response = await _client.GetAsync($"/api/{folderId}/bookmarks/count", TestContext.Current.CancellationToken);
    response.EnsureSuccessStatusCode();

    //Assert
    var result = await response.Content.ReadFromJsonAsync<int>(TestContext.Current.CancellationToken);

    result.Should().Be(0);
  }

  [Fact]
  public async Task Count_ReturnsTotalNumberOfBookmarksInDatabase_WhenUserHasBookmarks()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;

      context.Bookmarks.Add(new Bookmark("https://example1.com", "Example 1", folder.Id, user.Id));
      context.Bookmarks.Add(new Bookmark("https://example2.com", "Example 2", folder.Id, user.Id));
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    //Act
    var response = await _client.GetAsync($"/api/{folderId}/bookmarks/count", TestContext.Current.CancellationToken);
    response.EnsureSuccessStatusCode();

    //Assert
    var result = await response.Content.ReadFromJsonAsync<int>(TestContext.Current.CancellationToken);

    result.Should().Be(2);
  }

  [Fact]
  public async Task Create_ReturnsStatusCode400_WhenBookmarkCreationFieldsAreInvalid()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;

      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "",
      Url = "invalidUrl.com"
    };

    //Act
    var response = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, TestContext.Current.CancellationToken);

    //Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Create_ReturnValidationErrors_WhenBookmarkCreationFieldsAreInvalid()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "",
      Url = "invalidUrl.com"
    };

    string[] expectedErrorMessages =
    [
      "The field Name is required",
      "The field Url must be a valid Url"
    ];

    //Act
    var response = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

    //Assert
    var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
    var errorMessages = problemDetails!.Errors.Values.SelectMany(x => x).ToList();

    errorMessages.Should().BeEquivalentTo(expectedErrorMessages);
  }

  [Fact]
  public async Task Create_PersistsDataInDatabase()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "Example bookmark",
      Url = "https://example.com"
    };

    //Act
    var response = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
    response.EnsureSuccessStatusCode();

    //Assert
    using var scope = _webApplicationFactory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var bookmarkInDb = await dbContext.Bookmarks
      .Where(b => b.UserId == user.Id && b.FolderId == folderId)
      .FirstOrDefaultAsync(TestContext.Current.CancellationToken);

    bookmarkInDb.Should().NotBeNull();
    bookmarkInDb!.Name.Should().Be(bookmarkCreationDTO.Name);
    bookmarkInDb.Url.Should().Be(bookmarkCreationDTO.Url);
  }

  [Fact]
  public async Task Create_ReturnsStatusCode201_WhenBookmarkIsCreated()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "Example bookmark",
      Url = "https://example.com"
    };

    //Act
    var response = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

    //Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
  }

  [Fact]
  public async Task Update_ReturnsStatusCode204_WhenUpdateIsSuccessful()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "Original bookmark",
      Url = "https://example.com"
    };

    var createResponse = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
    createResponse.EnsureSuccessStatusCode();
    var createdBookmark = await createResponse.Content.ReadFromJsonAsync<BookmarkDTO>(TestContext.Current.CancellationToken);

    var bookmarkUpdateDTO = new BookmarkUpdateDTO
    {
      Id = createdBookmark!.Id,
      Name = "Updated bookmark",
      Url = "https://updated-example.com",
      IsMarkedFavorite = true
    };

    //Act
    var updateResponse = await _client.PutAsJsonAsync($"{baseUrl}/{folderId}/bookmarks/{createdBookmark.Id}", bookmarkUpdateDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);

    //Assert
    updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
  }

  [Fact]
  public async Task Update_PersistsModifiedDataInDatabase()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "Original bookmark",
      Url = "https://example.com"
    };

    var createResponse = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
    createResponse.EnsureSuccessStatusCode();
    var createdBookmark = await createResponse.Content.ReadFromJsonAsync<BookmarkDTO>(TestContext.Current.CancellationToken);

    var bookmarkUpdateDTO = new BookmarkUpdateDTO
    {
      Id = createdBookmark!.Id,
      Name = "Updated bookmark",
      Url = "https://updated-example.com",
      IsMarkedFavorite = true
    };

    //Act
    var updateResponse = await _client.PutAsJsonAsync($"{baseUrl}/{folderId}/bookmarks/{createdBookmark.Id}", bookmarkUpdateDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
    updateResponse.EnsureSuccessStatusCode();

    //Assert
    using var scope = _webApplicationFactory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var bookmarkInDb = await dbContext.Bookmarks
      .Where(b => b.Id == createdBookmark.Id)
      .FirstOrDefaultAsync(TestContext.Current.CancellationToken);

    bookmarkInDb.Should().NotBeNull();
    bookmarkInDb!.Name.Should().Be(bookmarkUpdateDTO.Name);
    bookmarkInDb.Url.Should().Be(bookmarkUpdateDTO.Url);
    bookmarkInDb.IsMarkedFavorite.Should().Be(bookmarkUpdateDTO.IsMarkedFavorite!.Value);
    bookmarkInDb.UserId.Should().Be(user.Id);
    bookmarkInDb.FolderId.Should().Be(folderId);
  }

  [Fact]
  public async Task Delete_RemovesBookmarkFromPersistenceLayer_WhenDeletionIsSuccessful()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "Bookmark to delete",
      Url = "https://example.com"
    };

    var createResponse = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
    createResponse.EnsureSuccessStatusCode();
    var createdBookmark = await createResponse.Content.ReadFromJsonAsync<BookmarkDTO>(TestContext.Current.CancellationToken);

    //Act
    var deleteResponse = await _client.DeleteAsync($"{baseUrl}/{folderId}/bookmarks/{createdBookmark!.Id}", TestContext.Current.CancellationToken);
    deleteResponse.EnsureSuccessStatusCode();

    //Assert
    using var scope = _webApplicationFactory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var bookmarkInDb = await dbContext.Bookmarks
      .Where(b => b.Id == createdBookmark.Id)
      .FirstOrDefaultAsync(TestContext.Current.CancellationToken);

    bookmarkInDb.Should().BeNull();
  }

  [Fact]
  public async Task Delete_ReturnsStatusCode204_WhenDeletionIsSuccessful()
  {
    //Arrange
    var (user, token) = await CreateAndLoginUserAsync();
    SetAuthToken(token);

    Guid folderId;
    using (var context = BuildContext(dbName))
    {
      var folder = new Folder("test folder", user.Id);
      context.Folders.Add(folder);
      await context.SaveChangesAsync(TestContext.Current.CancellationToken);
      folderId = folder.Id;
    }

    var bookmarkCreationDTO = new BookmarkCreationDTO
    {
      Name = "Bookmark to delete",
      Url = "https://example.com"
    };

    var createResponse = await _client.PostAsJsonAsync($"{baseUrl}/{folderId}/bookmarks", bookmarkCreationDTO, _jsonSerializerOptions, TestContext.Current.CancellationToken);
    createResponse.EnsureSuccessStatusCode();
    var createdBookmark = await createResponse.Content.ReadFromJsonAsync<BookmarkDTO>(TestContext.Current.CancellationToken);

    //Act
    var deleteResponse = await _client.DeleteAsync($"{baseUrl}/{folderId}/bookmarks/{createdBookmark!.Id}", TestContext.Current.CancellationToken);

    //Assert
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
  }

  private async Task<(ApplicationUser user, string Token)>
      CreateAndLoginUserAsync(string email = "fakeUser@test.com", string password = "#fakeUserpassword123")
  {
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
