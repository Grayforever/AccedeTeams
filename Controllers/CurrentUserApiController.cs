using AccedeTeams.Domain;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite
{
    [AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
    [Route("api/members/current")]
    public class CurrentMemberApiController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IUserContextService _userContextService;

        public CurrentMemberApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper,
            IUserContextService userContextService
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _userContextService = userContextService;
        }

        [HttpGet("cats/liked")]
        public async Task<IActionResult> GetLikedCats()
        {
            // Here we get the userId of the currently logged in member. We could have
            // done this in the query handler, but instead we've chosen to keep the query 
            // flexible so it can be re-used in a more generic fashion
            var userContext = await _userContextService.GetCurrentContextAsync();
            var query = new GetPlayerSummariesByMemberLikedQuery(userContext.UserId.Value);
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
    }
}