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

                var conflito = await _context.Consultas
                    .AnyAsync(c =>
                        c.Data == consultaDTO.Data && // Comparando diretamente o DateOnly
                        c.Horario == consultaDTO.Horario && // Comparando o TimeOnly
                        c.Status != "Cancelada");


                if (conflito)
                {
                    response.Status = false;
                    response.Message = "Este horário já está ocupado.";
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
                        p.nome.ToLower().Contains(valor.ToLower()) || p.Id_Usuario.ToString() == valor);

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
            TimeOnly inicio = new(8, 0);
            TimeOnly fim = new(21, 0);
            TimeSpan duracao = TimeSpan.FromMinutes(40);
            TimeSpan intervalo = TimeSpan.FromMinutes(15);

            var horariosOcupados = await _context.Consultas
                .Where(c => c.Data == data &&
                            c.Area == area &&
                            c.Especialidade == especialidade &&
                            c.Status != "Cancelada")
                .Select(c => c.Horario)
                .ToListAsync();

            List<string> horariosDisponiveis = new();
            TimeOnly horarioAtual = inicio;

            while (horarioAtual.Add(duracao) <= fim)
            {
                if (!horariosOcupados.Contains(horarioAtual))
                {
                    horariosDisponiveis.Add(horarioAtual.ToString("HH:mm"));
                }

                horarioAtual = horarioAtual.Add(duracao + intervalo);
            }

            return new ResponseModel<List<string>>
            {
                Status = true,
                Message = "Horários disponíveis encontrados.",
                Data = horariosDisponiveis
            };
        }

    }
}