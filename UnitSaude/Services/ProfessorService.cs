
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
                var professor = new Professor
                {
                    cpf = professorDto.cpf,
                    nome = professorDto.nome,
                    email = professorDto.email,
                    senhaHash = PasswordHasher.HashPassword(professorDto.senhaHash),
                    telefone = professorDto.telefone,
                    dataCadastro = DateOnly.FromDateTime(DateTime.UtcNow),
                    dataNascimento = professorDto.dataNascimento,
                    area = professorDto.area,
                    especialidade = professorDto.especialidade,
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
                    especialidade = professor.especialidade,
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
                        especialidade = x.especialidade,
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
                    response.Message = "professor não encontrado.";
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
                    especialidade = professor.especialidade,
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
                    response.Message = "Paciente não encontrado.";
                    return response;
                }

                professorExistente.cpf = professorDto.cpf;
                professorExistente.nome = professorDto.nome;
                professorExistente.email = professorDto.email;
                professorExistente.telefone = professorDto.telefone;
                professorExistente.dataNascimento = professorDto.dataNascimento;
                professorExistente.area = professorDto.area;
                professorExistente.especialidade = professorDto.especialidade;
                professorExistente.codigoProfissional = professorDto.codigoProfissional;

                await _context.SaveChangesAsync();

                var ReadProfessorDto = new ReadProfessorDto
                {
                    id = professorExistente.Id_Usuario,
                    cpf = professorExistente.cpf,
                    nome = professorExistente.nome,
                    email = professorExistente.email,
                    telefone = professorExistente.telefone,
                    dataNascimento = professorExistente.dataNascimento,
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
                    response.Message = "Professor não encontrado.";
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
                    response.Message = "Professor não encontrado.";
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
    }
}