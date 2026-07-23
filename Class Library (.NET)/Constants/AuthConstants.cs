using System;
using System.Collections.Generic;
using System.Text;

namespace Class_Library__.NET_.Constants
{
    public static class AuthConstants
    {
        public static class TokenSettings
        {
            public const int AccessTokenExpirationMinutes = 15;
            public const int RefreshTokenExpirationDays = 7;
            public const string Issuer = "AmazonClone.API";
            public const string Audience = "AmazonClone.Angular";
        }
    }
}
