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


    }
}
