using UnitSaude.Dto.Disponibilidade;
using UnitSaude.Interfaces;
using UnitSaude.Models;
using UnitSaude.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace UnitSaude.Services
{
    public class DisponibilidadeService : DisponibilidadeInterface
    {
        private readonly ClinicaDbContext _context;
        public DisponibilidadeService(ClinicaDbContext context)
        {
            _context = context;
        }


        public async Task<ResponseModel<ReadDisponibilidadeDto>> CadastrarDisponibilidade(CreateDisponibilidadeDto dto)
        {
            ResponseModel<ReadDisponibilidadeDto> response = new();

            try
            {
                // Validação da área
                if (!DadosFixosConsulta.ObterAreas().Contains(dto.Area))
                {
                    response.Status = false;
                    response.Message = "Área inválida.";
                    return response;
                }

                // Validação da especialidade
                var especialidadesValidas = DadosFixosConsulta.ObterEspecialidadesPorArea(dto.Area);
                if (!especialidadesValidas.Contains(dto.Especialidade))
                {
                    response.Status = false;
                    response.Message = "Especialidade inválida para a área selecionada.";
                    return response;
                }

                // VALIDAÇÃO DE DOMINGOS - DataInicio
                if (dto.DataInicio.DayOfWeek == DayOfWeek.Sunday)
                {
                    response.Status = false;
                    response.Message = "Não é possível cadastrar disponibilidade começando em domingo.";
                    return response;
                }

                // VALIDAÇÃO DE DOMINGOS - DataFim
                if (dto.DataFim.DayOfWeek == DayOfWeek.Sunday)
                {
                    response.Status = false;
                    response.Message = "Não é possível cadastrar disponibilidade terminando em domingo.";
                    return response;
                }

                // Validação adicional: DataFim não pode ser anterior à DataInicio
                if (dto.DataFim < dto.DataInicio)
                {
                    response.Status = false;
                    response.Message = "A data final não pode ser anterior à data inicial.";
                    return response;
                }

                var sobreposicao = await _context.Disponibilidades
           .Where(d => d.Area == dto.Area && d.Especialidade == dto.Especialidade && d.Ativo)
           .AnyAsync(d =>
               // Verifica se o novo período sobrepõe algum existente
               (dto.DataInicio >= d.DataInicio && dto.DataInicio <= d.DataFim) || // Novo início dentro de um período existente
               (dto.DataFim >= d.DataInicio && dto.DataFim <= d.DataFim) ||      // Novo fim dentro de um período existente
               (dto.DataInicio <= d.DataInicio && dto.DataFim >= d.DataFim)      // Novo período engloba um existente
           );

                if (sobreposicao)
                {
                    response.Status = false;
                    response.Message = "Já existe uma disponibilidade cadastrada para este período.";
                    return response;
                }


                var disponibilidade = new Disponibilidade
                {
                    DataInicio = dto.DataInicio,
                    DataFim = dto.DataFim,
                    HorarioInicio = dto.HorarioInicio,
                    HorarioFim = dto.HorarioFim,
                    Area = dto.Area,
                    Especialidade = dto.Especialidade,
                    Ativo = true
                };

                _context.Disponibilidades.Add(disponibilidade);
                await _context.SaveChangesAsync();

                var readDto = new ReadDisponibilidadeDto
                {
                    Id = disponibilidade.Id,
                    DataInicio = disponibilidade.DataInicio,
                    DataFim = disponibilidade.DataFim,
                    HorarioInicio = disponibilidade.HorarioInicio,
                    HorarioFim = disponibilidade.HorarioFim,
                    Area = disponibilidade.Area,
                    Especialidade = disponibilidade.Especialidade,
                };

                response.Data = readDto;
                response.Message = "Disponibilidade cadastrada com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<List<ReadDisponibilidadeDto>>> ListarDisponibilidades()
        {
            ResponseModel<List<ReadDisponibilidadeDto>> response = new();

            try
            {
                var disponibilidades = await _context.Disponibilidades
                    .Select(d => new ReadDisponibilidadeDto
                    {
                        Id = d.Id,
                        DataInicio = d.DataInicio,
                        DataFim = d.DataFim,
                        HorarioInicio = d.HorarioInicio,
                        HorarioFim = d.HorarioFim,
                        Area = d.Area,
                        Especialidade = d.Especialidade,
                    }).ToListAsync();

                response.Data = disponibilidades;
                response.Message = "Disponibilidades listadas!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel<Disponibilidade>> RemoverDisponibilidade(int disponibilidadeId)
        {
            ResponseModel<Disponibilidade> response = new();

            try
            {
                var disponibilidade = await _context.Disponibilidades.FindAsync(disponibilidadeId);

                if (disponibilidade == null)
                {
                    response.Status = false;
                    response.Message = "Disponibilidade.";
                    return response;
                }

                _context.Disponibilidades.Remove(disponibilidade);

                await _context.SaveChangesAsync();

                response.Data = disponibilidade;
                response.Message = "Disponibilidade removida com sucesso!";
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
