namespace GestaoResiduos.ViewModel
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Tipo { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    public class UsuarioCreateViewModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Tipo { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
    }

    public class UsuarioUpdateViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Tipo { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
    }
}
