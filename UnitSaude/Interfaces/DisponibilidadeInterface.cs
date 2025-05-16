using UnitSaude.Dto.Disponibilidade;
using UnitSaude.Models;

namespace UnitSaude.Interfaces
{
    public interface DisponibilidadeInterface
    {
        public Task<ResponseModel<ReadDisponibilidadeDto>> CadastrarDisponibilidade(CreateDisponibilidadeDto dto);
        public Task<ResponseModel<List<ReadDisponibilidadeDto>>> ListarDisponibilidades();

        public Task<ResponseModel<Disponibilidade>> RemoverDisponibilidade(int DisponibilidadeId);

    }
}
