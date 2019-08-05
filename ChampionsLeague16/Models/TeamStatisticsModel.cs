namespace ChampionsLeague16.Models
{
    public class TeamStatisticsModel
    {
        public int Rank { get; set; }
        public string Team { get; set; }
        public int PlayedGames { get; set; }
        public int Points { get; set; }
        public int Goals { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int Draw { get; set; }
    }
}