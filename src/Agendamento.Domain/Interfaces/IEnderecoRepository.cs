using Agendamento.Domain.Models;

namespace Agendamento.Domain.Interfaces;

public interface IEnderecoRepository : IRepository<Endereco>
{
    Task<Endereco> ObterPorLogradouro(string logradouro);
    Task<Endereco> ObterPorCEP(string cep);
}
