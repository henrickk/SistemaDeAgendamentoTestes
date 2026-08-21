using Agendamento.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agendamento.Infrastructure.Mappings;
public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.Property(e => e.Logradouro)
            .IsRequired()
            .HasColumnType("varchar(200)");

        builder.Property(e => e.Numero)
            .IsRequired()
            .HasColumnType("varchar(20)");

        builder.Property(e => e.Complemento)
            .HasColumnType("varchar(100)");

        builder.Property(e => e.Bairro)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(e => e.Cidade)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(e => e.UF)
            .IsRequired()
            .HasColumnType("varchar(2)");

        builder.Property(e => e.CEP)
            .IsRequired()
            .HasColumnType("varchar(10)");
    }
}
