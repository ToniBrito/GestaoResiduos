using GestaoResiduos.Controllers;
using GestaoResiduos.Data.Contexts;
using GestaoResiduos.Data.Repositories;
using GestaoResiduos.Models;
using GestaoResiduos.Repositories;
using GestaoResiduos.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoResiduos.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Gestao_Residuos.UnitTests.ControllersTests
{
    public class ColetaControllerTests
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
        public async Task GetColetas_DeveRetornarListaDeColetas()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now };
            var tipoResiduo = new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(new Coleta
            {
                Id = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 10.5m,
                DataColeta = DateTime.Now,
                Local = "Local A",
                Status = "Agendada",
                Observacoes = "Nenhuma",
                Usuario = usuario,
                TipoResiduo = tipoResiduo
            });
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.GetColetas();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ColetaViewModel>>>(result);
            var coletas = GetActionResultValue(actionResult).ToList();
            Assert.Single(coletas);
        }

        [Fact]
        public async Task GetColeta_ComIdValido_DeveRetornarColeta()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now };
            var tipoResiduo = new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(new Coleta
            {
                Id = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 10.5m,
                DataColeta = DateTime.Now,
                Local = "Local A",
                Status = "Agendada",
                Observacoes = "Nenhuma",
                Usuario = usuario,
                TipoResiduo = tipoResiduo
            });
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.GetColeta(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ColetaViewModel>>(result);
            var viewModel = GetActionResultValue(actionResult);
            Assert.Equal(1, viewModel.Id);
            Assert.Equal("João", viewModel.UsuarioNome);
        }

        [Fact]
        public async Task GetColeta_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.GetColeta(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ColetaViewModel>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostColeta_ComDadosValidos_DeveCriarColeta()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            context.Usuarios.Add(new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now });
            context.TipoResiduos.Add(new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" });
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);
            var viewModel = new ColetaCreateViewModel
            {
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 15.0m,
                Local = "Local B",
                Status = "Agendada",
                Observacoes = "Nenhuma"
            };

            // Act
            var result = await controller.PostColeta(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ColetaViewModel>>(result);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var viewModelResult = Assert.IsType<ColetaViewModel>(createdAtResult.Value);
            Assert.Equal(15.0m, viewModelResult.Quantidade);
            Assert.True(context.Coletas.Any(c => c.Quantidade == 15.0m));
        }

        [Fact]
        public async Task PostColeta_ComUsuarioInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);
            var viewModel = new ColetaCreateViewModel
            {
                UsuarioId = 999,
                TipoResiduoId = 1,
                Quantidade = 15.0m,
                Local = "Local B"
            };

            // Act
            var result = await controller.PostColeta(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ColetaViewModel>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Usuário não existe", badRequestResult.Value);
        }

        [Fact]
        public async Task PostColeta_ComTipoResiduoInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            context.Usuarios.Add(new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now });
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);
            var viewModel = new ColetaCreateViewModel
            {
                UsuarioId = 1,
                TipoResiduoId = 999,
                Quantidade = 15.0m,
                Local = "Local B"
            };

            // Act
            var result = await controller.PostColeta(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ColetaViewModel>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Tipo de resíduo não existe", badRequestResult.Value);
        }

        [Fact]
        public async Task PutColeta_ComIdValido_DeveAtualizarColeta()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now };
            var tipoResiduo = new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(new Coleta
            {
                Id = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 10.5m,
                DataColeta = DateTime.Now,
                Local = "Local A",
                Status = "Agendada",
                Observacoes = "Nenhuma"
            });
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);
            var viewModel = new ColetaUpdateViewModel
            {
                Id = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 20.0m,
                DataColeta = DateTime.Now,
                Local = "Local Atualizado",
                Status = "Realizada",
                Observacoes = "Atualizada"
            };

            // Act
            var result = await controller.PutColeta(1, viewModel);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var coletaAtualizada = await context.Coletas.FindAsync(1);
            Assert.Equal(20.0m, coletaAtualizada.Quantidade);
            Assert.Equal("Local Atualizado", coletaAtualizada.Local);
        }

        [Fact]
        public async Task DeleteColeta_ComIdValido_DeveRemoverColeta()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now };
            var tipoResiduo = new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(new Coleta
            {
                Id = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 10.5m,
                DataColeta = DateTime.Now,
                Local = "Local A",
                Status = "Agendada",
                Observacoes = "Nenhuma"
            });
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.DeleteColeta(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(context.Coletas.Any(c => c.Id == 1));
        }

        [Fact]
        public async Task DeleteColeta_ComProcessamentoAssociado_DeveRetornarBadRequest()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Coletor", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now };
            var tipoResiduo = new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" };
            var coleta = new Coleta
            {
                Id = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 10.5m,
                DataColeta = DateTime.Now,
                Local = "Local A",
                Status = "Agendada",
                Observacoes = "Nenhuma"
            };
            var processamento = new Processamento
            {
                Id = 1,
                ColetaId = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                DataProcessamento = DateTime.Now,
                Metodo = "Reciclagem",
                Eficiencia = 80.0m,
                QuantidadeProcessada = 10.5m,
                QuantidadeProduzida = 8.4m,
                Status = "Concluído",
                Resultado = "Sucesso",
                Coleta = coleta
            };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(coleta);
            context.Processamentos.Add(processamento);
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.DeleteColeta(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Não pode excluir coleta com processamento", badRequestResult.Value);
        }

        [Fact]
        public async Task GetEstatisticas_DeveRetornarEstatisticas()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            context.Coletas.AddRange(new List<Coleta>
            {
                new Coleta { Id = 1, UsuarioId = 1, TipoResiduoId = 1, Quantidade = 10.0m, DataColeta = DateTime.Now.AddDays(-2), Local = "Local A", Status = "Realizada", Observacoes = "Nenhuma" },
                new Coleta { Id = 2, UsuarioId = 1, TipoResiduoId = 1, Quantidade = 15.0m, DataColeta = DateTime.Now.AddDays(-1), Local = "Local B", Status = "Realizada", Observacoes = "Nenhuma" }
            });
            await context.SaveChangesAsync();

            var controller = new ColetaController(repository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.GetEstatisticas();

            // Assert
            var actionResult = Assert.IsType<ActionResult<ColetaEstatisticasViewModel>>(result);
            var estatisticas = GetActionResultValue(actionResult);
            Assert.Equal(2, estatisticas.TotalColetas);
            Assert.Equal(25.0m, estatisticas.TotalQuantidade);
        }
    }
}
