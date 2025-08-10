namespace GauTracker.API.Endpoints.AuthEndpoints.AuthRegister;

public class AuthRegisterRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;
}
