namespace UnitSaude.Dto.Disponibilidade
{
    public class CreateDisponibilidadeDto
    {
        public DateOnly DataInicio { get; set; } // Data de início
        public DateOnly DataFim { get; set; } // Data de fim
        public TimeOnly HorarioInicio { get; set; }
        public TimeOnly HorarioFim { get; set; }
        public string Area { get; set; }
        public string Especialidade { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
