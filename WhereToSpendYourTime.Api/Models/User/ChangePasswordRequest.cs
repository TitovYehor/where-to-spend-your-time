﻿namespace WhereToSpendYourTime.Api.Models.User;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    
    public string NewPassword { get; set; } = string.Empty;
}
