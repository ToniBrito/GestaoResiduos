namespace GestaoResiduos.ViewModel
{
    public class TipoResiduoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string CorIdentificacao { get; set; }
        public string Codigo { get; set; }
        public string Reciclavel { get; set; }
        public decimal TaxaReciclagem { get; set; }
        public string MetodoProcessamento { get; set; }
    }

    public class TipoResiduoCreateViewModel
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string CorIdentificacao { get; set; }
        public string Codigo { get; set; }
        public string Reciclavel { get; set; }
        public decimal TaxaReciclagem { get; set; }
        public string MetodoProcessamento { get; set; }
    }

    public class TipoResiduoUpdateViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string CorIdentificacao { get; set; }
        public string Codigo { get; set; }
        public string Reciclavel { get; set; }
        public decimal TaxaReciclagem { get; set; }
        public string MetodoProcessamento { get; set; }
    }
}
