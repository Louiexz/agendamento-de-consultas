
using Microsoft.EntityFrameworkCore;
using UnitSaude.Data;
using UnitSaude.Dto.Paciente;
using UnitSaude.Dto.Professor;
using UnitSaude.Interfaces;
using UnitSaude.Models;
using UnitSaude.Utils;

namespace UnitSaude.Services
{
    public class ProfessorService : ProfessorInterface
    {
        private readonly ClinicaDbContext _context;
        public ProfessorService(ClinicaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<ReadProfessorDto>> CadastrarProfessor(CreateProfessorDto professorDto)
        {
            ResponseModel<ReadProfessorDto> response = new();

            try
            {
                var cpfExiste = await _context.Pacientes.AnyAsync(p => p.cpf == professorDto.cpf);
                if (cpfExiste)
                {
                    response.Status = false;
                    response.Message = "Já existe um paciente cadastrado com esse CPF.";
                    return response;
                }

                // Valida��o da �rea
                if (!DadosFixosConsulta.ObterAreas().Contains(professorDto.area))
                {
                    response.Status = false;
                    response.Message = "�rea inv�lida.";
                    return response;
                }

                // Validação das especialidades
                var especialidadesPermitidas = DadosFixosConsulta.ObterEspecialidadesPorArea(professorDto.area);
                foreach (var especialidade in professorDto.especialidades)
                {
                    if (!especialidadesPermitidas.Contains(especialidade))
                    {
                        response.Status = false;
                        response.Message = $"Especialidade '{especialidade}' inválida para a área especificada.";
                        return response;
                    }
                }

                var professor = new Professor
                {
                    cpf = professorDto.cpf.Trim(),
                    nome = professorDto.nome,
                    email = professorDto.email,
                    senhaHash = PasswordHasher.HashPassword(professorDto.senhaHash),
                    telefone = professorDto.telefone,
                    dataCadastro = DateOnly.FromDateTime(DateTime.UtcNow),
                    dataNascimento = professorDto.dataNascimento,
                    area = professorDto.area,
                    especialidades = professorDto.especialidades, // Agora é uma lista
                    codigoProfissional = professorDto.codigoProfissional,
                    TipoUsuario = "Professor",
                    ativo = true

                };

                _context.Professores.Add(professor);
                await _context.SaveChangesAsync();

                var ReadProfessorDto = new ReadProfessorDto
                {
                    id = professor.Id_Usuario,
                    cpf = professor.cpf,
                    nome = professor.nome,
                    email = professor.email,
                    telefone = professor.telefone,
                    dataNascimento = professor.dataNascimento,
                    area = professor.area,
                    especialidades = professorDto.especialidades, // Agora é uma lista
                    codigoProfissional = professor.codigoProfissional,

                };
                response.Data = ReadProfessorDto;
                response.Message = "Professor cadastrado com sucesso!";

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"Erro ao salvar: {ex.InnerException?.Message ?? ex.Message}";

            }

            return response;
        }

        public async Task<ResponseModel<List<ReadProfessorDto>>> ListarProfessores()
        {
            ResponseModel<List<ReadProfessorDto>> response = new();

            try
            {
                var professores = _context.Professores
                    .Include(x => x.Consultas)
                    .Select(x => new ReadProfessorDto
                    {
                        id = x.Id_Usuario,
                        cpf = x.cpf,
                        nome = x.nome,
                        email = x.email,
                        telefone = x.telefone,
                        dataNascimento = x.dataNascimento,
                        area = x.area,
                        especialidades = x.especialidades, // Alterado para a lista
                        codigoProfissional = x.codigoProfissional,

                    }).ToListAsync();

                response.Data = await professores;
                response.Message = "Professores listados!";
            }

            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel<ReadProfessorDto>> ListarProfessor(int professorId)
        {
            ResponseModel<ReadProfessorDto> response = new();

            try
            {
                var professor = await _context.Professores.FindAsync(professorId);


                if (professor == null)
                {
                    response.Message = "professor n�o encontrado.";
                    return response;
                }


                var professorDTO = new ReadProfessorDto
                {
                    id = professor.Id_Usuario,
                    cpf = professor.cpf,
                    nome = professor.nome,
                    email = professor.email,
                    telefone = professor.telefone,
                    dataNascimento = professor.dataNascimento,
                    area = professor.area,
                    especialidades = professor.especialidades, // Alterado para a lista
                    codigoProfissional = professor.codigoProfissional,

                };

                response.Data = professorDTO;
                response.Message = "Professor encontrado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;

            }

            return response;
        }

        public async Task<ResponseModel<ReadProfessorDto>> GerenciarProfessor(UpdateProfessorDto professorDto, int professorId)
        {
            ResponseModel<ReadProfessorDto> response = new();

            try
            {
                var professorExistente = await _context.Professores.FindAsync(professorId);

                if (professorExistente == null)
                {
                    response.Status = false;
                    response.Message = "Paciente n�o encontrado.";
                    return response;
                }

                // Valida��o da �rea
                if (professorDto.area != null && !DadosFixosConsulta.ObterAreas().Contains(professorDto.area))
                {
                    response.Status = false;
                    response.Message = "�rea inv�lida.";
                    return response;
                }
                if (professorDto.area != null)
                {
                    var especialidadesPermitidas = DadosFixosConsulta.ObterEspecialidadesPorArea(professorDto.area);
                    if (professorDto.especialidades != null)
                    {
                        foreach (var especialidade in professorDto.especialidades)
                        {
                            if (!especialidadesPermitidas.Contains(especialidade))
                            {
                                response.Status = false;
                                response.Message = $"Especialidade '{especialidade}' inválida para a área especificada.";
                                return response;
                            }
                        }
                    }
                }



                foreach (var property in professorDto.GetType().GetProperties())
                {
                    var newValue = property.GetValue(professorDto);
                    if (newValue != null)
                    {
                        var userProperty = professorExistente.GetType().GetProperty(property.Name);
                        if (userProperty != null && userProperty.CanWrite)
                        {
                            userProperty.SetValue(professorExistente, newValue);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                var ReadProfessorDto = new ReadProfessorDto
                {
                    id = professorExistente.Id_Usuario,
                    cpf = professorExistente.cpf,
                    nome = professorExistente.nome,
                    email = professorExistente.email,
                    telefone = professorExistente.telefone,
                    codigoProfissional = professorExistente.codigoProfissional,
                    dataNascimento = professorExistente.dataNascimento,
                    especialidades = professorExistente.especialidades, // Alterado para lista
                    area = professorExistente.area,
                };

                response.Data = ReadProfessorDto;
                response.Message = "Professor atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<string>> AlterarSenhaProfessor(int professorId, UpdateSenhaProfessorDto dto)
        {
            var response = new ResponseModel<string>();

            try
            {
                var professor = await _context.Professores.FindAsync(professorId);

                if (professor == null)
                {
                    response.Status = false;
                    response.Message = "Professor n�o encontrado.";
                    return response;
                }

                // Verifica a senha atual
                if (!PasswordHasher.VerifyPassword(dto.SenhaAtual, professor.senhaHash))
                {
                    response.Status = false;
                    response.Message = "Senha atual incorreta.";
                    return response;
                }

                // Aplica nova hash
                professor.senhaHash = PasswordHasher.HashPassword(dto.NovaSenha);

                await _context.SaveChangesAsync();

                response.Data = "Senha atualizada com sucesso!";
                response.Message = "Senha alterada com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<Professor>> RemoverProfessor(int professorId)
        {
            ResponseModel<Professor> response = new();

            try
            {
                var professor = await _context.Professores.FindAsync(professorId);

                if (professor == null)
                {
                    response.Status = false;
                    response.Message = "Professor n�o encontrado.";
                    return response;
                }

                _context.Professores.Remove(professor);

                await _context.SaveChangesAsync();

                response.Data = professor;
                response.Message = "professor removido com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<bool> ResetarSenhaProfessor(int id, string novaSenhaHash)
        {
            var admin = await _context.Professores.FindAsync(id);
            if (admin == null) return false;

            admin.senhaHash = novaSenhaHash;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ResponseModel<List<ReadProfessorDto>>> ListarProfessoresPorEspecialidade(string especialidade)
        {
            var response = new ResponseModel<List<ReadProfessorDto>>();

            try
            {
                // Primeiro carrega os professores com suas especialidades
                var professores = await _context.Professores
                    .Include(p => p.Consultas)
                    .ToListAsync();

                // Filtra na memória (client-side)
                var professoresFiltrados = professores
                    .Where(p => p.especialidades != null &&
                                p.especialidades.Any(e =>
                                    e.Equals(especialidade, StringComparison.OrdinalIgnoreCase)))
                    .Select(p => new ReadProfessorDto
                    {
                        id = p.Id_Usuario,
                        cpf = p.cpf,
                        nome = p.nome,
                        email = p.email,
                        telefone = p.telefone,
                        dataNascimento = p.dataNascimento,
                        area = p.area,
                        especialidades = p.especialidades,
                        codigoProfissional = p.codigoProfissional
                    })
                    .ToList();

                response.Data = professoresFiltrados;
                response.Message = professoresFiltrados.Any()
                    ? "Professores encontrados com sucesso"
                    : "Nenhum professor encontrado para esta especialidade";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"Erro ao buscar professores: {ex.Message}";
                // Log adicional para debug
                Console.WriteLine($"ERRO DETALHADO: {ex}");
            }

            return response;
        }


        public async Task<ResponseModel<List<ReadProfessorDto>>> ListarProfessoresComFiltro(FiltroProfessorDto filtro)
        {
            var response = new ResponseModel<List<ReadProfessorDto>>();

            try
            {
                var query = _context.Professores.AsQueryable();

                if (!string.IsNullOrEmpty(filtro.Nome) || !string.IsNullOrEmpty(filtro.CodigoProfissional) || !string.IsNullOrEmpty(filtro.Especialidade))
                {
                    query = query.Where(p =>
                        (!string.IsNullOrEmpty(filtro.Nome) && p.nome.Contains(filtro.Nome)) ||
                        (!string.IsNullOrEmpty(filtro.CodigoProfissional) && p.codigoProfissional == filtro.CodigoProfissional) ||
                        (!string.IsNullOrEmpty(filtro.Especialidade) && p.especialidades.Contains(filtro.Especialidade))
                    );
                }


                var professores = await query.ToListAsync();

                if (!professores.Any())
                {
                    response.Message = "Nenhum professor encontrado com os filtros fornecidos.";
                    return response;
                }

                response.Data = professores.Select(professor => new ReadProfessorDto
                {
                    id = professor.Id_Usuario,
                    cpf = professor.cpf,
                    nome = professor.nome,
                    email = professor.email,
                    telefone = professor.telefone,
                    dataNascimento = professor.dataNascimento,
                    area = professor.area,
                    especialidades = professor.especialidades,
                    codigoProfissional = professor.codigoProfissional,
                }).ToList();

                response.Status = true;
                response.Message = "Professores encontrados com sucesso!";
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