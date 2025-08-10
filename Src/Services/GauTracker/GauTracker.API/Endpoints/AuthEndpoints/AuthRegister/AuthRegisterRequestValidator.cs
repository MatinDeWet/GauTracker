using FastEndpoints;
using FluentValidation;
using GauTracker.API.Common.Validation;

namespace GauTracker.API.Endpoints.AuthEndpoints.AuthRegister;

public class AuthRegisterRequestValidator : Validator<AuthRegisterRequest>
{
    public AuthRegisterRequestValidator()
    {
        RuleFor(x => x.Email)
        .StringInput(256)
        .EmailAddress()
        .WithMessage("{PropertyName} is not valid");

        RuleFor(x => x.Password)
            .StringInput(512);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("{PropertyName} does not match");
    }
}
