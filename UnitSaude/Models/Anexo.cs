namespace UnitSaude.Models
{
    public class Anexo
    {
        public int id_Anexo { get; set; }
        public string arquivoExterno { get; set; } // URL do arquivo no S3/Firebase
        public string nome_Arquivo { get; set; }
        public string tipo_Arquivo { get; set; }
        public DateTime data_Upload { get; set; } = DateTime.UtcNow;

        // Chave estrangeira
        public int id_prontuario { get; set; }

        // Navegação
        //public Prontuario Prontuario { get; set; }
    }
}
