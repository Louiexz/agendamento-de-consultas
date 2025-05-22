using Microsoft.EntityFrameworkCore;
using UnitSaude.Data;
using UnitSaude.Dto.Consulta;
using UnitSaude.Dto.Paciente;
using UnitSaude.Dtos.Consulta;
using UnitSaude.Interfaces;
using UnitSaude.Models;
using Hangfire;
using UnitSaude.Utils;


namespace UnitSaude.Services
{
    public class ConsultaService : ConsultaInterface
    {
        private readonly ClinicaDbContext _context;
        private readonly EmailService _emailService;  // Incluir o serviço de e-mail
        private readonly IBackgroundJobClient _backgroundJobClient; // Substitui IBackgroundTaskQueue

        public ConsultaService(ClinicaDbContext context, EmailService emailService, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _emailService = emailService;  // Inicializando o serviço de e-mail
            _backgroundJobClient = backgroundJobClient;

        }

        public async Task<ResponseModel<ReadConsultaDto>> CadastrarConsulta(CreateConsultaDto consultaDTO)
        {
            ResponseModel<ReadConsultaDto> response = new();

            try
            {
                // Verifica se o paciente existe
                var paciente = await _context.Pacientes
                    .FirstOrDefaultAsync(p => p.Id_Usuario == consultaDTO.PacienteId);

                if (paciente == null)
                {
                    response.Status = false;
                    response.Message = "Paciente não encontrado.";
                    return response;
                }

                // Verifica se o professor existe
                var professor = await _context.Professores
                    .FirstOrDefaultAsync(p => p.Id_Usuario == consultaDTO.ProfessorId);

                if (professor == null)
                {
                    response.Status = false;
                    response.Message = "Professor não encontrado.";
                    return response;
                }

                // Verifica se a data é um domingo
                if (consultaDTO.Data.HasValue &&
                    consultaDTO.Data.Value.ToDateTime(TimeOnly.MinValue).DayOfWeek == DayOfWeek.Sunday)
                {
                    response.Status = false;
                    response.Message = "Não é possível agendar consultas aos domingos.";
                    return response;
                }

                // Validação de área e especialidade
                if (!DadosFixosConsulta.EspecialidadesPorArea.ContainsKey(consultaDTO.Area))
                {
                    response.Status = false;
                    response.Message = "Área inválida.";
                    return response;
                }

                var especialidadesPermitidas = DadosFixosConsulta.EspecialidadesPorArea[consultaDTO.Area];
                if (!especialidadesPermitidas.Contains(consultaDTO.Especialidade))
                {
                    response.Status = false;
                    response.Message = "Especialidade inválida para a área selecionada.";
                    return response;
                }

                // Verifica se a data está dentro de alguma disponibilidade
                var disponibilidade = await _context.Disponibilidades
                    .Where(d => d.Area == consultaDTO.Area &&
                                d.Especialidade == consultaDTO.Especialidade &&
                                d.DataInicio <= consultaDTO.Data &&
                                d.DataFim >= consultaDTO.Data &&
                                d.Ativo)
                    .FirstOrDefaultAsync();

                // Verifica se já existe alguma consulta AGENDADA ou PENDENTE no mesmo horário
                var conflito = await _context.Consultas
                    .AnyAsync(c =>
                        c.Area == consultaDTO.Area &&
                        c.Especialidade == consultaDTO.Especialidade &&
                        (c.Status == "Agendada" || c.Status == "Pendente") && // Agora verifica ambos status
                        c.Data == consultaDTO.Data &&
                        c.Horario == consultaDTO.Horario);


                // Verifica se o paciente já possui uma consulta no mesmo dia e horário
                var pacienteConflito = await _context.Consultas
                    .AnyAsync(c =>
                        c.PacienteId == consultaDTO.PacienteId &&
                        c.Data == consultaDTO.Data &&
                        c.Horario == consultaDTO.Horario &&
                        c.Status != "Concluída" &&
                        c.Status != "Cancelada");

                if (pacienteConflito)
                {
                    response.Status = false;
                    response.Message = "O paciente já possui uma consulta marcada nesse horário.";
                    return response;
                }


                // Define o status com base nas condições
                string status;
                if (disponibilidade == null || conflito)
                {
                    status = "Em Espera";
                }
                else
                {
                    status = "Pendente";
                }

                if (!DadosFixosConsulta.Status.Contains(status))
                {
                    response.Status = false;
                    response.Message = "Status da consulta inválido.";
                    return response;
                }

                // Criação da consulta
                var consulta = new Consulta
                {
                    Data = consultaDTO.Data,
                    Horario = consultaDTO.Horario,
                    Status = status,
                    Area = consultaDTO.Area,
                    Especialidade = consultaDTO.Especialidade,
                    Anamnese = consultaDTO.Anamnese,
                    PacienteId = consultaDTO.PacienteId,
                    ProfessorId = consultaDTO.ProfessorId,
                    Professor = professor,
                    Paciente = paciente,
                    DataCadastro = consultaDTO.DataCadastro = DateTime.UtcNow
                };

                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();

                var readConsultaDto = new ReadConsultaDto
                {
                    id_Consulta = consulta.id_Consulta,
                    Data = consulta.Data,
                    Horario = consulta.Horario,
                    Status = consulta.Status,
                    Area = consulta.Area,
                    Especialidade = consulta.Especialidade,
                    Anamnese = consulta.Anamnese,
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = paciente.nome,
                    NomeProfessor = professor.nome
                };

                response.Status = true;
                response.Data = readConsultaDto;
                response.Message = status == "Pendente"
                    ? "Consulta agendada, aguardando confirmação"
                    : "Sem horários disponíveis. Consulta cadastrada como 'Em Espera'.";

                // Enviar notificação de e-mail para o paciente e professor
                if (status == "Agendada")
                {
                    await _emailService.EnviarAsync(paciente.email, "Consulta Agendada", $"Sua consulta com o professor {professor.nome} foi agendada para {consulta.Data} às {consulta.Horario}.");
                    await _emailService.EnviarAsync(professor.email, "Consulta Agendada", $"Você tem uma nova consulta agendada com o paciente {paciente.nome} para {consulta.Data} às {consulta.Horario}.");
                }
                else
                {
                    await _emailService.EnviarAsync(paciente.email, "Consulta em Espera", "Sua consulta foi cadastrada e está aguardando disponibilidade. Você será notificado quando ela for confirmada.");
                }

                if (consulta.Data.HasValue)
                {
                    var dataConsulta = consulta.Data.Value.ToDateTime((TimeOnly)consulta.Horario);
                    if (dataConsulta > DateTime.Now)
                    {
                        var delay = dataConsulta - DateTime.Now;

                        // Agenda o job com Hangfire
                        _backgroundJobClient.Schedule(
                            () => VerificarEConcluirConsultaAutomaticamente(consulta.id_Consulta),
                            delay
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task VerificarEConcluirConsultaAutomaticamente(int consultaId)
        {
            try
            {
                using (var scope = ServiceActivator.GetScope()) // Para resolver dependências
                {
                    var context = scope.ServiceProvider.GetService<ClinicaDbContext>();
                    var emailService = scope.ServiceProvider.GetService<EmailService>();

                    var consulta = await context.Consultas.FindAsync(consultaId);
                    if (consulta == null || consulta.Status == "Concluída" || consulta.Status == "Cancelada")
                        return;

                    // Verifica se a data da consulta já passou
                    var dataConsulta = consulta.Data.Value.ToDateTime((TimeOnly)consulta.Horario);
                    if (dataConsulta < DateTime.Now)
                    {
                        consulta.Status = "Concluída";
                        await context.SaveChangesAsync();

                        // Notificar paciente e professor
                        var paciente = await context.Pacientes.FindAsync(consulta.PacienteId);
                        var professor = await context.Professores.FindAsync(consulta.ProfessorId);

                        if (paciente != null)
                        {
                            await emailService.EnviarAsync(
                                paciente.email,
                                "Consulta Concluída Automaticamente",
                                $"Sua consulta com {professor?.nome} foi marcada como concluída automaticamente pois não foi atualizada após a data agendada."
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Logar o erro - Hangfire tem seu próprio sistema de logs
                Console.WriteLine($"Erro ao concluir consulta automaticamente: {ex.Message}");
                throw; // Permite que o Hangfire gerente novas tentativas
            }
        }

        // Novo endpoint para confirmar consulta
        public async Task<ResponseModel<ReadConsultaDto>> ConfirmarConsulta(int consultaId)
        {
            var response = new ResponseModel<ReadConsultaDto>();

            try
            {
                var consulta = await _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .FirstOrDefaultAsync(c => c.id_Consulta == consultaId);

                if (consulta == null)
                {
                    response.Status = false;
                    response.Message = "Consulta não encontrada.";
                    return response;
                }

                if (consulta.Status != "Pendente")
                {
                    response.Status = false;
                    response.Message = $"A consulta já está com status {consulta.Status} e não pode ser confirmada.";
                    return response;
                }

                // Verifica disponibilidade
                var disponibilidade = await _context.Disponibilidades
                    .Where(d => d.Area == consulta.Area &&
                                d.Especialidade == consulta.Especialidade &&
                                d.DataInicio <= consulta.Data &&
                                d.DataFim >= consulta.Data &&
                                d.Ativo)
                    .FirstOrDefaultAsync();

                // Verifica se já existe consulta AGENDADA no mesmo horário
                var conflito = await _context.Consultas
                    .AnyAsync(c => c.id_Consulta != consulta.id_Consulta &&
                                   c.Area == consulta.Area &&
                                   c.Especialidade == consulta.Especialidade &&
                                   c.Status == "Agendada" && // Apenas verificar consultas já agendadas
                                   c.Data == consulta.Data &&
                                   c.Horario == consulta.Horario);

                if (disponibilidade != null && !conflito)
                {
                    // Atualiza para Agendada
                    consulta.Status = "Agendada";

                    // Notificar paciente e professor
                    await _emailService.EnviarAsync(consulta.Paciente.email, "Consulta Confirmada",
                        $"Sua consulta com {consulta.Professor.nome} foi confirmada para {consulta.Data} às {consulta.Horario}.");

                    await _emailService.EnviarAsync(consulta.Professor.email, "Consulta Confirmada",
                        $"A consulta com {consulta.Paciente.nome} foi confirmada para {consulta.Data} às {consulta.Horario}.");
                }
                else
                {
                    // Mantém como Pendente se não houver disponibilidade
                    response.Status = false;
                    response.Message = "Não há disponibilidade para confirmar esta consulta no momento.";
                    return response;
                }

                await _context.SaveChangesAsync();

                // Verifica consultas em espera APÓS confirmar esta
                await VerificarConsultasEmEspera(consulta.Area, consulta.Especialidade, consulta.Data.Value, (TimeOnly)consulta.Horario);

                response.Data = new ReadConsultaDto
                {
                    id_Consulta = consulta.id_Consulta,
                    Data = consulta.Data,
                    Horario = consulta.Horario,
                    Status = consulta.Status,
                    Area = consulta.Area,
                    Especialidade = consulta.Especialidade,
                    Anamnese = consulta.Anamnese,
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = consulta.Paciente.nome,
                    NomeProfessor = consulta.Professor.nome
                };

                response.Message = "Consulta confirmada com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<string>> CancelarConsulta(int consultaId)
        {
            var response = new ResponseModel<string>();

            try
            {
                // Busca a consulta incluindo relacionamentos para envio de e-mail
                var consulta = await _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .FirstOrDefaultAsync(c => c.id_Consulta == consultaId);

                if (consulta == null)
                {
                    response.Status = false;
                    response.Message = "Consulta não encontrada.";
                    return response;
                }

                // Verifica se a consulta já está cancelada ou concluída
                if (consulta.Status == "Cancelada" || consulta.Status == "Concluída")
                {
                    response.Status = false;
                    response.Message = $"Não é possível cancelar uma consulta com status '{consulta.Status}'.";
                    return response;
                }

                // Regras de cancelamento:
                // - Consultas com status "Pendente" ou "Em Espera" podem ser canceladas imediatamente
                // - Consultas "Agendadas" só podem ser canceladas após 24h da confirmação
                if (consulta.Status == "Agendada")
                {
                    // Precisamos encontrar quando a consulta foi confirmada (mudou para Agendada)
                    // Como não temos esse registro, vamos assumir que foi quando o status foi alterado
                    // Ou usar DataCadastro como fallback (não ideal)

                    // Esta é uma limitação - idealmente você deveria ter um campo DataConfirmacao
                    var dataConfirmacao = consulta.DataCadastro; // Isso é aproximado

                    // Verifica se passaram menos de 24 horas
                    if (DateTime.UtcNow < dataConfirmacao.AddHours(24))
                    {
                        response.Status = false;
                        response.Message = "Consultas agendadas só podem ser canceladas após 24 horas da confirmação.";
                        return response;
                    }
                }

                // Guarda o status anterior para verificar a fila depois
                var statusAnterior = consulta.Status;

                // Atualiza o status para Cancelada
                consulta.Status = "Cancelada";
                await _context.SaveChangesAsync();

                // Se a consulta estava agendada ou pendente, verifica se há consultas em espera
                if (statusAnterior == "Agendada" || statusAnterior == "Pendente")
                {
                    await VerificarConsultasEmEspera(
                        consulta.Area,
                        consulta.Especialidade,
                        consulta.Data.Value,
                        (TimeOnly)consulta.Horario);
                }

                // Envia e-mails de notificação
                await _emailService.EnviarAsync(
                    consulta.Paciente.email,
                    "Consulta Cancelada",
                    $"Sua consulta com {consulta.Professor.nome} para {consulta.Data} às {consulta.Horario} foi cancelada.");

                await _emailService.EnviarAsync(
                    consulta.Professor.email,
                    "Consulta Cancelada",
                    $"A consulta com {consulta.Paciente.nome} para {consulta.Data} às {consulta.Horario} foi cancelada.");

                response.Status = true;
                response.Message = "Consulta cancelada com sucesso!";
                response.Data = consulta.Status;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task VerificarConsultasEmEspera(string area, string especialidade, DateOnly data, TimeOnly horario)
        {
            try
            {
                // Verifica se há disponibilidade para este horário
                var disponibilidade = await _context.Disponibilidades
                    .AnyAsync(d => d.Area == area &&
                                  d.Especialidade == especialidade &&
                                  d.DataInicio <= data &&
                                  d.DataFim >= data &&
                                  d.HorarioInicio <= horario &&
                                  d.HorarioFim >= horario &&
                                  d.Ativo);

                if (!disponibilidade)
                {
                    Console.WriteLine("[Fila] Nenhuma disponibilidade encontrada para este horário.");
                    return;
                }

                // Verifica se já existe consulta AGENDADA ou PENDENTE neste horário
                var horarioOcupado = await _context.Consultas
                    .AnyAsync(c => c.Area == area &&
                                  c.Especialidade == especialidade &&
                                  (c.Status == "Agendada" || c.Status == "Pendente") &&
                                  c.Data == data &&
                                  c.Horario == horario);

                if (horarioOcupado)
                {
                    Console.WriteLine("[Fila] Horário já ocupado por outra consulta ativa.");
                    return;
                }

                // Busca a próxima consulta em espera para este horário (ordem de chegada)
                var proximaConsultaEmEspera = await _context.Consultas
                    .Where(c => c.Area == area &&
                               c.Especialidade == especialidade &&
                               c.Status == "Em Espera" &&
                               c.Data == data &&
                               c.Horario == horario)
                    .OrderBy(c => c.DataCadastro)
                    .FirstOrDefaultAsync();

                if (proximaConsultaEmEspera != null)
                {
                    // Muda para PENDENTE (precisa ser confirmada)
                    proximaConsultaEmEspera.Status = "Pendente";
                    await _context.SaveChangesAsync();

                    // Notifica o paciente
                    var paciente = await _context.Pacientes.FindAsync(proximaConsultaEmEspera.PacienteId);
                    if (paciente != null)
                    {
                        await _emailService.EnviarAsync(
                            paciente.email,
                            "Consulta Disponível - Confirmação Necessária",
                            $"Um horário ficou disponível para sua consulta em {data} às {horario}. " +
                            "Sua consulta está como 'Pendente' e será confirmada em breve."
                        );
                    }

                    Console.WriteLine($"[Fila] Consulta {proximaConsultaEmEspera.id_Consulta} movida para Pendente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Fila] Erro ao verificar consultas em espera: {ex.Message}");
            }
        }

        public async Task<ResponseModel<List<HorarioDisponivelDto>>> ObterHorariosDisponiveis(DateOnly data, string area, string especialidade)
        {
            var response = new ResponseModel<List<HorarioDisponivelDto>>();

            try
            {
                // Busca as disponibilidades para a área e especialidade
                var disponibilidades = await _context.Disponibilidades
                    .Where(d => d.Area == area && d.Especialidade == especialidade
                                && d.DataInicio <= data && d.DataFim >= data)
                    .ToListAsync();

                if (disponibilidades.Count == 0)
                {
                    response.Status = false;
                    response.Message = "Não há disponibilidade para a data e especialidade informadas.";
                    response.Data = new List<HorarioDisponivelDto>();
                    return response;
                }

                List<HorarioDisponivelDto> horariosDisponiveis = new();

                // Para cada disponibilidade encontrada, adiciona os horários válidos
                foreach (var disponibilidade in disponibilidades)
                {
                    TimeOnly horarioAtual = disponibilidade.HorarioInicio;

                    while (horarioAtual.Add(TimeSpan.FromMinutes(55)) <= disponibilidade.HorarioFim)
                    {
                        // Verifica se esse horário está ocupado por uma consulta
                        var consultaExistente = await _context.Consultas
                            .AnyAsync(c => c.Area == area && c.Especialidade == especialidade
                                           && c.Data == data && c.Horario == horarioAtual);

                        // Se não houver consulta nesse horário, marca como "Disponível"
                        var status = !consultaExistente ? "Disponível" : "Indisponível, entrar na fila de espera";

                        // Adiciona o horário e seu status na lista de horários
                        horariosDisponiveis.Add(new HorarioDisponivelDto
                        {
                            Horario = horarioAtual.ToString("HH:mm"),
                            Status = status
                        });

                        horarioAtual = horarioAtual.Add(TimeSpan.FromMinutes(55));
                    }
                }

                response.Status = true;
                response.Message = "Horários disponíveis encontrados.";
                response.Data = horariosDisponiveis;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                response.Data = new List<HorarioDisponivelDto>();
            }

            return response;
        }


        public async Task<ResponseModel<string>> AtualizarStatusConsulta(int id, UpdateStatusConsultaDto dto)
        {
            var response = new ResponseModel<string>();

            try
            {
                // Verifica se o novo status é válido
                if (!DadosFixosConsulta.Status.Contains(dto.NovoStatus))
                {
                    response.Status = false;
                    response.Message = "Status inválido.";
                    return response;
                }

                // Encontra a consulta pelo ID
                var consulta = await _context.Consultas.FindAsync(id);

                if (consulta == null)
                {
                    response.Status = false;
                    response.Message = "Consulta não encontrada.";
                    return response;
                }
                var statusAnterior = consulta.Status;

                // Caso a consulta esteja em "Em Espera", realiza a verificação de disponibilidade
                if (consulta.Status == "Em Espera" && dto.NovoStatus == "Agendada")
                {
                    var disponibilidade = await _context.Disponibilidades
                        .Where(d => d.Area == consulta.Area &&
                                    d.Especialidade == consulta.Especialidade &&
                                    d.DataInicio <= consulta.Data &&
                                    d.DataFim >= consulta.Data &&
                                    d.Ativo)
                        .FirstOrDefaultAsync();

                    // Verifica se o horário está disponível para a consulta
                    var conflito = await _context.Consultas
                        .AnyAsync(c => c.Area == consulta.Area &&
                                       c.Especialidade == consulta.Especialidade &&
                                       c.Status != "Concluída" &&
                                       c.Status != "Cancelada" &&
                                       c.Data == consulta.Data &&
                                       c.Horario == consulta.Horario);



                    // Se não houver conflito e houver disponibilidade, muda o status para "Agendada"
                    if (disponibilidade != null && !conflito)
                    {
                        consulta.Status = "Agendada";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Não há disponibilidade para agendar essa consulta.";
                        return response;
                    }
                }
                else
                {
                    // Caso contrário, apenas atualiza o status normalmente
                    consulta.Status = dto.NovoStatus;
                }

                await _context.SaveChangesAsync();

                // Se a consulta foi concluída ou cancelada, verifica se há consultas em espera
                if ((dto.NovoStatus == "Concluída" || dto.NovoStatus == "Cancelada") &&
                    (statusAnterior == "Agendada" || statusAnterior == "Pendente"))
                {
                    await VerificarConsultasEmEspera(consulta.Area, consulta.Especialidade, consulta.Data.Value, (TimeOnly)consulta.Horario);
                }

                // Busca os dados do paciente e do professor para envio de e-mails
                var paciente = await _context.Pacientes.FindAsync(consulta.PacienteId);
                var professor = await _context.Professores.FindAsync(consulta.ProfessorId);



                if (consulta.Status == "Agendada")
                {
                    await _emailService.EnviarAsync(paciente.email, "Consulta Agendada",
                        $"Sua consulta foi agendada para o dia {consulta.Data} às {consulta.Horario} com o professor {professor.nome}.");

                    await _emailService.EnviarAsync(professor.email, "Nova Consulta Agendada",
                        $"Você tem uma nova consulta agendada com o paciente {paciente.nome} para {consulta.Data} às {consulta.Horario}.");
                }
                else if (consulta.Status == "Cancelada")
                {
                    await _emailService.EnviarAsync(paciente.email, "Consulta Cancelada",
                        $"Sua consulta com o professor {professor.nome} no dia {consulta.Data} às {consulta.Horario} foi cancelada.");

                    await _emailService.EnviarAsync(professor.email, "Consulta Cancelada",
                        $"A consulta com o paciente {paciente.nome} marcada para o dia {consulta.Data} às {consulta.Horario} foi cancelada.");
                }
                else if (consulta.Status == "Concluída")
                {
                    await _emailService.EnviarAsync(paciente.email, "Consulta Concluída",
                        $"Sua consulta com o professor {professor.nome} no dia {consulta.Data} às {consulta.Horario} foi concluída com sucesso.");
                }

                response.Status = true;
                response.Message = "Status atualizado com sucesso!";
                response.Data = consulta.Status;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<List<ConsultasEmEsperaResumoDto>>> ObterResumoConsultasEmEspera()
        {
            var response = new ResponseModel<List<ConsultasEmEsperaResumoDto>>();

            try
            {
                var resumo = await _context.Consultas
                    .Where(c => c.Status == "Em Espera")
                    .GroupBy(c => new { c.Area, c.Especialidade, c.Data })
                    .Select(g => new ConsultasEmEsperaResumoDto
                    {
                        Area = g.Key.Area,
                        Especialidade = g.Key.Especialidade,
                        Data = (DateOnly)g.Key.Data,
                        TotalEmEspera = g.Count()
                    })
                    .ToListAsync();

                response.Status = true;
                response.Message = "Resumo da fila de espera obtido com sucesso.";
                response.Data = resumo;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"Erro ao obter resumo: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseModel<string>> ReagendarConsulta(int id, ReagendarConsultaDto dto)
        {
            var response = new ResponseModel<string>();

            try
            {
                var consulta = await _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .FirstOrDefaultAsync(c => c.id_Consulta == id);

                if (consulta == null)
                {
                    response.Status = false;
                    response.Message = "Consulta não encontrada.";
                    return response;
                }

                if (consulta.Status == "Concluída" || consulta.Status == "Cancelada")
                {
                    response.Status = false;
                    response.Message = "Não é possível reagendar uma consulta concluída ou cancelada.";
                    return response;
                }

                // Guarda os valores ANTIGOS para verificar a fila depois
                var dataOriginal = consulta.Data;
                var horarioOriginal = consulta.Horario;
                var areaOriginal = consulta.Area;
                var especialidadeOriginal = consulta.Especialidade;

                // Verifica conflitos no NOVO horário
                var conflito = await _context.Consultas.AnyAsync(c =>
                    c.id_Consulta != consulta.id_Consulta &&
                    c.Data == dto.NovaData &&
                    c.Horario == dto.NovoHorario &&
                    c.Area == consulta.Area &&
                    c.Especialidade == consulta.Especialidade &&
                    (c.Status == "Agendada" || c.Status == "Pendente")); // Apenas verificar consultas ativas

                if (conflito)
                {
                    response.Status = false;
                    response.Message = "Já existe uma consulta ativa nesse horário.";
                    return response;
                }

                // Atualiza para os novos valores
                consulta.Data = dto.NovaData;
                consulta.Horario = dto.NovoHorario;

                // Se estava agendada, mantém como agendada
                // Se estava pendente ou em espera, mantém o status
                await _context.SaveChangesAsync();

                // VERIFICA CONSULTAS EM ESPERA PARA O HORÁRIO ORIGINAL (que ficou livre)
                if (consulta.Status == "Agendada") // Só libera se estava agendada
                {
                    await VerificarConsultasEmEspera(areaOriginal, especialidadeOriginal, dataOriginal.Value, (TimeOnly)horarioOriginal);
                }

                // Notificações por e-mail
                await _emailService.EnviarAsync(consulta.Paciente.email, "Consulta Reagendada",
                    $"Sua consulta foi reagendada para {dto.NovaData} às {dto.NovoHorario}.");

                await _emailService.EnviarAsync(consulta.Professor.email, "Consulta Reagendada",
                    $"A consulta com {consulta.Paciente.nome} foi reagendada para {dto.NovaData} às {dto.NovoHorario}.");

                response.Status = true;
                response.Message = "Consulta reagendada com sucesso!";
                response.Data = $"{dto.NovaData} às {dto.NovoHorario}";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<ResponseModel<ReadConsultaDto>> ListarConsultaPorId(int ConsultaId)
        {

            ResponseModel<ReadConsultaDto> response = new();

            try
            {
                var consulta = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Professor)
                .FirstOrDefaultAsync(c => c.id_Consulta == ConsultaId);


                if (consulta == null)
                {
                    response.Message = "Consulta n�o encontrado.";
                    return response;
                }

                var consultaDTO = new ReadConsultaDto
                {
                    id_Consulta = consulta.id_Consulta,
                    Data = consulta.Data,
                    Horario = consulta.Horario,
                    Status = consulta.Status,
                    Area = consulta.Area,
                    Especialidade = consulta.Especialidade,
                    Anamnese = consulta.Anamnese,
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = consulta.Paciente.nome,
                    NomeProfessor = consulta.Professor.nome,
                };

                response.Data = consultaDTO;
                response.Message = "Consulta encontrada com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;

            }

            return response;
        }

        public async Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultasPorNomeOuCpf(string valor)
        {
            ResponseModel<List<ReadConsultaDto>> response = new();

            try
            {
                var paciente = await _context.Pacientes
                    .FirstOrDefaultAsync(p =>
                        p.nome.ToLower().Contains(valor.ToLower()) || p.cpf == valor);


                if (paciente == null)
                {
                    response.Message = "Paciente n�o encontrado.";
                    return response;
                }

                var consultas = await _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .Where(c => c.PacienteId == paciente.Id_Usuario)
                    .ToListAsync();

                if (!consultas.Any())
                {
                    response.Message = "Nenhuma consulta encontrada para esse paciente.";
                    return response;
                }

                var consultaDTOs = consultas.Select(consulta => new ReadConsultaDto
                {
                    id_Consulta = consulta.id_Consulta,
                    Data = consulta.Data,
                    Horario = consulta.Horario,
                    Status = consulta.Status,
                    Area = consulta.Area,
                    Especialidade = consulta.Especialidade,
                    Anamnese = consulta.Anamnese,
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = consulta.Paciente.nome,
                    NomeProfessor = consulta.Professor.nome,
                }).ToList();

                response.Data = consultaDTOs;
                response.Message = "Consultas encontradas com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorPaciente(int pacienteId)
        {
            ResponseModel<List<ReadConsultaDto>> response = new();

            try
            {
                var consultas = await _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .Where(c => c.PacienteId == pacienteId)
                    .ToListAsync();

                if (consultas == null || !consultas.Any())
                {
                    response.Message = "Nenhuma consulta com esse paciente.";
                    return response;
                }

                var consultaDTOs = consultas.Select(consulta => new ReadConsultaDto
                {
                    id_Consulta = consulta.id_Consulta,
                    Data = consulta.Data,
                    Horario = consulta.Horario,
                    Status = consulta.Status,
                    Area = consulta.Area,
                    Especialidade = consulta.Especialidade,
                    Anamnese = consulta.Anamnese,
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = consulta.Paciente.nome,
                    NomeProfessor = consulta.Professor.nome,
                }).ToList();

                response.Data = consultaDTOs;
                response.Message = "Consultas encontradas com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

   
        public async Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaComFiltro(FiltroConsultaDto filtro)
        {
            var response = new ResponseModel<List<ReadConsultaDto>>();

            try
            {
                var query = _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(filtro.Status))
                    query = query.Where(c => c.Status == filtro.Status);

                if (!string.IsNullOrEmpty(filtro.Area))
                    query = query.Where(c => c.Area == filtro.Area);

                if (!string.IsNullOrEmpty(filtro.Especialidade))
                    query = query.Where(c => c.Especialidade == filtro.Especialidade);

                if (filtro.Data.HasValue)
                    query = query.Where(c => c.Data == filtro.Data.Value);

                var consultas = await query.ToListAsync();

                if (!consultas.Any())
                {
                    response.Message = "Nenhuma consulta encontrada com os filtros fornecidos.";
                    return response;
                }

                response.Data = consultas.Select(consulta => new ReadConsultaDto
                {
                    id_Consulta = consulta.id_Consulta,
                    Data = consulta.Data,
                    Horario = consulta.Horario,
                    Status = consulta.Status,
                    Area = consulta.Area,
                    Especialidade = consulta.Especialidade,
                    Anamnese = consulta.Anamnese,
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = consulta.Paciente.nome,
                    NomeProfessor = consulta.Professor.nome,
                }).ToList();

                response.Status = true;
                response.Message = "Consultas encontradas com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }



        public async Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultas()
        {
            ResponseModel<List<ReadConsultaDto>> response = new();

            try
            {
                var consultas = _context.Consultas
                    .Include(x => x.Paciente)
                    .Include(x => x.Professor)
                    .Select(x => new ReadConsultaDto
                    {
                        id_Consulta = x.id_Consulta,
                        Data = x.Data,
                        Horario = x.Horario,
                        Status = x.Status,
                        Area = x.Area,
                        Especialidade = x.Especialidade,
                        Anamnese = x.Anamnese,
                        PacienteId = x.PacienteId,
                        ProfessorId = x.ProfessorId,
                        NomePaciente = x.Paciente.nome,
                        NomeProfessor = x.Professor.nome,

                    }).ToListAsync();

                response.Data = await consultas;
                response.Message = "Consultas listadas!";
            }

            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorProfessor(int professorID)
        {
            ResponseModel<List<ReadConsultaDto>> response = new();

            try
            {
                var consultas = await _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .Where(c => c.ProfessorId == professorID)
                    .ToListAsync();

                if (consultas == null || !consultas.Any())
                {
                    response.Message = "Nenhuma consulta com esse professor.";
                    return response;
                }

                var consultaDTOs = consultas.Select(consulta => new ReadConsultaDto
                {
                    id_Consulta = consulta.id_Consulta,
                    Data = consulta.Data,
                    Horario = consulta.Horario,
                    Status = consulta.Status,
                    Area = consulta.Area,
                    Especialidade = consulta.Especialidade,
                    Anamnese = consulta.Anamnese,
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = consulta.Paciente.nome,
                    NomeProfessor = consulta.Professor.nome,
                }).ToList();

                response.Data = consultaDTOs;
                response.Message = "Consultas encontradas com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }



        public async Task<ResponseModel<List<ReadConsultaDto>>> ListarHistoricoPaciente(int pacienteId)
        {
            var response = new ResponseModel<List<ReadConsultaDto>>();

            try
            {
                var consultas = await _context.Consultas
                    .Include(c => c.Professor)
                    .Where(c => c.PacienteId == pacienteId &&
                                (c.Status == "Concluída" || c.Status == "Cancelada"))
                    .OrderByDescending(c => c.Data)
                    .ToListAsync();

                if (!consultas.Any())
                {
                    response.Message = "Nenhuma consulta encontrada no histórico.";
                    return response;  // Retorna se não houver consultas
                }

                var consultaDTOs = consultas.Select(c => new ReadConsultaDto
                {
                    id_Consulta = c.id_Consulta,
                    Data = c.Data,
                    Horario = c.Horario,
                    Status = c.Status,
                    Area = c.Area,
                    Especialidade = c.Especialidade,
                    Anamnese = c.Anamnese,
                    NomeProfessor = c.Professor?.nome,  // Se houver professor, coloca o nome
                    NomePaciente= c.Paciente?.nome  // Se houver professor, coloca o nome

                }).ToList();

                response.Data = consultaDTOs;
                response.Message = "Histórico carregado com sucesso.";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"Erro ao carregar o histórico: {ex.Message}";
            }

            return response;  // Retorna a resposta com sucesso ou falha
        }
    }
}