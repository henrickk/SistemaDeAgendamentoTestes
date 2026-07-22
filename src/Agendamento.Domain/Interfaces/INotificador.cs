using Agendamento.Domain.Notificacoes;

namespace Agendamento.Domain.Interfaces;
public interface INotificador
{
    bool TemNotificacao();
    List<Notificacao> ObterNotificacoes();
    void Handle(Notificacao notificacao);
}
