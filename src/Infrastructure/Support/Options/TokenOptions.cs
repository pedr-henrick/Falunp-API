﻿namespace Infrastructure.Support.Options
{
    public class TokenOptions
    {
        public const string SectionName = "JwtSettings";

        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; } = 60;
    }
}
