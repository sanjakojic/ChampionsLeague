using System.Collections.Generic;

namespace ChampionsLeague16.Models
{
    public class ChampionsLeagueModel
    {
        public string LeagueTitle { get; set; }
        public int MatchDay { get; set; }
        public string Group { get; set; }
        public IList<TeamStatisticsModel> Standing { get; set; }
    }
}