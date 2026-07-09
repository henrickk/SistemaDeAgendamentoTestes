using FluentValidation;

namespace Agendamento.Domain.Models.Validations;

public class EnderecoValidation : AbstractValidator<Endereco>
{
    public EnderecoValidation()
    {
        RuleFor(e => e.Logradouro)
            .NotEmpty().WithMessage("O logradouro é obrigatório.")
            .MaximumLength(100).WithMessage("O logradouro não pode exceder 100 caracteres.");
        
        RuleFor(e => e.Numero)
            .NotEmpty().WithMessage("O número é obrigatório.")
            .MaximumLength(10).WithMessage("O número não pode exceder 10 caracteres.");
        
        RuleFor(e => e.CEP)
            .NotEmpty().WithMessage("O CEP é obrigatório.");
    }
}
