using FluentValidation;

namespace Agendamento.Domain.Models.Validations;

public class AgendaValidation : AbstractValidator<Agenda>
{
    public AgendaValidation() 
    {
        RuleFor(a => a.ProfissionalId)
            .NotEmpty().WithMessage("O ID do profissional é obrigatório.");

        RuleFor(a => a.DataInicio)
            .NotEmpty().WithMessage("A data de início é obrigatória.")
            .NotEmpty().WithMessage("A data de início deve ser anterior à data de fim.");

        RuleFor(a => a.DataFim)
            .NotEmpty().WithMessage("A data de fim é obrigatória.")
            .GreaterThan(a => a.DataInicio).WithMessage("A data de fim deve ser posterior à data de início.");
    }
}
