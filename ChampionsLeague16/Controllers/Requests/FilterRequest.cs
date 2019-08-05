using System.Collections.Generic;

namespace ChampionsLeague16.Controllers.Requests
{
    public class FilterRequest
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Group { get; set; }
        public string TeamName { get; set; }
        public IEnumerable<GameRequest> Data { get; set; }
    }
}