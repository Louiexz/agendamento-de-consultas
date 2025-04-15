namespace UnitSaude.Models
{
    public static class DadosFixosConsulta
    {
        public static readonly Dictionary<string, List<string>> EspecialidadesPorArea = new()
    {
        { "Odontologia", new List<string> { "Ortodontia", "Endodontia" } },
        { "Nutrição", new List<string> { "Nutrição Esportiva", "Nutrição Clínica" } },
        { "Enfermagem", new List<string> { "Urgência e Emergência", "Geriatria" } },
        { "Estética", new List<string> { "Dermatofuncional", "Estética Facial" } },
        { "Fisioterapia", new List<string> { "Ortopédica", "Neurológica" } },
        { "Psicologia", new List<string> { "Clínica", "Escolar" } }
    };

        public static readonly List<string> Status = new List<string> { "Em Espera", "Agendada", "Concluída", "Cancelada" };

        public static List<string> ObterAreas()
            => EspecialidadesPorArea.Keys.ToList();

        public static List<string> ObterEspecialidadesPorArea(string area)
            => EspecialidadesPorArea.TryGetValue(area, out var especialidades)
                ? especialidades
                : new List<string>();
    }

}
