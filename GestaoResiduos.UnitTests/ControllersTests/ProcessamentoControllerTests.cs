using GestaoResiduos.Controllers;
using GestaoResiduos.Data.Repositories;
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
    public class ProcessamentoControllerTests
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
        public async Task GetProcessamentos_DeveRetornarListaDeProcessamentos()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ProcessamentoRepository(context);
            var coletaRepository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Processador", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now };
            var tipoResiduo = new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" };
            var coleta = new Coleta { Id = 1, UsuarioId = 1, TipoResiduoId = 1, Quantidade = 10.0m, DataColeta = DateTime.Now, Local = "Local A", Status = "Realizada", Observacoes = "Nenhuma" };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(coleta);
            context.Processamentos.Add(new Processamento
            {
                Id = 1,
                ColetaId = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                DataProcessamento = DateTime.Now,
                Metodo = "Reciclagem",
                Eficiencia = 80.0m,
                QuantidadeProcessada = 10.0m,
                QuantidadeProduzida = 8.0m,
                Status = "Concluído",
                Resultado = "Material Reciclado com sucesso",
                Coleta = coleta,
                Usuario = usuario,
                TipoResiduo = tipoResiduo
            });
            await context.SaveChangesAsync();

            var controller = new ProcessamentoController(repository, coletaRepository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.GetProcessamentos();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ProcessamentoViewModel>>>(result);
            var processamentos = GetActionResultValue(actionResult).ToList();
            Assert.Single(processamentos);
        }

        [Fact]
        public async Task GetProcessamento_ComIdValido_DeveRetornarProcessamento()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ProcessamentoRepository(context);
            var coletaRepository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Processador", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now };
            var tipoResiduo = new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" };
            var coleta = new Coleta { Id = 1, UsuarioId = 1, TipoResiduoId = 1, Quantidade = 10.0m, DataColeta = DateTime.Now, Local = "Local A", Status = "Realizada", Observacoes = "Nenhuma" };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(coleta);
            context.Processamentos.Add(new Processamento
            {
                Id = 1,
                ColetaId = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                DataProcessamento = DateTime.Now,
                Metodo = "Reciclagem",
                Eficiencia = 80.0m,
                QuantidadeProcessada = 10.0m,
                QuantidadeProduzida = 8.0m,
                Status = "Concluído",
                Resultado = "Material Reciclado com sucesso",
                Coleta = coleta,
                Usuario = usuario,
                TipoResiduo = tipoResiduo
            });
            await context.SaveChangesAsync();

            var controller = new ProcessamentoController(repository, coletaRepository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.GetProcessamento(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ProcessamentoViewModel>>(result);
            var viewModel = GetActionResultValue(actionResult);
            Assert.Equal(1, viewModel.Id);
            Assert.Equal(80.0m, viewModel.Eficiencia);
        }

        [Fact]
        public async Task PostProcessamento_ComDadosValidos_DeveCriarProcessamento()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ProcessamentoRepository(context);
            var coletaRepository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            context.Usuarios.Add(new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Processador", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now });
            context.TipoResiduos.Add(new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" });
            context.Coletas.Add(new Coleta { Id = 1, UsuarioId = 1, TipoResiduoId = 1, Quantidade = 10.0m, DataColeta = DateTime.Now, Local = "Local A", Status = "Realizada", Observacoes = "Nenhuma" });
            await context.SaveChangesAsync();

            var controller = new ProcessamentoController(repository, coletaRepository, usuarioRepository, tipoResiduoRepository);
            var viewModel = new ProcessamentoCreateViewModel
            {
                ColetaId = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Metodo = "Reciclagem",
                Eficiencia = 80.0m,
                QuantidadeProcessada = 10.0m,
                Status = "Em Processamento",
                Resultado = "Processamento iniciado"
            };

            // Act
            var result = await controller.PostProcessamento(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ProcessamentoViewModel>>(result);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var viewModelResult = Assert.IsType<ProcessamentoViewModel>(createdAtResult.Value);
            Assert.Equal(80.0m, viewModelResult.Eficiencia);
            Assert.True(context.Processamentos.Any(p => p.ColetaId == 1));
        }

        [Fact]
        public async Task PostProcessamento_ComColetaInexistente_DeveRetornarBadRequest()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ProcessamentoRepository(context);
            var coletaRepository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var controller = new ProcessamentoController(repository, coletaRepository, usuarioRepository, tipoResiduoRepository);
            var viewModel = new ProcessamentoCreateViewModel
            {
                ColetaId = 999,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Metodo = "Reciclagem",
                Eficiencia = 80.0m
            };

            // Act
            var result = await controller.PostProcessamento(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ProcessamentoViewModel>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Coleta não existe", badRequestResult.Value);
        }

        [Fact]
        public async Task PostProcessamento_ComColetaJaProcessada_DeveRetornarBadRequest()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ProcessamentoRepository(context);
            var coletaRepository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            context.Usuarios.Add(new Usuario { Id = 1, Nome = "João", Email = "joao@test.com", Tipo = "Processador", Endereco = "Rua A", Telefone = "123456789", DataCadastro = DateTime.Now });
            context.TipoResiduos.Add(new TipoResiduo { Id = 1, Nome = "Plástico", Descricao = "Descrição", CorIdentificacao = "Azul", Codigo = "PLA001", Reciclavel = "S", TaxaReciclagem = 50.5m, MetodoProcessamento = "Reciclagem" });
            context.Coletas.Add(new Coleta { Id = 1, UsuarioId = 1, TipoResiduoId = 1, Quantidade = 10.0m, DataColeta = DateTime.Now, Local = "Local A", Status = "Realizada", Observacoes = "Nenhuma" });
            context.Processamentos.Add(new Processamento
            {
                Id = 1,
                ColetaId = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                DataProcessamento = DateTime.Now,
                Metodo = "Reciclagem",
                Eficiencia = 80.0m,
                QuantidadeProcessada = 10.0m,
                QuantidadeProduzida = 8.0m,
                Status = "Concluído",
                Resultado = "Material Reciclado com sucesso"
            });
            await context.SaveChangesAsync();

            var controller = new ProcessamentoController(repository, coletaRepository, usuarioRepository, tipoResiduoRepository);
            var viewModel = new ProcessamentoCreateViewModel
            {
                ColetaId = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Metodo = "Reciclagem",
                Eficiencia = 80.0m
            };

            // Act
            var result = await controller.PostProcessamento(viewModel);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ProcessamentoViewModel>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Coleta já tem processamento", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteProcessamento_ComIdValido_DeveRemoverProcessamento()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ProcessamentoRepository(context);
            var coletaRepository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            var usuario = new Usuario
            {
                Id = 1,
                Nome = "João",
                Email = "joao@test.com",
                Tipo = "Processador",
                Endereco = "Rua A",
                Telefone = "123456789",
                DataCadastro = DateTime.Now
            };

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

            var coleta = new Coleta
            {
                Id = 1,
                UsuarioId = 1,
                TipoResiduoId = 1,
                Quantidade = 10.0m,
                DataColeta = DateTime.Now,
                Local = "Local A",
                Status = "Processando",
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
                QuantidadeProcessada = 10.0m,
                QuantidadeProduzida = 8.0m,
                Status = "Concluído",
                Resultado = "Material Reciclado com sucesso",
                Coleta = coleta,
                Usuario = usuario,
                TipoResiduo = tipoResiduo
            };

            context.Usuarios.Add(usuario);
            context.TipoResiduos.Add(tipoResiduo);
            context.Coletas.Add(coleta);
            context.Processamentos.Add(processamento);
            await context.SaveChangesAsync();

            var processamentoId = processamento.Id;
            var coletaId = coleta.Id;

            var controller = new ProcessamentoController(repository, coletaRepository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.DeleteProcessamento(processamentoId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verificar se o processamento foi deletado
            var processamentoDeletado = await context.Processamentos.FindAsync(processamentoId);
            Assert.Null(processamentoDeletado);

            // Verificar se o status da coleta foi atualizado para "Aguardando Processamento"
            context.ChangeTracker.Clear();
            var coletaAtualizada = await context.Coletas.FindAsync(coletaId);
            Assert.NotNull(coletaAtualizada);
            Assert.Equal("Aguardando Processamento", coletaAtualizada.Status);
        }

        [Fact]
        public async Task GetEstatisticas_DeveRetornarEstatisticas()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new ProcessamentoRepository(context);
            var coletaRepository = new ColetaRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var tipoResiduoRepository = new TipoResiduoRepository(context);

            context.Processamentos.AddRange(new List<Processamento>
            {
                new Processamento { Id = 1, ColetaId = 1, UsuarioId = 1, TipoResiduoId = 1, DataProcessamento = DateTime.Now, Metodo = "Reciclagem", Eficiencia = 80.0m, QuantidadeProcessada = 10.0m, QuantidadeProduzida = 8.0m, Status = "Concluído", Resultado = "Material reciclado com sucesso" },
                new Processamento { Id = 2, ColetaId = 2, UsuarioId = 1, TipoResiduoId = 1, DataProcessamento = DateTime.Now, Metodo = "Reciclagem", Eficiencia = 70.0m, QuantidadeProcessada = 20.0m, QuantidadeProduzida = 14.0m, Status = "Concluído", Resultado = "Material reciclado com sucesso" }
            });
            await context.SaveChangesAsync();

            var controller = new ProcessamentoController(repository, coletaRepository, usuarioRepository, tipoResiduoRepository);

            // Act
            var result = await controller.GetEstatisticas();

            // Assert
            var actionResult = Assert.IsType<ActionResult<ProcessamentoEstatisticasViewModel>>(result);
            var estatisticas = GetActionResultValue(actionResult);
            Assert.Equal(2, estatisticas.TotalProcessamentos);
            Assert.Equal(30.0m, estatisticas.TotalProcessado);
            Assert.Equal(22.0m, estatisticas.TotalProduzido);
        }
    }
}
