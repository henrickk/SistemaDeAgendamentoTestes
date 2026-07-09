using FluentValidation;

namespace Agendamento.Domain.Models.Validations;

public class ProfissionalValidation : AbstractValidator<Profissional>
{
    public ProfissionalValidation()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("O nome do profissional é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do profissional não pode exceder 100 caracteres.");

        RuleFor(p => p.CRO)
            .NotEmpty().WithMessage("O CRO é obrigatório.");

        RuleFor(p => p.CPF)
            .NotEmpty().WithMessage("O CPF é obrigatório.");

        RuleFor(p => p.Contato)
            .NotEmpty().WithMessage("O contato do profissional é obrigatório.");
        
        RuleFor(p => p.Endereco)
                .NotEmpty().WithMessage("O endereço do paciente é obrigatório.");
    }
}
