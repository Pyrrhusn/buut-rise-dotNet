using System;
using FluentValidation;

namespace Rise.Shared.Users;

public record class AddMemberRoleDto
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }

    public class Validator : AbstractValidator<AddMemberRoleDto>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required")
            .GreaterThan(0).WithMessage("User ID must be a positive number");

        }


    }

}
