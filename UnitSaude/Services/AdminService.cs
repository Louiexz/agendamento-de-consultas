using Microsoft.EntityFrameworkCore;
using UnitSaude.Data;
using UnitSaude.Dto.Admin;
using UnitSaude.Dto.Paciente;
using UnitSaude.Interfaces;
using UnitSaude.Models;
using UnitSaude.Utils;

namespace UnitSaude.Services
{
    public class AdminService : AdminInterface
    {
        private readonly ClinicaDbContext _context;
        public AdminService(ClinicaDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<ReadAdminDto>> CadastrarAdmin(CreateAdminDto adminDTO)
        {
            ResponseModel<ReadAdminDto> response = new();

            try
            {
                var admin = new Administrador
                {
                    cpf = adminDTO.cpf,
                    nome = adminDTO.nome,
                    email = adminDTO.email,
                    senhaHash = PasswordHasher.HashPassword(adminDTO.senhaHash),
                    telefone = adminDTO.telefone,
                    dataCadastro = DateOnly.FromDateTime(DateTime.UtcNow),
                    dataNascimento = adminDTO.dataNascimento,
                    TipoUsuario = "Administrador",
                    ativo = true

                };

                _context.Administradores.Add(admin);
                await _context.SaveChangesAsync();

                var ReadAdminDto = new ReadAdminDto
                {
                    id = admin.Id_Usuario,
                    cpf = admin.cpf,
                    nome = admin.nome,
                    email = admin.email,
                    telefone = admin.telefone,
                    dataNascimento = admin.dataNascimento

                };
                response.Data = ReadAdminDto;
                response.Message = "Administrador cadastrado com sucesso!";

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;

            }

            return response;
        }

        public async Task<ResponseModel<List<ReadAdminDto>>> ListarAdmins()
        {
            ResponseModel<List<ReadAdminDto>> response = new();

            try
            {
                var admins = _context.Administradores
                    .Select(x => new ReadAdminDto
                    {
                        id = x.Id_Usuario,
                        cpf = x.cpf,
                        nome = x.nome,
                        email = x.email,
                        telefone = x.telefone,
                        dataNascimento = x.dataNascimento

                    }).ToListAsync();

                response.Data = await admins;
                response.Message = "Admins listados!";
            }

            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel<ReadAdminDto>> ListarAdmin(int adminId)
        {
            ResponseModel<ReadAdminDto> response = new();

            try
            {
                var admin = await _context.Administradores.FindAsync(adminId);


                if (admin == null)
                {
                    response.Message = "Administrador n�o encontrado.";
                    return response;
                }

                var AdminDTO = new ReadAdminDto
                {
                    id = admin.Id_Usuario,
                    cpf = admin.cpf,
                    nome = admin.nome,
                    email = admin.email,
                    telefone = admin.telefone,
                    dataNascimento = admin.dataNascimento
                };

                response.Data = AdminDTO;
                response.Message = "Administrador encontrado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;

            }

            return response;
        }

        public async Task<ResponseModel<ReadAdminDto>> GerenciarAdmin(UpdateAdminDto adminDTO, int adminId)
        {
            ResponseModel<ReadAdminDto> response = new();

            try
            {
                var adminExistente = await _context.Administradores.FindAsync(adminId);

                if (adminExistente == null)
                {
                    response.Status = false;
                    response.Message = "Admin n�o encontrado.";
                    return response;
                }

                foreach (var property in adminDTO.GetType().GetProperties())
                {
                    var newValue = property.GetValue(adminDTO);
                    if (newValue != null)
                    {
                        var userProperty = adminExistente.GetType().GetProperty(property.Name);
                        if (userProperty != null && userProperty.CanWrite)
                        {
                            userProperty.SetValue(adminExistente, newValue);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                var ReadAdminDto = new ReadAdminDto
                {
                    id = adminExistente.Id_Usuario,
                    cpf = adminExistente.cpf,
                    nome = adminExistente.nome,
                    email = adminExistente.email,
                    telefone = adminExistente.telefone,
                    dataNascimento = adminExistente.dataNascimento

                };

                response.Data = ReadAdminDto;
                response.Message = "Administrador atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseModel<string>> AlterarSenhaAdmin(int adminId, UpdateSenhaAdminDto dto)
        {
            var response = new ResponseModel<string>();

            try
            {
                var admin = await _context.Administradores.FindAsync(adminId);

                if (admin == null)
                {
                    response.Status = false;
                    response.Message = "Administrador n�o encontrado.";
                    return response;
                }

                // Verifica a senha atual
                if (!PasswordHasher.VerifyPassword(dto.SenhaAtual, admin.senhaHash))
                {
                    response.Status = false;
                    response.Message = "Senha atual incorreta.";
                    return response;
                }

                // Aplica nova hash
                admin.senhaHash = PasswordHasher.HashPassword(dto.NovaSenha);

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

        public async Task<ResponseModel<Administrador>> RemoverAdmin(int adminId)
        {
            ResponseModel<Administrador> response = new();

            try
            {
                var admin = await _context.Administradores.FindAsync(adminId);

                if (admin == null)
                {
                    response.Status = false;
                    response.Message = "Administrador n�o encontrado.";
                    return response;
                }

                _context.Administradores.Remove(admin);

                await _context.SaveChangesAsync();

                response.Data = admin;
                response.Message = "Administrador removido com sucesso!";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<bool> ResetarSenhaAdministrador(int id, string novaSenhaHash)
        {
            var admin = await _context.Administradores.FindAsync(id);
            if (admin == null) return false;

            admin.senhaHash = novaSenhaHash;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}