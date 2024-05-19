using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chatroom.Models;
using FluentValidation;

namespace chatroom.FluentValidations 
{
    public class UsersModelValidator : AbstractValidator<UserData>
    {

        public UsersModelValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                 .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                                     .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }

    }
    public class UsersModeValidator : AbstractValidator<UserData>
    {
        public UsersModeValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                .EmailAddress().WithMessage("Invalid email format.");
        }
      
    }

    //public class Pagination<T>
    //{
    //    public List<T> Records { get; set; }
    //    public int TotalPages { get; set; }
    //    public int CurrentPage { get; set; }
    //    public int PageSize { get; set; }
    //}
}