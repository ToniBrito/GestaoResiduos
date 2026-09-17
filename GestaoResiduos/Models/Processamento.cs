namespace GestaoResiduos.Models
{
    public class Processamento
    {
        public int Id { get; set; }
        public int ColetaId { get; set; }
        public int UsuarioId { get; set; }
        public int TipoResiduoId { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string Metodo { get; set; } // Reciclagem, Compostagem, Incineração, Aterro
        public decimal Eficiencia { get; set; } // porcentagem
        public decimal QuantidadeProcessada { get; set; }
        public decimal QuantidadeProduzida { get; set; } // material reciclado
        public string Status { get; set; } // Em Processamento, Concluído, Falha
        public string Resultado { get; set; }
        public Coleta Coleta { get; set; }
        public Usuario Usuario { get; set; }
        public TipoResiduo TipoResiduo { get; set; }
    }
}
