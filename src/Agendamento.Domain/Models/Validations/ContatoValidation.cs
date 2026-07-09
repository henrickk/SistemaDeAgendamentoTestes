using FluentValidation;

namespace Agendamento.Domain.Models.Validations
{
    public class ContatoValidation : AbstractValidator<Contato>
    {
        public ContatoValidation()
        {
            RuleFor(c => c.NumeroCelular)
                .NotEmpty().WithMessage("O número de celular é obrigatório.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("O número de celular informado não é válido.");
        }
    }
}
