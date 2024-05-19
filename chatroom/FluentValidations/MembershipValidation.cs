using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using chatroom.Models;
public class MembershipValidator : AbstractValidator<Membership>
{
    public MembershipValidator()
    {
        RuleFor(membership => membership.UserName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(membership => membership.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }
}