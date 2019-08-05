using System;

namespace ChampionsLeague16.Controllers.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public Exception ExceptionObject { get; set; }
        public object Result { get; set; }
    }
}
