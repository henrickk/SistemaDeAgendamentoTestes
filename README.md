# Sistema de Agendamento

API REST para gerenciamento de pacientes, profissionais e agendamentos de consultas. O projeto foi desenvolvido para praticar arquitetura em camadas, regras de negócio e testes automatizados.

## Sobre o projeto

O sistema permite cadastrar e consultar pacientes e profissionais, além de criar, atualizar e acompanhar agendamentos. As regras de negócio ficam na camada de aplicação e as operações de persistência usam Entity Framework Core.

Este projeto também foi usado para praticar **testes unitários** e **TDD em fluxos dos serviços**: escrever o teste para descrever o comportamento esperado, implementar a regra e ajustar a solução até o teste passar.

## Tecnologias

- C# e .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server (LocalDB na configuração padrão)
- AutoMapper
- xUnit e Moq
- SQLite em memória nos testes de infraestrutura

## Organização da solução

```text
src/
├── Agendamento.API/             # Controllers, configuração e inicialização da API
├── Agendamento.Application/     # Serviços, interfaces e DTOs
├── Agendamento.Domain/          # Entidades, interfaces, validações e notificações
└── Agendamento.Infrastructure/  # DbContext, mapeamentos, migrations e repositórios

teste/
├── Agendamento.API.Tests/           # Testes dos controllers e configuração do AutoMapper
├── Agendamento.Application.Tests/   # Testes unitários dos serviços e regras de negócio
├── Agendamento.Domain.Tests/        # Testes de notificações
└── Agendamento.Infrastructure.Tests/# Testes de contexto e repositórios
```

## Requisitos

- .NET SDK 8.0
- SQL Server LocalDB ou outra instância SQL Server acessível
- Entity Framework Core CLI (`dotnet-ef`) para aplicar migrations pela linha de comando

## Configuração e execução

1. Clone o repositório e abra um terminal na pasta do projeto.

2. Confira a connection string em `src/Agendamento.API/appsettings.json`. A configuração padrão usa SQL Server LocalDB e cria/usa o banco `Agendamento`. Se necessário, substitua `DefaultConnection` pela connection string do seu SQL Server.

3. Aplique as migrations:

   ```bash
   dotnet ef database update --project src/Agendamento.Infrastructure --startup-project src/Agendamento.API
   ```

4. Inicie a API:

   ```bash
   dotnet run --project src/Agendamento.API
   ```

No ambiente Development, o Swagger é habilitado pela aplicação. Use a URL indicada no terminal e acrescente `/swagger` para abrir a documentação interativa.

## Endpoints

### Pacientes

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/paciente/consultar-pacientes` | Lista pacientes |
| `GET` | `/api/paciente/consultar-paciente/{id}` | Consulta paciente por ID |
| `GET` | `/api/paciente/consultar-paciente-por-cpf/{cpf}` | Consulta paciente por CPF |
| `GET` | `/api/paciente/consultar-paciente-por-nome/{nome}` | Busca paciente por nome |
| `POST` | `/api/paciente/criar-paciente` | Cadastra paciente |
| `PUT` | `/api/paciente/atualizar-paciente/{id}` | Atualiza paciente |
| `DELETE` | `/api/paciente/excluir-paciente/{id}` | Exclui paciente |

### Profissionais

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/profissional/consultar-profissionais` | Lista profissionais |
| `GET` | `/api/profissional/consultar-profissional-por-id/{id}` | Consulta profissional por ID |
| `GET` | `/api/profissional/consultar-profissional-por-nome/{nome}` | Busca profissionais por nome |
| `GET` | `/api/profissional/consultar-profissional-por-cro/{cro}` | Consulta profissional por CRO |
| `POST` | `/api/profissional/cadastrar-profissional` | Cadastra profissional |
| `PUT` | `/api/profissional/atualizar-profissional/{id}` | Atualiza profissional |
| `DELETE` | `/api/profissional/excluir-profissional/{id}` | Exclui profissional |

### Agendamentos

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/agenda/consultar-agendamentos` | Lista agendamentos |
| `GET` | `/api/agenda/obter-por-id/{id}` | Consulta agendamento por ID |
| `POST` | `/api/agenda/agendar-consulta` | Cria agendamento |
| `PUT` | `/api/agenda/atualizar/{id}` | Atualiza agendamento |
| `DELETE` | `/api/agenda/cancelar-agendamento/{id}` | Cancela agendamento |

Os endpoints de criação e atualização recebem os DTOs definidos no projeto `Agendamento.Application`. Consulte o Swagger para ver os campos e tipos esperados em cada requisição.

## Testes

Os testes estão divididos por camada:

- **Domínio:** notificações e notificador.
- **Aplicação:** serviços e regras de negócio para pacientes, profissionais e agendamentos; uso de Moq para simular dependências.
- **Infraestrutura:** operações dos repositórios, buscas, relacionamentos, conflitos de horário, mapeamentos do EF Core e comportamento do `MeuDbContext`. SQLite em memória mantém os testes isolados do SQL Server.
- **API:** configuração do AutoMapper e respostas do `PacienteController`.

Para executar todos os testes:

```bash
dotnet test SistemaDeAgendamentoTestes.sln
```

Na última execução registrada durante a preparação deste README, a solução tinha **86 testes aprovados**. A quantidade pode mudar conforme novos testes forem adicionados.

## Próximos passos

- Ampliar os testes dos controllers de agenda e profissionais.
- Criar testes de integração HTTP para validar os fluxos completos da API.
- Expandir os testes das validações do domínio.

## Licença

Nenhuma licença foi definida neste repositório até o momento.
