using System.Collections.Generic;
using ChampionsLeague16.Models;

namespace ChampionsLeague16.Helper
{
    /// <summary>
    /// Class responsible for determination of rank for teams  
    /// </summary>
    public class RankComparer : IComparer<TeamStatisticsModel>
    {
        /// <summary>
        /// Compares two scores 
        /// </summary>
        /// <param name="a">First score for comparing</param>
        /// <param name="b">Second score for comparing</param>
        /// <returns></returns>
        public int Compare(TeamStatisticsModel a, TeamStatisticsModel b)
        {
            if ((a.Points == b.Points))
            {
                if ((a.Goals == b.Goals))
                    return a.GoalDifference.CompareTo(b.GoalDifference);
                if (a.Goals < b.Goals)
                    return -1;
                return 1;
            }

            if (a.Points < b.Points)
                return -1;
            return 1;
        }
    }
}