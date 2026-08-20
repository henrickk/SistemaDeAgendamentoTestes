using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Agendamento.API.Controllers;
[ApiController]
[Route("[controller]")]
public class AgendaController : ControllerBase
{
    private readonly INotificador _notificador;
    private readonly ILogger<AgendaController> _logger;
    private readonly IAgendaService _agendaService;
    private readonly IAgendaRepository _agendaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IProfissionalRepository _profissionalRepository;
    private readonly Context _context;

    public AgendaController(INotificador notificador,
                            ILogger<AgendaController> logger,
                            IAgendaService agendaService,
                            IAgendaRepository agendaRepository,
                            IPacienteRepository pacienteRepository,
                            IProfissionalRepository profissionalRepository,
                            Context context)
    {
        _notificador = notificador;
        _logger = logger;
        _agendaService = agendaService;
        _agendaRepository = agendaRepository;
        _pacienteRepository = pacienteRepository;
        _profissionalRepository = profissionalRepository;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Agenda>>> ObterTodosAgendamentos()
    {
        var agendamentos = await _agendaRepository.ObterTodos();

        return Ok(agendamentos);
    }

    [HttpGet]
    public async Task<ActionResult<Agenda>> ObterAgendamentoPorId(int id)
    {
        var agendamento = await _context.Agendas.FindAsync(id);

        if (agendamento == null)
        {
            return NotFound(new { message = "Agendamento não encontrado." });
        }

        return Ok(agendamento);
    }
}
