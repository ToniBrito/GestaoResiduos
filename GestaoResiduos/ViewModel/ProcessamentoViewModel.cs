namespace GestaoResiduos.ViewModel
{
    public class ProcessamentoViewModel
    {
        public int Id { get; set; }
        public int ColetaId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public int TipoResiduoId { get; set; }
        public string TipoResiduoNome { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string Metodo { get; set; }
        public decimal Eficiencia { get; set; }
        public decimal QuantidadeProcessada { get; set; }
        public decimal QuantidadeProduzida { get; set; }
        public string Status { get; set; }
        public string Resultado { get; set; }
    }

    public class ProcessamentoCreateViewModel
    {
        public int ColetaId { get; set; }
        public int UsuarioId { get; set; }
        public int TipoResiduoId { get; set; }
        public DateTime? DataProcessamento { get; set; }
        public string Metodo { get; set; }
        public decimal Eficiencia { get; set; }
        public decimal QuantidadeProcessada { get; set; }
        public decimal QuantidadeProduzida { get; set; }
        public string Status { get; set; }
        public string Resultado { get; set; }
    }

    public class ProcessamentoUpdateViewModel
    {
        public int Id { get; set; }
        public int ColetaId { get; set; }
        public int UsuarioId { get; set; }
        public int TipoResiduoId { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string Metodo { get; set; }
        public decimal Eficiencia { get; set; }
        public decimal QuantidadeProcessada { get; set; }
        public decimal QuantidadeProduzida { get; set; }
        public string Status { get; set; }
        public string Resultado { get; set; }
    }

    public class ProcessamentoEstatisticasViewModel
    {
        public int TotalProcessamentos { get; set; }
        public decimal MediaEficiencia { get; set; }
        public decimal TotalProcessado { get; set; }
        public decimal TotalProduzido { get; set; }
    }
}
