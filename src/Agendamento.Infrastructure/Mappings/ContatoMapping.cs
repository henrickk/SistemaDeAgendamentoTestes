using Agendamento.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agendamento.Infrastructure.Mappings;
public class ContatoMapping : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.Property(c => c.Email)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(c => c.NumeroCelular)
            .IsRequired()
            .HasColumnType("varchar(20)");
    }
}
