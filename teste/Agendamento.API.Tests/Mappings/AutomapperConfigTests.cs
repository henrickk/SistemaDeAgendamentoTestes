using Agendamento.API.Configurations;
using AutoMapper;

namespace Agendamento.API.Tests.Mappings;

public class AutomapperConfigTests
{
    [Fact]
    public void Configuracao_DeveMapearTodosOsMembros()
    {
        var configuration = new MapperConfiguration(config => config.AddProfile<AutomapperConfig>());

        configuration.AssertConfigurationIsValid();
    }
}
