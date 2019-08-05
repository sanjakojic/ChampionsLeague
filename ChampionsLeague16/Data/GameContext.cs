using ChampionsLeague16.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace ChampionsLeague16.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options)
        { }

        public DbSet<ScoreModel> Scores { get; set; }

    }
}