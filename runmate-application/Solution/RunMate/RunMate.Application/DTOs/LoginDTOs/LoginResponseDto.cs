﻿namespace RunMate.Authentication.RunMate.Application.DTOs.LoginDTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Expiration { get; set; }
    }
}
