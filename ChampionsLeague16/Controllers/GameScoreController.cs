using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChampionsLeague16.Controllers.Requests;
using ChampionsLeague16.Controllers.Responses;
using ChampionsLeague16.Data;
using ChampionsLeague16.Helper;
using ChampionsLeague16.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChampionsLeague16.Controllers
{
    [Route("api/[controller]")]
    public class GameScoreController : Controller
    {
        #region Constants

        private const string NotValidRequest = "REQUEST_NOT_VALID";

        #endregion

        #region Private

        private readonly GameContext _context;


        #endregion

        #region Constructor

        public GameScoreController(GameContext context)
        {
            _context = context;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// API 1 - for parsing json prilog-1
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AddNewOrUpdateScores([FromBody] GroupScoresRequest request)
        {
            BaseResponse response = new BaseResponse();

            if (!ModelState.IsValid)
            {
                response.Success = false;
                response.ExceptionObject = new Exception(NotValidRequest, new Exception(ModelState.Values.First().Errors.First().ErrorMessage));
                return BadRequest(response);
            }

            try
            {
                ScoresDatabase scoresDatabase = new ScoresDatabase(_context);

                var validateResult = ValidateRequest(request.Data);

                if (validateResult != null)
                {
                    return validateResult;
                }

                ScoreBuilder scoreBuilder = new ScoreBuilder();

                scoresDatabase.AddNewOrUpdateScores(scoreBuilder.BuildScores(request.Data));
                await _context.SaveChangesAsync();

                var scores = scoresDatabase.GetAllFromTheTableScore().ToList();

                if (scores.Any())
                {
                    GroupScoreCounter groupScoreCounter = new GroupScoreCounter(scores);
                    IDictionary<string, IDictionary<string, int>> groupResults =
                        groupScoreCounter.SortListAccordingGroup(request.RetrieveGroups);

                    response.Result = groupResults;
                }

                response.Success = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ExceptionObject = e;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// API 2 for json prilog-2
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult CalculateStatisticsForResults([FromBody] GroupScoresRequest request)
        {
            BaseResponse response = new BaseResponse();

            if (!ModelState.IsValid)
            {
                response.Success = false;
                response.ExceptionObject = new Exception(NotValidRequest, new Exception(ModelState.Values.First().Errors.First().ErrorMessage));
                return BadRequest(response);
            }

            try
            {
                var validateResult = ValidateRequest(request.Data);

                if (validateResult != null)
                {
                    return validateResult;
                }

                ScoreBuilder scoreBuilder = new ScoreBuilder();

                GroupScoreCounter groupScoreCounter = new GroupScoreCounter(scoreBuilder.BuildScores(request.Data));
                IList<ChampionsLeagueModel> championsLeague = groupScoreCounter.CalculateStatistics(request.RetrieveGroups);

                response.Success = true;
                response.Result = championsLeague;
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ExceptionObject = e;
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Request for filtering data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult FilterData([FromBody] FilterRequest request)
        {
            try
            {
                var validateResult = ValidateRequest(request.Data);

                if (validateResult != null)
                {
                    return validateResult;
                }

                BaseResponse response = new BaseResponse();
                ScoreBuilder scoreBuilder = new ScoreBuilder();

                bool isDatePresent = false;
                bool isGroupPresent = false;
                bool isTeamNamePresent = false;

                DateTime dateFrom = new DateTime();
                DateTime dateTo = new DateTime();

                if (!string.IsNullOrEmpty(request.DateFrom) && !string.IsNullOrEmpty(request.DateTo))
                {
                    isDatePresent = true;

                    dateFrom = DateTime.Parse(request.DateFrom);
                    dateTo = DateTime.Parse(request.DateTo);

                    if (dateFrom > dateTo)
                    {
                        response.Success = false;
                        response.ExceptionObject = new Exception("DateFrom can't have greater value than DateTo.");
                        return BadRequest(response);
                    }
                }

                if (!string.IsNullOrEmpty(request.Group))
                {
                    isGroupPresent = true;
                }

                if (!string.IsNullOrEmpty(request.TeamName))
                {
                    isTeamNamePresent = true;
                }

                GroupScoreCounter groupScoreCounter = new GroupScoreCounter(scoreBuilder.BuildScores(request.Data));

                var result = groupScoreCounter.FilterData(dateFrom, dateTo, request.Group, request.TeamName, isDatePresent,
                    isGroupPresent, isTeamNamePresent);

                response.Success = true;
                response.Result = result;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        #endregion

        #region Helper methods - private

        private IActionResult ValidateRequest(IEnumerable<GameRequest> requestData)
        {
            BaseResponse response = new BaseResponse();

            if (requestData == null)
            {
                response.Success = false;
                response.ExceptionObject = new Exception("Data sent in the request shouldn't be empty.");
                return BadRequest(response);
            }

            foreach (var game in requestData)
            {
                char gameGroup = char.Parse(game.Group.ToUpper());
                if (!char.IsLetter(gameGroup))
                {
                    response.Success = false;
                    response.ExceptionObject = new Exception("Group must be letter.");
                    return BadRequest(response);
                }

                if (!game.Score.Contains(":"))
                {
                    response.Success = false;
                    response.ExceptionObject = new Exception("Score must be in correct format.");
                    return BadRequest(response);
                }
            }

            return null;
        }

        #endregion
    }
}
