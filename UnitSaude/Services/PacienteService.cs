using Microsoft.EntityFrameworkCore;
using UnitSaude.Data;
using UnitSaude.Dto.Paciente;
using UnitSaude.Interfaces;
using UnitSaude.Models;
using UnitSaude.Utils;

namespace UnitSaude.Services
{
    public class PacienteService : PacienteInterface
    {
        private readonly ClinicaDbContext _context;
        public PacienteService(ClinicaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<ReadPacienteDto>> CadastrarPaciente(CreatePacienteDto pacienteDTO)
        {
            ResponseModel<ReadPacienteDto> response = new();

            try
            {
                var paciente = new Paciente
                {
                    cpf = pacienteDTO.cpf,
                    nome = pacienteDTO.nome,
                    email = pacienteDTO.email,
                    senhaHash = PasswordHasher.HashPassword(pacienteDTO.senhaHash),
                    telefone = pacienteDTO.telefone,
                    dataCadastro = DateOnly.FromDateTime(DateTime.UtcNow),
                    dataNascimento = pacienteDTO.dataNascimento,
                    TipoUsuario = "Paciente",
                    ativo = true
                };

                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                var readPacienteDto = new ReadPacienteDto
                {
                    id = paciente.Id_Usuario,
                    cpf = paciente.cpf,
                    nome = paciente.nome,
                    email = paciente.email,
                    telefone = paciente.telefone,
                    dataNascimento = paciente.dataNascimento
                };

                response.Data = readPacienteDto;
                response.Message = "Paciente cadastrado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<List<ReadPacienteDto>>> ListarPacientes()
        {
            ResponseModel<List<ReadPacienteDto>> response = new();

            try
            {
                var pacientes = _context.Pacientes
                    .Include(x => x.Consultas)
                    .Select(x => new ReadPacienteDto
                    {
                        id = x.Id_Usuario,
                        cpf = x.cpf,
                        nome = x.nome,
                        email = x.email,
                        telefone = x.telefone,
                        dataNascimento = x.dataNascimento

                    }).ToListAsync();

                response.Data = await pacientes;
                response.Message = "Pacientes listados!";
            }

            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel<ReadPacienteDto>> ListarPaciente (int pacienteId)
        {
            ResponseModel<ReadPacienteDto> response = new();

            try
            {
                var paciente = await _context.Pacientes.FindAsync(pacienteId);


                if (paciente == null)
                {
                    response.Message = "Paciente n�o encontrado.";
                    return response;
                }

                var pacienteDTO = new ReadPacienteDto
                {
                    id = paciente.Id_Usuario,
                    cpf = paciente.cpf,
                    nome = paciente.nome,
                    email = paciente.email,
                    telefone = paciente.telefone,
                    dataNascimento = paciente.dataNascimento
                };

                response.Data = pacienteDTO;
                response.Message = "Paciente encontrado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;

            }

            return response;
        }

        public async Task<ResponseModel<ReadPacienteDto>> GerenciarPaciente(UpdatePacienteDto pacienteDto, int pacienteId)
        {
            ResponseModel<ReadPacienteDto> response = new();

            try
            {
                var pacienteExistente = await _context.Pacientes.FindAsync(pacienteId);

                if (pacienteExistente == null)
                {
                    response.Status = false;
                    response.Message = "Paciente n�o encontrado.";
                    return response;
                }

                foreach (var property in pacienteDto.GetType().GetProperties())
                {
                    var newValue = property.GetValue(pacienteDto);
                    if (newValue != null)
                    {
                        var userProperty = pacienteExistente.GetType().GetProperty(property.Name);
                        if (userProperty != null && userProperty.CanWrite)
                        {
                            userProperty.SetValue(pacienteExistente, newValue);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                var ReadPacienteDto = new ReadPacienteDto
                {
                    id = pacienteExistente.Id_Usuario,
                    cpf = pacienteExistente.cpf,
                    nome = pacienteExistente.nome,
                    email = pacienteExistente.email,
                    telefone = pacienteExistente.telefone,
                    dataNascimento = pacienteExistente.dataNascimento

                };

                response.Data = ReadPacienteDto;
                response.Message = "Paciente atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<string>> AlterarSenhaPaciente(int pacienteId, UpdateSenhaPacienteDto dto)
        {
            var response = new ResponseModel<string>();

            try
            {
                var paciente = await _context.Pacientes.FindAsync(pacienteId);

                if (paciente == null)
                {
                    response.Status = false;
                    response.Message = "Paciente n�o encontrado.";
                    return response;
                }

                // Verifica a senha atual
                if (!PasswordHasher.VerifyPassword(dto.SenhaAtual, paciente.senhaHash))
                {
                    response.Status = false;
                    response.Message = "Senha atual incorreta.";
                    return response;
                }

                // Aplica nova hash
                paciente.senhaHash = PasswordHasher.HashPassword(dto.NovaSenha);

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

        public async Task<ResponseModel<Paciente>> RemoverPaciente(int pacienteId)
        {
            ResponseModel<Paciente> response = new();

            try
            {
                var paciente = await _context.Pacientes.FindAsync(pacienteId);

                if (paciente == null)
                {
                    response.Status = false;
                    response.Message = "Paciente n�o encontrado.";
                    return response;
                }

                _context.Pacientes.Remove(paciente);

                await _context.SaveChangesAsync();

                response.Data = paciente;
                response.Message = "Paciente removido com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<bool> ResetarSenhaPaciente(int id, string novaSenhaHash)
        {
            var admin = await _context.Pacientes.FindAsync(id);
            if (admin == null) return false;

            admin.senhaHash = novaSenhaHash;
            await _context.SaveChangesAsync();
            return true;
        }


    }
}