using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChampionsLeague16.Controllers.Requests
{
    public class GroupScoresRequest
    {
        public string RetrieveGroups { get; set; }

        [Required]
        public IEnumerable<GameRequest> Data { get; set; }
    }
}
