﻿namespace ChatReader.Core.Models
{
    public class TokenAuthDto
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public List<string> scope { get; set; }
        public string token_type { get; set; }
    }
}
