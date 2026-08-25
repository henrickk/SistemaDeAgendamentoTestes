using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Net;

namespace Agendamento.API.Controllers;
[ApiController]
[Route("api/agenda")]
public class AgendaController : MainController
{
    private readonly INotificador _notificador;
    private readonly ILogger<AgendaController> _logger;
    private readonly IAgendaService _agendaService;
    private readonly IAgendaRepository _agendaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IProfissionalRepository _profissionalRepository;
    private readonly MeuDbContext _context;
    private readonly IMapper _mapper;

    public AgendaController(INotificador notificador,
                            ILogger<AgendaController> logger,
                            IAgendaService agendaService,
                            IAgendaRepository agendaRepository,
                            IPacienteRepository pacienteRepository,
                            IProfissionalRepository profissionalRepository,
                            MeuDbContext context,
                            IMapper mapper) : base(notificador)
    {
        _notificador = notificador;
        _logger = logger;
        _agendaService = agendaService;
        _agendaRepository = agendaRepository;
        _pacienteRepository = pacienteRepository;
        _profissionalRepository = profissionalRepository;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("consultar-agendamentos")]
    public async Task<ActionResult<IEnumerable<AgendadosDto>>> ObterTodosAgendamentos()
    {
        var agendamentos = await _agendaRepository.ObterTodos();

        return Ok(agendamentos);
    }

    [HttpGet]
    [Route("consultar-agendamento/{id:guid}")]
    public async Task<ActionResult<AgendadosDto>> ObterAgendamentoPorId(int id)
    {
        var agendamento = await _context.Agendas.FindAsync(id);

        if (agendamento == null)
        {
            return NotFound(new { message = "Agendamento não encontrado." });
        }

        return Ok(agendamento);
    }

    [HttpPost]
    [Route("agendar-consulta")]
    public async Task<ActionResult<Agenda>> AgendarNovaConsulta(NovoAgendamentoDto novoAgendamentoDto)
    {
        if (!ModelState.IsValid)
        {
            return CustomResponse(ModelState);
        }

        await _agendaService.Agendar(novoAgendamentoDto);

        return CustomResponse(HttpStatusCode.Created, novoAgendamentoDto);
    }
}
