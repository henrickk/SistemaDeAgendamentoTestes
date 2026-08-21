using Agendamento.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agendamento.Infrastructure.Mappings;
public class AgendaMapping : IEntityTypeConfiguration<Agenda>
{
    public void Configure(EntityTypeBuilder<Agenda> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.DataInicio)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(a => a.DataFim)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(a => a.Observacao)
                .HasColumnType("varchar(1000)");

        builder.Property(a => a.StatusAgendamento)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("varchar(50)");

        builder.HasOne(a => a.Paciente)
            .WithMany()
            .HasForeignKey(a => a.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Profissional)
            .WithMany()
            .HasForeignKey(a => a.ProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
