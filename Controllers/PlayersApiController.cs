using AccedeTeams.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AccedeTeams
{
    [Route("api/players")]
    [AutoValidateAntiforgeryToken]
    public class PlayersApiController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public PlayersApiController(IQueryExecutor queryExecutor, IApiResponseHelper apiResponseHelper)
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] SearchPlayerSummariesQuery query)
        {
            if (query == null) query = new SearchPlayerSummariesQuery();
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet("{playerId:int}")]
        public async Task<IActionResult> Get(int PlayerId)
        {
            var query = new GetPlayerDetailsByIdQuery(PlayerId);
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }


        [AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
        [HttpPost("{playerId:int}/likes")]
        public Task<IActionResult> Like(int PlayerId)
        {
            var command = new SetPlayerLikedCommand()
            {
                PlayerId = PlayerId,
                IsLiked = true
            };

            return _apiResponseHelper.RunCommandAsync(this, command);
        }

        [AuthorizeUserArea(MemberUserArea.MemberUserAreaCode)]
        [HttpDelete("{playerId:int}/likes")]
        public Task<IActionResult> UnLike(int PlayerId)
        {
            var command = new SetPlayerLikedCommand()
            {
                PlayerId = PlayerId
            };
            return _apiResponseHelper.RunCommandAsync(this, command);
        }
    }
}