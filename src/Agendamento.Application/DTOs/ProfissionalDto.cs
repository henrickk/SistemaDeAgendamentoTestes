namespace Agendamento.Application.DTOs;
public class ProfissionalDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string CRO { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public ContatoDto Contato { get; set; }
}
