namespace GestaoResiduos.ViewModel
{
    public class ColetaViewModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public int TipoResiduoId { get; set; }
        public string TipoResiduoNome { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataColeta { get; set; }
        public string Local { get; set; }
        public string Status { get; set; }
        public string Observacoes { get; set; }
        public bool TemProcessamento { get; set; }
    }

    public class ColetaCreateViewModel
    {
        public int UsuarioId { get; set; }
        public int TipoResiduoId { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime? DataColeta { get; set; }
        public string Local { get; set; }
        public string Status { get; set; }
        public string Observacoes { get; set; }
    }

    public class ColetaUpdateViewModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int TipoResiduoId { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataColeta { get; set; }
        public string Local { get; set; }
        public string Status { get; set; }
        public string Observacoes { get; set; }
    }

    public class ColetaEstatisticasViewModel
    {
        public int TotalColetas { get; set; }
        public decimal TotalQuantidade { get; set; }
        public DateTime? ColetaMaisRecente { get; set; }
    }
}

