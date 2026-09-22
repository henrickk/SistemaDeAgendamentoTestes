using Agendamento.Application.DTOs;
using Agendamento.Domain.Models;
using AutoMapper;

namespace Agendamento.API.Configurations;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<Profissional, ProfissionalDto>().ReverseMap();
        CreateMap<Agenda, AgendadosDto>().ReverseMap();
        CreateMap<Profissional, AtualizarProfissionalDto>().ReverseMap();

        CreateMap<NovoPacienteDto, Paciente>().ReverseMap();
        CreateMap<Paciente, PacienteDto>().ReverseMap();

        CreateMap<Contato, ContatoDto>().ReverseMap();
    }
}
