using GestaoResiduos.Data.Repositories;
using GestaoResiduos.Models;
using GestaoResiduos.Repositories;
using GestaoResiduos.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoResiduos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessamentoController : ControllerBase
    {
        private readonly ProcessamentoRepository _repository;
        private readonly ColetaRepository _coletaRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly TipoResiduoRepository _tipoResiduoRepository;

        public ProcessamentoController(
            ProcessamentoRepository repository,
            ColetaRepository coletaRepository,
            UsuarioRepository usuarioRepository,
            TipoResiduoRepository tipoResiduoRepository)
        {
            _repository = repository;
            _coletaRepository = coletaRepository;
            _usuarioRepository = usuarioRepository;
            _tipoResiduoRepository = tipoResiduoRepository;
        }

        // Listar todos
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProcessamentoViewModel>>> GetProcessamentos()
        {
            var processamentos = await _repository.GetAllAsync();

            return processamentos.Select(p => new ProcessamentoViewModel
            {
                Id = p.Id,
                ColetaId = p.ColetaId,
                UsuarioId = p.UsuarioId,
                UsuarioNome = p.Usuario?.Nome ?? string.Empty,
                TipoResiduoId = p.TipoResiduoId,
                TipoResiduoNome = p.TipoResiduo?.Nome ?? string.Empty,
                DataProcessamento = p.DataProcessamento,
                Metodo = p.Metodo,
                Eficiencia = p.Eficiencia,
                QuantidadeProcessada = p.QuantidadeProcessada,
                QuantidadeProduzida = p.QuantidadeProduzida,
                Status = p.Status,
                Resultado = p.Resultado
            }).ToList();
        }

        // Busca por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessamentoViewModel>> GetProcessamento(int id)
        {
            var processamento = await _repository.GetByIdAsync(id);

            if (processamento == null)
            {
                return NotFound();
            }

            return new ProcessamentoViewModel
            {
                Id = processamento.Id,
                ColetaId = processamento.ColetaId,
                UsuarioId = processamento.UsuarioId,
                UsuarioNome = processamento.Usuario?.Nome ?? string.Empty,
                TipoResiduoId = processamento.TipoResiduoId,
                TipoResiduoNome = processamento.TipoResiduo?.Nome ?? string.Empty,
                DataProcessamento = processamento.DataProcessamento,
                Metodo = processamento.Metodo,
                Eficiencia = processamento.Eficiencia,
                QuantidadeProcessada = processamento.QuantidadeProcessada,
                QuantidadeProduzida = processamento.QuantidadeProduzida,
                Status = processamento.Status,
                Resultado = processamento.Resultado
            };
        }

        // Cadastrar
        [HttpPost]
        public async Task<ActionResult<ProcessamentoViewModel>> PostProcessamento(ProcessamentoCreateViewModel viewModel)
        {
            // Validações básicas
            var coleta = await _coletaRepository.GetByIdAsync(viewModel.ColetaId);
            if (coleta == null) return BadRequest("Coleta não existe");

            var usuario = await _usuarioRepository.GetByIdAsync(viewModel.UsuarioId);
            if (usuario == null) return BadRequest("Usuário não existe");

            var tipoResiduo = await _tipoResiduoRepository.GetByIdAsync(viewModel.TipoResiduoId);
            if (tipoResiduo == null) return BadRequest("Tipo de resíduo não existe");

            // Verifica se coleta já tem processamento
            var existeProcessamento = await _repository.ColetaHasProcessamentoAsync(viewModel.ColetaId);
            if (existeProcessamento) return BadRequest("Coleta já tem processamento");

            var processamento = new Processamento
            {
                ColetaId = viewModel.ColetaId,
                UsuarioId = viewModel.UsuarioId,
                TipoResiduoId = viewModel.TipoResiduoId,
                DataProcessamento = viewModel.DataProcessamento ?? DateTime.Now,
                Metodo = viewModel.Metodo,
                Eficiencia = viewModel.Eficiencia,
                QuantidadeProcessada = viewModel.QuantidadeProcessada == 0 ? coleta.Quantidade : viewModel.QuantidadeProcessada,
                QuantidadeProduzida = viewModel.QuantidadeProduzida,
                Status = viewModel.Status ?? "Em Processamento",
                Resultado = viewModel.Resultado
            };

            // Calcula quantidade produzida se não informada e eficiência fornecida
            if (processamento.QuantidadeProduzida == 0 && processamento.Eficiencia > 0)
            {
                processamento.QuantidadeProduzida = processamento.QuantidadeProcessada *
                                                   (processamento.Eficiencia / 100);
            }

            // Atualiza status da coleta
            coleta.Status = "Processando";

            await _repository.AddAsync(processamento);
            await _coletaRepository.UpdateAsync(coleta);

            // Buscar com includes para retornar ViewModel completo
            var processamentoCompleto = await _repository.GetByIdAsync(processamento.Id);

            var result = new ProcessamentoViewModel
            {
                Id = processamentoCompleto.Id,
                ColetaId = processamentoCompleto.ColetaId,
                UsuarioId = processamentoCompleto.UsuarioId,
                UsuarioNome = processamentoCompleto.Usuario?.Nome ?? string.Empty,
                TipoResiduoId = processamentoCompleto.TipoResiduoId,
                TipoResiduoNome = processamentoCompleto.TipoResiduo?.Nome ?? string.Empty,
                DataProcessamento = processamentoCompleto.DataProcessamento,
                Metodo = processamentoCompleto.Metodo,
                Eficiencia = processamentoCompleto.Eficiencia,
                QuantidadeProcessada = processamentoCompleto.QuantidadeProcessada,
                QuantidadeProduzida = processamentoCompleto.QuantidadeProduzida,
                Status = processamentoCompleto.Status,
                Resultado = processamentoCompleto.Resultado
            };

            return CreatedAtAction("GetProcessamento", new { id = processamento.Id }, result);
        }

        // Atualizar
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcessamento(int id, ProcessamentoUpdateViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            var processamento = await _repository.GetByIdAsync(id);
            if (processamento == null) return NotFound();

            // Validações básicas
            var coleta = await _coletaRepository.GetByIdAsync(viewModel.ColetaId);
            if (coleta == null) return BadRequest("Coleta não existe");

            var usuario = await _usuarioRepository.GetByIdAsync(viewModel.UsuarioId);
            if (usuario == null) return BadRequest("Usuário não existe");

            var tipoResiduo = await _tipoResiduoRepository.GetByIdAsync(viewModel.TipoResiduoId);
            if (tipoResiduo == null) return BadRequest("Tipo de resíduo não existe");

            processamento.ColetaId = viewModel.ColetaId;
            processamento.UsuarioId = viewModel.UsuarioId;
            processamento.TipoResiduoId = viewModel.TipoResiduoId;
            processamento.DataProcessamento = viewModel.DataProcessamento;
            processamento.Metodo = viewModel.Metodo;
            processamento.Eficiencia = viewModel.Eficiencia;
            processamento.QuantidadeProcessada = viewModel.QuantidadeProcessada;
            processamento.QuantidadeProduzida = viewModel.QuantidadeProduzida;
            processamento.Status = viewModel.Status;
            processamento.Resultado = viewModel.Resultado;

            await _repository.UpdateAsync(processamento);

            return NoContent();
        }

        // Remover
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcessamento(int id)
        {
            var processamento = await _repository.GetByIdAsync(id);

            if (processamento == null) return NotFound();

            // Reverte status da coleta
            if (processamento.Coleta != null)
            {
                processamento.Coleta.Status = "Aguardando Processamento";
                await _coletaRepository.UpdateAsync(processamento.Coleta);
            }

            await _repository.DeleteAsync(id);

            return NoContent();
        }

        // Verificar Estatisticas
        [HttpGet("estatisticas")]
        public async Task<ActionResult<ProcessamentoEstatisticasViewModel>> GetEstatisticas()
        {
            var total = await _repository.GetTotalProcessamentosAsync();
            var mediaEficiencia = await _repository.GetMediaEficienciaAsync();
            var totalProcessado = await _repository.GetTotalProcessadoAsync();
            var totalProduzido = await _repository.GetTotalProduzidoAsync();

            return new ProcessamentoEstatisticasViewModel
            {
                TotalProcessamentos = total,
                MediaEficiencia = Math.Round(mediaEficiencia, 2),
                TotalProcessado = totalProcessado,
                TotalProduzido = totalProduzido
            };
        }
    }
}