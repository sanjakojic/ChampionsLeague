using System;
using System.Collections.Generic;
using ChampionsLeague16.Controllers.Requests;
using ChampionsLeague16.Data.Model;

namespace ChampionsLeague16.Helper
{
    /// <summary>
    /// Class responsible for score creation 
    /// </summary>
    public class ScoreBuilder
    {
        /// <summary>
        /// Retrieving list od scores from json request
        /// </summary>
        /// <param name="scores">Data provided in the request</param>
        /// <returns></returns>
        public IList<ScoreModel> BuildScores(IEnumerable<GameRequest> scores)
        {
            IList<ScoreModel> listOfScores = new List<ScoreModel>();

            foreach (var game in scores)
            {
                ScoreModel score = new ScoreModel
                {
                    LeagueTitle = game.LeagueTitle,
                    MatchDay = game.MatchDay,
                    Group = game.Group,
                    HomeTeam = game.HomeTeam,
                    AwayTeam = game.AwayTeam,
                    KickOffDate = DateTime.Parse(game.KickoffAt),
                    Score = game.Score
                };
                listOfScores.Add(score);
            }

            return listOfScores;
        }
    }
}
