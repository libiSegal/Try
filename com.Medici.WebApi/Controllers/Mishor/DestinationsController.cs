using com.Instant.Mishor.Data.Entities;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.QueryStrings;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace com.Medici.WebApi.Controllers.Mishor
{
    [DisableRoutingConvention]
    [Route("mishor/[controller]")]
    //[DisableQueryString(JsonApiQueryStringParameters.Include)]
    public class DestinationsController : JsonApiController<Destination, int>
    {
        public DestinationsController(IAuthenticationService authService, IJsonApiOptions options,
    IResourceGraph resourceGraph, ILoggerFactory loggerFactory,
    IResourceService<Destination, int> resourceService)
: base(options, resourceGraph, loggerFactory, resourceService)
        {
        }
    }
}