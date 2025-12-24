
using OnlineBanking.Application.Models.Auth.Requests;

namespace OnlineBanking.Application.Features.Auth.Validators;

public class SignupRequestValidator : AbstractValidator<SignupRequest>
{
    private const int MinimumPasswordLength = 8;
    private const int MinimumUsernameLength = 3;
    private const int MaximumUsernameLength = 50;
    private const int MaximumDisplayNameLength = 100;

    public SignupRequestValidator()
    {
        /// <summary>
        /// Configure validation rules for the Username property.
        /// </summary>
        RuleFor(c => c.Username)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .Length(MinimumUsernameLength, MaximumUsernameLength)
        .WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters")
        .Matches(@"^[a-zA-Z0-9_.-]+$")
        .WithMessage("{PropertyName} can only contain letters, numbers, dots, hyphens, and underscores");

        /// <summary>
        /// Configure validation rules for the Password property.
        /// </summary>
        RuleFor(c => c.Password)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .NotEmpty()
        .WithMessage("{PropertyName} is required")
        .Matches(@"[A-Z]")
        .WithMessage("{PropertyName} must contain at least one uppercase letter")
        .Matches(@"[a-z]")
        .WithMessage("{PropertyName} must contain at least one lowercase letter")
        .Matches(@"[0-9]")
        .WithMessage("{PropertyName} must contain at least one digit")
        .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\|,.<>\/?]")
        .WithMessage("{PropertyName} must contain at least one special character");

        /// <summary>
        /// Configures validation rules for the DisplayName property.
        /// </summary>
        RuleFor(c => c.DisplayName)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .MaximumLength(MaximumDisplayNameLength)
        .WithMessage("{PropertyName} must not exceed {MaxLength} characters")
        .Matches(@"^[a-zA-Z\s'-]+$")
        .WithMessage("{PropertyName} can only contain letters, spaces, hyphens, and apostrophes");


        /// <summary>
        /// Configures validation rules for the Email property.
        /// </summary>
        RuleFor(c => c.Email)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .EmailAddress()
        .WithMessage("{PropertyName} must be a valid email address")
        .MaximumLength(254)
        .WithMessage("{PropertyName} must not exceed {MaxLength} characters (RFC 5321)");
    }
}
