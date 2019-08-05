using System;
using System.Collections.Generic;
using System.Linq;
using ChampionsLeague16.Data;
using ChampionsLeague16.Data.Model;

namespace ChampionsLeague16.Helper
{
    /// <summary>
    /// Class responsible for working with database
    /// </summary>
    public class ScoresDatabase
    {
        #region Private

        private readonly GameContext _context;
        private readonly object _obj = new object();

        #endregion

        #region Constructor

        public ScoresDatabase(GameContext context)
        {
            _context = context;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adding new scores to database or updating existing ones
        /// </summary>
        /// <param name="listOfScores"></param>
        public void AddNewOrUpdateScores(IList<ScoreModel> listOfScores)
        {
            foreach (var score in listOfScores)
            {
                ScoreModel teamFrom;

                lock (_obj)
                {
                    teamFrom = _context.Scores.SingleOrDefault(x => x.Group.Equals(score.Group)
                                                                            && x.AwayTeam.Equals(score.AwayTeam)
                                                                            && x.HomeTeam.Equals(score.HomeTeam)
                                                                            && x.KickOffDate.Equals(score.KickOffDate));

                    if (teamFrom == null)
                    {
                        _context.Add(score);
                        continue;
                    }
                }

                teamFrom.Score = score.Score;
                _context.Scores.Update(teamFrom);
            }
        }

        /// <summary>
        /// Gets all scores from the database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ScoreModel> GetAllFromTheTableScore()
        {
            try
            {
                var scores = _context.Scores;

                if (scores.Any())
                {
                    return scores;
                }
            }
            catch (Exception e)
            {
                throw new Exception("[Method: GetAllFromTheTableScore] Couldn't retrieve data from database.", e);
            }

            return null;
        }
        
        #endregion
    }
}
