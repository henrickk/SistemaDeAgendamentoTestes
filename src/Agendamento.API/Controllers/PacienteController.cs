using Agendamento.Application.DTOs;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Agendamento.API.Controllers;

[ApiController]
[Route("api/paciente")]
public class PacienteController : MainController
{
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMapper _mapper;

    public PacienteController(IPacienteRepository pacienteRepository, IMapper mapper, INotificador notificador) : base(notificador)
    {
        _pacienteRepository = pacienteRepository;
        _mapper = mapper;
    }

    [HttpGet("consultar-pacientes")]
    public async Task<ActionResult<IEnumerable<PacienteDto>>> ObterTodosPacientes()
    {
        var pacientes = _mapper.Map<IEnumerable<PacienteDto>>(await _pacienteRepository.ObterTodos());
        return Ok(pacientes);
    }

    [HttpGet("consultar-paciente/{id:guid}")]
    public async Task<ActionResult<PacienteDto>> ObterPacientePorId(Guid id)
    {
        var paciente = _mapper.Map<PacienteDto>(await _pacienteRepository.ObterPorId(id));
        if (paciente == null)
        {
            NotificarErro("Paciente não encontrado.");
            return NotFound();
        }
        return Ok(paciente);
    }

    [HttpGet("consultar-paciente-por-cpf/{cpf}")]
    public async Task<ActionResult<PacienteDto>> ObterPacientePorCPF(string cpf)
    {
        var paciente = _mapper.Map<PacienteDto>(await _pacienteRepository.ObterPorCPF(cpf));
        if (paciente == null)
        {
            NotificarErro("Paciente não encontrado.");
            return NotFound();
        }
        return Ok(paciente);
    }

    [HttpGet("consultar-paciente-por-nome/{nome}")]
    public async Task<ActionResult<IEnumerable<PacienteDto>>> ObterPacientePorNome(string nome)
    {
        var pacientes = _mapper.Map<IEnumerable<PacienteDto>>(await _pacienteRepository.ObterPacientesPorNome(nome));
        if (pacientes == null || !pacientes.Any())
        {
            NotificarErro("Nenhum paciente encontrado com o nome fornecido.");
            return NotFound();
        }
        return Ok(pacientes.ToList().FirstOrDefault());
    }

    [HttpPost("criar-paciente")]
    public async Task<ActionResult<PacienteDto>> CriarPaciente([FromBody] NovoPacienteDto novoPacienteDto)
    {
        if (!ModelState.IsValid)
        {
            NotificarErroModelInvalida(ModelState);
            return CustomResponse(ModelState);
        }

        var paciente = _mapper.Map<Paciente>(novoPacienteDto);

        await _pacienteRepository.Adicionar(paciente);
        await _pacienteRepository.SaveChanges();

        var pacienteRetornoDto = _mapper.Map<PacienteDto>(paciente);

        return CustomResponse(HttpStatusCode.Created, pacienteRetornoDto);
    }

    [HttpPut("atualizar-paciente/{id:guid}")]
    public async Task<ActionResult<PacienteDto>> AtualizarPaciente(Guid id, [FromBody] AtualizarPacienteDto pacienteDto)
    {
        if (!ModelState.IsValid)
        {
            NotificarErroModelInvalida(ModelState);
            return CustomResponse(ModelState);
        }

        var pacienteExistente = await _pacienteRepository.ObterPorId(id);

        if (pacienteExistente == null)
        {
            NotificarErro("Paciente não encontrado.");
            return NotFound();
        }

        await _pacienteRepository.Atualizar(pacienteExistente);
        await _pacienteRepository.SaveChanges();

        return Ok(pacienteDto);
    }

    [HttpDelete("excluir-paciente/{id:guid}")]
    public async Task<ActionResult> ExcluirPaciente(Guid id)
    {
        var paciente = _mapper.Map<PacienteDto>(await _pacienteRepository.ObterPorId(id));
        if (paciente == null)
        {
            NotificarErro("Paciente não encontrado.");
            return NotFound();
        }
        await _pacienteRepository.Remover(id);
        await _pacienteRepository.SaveChanges();
        return CustomResponse(HttpStatusCode.NoContent);
    }
}