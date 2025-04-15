using Microsoft.EntityFrameworkCore;
using UnitSaude.Data;
using UnitSaude.Dto.Consulta;
using UnitSaude.Dto.Paciente;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class ConsultaService : ConsultaInterface
    {
        private readonly ClinicaDbContext _context;
        public ConsultaService(ClinicaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<ReadConsultaDto>> CadastrarConsulta(CreateConsultaDto consultaDTO)
        {
            ResponseModel<ReadConsultaDto> response = new();

            try
            {
                // Verifica se o paciente existe
                var paciente = await _context.Pacientes
                    .Where(p => p.Id_Usuario == consultaDTO.PacienteId)
                    .Select(p => new { p.Id_Usuario, p.nome })
                    .FirstOrDefaultAsync();

                if (paciente == null)
                {
                    response.Status = false;
                    response.Message = "Paciente não encontrado.";
                    return response;
                }

                // Verifica se o professor existe
                var professor = await _context.Professores
                    .Where(p => p.Id_Usuario == consultaDTO.ProfessorId)
                    .Select(p => new { p.Id_Usuario, p.nome })
                    .FirstOrDefaultAsync();

                if (professor == null)
                {
                    response.Status = false;
                    response.Message = "Professor não encontrado.";
                    return response;
                }

                // Validação de Área
                if (!DadosFixosConsulta.EspecialidadesPorArea.ContainsKey(consultaDTO.Area))
                {
                    response.Status = false;
                    response.Message = "Área inválida.";
                    return response;
                }

                // Validação de Especialidade para a Área
                var especialidadesPermitidas = DadosFixosConsulta.EspecialidadesPorArea[consultaDTO.Area];
                if (!especialidadesPermitidas.Contains(consultaDTO.Especialidade))
                {
                    response.Status = false;
                    response.Message = "Especialidade inválida para a área selecionada.";
                    return response;
                }

                // Validação de Status
                if (!DadosFixosConsulta.Status.Contains(consultaDTO.Status))
                {
                    response.Status = false;
                    response.Message = "Status inválido.";
                    return response;
                }

                // Verifica se o horário está dentro dos horários válidos (multiplo de 40 minutos + 15 minutos de intervalo)
                TimeOnly horarioInicial = new(8, 0); // Início das consultas
                TimeSpan duracaoComIntervalo = TimeSpan.FromMinutes(55); // 40 minutos + 15 minutos

                bool horarioValido = false;

                // Verifica se o horário solicitado é um múltiplo de 55 minutos após o horário inicial
                while (horarioInicial.Add(duracaoComIntervalo) <= new TimeOnly(21, 0)) // Fim das consultas
                {
                    if (horarioInicial == consultaDTO.Horario)
                    {
                        horarioValido = true;
                        break;
                    }
                    horarioInicial = horarioInicial.Add(duracaoComIntervalo);
                }

                if (!horarioValido)
                {
                    response.Status = false;
                    response.Message = "Horário inválido. Escolha um horário disponível.";
                    return response;
                }

                // Verifica se já existe alguma consulta marcada para o mesmo horário
                var conflito = await _context.Consultas
                    .AnyAsync(c =>
                        c.Area == consultaDTO.Area && // Verifica a área
                        c.Especialidade == consultaDTO.Especialidade && // Verifica a especialidade
                        c.Status != "Concluída" && // Verifica se o status não é Concluída
                        c.Status != "Cancelada" && // Verifica se o status não é Cancelada
                        c.Data == consultaDTO.Data && // Verifica se a data é a mesma
                        c.Horario == consultaDTO.Horario); // Verifica se o horário é o mesmo

                if (conflito)
                {
                    response.Status = false;
                    response.Message = "Este horário já está reservado.";
                    return response;
                }

                // Criação da consulta
                var consulta = new Consulta
                {
                    Data = consultaDTO.Data,
                    Horario = consultaDTO.Horario,
                    Status = consultaDTO.Status,
                    Area = consultaDTO.Area,
                    Especialidade = consultaDTO.Especialidade,
                    PacienteId = consultaDTO.PacienteId,
                    ProfessorId = consultaDTO.ProfessorId
                };

                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();

                // Prepara DTO de retorno
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
                response.Message = "Consulta cadastrada com sucesso!";
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
                    response.Message = "Consulta não encontrado.";
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
                    response.Message = "Paciente não encontrado.";
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

        public async Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorStatus(string status)
        {
            ResponseModel<List<ReadConsultaDto>> response = new();

            try
            {
                var consultas = await _context.Consultas
                    .Include(c => c.Paciente)
                    .Include(c => c.Professor)
                    .Where(c => c.Status == status)
                    .ToListAsync();

                if (consultas == null || !consultas.Any())
                {
                    response.Message = "Nenhuma consulta com esse status.";
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

        public async Task<ResponseModel<List<string>>> ObterHorariosDisponiveis(DateOnly data, string area, string especialidade)
        {
            TimeOnly inicio = new(8, 0);  // Início das consultas
            TimeOnly fim = new(21, 0);    // Fim das consultas
            TimeSpan duracao = TimeSpan.FromMinutes(40);  // Duração de cada consulta
            TimeSpan intervalo = TimeSpan.FromMinutes(15); // Intervalo entre as consultas

            // Obter os horários ocupados considerando o status diferente de "Cancelada" ou "Concluída"
            var horariosOcupados = await _context.Consultas
                .Where(c => c.Data == data &&
                            c.Area == area &&
                            c.Especialidade == especialidade &&
                            (c.Status != "Cancelada" && c.Status != "Concluída")) // Garantir que o status não seja "Cancelada" ou "Concluída"
                .Select(c => c.Horario)
                .ToListAsync();

            List<string> horariosDisponiveis = new();
            TimeOnly horarioAtual = inicio;

            // Loop para verificar todos os horários possíveis dentro do intervalo permitido
            while (horarioAtual.Add(duracao) <= fim)
            {
                // Verifica se o horário atual não está na lista de horários ocupados
                if (!horariosOcupados.Contains(horarioAtual))
                {
                    horariosDisponiveis.Add(horarioAtual.ToString("HH:mm"));
                }

                // Incrementa o horário pelo tempo de duração mais o intervalo (55 minutos no total)
                horarioAtual = horarioAtual.Add(duracao + intervalo);
            }

            // Retorna os horários disponíveis encontrados
            return new ResponseModel<List<string>>
            {
                Status = true,
                Message = "Horários disponíveis encontrados.",
                Data = horariosDisponiveis
            };
        }



        public async Task<ResponseModel<string>> AtualizarStatusConsulta(int id, UpdateStatusConsultaDto dto)
        {
            var response = new ResponseModel<string>();

            try
            {
                if (!DadosFixosConsulta.Status.Contains(dto.NovoStatus))
                {
                    response.Status = false;
                    response.Message = "Status inválido.";
                    return response;
                }

                var consulta = await _context.Consultas.FindAsync(id);

                if (consulta == null)
                {
                    response.Status = false;
                    response.Message = "Consulta não encontrada.";
                    return response;
                }

                consulta.Status = dto.NovoStatus;
                await _context.SaveChangesAsync();

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


    }
}