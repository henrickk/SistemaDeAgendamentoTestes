using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Agendamento.API.Controllers;

[ApiController]
[Route("api/agenda")]
public class PacienteController : MainController
{
    private readonly IPacienteRepository _pacienteRepository;

    public PacienteController(IPacienteRepository pacienteRepository, INotificador notificador) : base(notificador)
    {
        _pacienteRepository = pacienteRepository;
    }

    [HttpGet]
    [Route("api/paciente/consultar-pacientes")]
    public async Task<ActionResult<IEnumerable<Paciente>>> ObterTodosPacientes()
    {
        // Lógica para obter todos os pacientes
        var pacientes = await _pacienteRepository.ObterTodos();
        return Ok(pacientes);
    }

    [HttpGet]
    [Route("api/paciente/consultar-paciente/{id:guid}")]
    public async Task<ActionResult<Paciente>> ObterPacientePorId(Guid id)
    {
        var paciente = await _pacienteRepository.ObterPorId(id);
        if (paciente == null)
        {
            NotificarErro("Paciente não encontrado.");
            return NotFound();
        }
        return Ok(paciente);
    }

    [HttpGet]
    [Route("api/paciente/consultar-paciente-por-cpf/{cpf}")]
    public async Task<ActionResult<Paciente>> ObterPacientePorCPF(string cpf)
    {
        var paciente = await _pacienteRepository.ObterPorCPF(cpf);
        if (paciente == null)
        {
            NotificarErro("Paciente não encontrado.");
            return NotFound();
        }
        return Ok(paciente);
    }

    [HttpGet]
    [Route("api/paciente/consultar-paciente-por-nome/{nome}")]
    public async Task<ActionResult<IEnumerable<Paciente>>> ObterPacientePorNome(string nome)
    {
        var pacientes = await _pacienteRepository.ObterPacientesPorNome(nome);
        if (pacientes == null || !pacientes.Any())
        {
            NotificarErro("Nenhum paciente encontrado com o nome fornecido.");
            return NotFound();
        }
        return Ok(pacientes.ToList().FirstOrDefault());
    }

    [HttpPost]
    [Route("api/paciente/criar-paciente")]
    public async Task<ActionResult<Paciente>> CriarPaciente([FromBody] Paciente paciente)
    {
        if (!ModelState.IsValid)
        {
            NotificarErroModelInvalida(ModelState);
            return CustomResponse(ModelState);
        }
        await _pacienteRepository.Adicionar(paciente);
        return CustomResponse(HttpStatusCode.Created, paciente);
    }

    [HttpPut]
    [Route("api/paciente/atualizar-paciente")]
    public async Task<ActionResult<Paciente>> AtualizarPaciente([FromBody] Paciente paciente)
    {
        if (!ModelState.IsValid)
        {
            NotificarErroModelInvalida(ModelState);
            return CustomResponse(ModelState);
        }
        await _pacienteRepository.Atualizar(paciente);
        return CustomResponse(HttpStatusCode.NoContent);
    }

    [HttpDelete]
    [Route("api/paciente/excluir-paciente/{id:guid}")]
    public async Task<ActionResult> ExcluirPaciente(Guid id)
    {
        var paciente = await _pacienteRepository.ObterPorId(id);
        if (paciente == null)
        {
            NotificarErro("Paciente não encontrado.");
            return NotFound();
        }
        await _pacienteRepository.Remover(id);
        return CustomResponse(HttpStatusCode.NoContent);
    }
}