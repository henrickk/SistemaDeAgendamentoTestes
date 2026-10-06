using Agendamento.Application.DTOs;
using Agendamento.Domain.Models;
using AutoMapper;

namespace Agendamento.API.Configurations;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<NovoAgendamentoDto, Agenda>()
            .ForMember(dest => dest.StatusAgendamento, opt => opt.MapFrom(src => StatusAgendamento.Agendado))
            .ForMember(dest => dest.Paciente, opt => opt.Ignore())
            .ForMember(dest => dest.Profissional, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCadastro, opt => opt.Ignore());

        CreateMap<Agenda, AgendadosDto>()
            .ForMember(dest => dest.PacienteNome, opt => opt.MapFrom(src => src.Paciente.Nome))
            .ForMember(dest => dest.PacienteContato, opt => opt.MapFrom(src => src.Paciente.Contato))
            .ForMember(dest => dest.ProfissionalNome, opt => opt.MapFrom(src => src.Profissional.Nome))
            .ForMember(dest => dest.CRO, opt => opt.MapFrom(src => src.Profissional.CRO));

        CreateMap<Profissional, ProfissionalDto>().ReverseMap();
        CreateMap<Profissional, AtualizarProfissionalDto>().ReverseMap();
        CreateMap<NovoProfissionalDto, Profissional>()
            .ForMember(dest => dest.HoraInicio, opt => opt.Ignore())
            .ForMember(dest => dest.HoraFim, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<NovoPacienteDto, Paciente>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<Paciente, PacienteDto>().ReverseMap();

        CreateMap<Endereco, EnderecoDto>().ReverseMap();

        CreateMap<Contato, ContatoDto>().ReverseMap();
    }
}
