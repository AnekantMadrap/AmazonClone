using System;
using System.Collections.Generic;

namespace AmazonClone.Application.DTOs
{
    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public AuthResponseDto? Data { get; set; }

        public static AuthResultDto Success(AuthResponseDto data) => new AuthResultDto
        {
            Succeeded = true,
            Data = data
        };

        public static AuthResultDto Failure(IEnumerable<string> errors) => new AuthResultDto
        {
            Succeeded = false,
            Errors = new List<string>(errors)
        };

        public static AuthResultDto Failure(string error) => new AuthResultDto
        {
            Succeeded = false,
            Errors = new List<string> { error }
        };
    }
}
