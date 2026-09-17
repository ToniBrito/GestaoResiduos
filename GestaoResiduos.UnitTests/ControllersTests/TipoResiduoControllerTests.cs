using GestaoResiduos.Controllers;
using GestaoResiduos.Data.Repositories;
using GestaoResiduos.Models;
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
    public class TipoResiduoControllerTests
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
        public async Task GetTiposResiduo_DeveRetornarListaDeTiposResiduo()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            context.TipoResiduos.AddRange(new List<TipoResiduo>
            {
                new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" },
                new TipoResiduo { Id = 2, Nome = "Papel", Descricao = "Descrição", CorIdentificacao = "Verde", Codigo = "PAP001", Reciclavel = "S", TaxaReciclagem = 70.0m, MetodoProcessamento = "Reciclagem" }
            });
            await context.SaveChangesAsync();

            var controller = new TipoResiduoController(repository);

            // Act
            var result = await controller.GetTiposResiduo();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TipoResiduoViewModel>>>(result);
            var tiposResiduo = GetActionResultValue(actionResult).ToList();
            Assert.Equal(2, tiposResiduo.Count);
        }

        [Fact]
        public async Task GetTipoResiduo_ComIdValido_DeveRetornarTipoResiduo()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            var tipoResiduo = new TipoResiduo
            {
                Id = 1,
                Nome = "Plástico",
                Descricao = "Descrição",
                CorIdentificacao = "Azul",
                Codigo = "PLA001",
                Reciclavel = "S",
                TaxaReciclagem = 50.5m,
                MetodoProcessamento = "Reciclagem"
            };
            context.TipoResiduos.Add(tipoResiduo);
            await context.SaveChangesAsync();

            var controller = new TipoResiduoController(repository);

            // Act
            var result = await controller.GetTipoResiduo(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TipoResiduoViewModel>>(result);
            var viewModel = GetActionResultValue(actionResult);
            Assert.Equal(1, viewModel.Id);
            Assert.Equal("Plástico", viewModel.Nome);
        }

        [Fact]
        public async Task GetTipoResiduo_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            var controller = new TipoResiduoController(repository);

            // Act
            var result = await controller.GetTipoResiduo(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TipoResiduoViewModel>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetReciclaveis_DeveRetornarApenasReciclaveis()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            context.TipoResiduos.AddRange(new List<TipoResiduo>
            {
                new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" },
                new TipoResiduo { Id = 2, Nome = "Orgânico", Descricao = "Descrição", CorIdentificacao = "Marrom", Codigo = "ORG001", Reciclavel = "N", TaxaReciclagem = 0m, MetodoProcessamento = "Compostagem" }
            });
            await context.SaveChangesAsync();

            var controller = new TipoResiduoController(repository);

            // Act
            var result = await controller.GetReciclaveis();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TipoResiduoViewModel>>>(result);
            var tiposResiduo = GetActionResultValue(actionResult).ToList();
            Assert.Single(tiposResiduo);
            Assert.All(tiposResiduo, tr => Assert.Equal("S", tr.Reciclavel));
        }

        [Fact]
        public async Task PostTipoResiduo_ComDadosValidos_DeveCriarTipoResiduo()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            var controller = new TipoResiduoController(repository);
            var viewModel = new TipoResiduoCreateViewModel
            {
                Nome = "Vidro",
                Descricao = "Descrição do vidro",
                CorIdentificacao = "Verde",
                Codigo = "VID001",
                Reciclavel = "S",
                TaxaReciclagem = 80.0m,
                MetodoProcessamento = "Reciclagem"
            };

            // Act
            var result = await controller.PostTipoResiduo(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TipoResiduoViewModel>>(result);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var viewModelResult = Assert.IsType<TipoResiduoViewModel>(createdAtResult.Value);
            Assert.Equal("Vidro", viewModelResult.Nome);
            Assert.True(context.TipoResiduos.Any(tr => tr.Nome == "Vidro"));
        }

        [Fact]
        public async Task PutTipoResiduo_ComIdValido_DeveAtualizarTipoResiduo()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            var tipoResiduo = new TipoResiduo
            {
                Id = 1,
                Nome = "Plástico",
                Descricao = "Descrição",
                CorIdentificacao = "Azul",
                Codigo = "PLA001",
                Reciclavel = "S",
                TaxaReciclagem = 50.5m,
                MetodoProcessamento = "Reciclagem"
            };
            context.TipoResiduos.Add(tipoResiduo);
            await context.SaveChangesAsync();

            var controller = new TipoResiduoController(repository);
            var viewModel = new TipoResiduoUpdateViewModel
            {
                Id = 1,
                Nome = "Plástico Atualizado",
                Descricao = "Nova descrição",
                CorIdentificacao = "Azul",
                Codigo = "PLA001",
                Reciclavel = "S",
                TaxaReciclagem = 60.0m,
                MetodoProcessamento = "Reciclagem"
            };

            // Act
            var result = await controller.PutTipoResiduo(1, viewModel);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var tipoResiduoAtualizado = await context.TipoResiduos.FindAsync(1);
            Assert.Equal("Plástico Atualizado", tipoResiduoAtualizado.Nome);
        }

        [Fact]
        public async Task PutTipoResiduo_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            var controller = new TipoResiduoController(repository);
            var viewModel = new TipoResiduoUpdateViewModel
            {
                Id = 999,
                Nome = "Teste",
                Descricao = "Descrição",
                CorIdentificacao = "Azul",
                Codigo = "TEST001",
                Reciclavel = "S",
                TaxaReciclagem = 50.0m,
                MetodoProcessamento = "Reciclagem"
            };

            // Act
            var result = await controller.PutTipoResiduo(999, viewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTipoResiduo_ComIdValido_DeveRemoverTipoResiduo()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            var tipoResiduo = new TipoResiduo
            {
                Id = 1,
                Nome = "Plástico",
                Descricao = "Descrição",
                CorIdentificacao = "Azul",
                Codigo = "PLA001",
                Reciclavel = "S",
                TaxaReciclagem = 50.5m,
                MetodoProcessamento = "Reciclagem"
            };
            context.TipoResiduos.Add(tipoResiduo);
            await context.SaveChangesAsync();

            var controller = new TipoResiduoController(repository);

            // Act
            var result = await controller.DeleteTipoResiduo(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(context.TipoResiduos.Any(tr => tr.Id == 1));
        }

        [Fact]
        public async Task DeleteTipoResiduo_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new TipoResiduoRepository(context);
            var controller = new TipoResiduoController(repository);

            // Act
            var result = await controller.DeleteTipoResiduo(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}