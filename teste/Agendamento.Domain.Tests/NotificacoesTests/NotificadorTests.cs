using Agendamento.Domain.Notificacoes;

namespace Agendamento.Domain.Tests.Notificacoes;

public class NotificadorTests
{
    [Fact]
    public void Notificador_DeveIniciar_SemNotificacoes()
    {
        // Arrange
        var notificador = new Notificador();

        // Act
        var possuiNotificacao = notificador.TemNotificacao();

        // Assert
        Assert.False(possuiNotificacao);
    }

    [Fact]
    public void Notificador_DeveAdicionar_QuandoHandleForChamado()
    {
        // Arrange
        var notificador = new Notificador();
        var notificacao = new Notificacao("Paciente não encontrado.");

        // Act
        notificador.Handle(notificacao);

        // Assert
        Assert.True(notificador.TemNotificacao());

        var notificacoes = notificador.ObterNotificacoes();

        Assert.Single(notificacoes);
        Assert.Same(notificacao, notificacoes[0]);
    }

    [Fact]
    public void Notificador_DeveRetornarTodasAsNotificacoes_QuandoExistirem()
    {
        // Arrange
        var notificador = new Notificador();

        var notificacao1 = new Notificacao("Paciente não encontrado.");
        var notificacao2 = new Notificacao("Profissional não encontrado.");

        // Act
        notificador.Handle(notificacao1);
        notificador.Handle(notificacao2);

        var notificacoes = notificador.ObterNotificacoes();

        // Assert
        Assert.Equal(2, notificacoes.Count);
        Assert.Contains(notificacao1, notificacoes);
        Assert.Contains(notificacao2, notificacoes);
    }
}