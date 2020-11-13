﻿using AccedeTeams.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AccedeTeams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : Controller
    {
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IAntiforgery _antiforgery;
        private readonly IQueryExecutor _queryExecutor;

        public AuthApiController(
            IApiResponseHelper apiResponseHelper,
            IAntiforgery antiforgery,
            IQueryExecutor queryExecutor
            )
        {
            _apiResponseHelper = apiResponseHelper;
            _antiforgery = antiforgery;
            _queryExecutor = queryExecutor;
        }


        /// <summary>
        /// Once we have logged in we need to re-fetch the csrf token because
        /// the user identity will have changed and the old token will be invalid
        /// </summary>
        [HttpGet("session")]
        public async Task<IActionResult> GetAuthSession()
        {
            var member = await _queryExecutor.ExecuteAsync(new GetCurrentMemberSummaryQuery());
            var token = _antiforgery.GetAndStoreTokens(HttpContext);

            var sessionInfo = new
            {
                Member = member,
                AntiForgeryToken = token.RequestToken
            };

            return _apiResponseHelper.SimpleQueryResponse(this, sessionInfo);
        }

        [HttpPost("register")]
        public Task<IActionResult> Register([FromBody] RegisterMemberAndLogInCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPost("login")]
        public Task<IActionResult> Login([FromBody] LogMemberInCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPost("sign-out")]
        public Task<IActionResult> SignOut()
        {
            var command = new LogMemberOutCommand();
            return _apiResponseHelper.RunCommandAsync(this, command);
        }

    }
}