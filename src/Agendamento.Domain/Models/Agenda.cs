using Agendamento.Domain.Interfaces;


namespace Agendamento.Domain.Models;

public class Agenda : Entity
{
    public Guid PacienteId { get; set; }
    public Guid ProfissionalId { get; set; }
    public StatusAgendamento StatusAgendamento { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string? Observacao { get; set; }
    public Paciente Paciente { get; private set; }
    public Profissional Profissional { get; private set; }

    public Agenda() { }

    public Agenda(Guid pacienteId, Guid profissionalId, StatusAgendamento statusAgendamento, DateTime dataInicio, DateTime dataFim, string? observacao, Paciente paciente, Profissional profissional)
    {
        PacienteId = pacienteId;
        ProfissionalId = profissionalId;
        StatusAgendamento = statusAgendamento;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Observacao = observacao;
        Paciente = paciente;
        Profissional = profissional;
    }

    public bool DatasValidas()
    {
        return DataInicio < DataFim;
    }
}
