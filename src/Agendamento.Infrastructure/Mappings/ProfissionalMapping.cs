using Agendamento.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agendamento.Infrastructure.Mappings;
public class ProfissionalMapping : IEntityTypeConfiguration<Profissional>
{
    public void Configure(EntityTypeBuilder<Profissional> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasColumnType("varchar(200)");

        builder.Property(p => p.CRO)
            .IsRequired()
            .HasColumnType("varchar(20)");

        builder.Property(p => p.CPF)
            .IsRequired()
            .HasColumnType("varchar(14)");

        builder.Property(p => p.HoraInicio)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(p => p.HoraFim)
            .IsRequired()
            .HasColumnType("time");
    }
}
