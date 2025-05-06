using Microsoft.EntityFrameworkCore;
using UnitSaude.Data;
using UnitSaude.Dto.Consulta;
using UnitSaude.Dto.Paciente;
using UnitSaude.Dtos.Consulta;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class ConsultaService : ConsultaInterface
    {
        private readonly ClinicaDbContext _context;
        private readonly EmailService _emailService;  // Incluir o serviço de e-mail

        public ConsultaService(ClinicaDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;  // Inicializando o serviço de e-mail
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

                // Verifica se já existe alguma consulta marcada para o mesmo horário
                var conflito = await _context.Consultas
                    .AnyAsync(c =>
                        c.Area == consultaDTO.Area &&
                        c.Especialidade == consultaDTO.Especialidade &&
                        c.Status != "Concluída" &&
                        c.Status != "Cancelada" &&
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
                    status = "Agendada";
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
                    PacienteId = consultaDTO.PacienteId,
                    ProfessorId = consultaDTO.ProfessorId,
                    Professor = professor,
                    Paciente = paciente,
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
                    PacienteId = consulta.PacienteId,
                    ProfessorId = consulta.ProfessorId,
                    NomePaciente = paciente.nome,
                    NomeProfessor = professor.nome
                };

                response.Status = true;
                response.Data = readConsultaDto;
                response.Message = status == "Agendada"
                    ? "Consulta agendada com sucesso!"
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

        /*  public async Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorStatus(
              string status,
              string? area = null,
              string? especialidade = null,
              DateOnly? data = null)
          {
              var response = new ResponseModel<List<ReadConsultaDto>>();

              try
              {
                  var query = _context.Consultas
                      .Include(c => c.Paciente)
                      .Include(c => c.Professor)
                      .Where(c => c.Status == status)
                      .AsQueryable();

                  if (!string.IsNullOrEmpty(area))
                      query = query.Where(c => c.Area == area);

                  if (!string.IsNullOrEmpty(especialidade))
                      query = query.Where(c => c.Especialidade == especialidade);

                  if (data.HasValue)
                      query = query.Where(c => c.Data == data.Value);

                  var consultas = await query.ToListAsync();

                  if (consultas == null || !consultas.Any())
                  {
                      response.Message = "Nenhuma consulta encontrada com esses filtros.";
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
          } */

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
                var consulta = await _context.Consultas.FindAsync(id);

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

                // NOVA VALIDAÇÃO: Verifica se a data é domingo
                if (dto.NovaData.DayOfWeek == DayOfWeek.Sunday)
                {
                    response.Status = false;
                    response.Message = "Não é possível reagendar consultas para domingos.";
                    return response;
                }

                var conflito = await _context.Consultas.AnyAsync(c =>
                    c.id_Consulta != consulta.id_Consulta &&
                    c.Data == dto.NovaData &&
                    c.Horario == dto.NovoHorario &&
                    c.Area == consulta.Area &&
                    c.Especialidade == consulta.Especialidade &&
                    c.Status != "Cancelada" && c.Status != "Concluída");

                if (conflito)
                {
                    response.Status = false;
                    response.Message = "Já existe uma consulta nesse horário.";
                    return response;
                }

                consulta.Data = dto.NovaData;
                consulta.Horario = dto.NovoHorario;

                await _context.SaveChangesAsync();

                response.Message = "Consulta reagendada com sucesso!";
                response.Data = $"{consulta.Data} às {consulta.Horario}";
                response.Status = true;

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