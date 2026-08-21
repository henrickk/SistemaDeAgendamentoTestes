using FluentValidation;

namespace Agendamento.Domain.Models.Validations;

public class PacienteValidation : AbstractValidator<Paciente>
{
    public PacienteValidation() 
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("O nome do paciente é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do paciente não pode exceder 100 caracteres.");
        
        RuleFor(p => p.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("A data de nascimento deve ser anterior à data atual.");

        RuleFor(p => p.CPF)
            .NotEmpty().WithMessage("O CPF é obrigatório.");

        RuleFor(p => p.Endereco)
            .NotEmpty().WithMessage("O endereço do paciente é obrigatório.");

        RuleFor(p => p.Contato)
            .NotEmpty().WithMessage("O contato do paciente é obrigatório.");
    }
}
