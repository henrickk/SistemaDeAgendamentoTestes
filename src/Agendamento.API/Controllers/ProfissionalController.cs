using Agendamento.Application.DTOs;
using Agendamento.Domain.Interfaces;
using Agendamento.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Agendamento.API.Controllers;

[ApiController]
[Route("api/profissional")]
public class ProfissionalController : MainController
{
    private readonly IProfissionalRepository _profissionalRepository;
    private readonly IMapper _mapper;

    public ProfissionalController(IProfissionalRepository profissionalRepository, IMapper mapper, INotificador notificador) : base(notificador)
    {
        _profissionalRepository = profissionalRepository;
        _mapper = mapper;
    }

    [HttpGet("consultar-profissionais")]
    public async Task<ActionResult<IEnumerable<ProfissionalDto>>> ObterTodosProfissionais()
    {
        var profissionais = _mapper.Map<IEnumerable<ProfissionalDto>>(await _profissionalRepository.ObterTodos());
        return Ok(profissionais);
    }

    [HttpGet("consultar-profissional-por-id/{id:guid}")]
    public async Task<ActionResult<ProfissionalDto>> ObterProfissionaisPorId(Guid id)
    {
        var profissional = _mapper.Map<ProfissionalDto>(await _profissionalRepository.ObterPorId(id));

        if (profissional == null)
        {
            NotificarErro("Profissional não encontrado.");
            return NotFound();
        }

        return Ok(profissional);
    }

    [HttpGet("consultar-profissional-por-nome/{nome}")]
    public async Task<ActionResult<IEnumerable<ProfissionalDto>>> ObterProfissionaisPorNome(string nome)
    {
        var profissionais = await _profissionalRepository.ObterPorNome(nome);
        if (profissionais == null || !profissionais.Any())
        {
            NotificarErro("Nenhum profissional encontrado com o nome fornecido.");
            return NotFound();
        }
        var profissionaisDto = _mapper.Map<IEnumerable<ProfissionalDto>>(profissionais);
        return Ok(profissionaisDto);
    }

    [HttpGet("consultar-profissional-por-cro/{cro}")]
    public async Task<ActionResult<ProfissionalDto>> ObterProfissionaisPorCRO(string cro)
    {
        var profissional = _mapper.Map<ProfissionalDto>(await _profissionalRepository.ObterPorCRO(cro));
        if (profissional == null)
        {
            NotificarErro("Profissional não encontrado.");
            return NotFound();
        }
        return Ok(profissional);
    }

    [HttpPost("cadastrar-profissional")]
    public async Task<ActionResult<ProfissionalDto>> CriarProfissional([FromBody] Profissional profissional)
    {
        if (!ModelState.IsValid)
        {
            NotificarErro("Dados inválidos.");
            return BadRequest(ModelState);
        }
        await _profissionalRepository.Adicionar(profissional);
        await _profissionalRepository.SaveChanges();
        var profesionalDto = _mapper.Map<ProfissionalDto>(profissional);
        return CreatedAtAction(nameof(ObterProfissionaisPorId), new { id = profissional.Id }, profesionalDto);
    }

    [HttpPut("atualizar-profissional")]
    public async Task<ActionResult<AtualizarProfissionalDto>> AtualizarProfissional([FromBody] AtualizarProfissionalDto profissionalDto)
    {
        if (!ModelState.IsValid)
        {
            NotificarErro("Dados inválidos.");
            return BadRequest(ModelState);
        }

        var profissionalExistente = await _profissionalRepository.ObterPorId(profissionalDto.Id);

        if (profissionalExistente == null)
        {
            NotificarErro("Profissional não encontrado.");
            return NotFound();
        }

        await _profissionalRepository.Atualizar(profissionalExistente);
        await _profissionalRepository.SaveChanges();

        return Ok(profissionalDto);
    }


    [HttpDelete("excluir-profissional/{id:guid}")]
    public async Task<ActionResult> ExcluirProfissional(Guid id)
    {
        var profissional = _mapper.Map<ProfissionalDto>(await _profissionalRepository.ObterPorId(id));
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
