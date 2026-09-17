using GestaoResiduos.Controllers;
using GestaoResiduos.Data.Contexts;
using GestaoResiduos.Models;
using GestaoResiduos.Repositories;
using GestaoResiduos.ViewModel;
using GestaoResiduos.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Gestao_Residuos.UnitTests.ControllersTests
{
    public class UsuarioControllerTests
    {
        private DatabaseContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new DatabaseContext(options);
        }

        private static T GetActionResultValue<T>(ActionResult<T> actionResult)
        {
            if (actionResult.Result != null)
            {
                if (actionResult.Result is ObjectResult obj)
                {
                    return (T)obj.Value!;
                }
                throw new InvalidOperationException($"Unexpected ActionResult type: {actionResult.Result.GetType().FullName}");
            }
            return actionResult.Value!;
        }

        [Fact]
        public async Task GetUsuarios_DeveRetornarListaDeUsuarios()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            context.Usuarios.AddRange(new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now },
                new Usuario { Id = 2, Nome = "Maria", Email = "maria@test.com", Tipo = "Processador", Endereco = "Rua B", Telefone = "987654321", DataCadastro = DateTime.Now }
            });
            await context.SaveChangesAsync();

            var controller = new UsuarioController(repository);

            // Act
            var result = await controller.GetUsuarios();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<UsuarioViewModel>>>(result);
            var usuarios = GetActionResultValue(actionResult).ToList();
            Assert.Equal(2, usuarios.Count);
        }

        [Fact]
        public async Task GetUsuario_ComIdValido_DeveRetornarUsuario()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            context.Usuarios.Add(new Usuario
            {
                Id = 1,
                Nome = "João",
                Email = "joao@test.com",
                Tipo = "Coletor",
                Endereco = "Rua A",
                Telefone = "123456789",
                DataCadastro = DateTime.Now
            });
            await context.SaveChangesAsync();

            var controller = new UsuarioController(repository);

            // Act
            var result = await controller.GetUsuario(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<UsuarioViewModel>>(result);
            var viewModel = GetActionResultValue(actionResult);
            Assert.Equal(1, viewModel.Id);
            Assert.Equal("João", viewModel.Nome);
            Assert.Equal("joao@test.com", viewModel.Email);
        }

        [Fact]
        public async Task GetUsuario_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            var controller = new UsuarioController(repository);

            // Act
            var result = await controller.GetUsuario(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<UsuarioViewModel>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetUsuariosPorTipo_DeveRetornarUsuariosDoTipo()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            context.Usuarios.AddRange(new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now },
                new Usuario { Id = 2, Nome = "Maria", Email = "maria@test.com", Tipo = "Processador", Endereco = "Rua B", Telefone = "987654321", DataCadastro = DateTime.Now },
                new Usuario { Id = 3, Nome = "Pedro", Email = "pedro@test.com", Tipo = "Coletor", Endereco = "Rua C", Telefone = "111222333", DataCadastro = DateTime.Now }
            });
            await context.SaveChangesAsync();

            var controller = new UsuarioController(repository);

            // Act
            var result = await controller.GetUsuariosPorTipo("Coletor");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<UsuarioViewModel>>>(result);
            var usuarios = GetActionResultValue(actionResult).ToList();
            Assert.Equal(2, usuarios.Count);
            Assert.All(usuarios, u => Assert.Equal("Coletor", u.Tipo));
        }

        [Fact]
        public async Task PostUsuario_ComDadosValidos_DeveCriarUsuario()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            var controller = new UsuarioController(repository);
            var viewModel = new UsuarioCreateViewModel
            {
                Nome = "João",
                Email = "joao@test.com",
                Tipo = "Coletor",
                Endereco = "Rua A",
                Telefone = "123456789"
            };

            // Act
            var result = await controller.PostUsuario(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<UsuarioViewModel>>(result);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var viewModelResult = Assert.IsType<UsuarioViewModel>(createdAtResult.Value);
            Assert.Equal("João", viewModelResult.Nome);
            Assert.True(context.Usuarios.Any(u => u.Nome == "João"));
        }

        [Fact]
        public async Task PutUsuario_ComIdValido_DeveAtualizarUsuario()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            context.Usuarios.Add(new Usuario
            {
                Id = 1,
                Nome = "João",
                Email = "joao@test.com",
                Tipo = "Coletor",
                Endereco = "Rua A",
                Telefone = "123456789",
                DataCadastro = DateTime.Now
            });
            await context.SaveChangesAsync();

            var controller = new UsuarioController(repository);
            var viewModel = new UsuarioUpdateViewModel
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao.silva@test.com",
                Tipo = "Coletor",
                Endereco = "Rua B",
                Telefone = "999888777"
            };

            // Act
            var result = await controller.PutUsuario(1, viewModel);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var usuarioAtualizado = await context.Usuarios.FindAsync(1);
            Assert.Equal("João Silva", usuarioAtualizado.Nome);
            Assert.Equal("joao.silva@test.com", usuarioAtualizado.Email);
        }

        [Fact]
        public async Task PutUsuario_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            var controller = new UsuarioController(repository);
            var viewModel = new UsuarioUpdateViewModel
            {
                Id = 999,
                Nome = "Teste",
                Email = "teste@test.com",
                Tipo = "Coletor",
                Endereco = "Rua A",
                Telefone = "123456789"
            };

            // Act
            var result = await controller.PutUsuario(999, viewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUsuario_ComIdValido_DeveRemoverUsuario()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new UsuarioRepository(context);
            context.Usuarios.Add(new Usuario
            {
                Id = 1,
                Nome = "João",
                Email = "joao@test.com",
                Tipo = "Coletor",
                Endereco = "Rua A",
                Telefone = "123456789",
                DataCadastro = DateTime.Now
            });
            await context.SaveChangesAsync();

            var controller = new UsuarioController(repository);

            // Act
            var result = await controller.DeleteUsuario(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(context.Usuarios.Any(u => u.Id == 1));
        }
    }
}