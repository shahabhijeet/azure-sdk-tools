﻿using System.Security.Claims;

namespace APIViewWeb
{
    public static class GitHubUserExtension
    {
        public static string GetGitHubLogin(this ClaimsPrincipal user)
        {
            return user.FindFirst(c => c.Type == "urn:github:login")?.Value;
        }
    }
}
