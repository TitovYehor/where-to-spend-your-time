﻿using Microsoft.AspNetCore.Identity;
using WhereToSpendYourTime.Api.Models.Auth;

namespace WhereToSpendYourTime.Api.Services.Auth;

public interface IAuthService
{
    Task<(bool Succeeded, IEnumerable<IdentityError> Errors)> RegisterAsync(RegisterRequest request);

    Task<bool> LoginAsync(LoginRequest request);

    Task LogoutAsync();
}
