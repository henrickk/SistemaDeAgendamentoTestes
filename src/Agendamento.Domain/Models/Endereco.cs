namespace Agendamento.Domain.Models;
public class Endereco
{
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string UF { get; set; }
    public string Complemento { get; set; }
    public string CEP { get; set; }

    public Endereco(string logradouro, string numero, string bairro, string cidade, string uf, string complemento, string CEP)
    {
        Logradouro = logradouro;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        UF = uf;
        Complemento = complemento;
        CEP = complemento;
    }
}
