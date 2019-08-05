using System;
using System.ComponentModel.DataAnnotations;

namespace ChampionsLeague16.Data.Model
{
    public class ScoreModel
    {
        [Key]
        public int Id { get; set; }
        public string LeagueTitle { get; set; }
        public int MatchDay { get; set; }
        [MaxLength(1)]
        public string Group { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Score { get; set; }

        private DateTime? _kickOffDate;
        public DateTime KickOffDate
        {
            get => _kickOffDate ?? DateTime.UtcNow;
            set => _kickOffDate = value;
        }
    }
}