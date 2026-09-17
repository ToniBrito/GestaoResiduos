namespace GestaoResiduos.Models
{
    public class Coleta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int TipoResiduoId { get; set; }
        public decimal Quantidade { get; set; } // em kg
        public DateTime DataColeta { get; set; }
        public string Local { get; set; }
        public string Status { get; set; } // Agendada, Realizada, Cancelada
        public string? Observacoes { get; set; }
        public Usuario Usuario { get; set; }
        public TipoResiduo TipoResiduo { get; set; }
        public Processamento? Processamento { get; set; }
    }
}
