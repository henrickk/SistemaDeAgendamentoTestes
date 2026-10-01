using Agendamento.Domain.Notificacoes;

namespace Agendamento.Domain.Tests.Notificacoes;

public class NotificacaoTests
{
    [Fact]
    public void Notificacao_DeveCriar_QuandoMensagemForInformada()
    {
        // Arrange
        var mensagem = "Paciente não encontrado.";

        // Act
        var notificacao = new Notificacao(mensagem);

        // Assert
        Assert.Equal(mensagem, notificacao.Mensagem);
    }
}