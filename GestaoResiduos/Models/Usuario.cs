namespace GestaoResiduos.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Tipo { get; set; } // Coletor, Processador, Administrador
        public string Senha { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public DateTime DataCadastro { get; set; }
        public ICollection<Coleta> Coletas { get; set; }
        public ICollection<Processamento> Processamentos { get; set; }
    }
}
