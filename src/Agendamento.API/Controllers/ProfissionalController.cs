using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Agendamento.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Agendamento.API.Controllers;

[ApiController]
[Route("api/profissional")]
public class ProfissionalController : MainController
{
    private readonly IProfissionalRepository _profissionalRepository;

    public ProfissionalController(IProfissionalRepository profissionalRepository, INotificador notificador) : base(notificador)
    {
        _profissionalRepository = profissionalRepository;
    }

    [HttpGet]
    [Route("api/profissional/consultar-profissionais")]
    public async Task<ActionResult<IEnumerable<Profissional>>> ObterTodosProfissionais()
    {
        var profissionais = await _profissionalRepository.ObterTodos();
        return Ok(profissionais);
    }

    [HttpGet]
    [Route("api/profissional/consultar-profissional/{id:guid}")]
    public async Task<ActionResult<Profissional>> ObterProfissionaisPorId(Guid Id)
    {
        var profissional = await _profissionalRepository.ObterPorId(Id);
        if (profissional == null)
        {
            NotificarErro("Profissional não encontrado.");
            return NotFound();
        }
        return Ok(profissional);
    }

    [HttpGet]
    [Route("api/profissional/consultar-profissional-por-nome/{nome}")]
    public async Task<ActionResult<IEnumerable<Profissional>>> ObterProfissionaisPorNome(string nome)
    {
        var profissionais = await _profissionalRepository.ObterPorNome(nome);
        if (profissionais == null || !profissionais.Any())
        {
            NotificarErro("Nenhum profissional encontrado com o nome fornecido.");
            return NotFound();
        }
        return Ok(profissionais);
    }

    [HttpGet]
    [Route("api/profissional/consultar-profissional-por-cro/{cro}")]
    public async Task<ActionResult<Profissional>> ObterProfissionaisPorCRO(string cro)
    {
        var profissional = await _profissionalRepository.ObterPorCRO(cro);
        if (profissional == null)
        {
            NotificarErro("Profissional não encontrado.");
            return NotFound();
        }
        return Ok(profissional);
    }

    [HttpPost]
    [Route("api/profissional/cadastrar-profissional")]
    public async Task<ActionResult<Profissional>> CriarProfissional([FromBody] Profissional profissional)
    {
        if (!ModelState.IsValid)
        {
            NotificarErro("Dados inválidos.");
            return BadRequest(ModelState);
        }
        await _profissionalRepository.Adicionar(profissional);
        await _profissionalRepository.SaveChanges();
        return CreatedAtAction(nameof(ObterProfissionaisPorId), new { id = profissional.Id }, profissional);
    }

    [HttpPut]
    [Route("api/profissional/atualizar-profissional")]
    public async Task<ActionResult<Profissional>> AtualizarProfissional([FromBody] Profissional profissional)
    {
        if (!ModelState.IsValid)
        {
            NotificarErro("Dados inválidos.");
            return BadRequest(ModelState);
        }
        await _profissionalRepository.Atualizar(profissional);
        await _profissionalRepository.SaveChanges();
        return Ok(profissional);
    }

    [HttpDelete]
    [Route("api/profissional/excluir-profissional/{id:guid}")]
    public async Task<ActionResult> ExcluirProfissional(Guid id)
    {
        var profissional = await _profissionalRepository.ObterPorId(id);
        if (profissional == null)
        {
            NotificarErro("Profissional não encontrado.");
            return NotFound();
        }
        await _profissionalRepository.Remover(id);
        await _profissionalRepository.SaveChanges();
        return NoContent();
    }
}