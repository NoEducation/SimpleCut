using FluentValidation;
using SimpleCut.Logic.Accounts.Commands;

namespace SimpleCut.Logic.Accounts.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private const int MIN_PASSWORD_LENGTH = 8;

        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty();

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(MIN_PASSWORD_LENGTH);

            //TODO.DA Regex pod hasło
            // - 8 znakow
            // - małe duże litry 
            // - znak specjalny
            // - możemy sie zastnowic nad bajjerami jak top google passowrd itp. w wolnym czasie
            //RuleFor(x => x.Password)
            //    .Matches();

            RuleFor(x => x.Login)
               .NotEmpty();

            RuleFor(x => x.Name)
              .NotEmpty();

            RuleFor(x => x.Surname)
              .NotEmpty();

            RuleFor(x => x.Gender)
                .IsInEnum();

            RuleFor(x => x.BrithDate)
                .LessThan(DateTime.Now);
        }
    }
}
