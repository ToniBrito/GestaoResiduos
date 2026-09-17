namespace GestaoResiduos.Models
{
    public class TipoResiduo
    {
        public int Id { get; set; }
        public string Nome { get; set; } // Plástico, Papel, Vidro, Metal, Orgânico, Eletrônico
        public string Descricao { get; set; }
        public string CorIdentificacao { get; set; }
        public string Codigo { get; set; } // Código de identificação
        public string Reciclavel { get; set; }
        public decimal TaxaReciclagem { get; set; }
        public string MetodoProcessamento { get; set; }
        public ICollection<Coleta> Coletas { get; set; }
        public ICollection<Processamento> Processamentos { get; set; }
    }
}
