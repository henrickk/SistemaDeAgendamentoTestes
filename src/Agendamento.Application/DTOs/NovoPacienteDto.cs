using Agendamento.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agendamento.Application.DTOs;
public class NovoPacienteDto
{
    public string Nome { get; set; }
    public DateOnly DataDeNascimento { get; set; }
    public string CPF { get; set; }
    public string RG { get; set; }
    public string OrgaoEmissor { get; set; }
    public StatusGenero StatusGenero { get; set; }
    public StatusEstadoCivil StatusEstadoCivil { get; set; }
    public StatusPaciente StatusPaciente { get; set; }
    public Endereco Endereco { get; set; }
    public Contato Contato { get; set; }
}
