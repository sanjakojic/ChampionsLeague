using System.ComponentModel.DataAnnotations;

namespace ChampionsLeague16.Controllers.Requests
{
    public class GameRequest
    {
        [Required]
        public string LeagueTitle { get; set; }
        [Required]
        public int MatchDay { get; set; }
        [Required]
        public string Group { get; set; }
        [Required]
        public string HomeTeam { get; set; }
        [Required]
        public string AwayTeam { get; set; }
        [Required]
        public string KickoffAt { get; set; }
        [Required]
        public string Score { get; set; }
    }
}
