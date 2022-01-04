using System;

namespace MecWise.Blazor.Common
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public DateTime LastRequestTime { get; set; }
        public DateTime ExpiredTime
        {
            get { return LastRequestTime.AddSeconds(expires_in); }
        }

    }
}
