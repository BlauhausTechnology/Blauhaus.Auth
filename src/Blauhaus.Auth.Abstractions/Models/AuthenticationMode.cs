﻿namespace Blauhaus.Auth.Abstractions.Models
{
    public enum AuthenticationMode
    {
        None,
        SilentLogin,
        ManualLogin,
        ResetPassword,
        RefreshToken,
        EditProfile
    }
}