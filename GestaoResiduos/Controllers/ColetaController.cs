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
    public class ColetaController : ControllerBase
    {
        private readonly ColetaRepository _repository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly TipoResiduoRepository _tipoResiduoRepository;

        public ColetaController(
            ColetaRepository repository,
            UsuarioRepository usuarioRepository,
            TipoResiduoRepository tipoResiduoRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _tipoResiduoRepository = tipoResiduoRepository;
        }

        // Listar
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ColetaViewModel>>> GetColetas()
        {
            var coletas = await _repository.GetAllAsync();

            return coletas.Select(c => new ColetaViewModel
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                UsuarioNome = c.Usuario?.Nome ?? string.Empty,
                TipoResiduoId = c.TipoResiduoId,
                TipoResiduoNome = c.TipoResiduo?.Nome ?? string.Empty,
                Quantidade = c.Quantidade,
                DataColeta = c.DataColeta,
                Local = c.Local,
                Status = c.Status,
                Observacoes = c.Observacoes,
                TemProcessamento = c.Processamento != null
            }).ToList();
        }

        // Listar por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ColetaViewModel>> GetColeta(int id)
        {
            var coleta = await _repository.GetByIdAsync(id);

            if (coleta == null)
            {
                return NotFound();
            }

            return new ColetaViewModel
            {
                Id = coleta.Id,
                UsuarioId = coleta.UsuarioId,
                UsuarioNome = coleta.Usuario?.Nome ?? string.Empty,
                TipoResiduoId = coleta.TipoResiduoId,
                TipoResiduoNome = coleta.TipoResiduo?.Nome ?? string.Empty,
                Quantidade = coleta.Quantidade,
                DataColeta = coleta.DataColeta,
                Local = coleta.Local,
                Status = coleta.Status,
                Observacoes = coleta.Observacoes,
                TemProcessamento = coleta.Processamento != null
            };
        }

        // Cadastrar
        [HttpPost]
        public async Task<ActionResult<ColetaViewModel>> PostColeta(ColetaCreateViewModel viewModel)
        {
            // Validações básicas
            var usuarioExiste = await _usuarioRepository.GetByIdAsync(viewModel.UsuarioId);
            if (usuarioExiste == null) return BadRequest("Usuário não existe");

            var tipoResiduoExiste = await _tipoResiduoRepository.GetByIdAsync(viewModel.TipoResiduoId);
            if (tipoResiduoExiste == null) return BadRequest("Tipo de resíduo não existe");

            var coleta = new Coleta
            {
                UsuarioId = viewModel.UsuarioId,
                TipoResiduoId = viewModel.TipoResiduoId,
                Quantidade = viewModel.Quantidade,
                DataColeta = viewModel.DataColeta ?? DateTime.Now,
                Local = viewModel.Local,
                Status = viewModel.Status ?? "Agendada",
                Observacoes = viewModel.Observacoes
            };

            await _repository.AddAsync(coleta);

            // Buscar com includes para retornar ViewModel completo
            var coletaCompleta = await _repository.GetByIdAsync(coleta.Id);

            var result = new ColetaViewModel
            {
                Id = coletaCompleta.Id,
                UsuarioId = coletaCompleta.UsuarioId,
                UsuarioNome = coletaCompleta.Usuario?.Nome ?? string.Empty,
                TipoResiduoId = coletaCompleta.TipoResiduoId,
                TipoResiduoNome = coletaCompleta.TipoResiduo?.Nome ?? string.Empty,
                Quantidade = coletaCompleta.Quantidade,
                DataColeta = coletaCompleta.DataColeta,
                Local = coletaCompleta.Local,
                Status = coletaCompleta.Status,
                Observacoes = coletaCompleta.Observacoes,
                TemProcessamento = coletaCompleta.Processamento != null
            };

            return CreatedAtAction("GetColeta", new { id = coleta.Id }, result);
        }

        // Atualizar
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColeta(int id, ColetaUpdateViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            var coleta = await _repository.GetByIdAsync(id);
            if (coleta == null) return NotFound();

            // Validações básicas
            var usuarioExiste = await _usuarioRepository.ExistsAsync(viewModel.UsuarioId);
            if (!usuarioExiste) return BadRequest("Usuário não existe");

            var tipoResiduoExiste = await _tipoResiduoRepository.ExistsAsync(viewModel.TipoResiduoId);
            if (!tipoResiduoExiste) return BadRequest("Tipo de resíduo não existe");

            coleta.UsuarioId = viewModel.UsuarioId;
            coleta.TipoResiduoId = viewModel.TipoResiduoId;
            coleta.Quantidade = viewModel.Quantidade;
            coleta.DataColeta = viewModel.DataColeta;
            coleta.Local = viewModel.Local;
            coleta.Status = viewModel.Status;
            coleta.Observacoes = viewModel.Observacoes;

            await _repository.UpdateAsync(coleta);

            return NoContent();
        }

        // Remover por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColeta(int id)
        {
            var coleta = await _repository.GetByIdAsync(id);

            if (coleta == null) return NotFound();

            // Verifica se tem processamento associado
            if (coleta.Processamento != null)
            {
                return BadRequest("Não pode excluir coleta com processamento");
            }

            await _repository.DeleteAsync(id);

            return NoContent();
        }

        // Listar Estatisticas
        [HttpGet("estatisticas")]
        public async Task<ActionResult<ColetaEstatisticasViewModel>> GetEstatisticas()
        {
            var totalColetas = await _repository.GetTotalColetasAsync();
            var totalQuantidade = await _repository.GetTotalQuantidadeAsync();
            var coletaMaisRecente = await _repository.GetDataMaisRecenteAsync();

            return new ColetaEstatisticasViewModel
            {
                TotalColetas = totalColetas,
                TotalQuantidade = totalQuantidade,
                ColetaMaisRecente = coletaMaisRecente
            };
        }
    }
}