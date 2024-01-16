using com.Instant.Mishor.Net.Clients;
using com.Instant.Mishor.Net.Clients.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace com.Medici.WebApi.Controllers.Mishor
{
    [ApiController]
    [Route("mishor/[controller]")]
    public class SearchController : ControllerBase
    {
        public ApiClient apiClient;

        public SearchController(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        [HttpGet]
        [Route("")]
        public ActionResult Search()
        {
            return Ok(apiClient.SearchHotels("2024-02-10", "2024-02-15", 208326));
        }
    }
}