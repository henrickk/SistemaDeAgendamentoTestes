using Agendamento.Application.DTOs;
using Agendamento.Application.Interfaces;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Context;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("consultar-agendamentos")]
    public async Task<ActionResult<IEnumerable<AgendadosDto>>> ObterTodosAgendamentos()
    {
        var agendamentos = _mapper.Map<IEnumerable<AgendadosDto>>(await _agendaRepository.ObterTodos());

        return Ok(agendamentos);
    }

    [HttpGet("obter-por-id/{id:guid}")]
    public async Task<ActionResult<AgendadosDto>> ObterAgendamentoPorId(Guid id)
    {
        var agendamento = await _agendaRepository.ObterPorIdComRelacionamentos(id);

        if (agendamento == null)
        {
            NotificarErro("Agendamento não encontrado.");
            return NotFound();
        }

        var agendamentoDto = _mapper.Map<AgendadosDto>(agendamento);

        return Ok(agendamentoDto);
    }


    [HttpPost("agendar-consulta")]
    public async Task<ActionResult<AgendadosDto>> AgendarNovaConsulta([FromBody] NovoAgendamentoDto novoAgendamentoDto)
    {
        if (!ModelState.IsValid)
        {
            NotificarErroModelInvalida(ModelState);
            return CustomResponse(ModelState);
        }

        var agendamento = _mapper.Map<Agenda>(novoAgendamentoDto);

        await _agendaRepository.Adicionar(agendamento);
        await _agendaRepository.SaveChanges();

        var agendamentoCompleto = await _agendaRepository.ObterPorIdComRelacionamentos(agendamento.Id);

        var agendaRetorno = _mapper.Map<AgendadosDto>(agendamentoCompleto);

        return CustomResponse(HttpStatusCode.Created, agendaRetorno);
    }

    [HttpPut("Atualizar/{id:guid}")]
    public async Task<ActionResult<AgendadosDto>> AtualizarAgendamento(Guid id, [FromBody] AtualizarAgendamentoDto atualizarAgendamentoDto)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var agendamentoExistente = await _agendaRepository.ObterPorId(id);
        if (agendamentoExistente == null)
        {
            NotificarErro("Agendamento não encontrado.");
            return CustomResponse();
        }

        agendamentoExistente.DataInicio = atualizarAgendamentoDto.DataInicio;
        agendamentoExistente.DataFim = atualizarAgendamentoDto.DataFim;
        agendamentoExistente.Observacao = atualizarAgendamentoDto.Observacao;
        agendamentoExistente.ProfissionalId = atualizarAgendamentoDto.ProfissionalId;
        agendamentoExistente.StatusAgendamento = atualizarAgendamentoDto.StatusAgendamento;

        await _agendaRepository.Atualizar(agendamentoExistente);
        await _agendaRepository.SaveChanges();
        return CustomResponse(HttpStatusCode.NoContent);
    }

    [HttpDelete("cancelar-agendamento/{id:guid}")]
    public async Task<ActionResult<AgendadosDto>> CancelarAgendamento(Guid id)
    {
        var agendamentoExistente = await _agendaRepository.ObterPorId(id);

        if (agendamentoExistente == null)
        {
            NotificarErro("Agendamento não encontrado.");
            return CustomResponse();
        }
        await _agendaService.CancelarAgendamento(id);
        await _agendaRepository.SaveChanges();
        return CustomResponse();
    }
}
