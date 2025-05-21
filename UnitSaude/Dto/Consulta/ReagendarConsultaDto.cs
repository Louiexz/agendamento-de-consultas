namespace UnitSaude.Dto.Consulta
{
    public class ReagendarConsultaDto
    {
        public required DateOnly NovaData { get; set; }
        public required TimeOnly NovoHorario { get; set; }
    }

}
