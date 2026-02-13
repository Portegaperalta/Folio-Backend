using Folio.Core.Application.DTOs.Auth;
using Folio_Backend_Integration_Tests.Utils;
using System.Net;
using System.Net.Http.Json;

namespace Folio_Backend_Integration_Tests.Controllers
{
    [TestClass]
    public class AuthControllerTests : TestsBase
    {
        private static readonly string Url = "/api/auth/register";
        private readonly string dbName = Guid.NewGuid().ToString();

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
            var response = await client.PostAsJsonAsync(Url, invalidRegistrationCredentials);

            //Assert
            var statusCode = response.StatusCode;
            Assert.AreEqual(expected: HttpStatusCode.BadRequest, actual: statusCode);
        }
    }
}
