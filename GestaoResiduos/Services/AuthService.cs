using GestaoResiduos.Models;
namespace GestaoResiduos.Services
{
    public class AuthService
    {
        private List<Usuario> _users = new List<Usuario>
                {
                    new Usuario { Id = 1, Nome = "Antonio Brito", Senha = "pass123", Tipo = "operador" },
                    new Usuario { Id = 2, Nome = "José Victor", Senha = "pass123", Tipo = "analista" }
                };
        public Usuario Authenticate(string usuarios, string senha)
        {
            // Aqui você normalmente faria a verificação de senha de forma segura
            return _users.FirstOrDefault(u => u.Nome == usuarios && u.Senha == senha);
        }
    }
}
