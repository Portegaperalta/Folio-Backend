using Folio_Backend_Integration_Tests.Utils;
using FolioWebAPI;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Xunit;

namespace Folio_Backend_Integration_Tests.Controllers
{
    public class FoldersControllerTests : TestsBase
    {

        private readonly string baseUrl = "/api/folders";
        private readonly string dbName = Guid.NewGuid().ToString();

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        private readonly HttpClient _client;
        
        public FoldersControllerTests()
        {
            _jsonSerializerOptions = _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _webApplicationFactory = BuildWebApplicationFactory(dbName);
            _client = _webApplicationFactory.CreateClient();
        }
    }
}
