using Agendamento.API.Controllers;
using Agendamento.API.Configurations;
using Agendamento.Application.DTOs;
using Agendamento.API.Tests.Auxiliar;
using Agendamento.Domain.Models;
using Agendamento.Domain.Notificacoes;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Agendamento.API.Tests.Controllers;

public class PacienteControllerTests
{
    private readonly FakePacienteRepository _repository = new();
    private readonly IMapper _mapper;
    private readonly Notificador _notificador = new();
    private readonly PacienteController _controller;

    public PacienteControllerTests()
    {
        var configuration = new MapperConfiguration(config => config.AddProfile<AutomapperConfig>());
        _mapper = configuration.CreateMapper();
        _controller = new PacienteController(_repository, _mapper, _notificador);
    }

    [Fact]
    public async Task ObterPacientePorId_DeveRetornarOkComPacienteMapeado()
    {
        var paciente = CriarPaciente();
        _repository.Pacientes.Add(paciente);

        var resultado = await _controller.ObterPacientePorId(paciente.Id);

        var resposta = Assert.IsType<OkObjectResult>(resultado.Result);
        var dto = Assert.IsType<PacienteDto>(resposta.Value);
        Assert.Equal(paciente.Id, dto.Id);
        Assert.Equal(paciente.Nome, dto.Nome);
        Assert.Equal(paciente.Contato.Email, dto.Contato.Email);
    }

    [Fact]
    public async Task ObterPacientePorId_DeveRetornarNotFoundQuandoNaoExistir()
    {
        var resultado = await _controller.ObterPacientePorId(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(resultado.Result);
        Assert.True(_notificador.TemNotificacao());
        Assert.Contains("Paciente não encontrado.", _notificador.ObterNotificacoes().Select(n => n.Mensagem));
    }

    [Fact]
    public async Task CriarPaciente_DevePersistirPacienteEDevolverCreated()
    {
        var novoPaciente = new NovoPacienteDto
        {
            Nome = "Ana Silva",
            DataNascimento = new DateOnly(1990, 5, 10),
            CPF = "12345678900",
            RG = "12345",
            StatusPaciente = StatusPaciente.Ativo,
            Contato = new ContatoDto { Email = "ana@teste.com", NumeroCelular = "11999999999" },
            Endereco = new EnderecoDto
            {
                Logradouro = "Rua A",
                Numero = "10",
                Bairro = "Centro",
                Cidade = "São Paulo",
                UF = "SP",
                Complemento = "",
                CEP = "01000000"
            }
        };

        var resultado = await _controller.CriarPaciente(novoPaciente);

        var resposta = Assert.IsType<ObjectResult>(resultado.Result);
        Assert.Equal(201, resposta.StatusCode);
        var pacienteSalvo = Assert.Single(_repository.Pacientes);
        var dto = Assert.IsType<PacienteDto>(resposta.Value);
        Assert.Equal("Ana Silva", pacienteSalvo.Nome);
        Assert.Equal("ana@teste.com", pacienteSalvo.Contato.Email);
        Assert.Equal(pacienteSalvo.Id, dto.Id);
        Assert.Equal(1, _repository.SaveChangesCount);
    }

    private static Paciente CriarPaciente() => new(
        "Paciente Teste", new DateOnly(1990, 1, 1), "12345678900", "12345",
        StatusGenero.Masculino, StatusEstadoCivil.Solteiro, StatusPaciente.Ativo,
        new Endereco("Rua A", "10", "Centro", "São Paulo", "SP", "", "01000000"),
        new Contato("paciente@teste.com", "11999999999"));

}
