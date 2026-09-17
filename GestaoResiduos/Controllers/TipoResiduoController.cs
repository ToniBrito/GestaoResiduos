using GestaoResiduos.Models;
using GestaoResiduos.ViewModel;
using GestaoResiduos.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GestaoResiduos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoResiduoController : ControllerBase
    {
        private readonly TipoResiduoRepository _repository;

        public TipoResiduoController(TipoResiduoRepository repository)
        {
            _repository = repository;
        }

        // Listar Todos
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TipoResiduoViewModel>>> GetTiposResiduo()
        {
            var tiposResiduo = await _repository.GetAllAsync();
            return tiposResiduo.Select(t => new TipoResiduoViewModel
            {
                Id = t.Id,
                Nome = t.Nome,
                Descricao = t.Descricao,
                CorIdentificacao = t.CorIdentificacao,
                Codigo = t.Codigo,
                Reciclavel = t.Reciclavel,
                TaxaReciclagem = t.TaxaReciclagem,
                MetodoProcessamento = t.MetodoProcessamento
            }).ToList();
        }

        // Busca por Id
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoResiduoViewModel>> GetTipoResiduo(int id)
        {
            var tipoResiduo = await _repository.GetByIdAsync(id);
            if (tipoResiduo == null) return NotFound();

            return new TipoResiduoViewModel
            {
                Id = tipoResiduo.Id,
                Nome = tipoResiduo.Nome,
                Descricao = tipoResiduo.Descricao,
                CorIdentificacao = tipoResiduo.CorIdentificacao,
                Codigo = tipoResiduo.Codigo,
                Reciclavel = tipoResiduo.Reciclavel,
                TaxaReciclagem = tipoResiduo.TaxaReciclagem,
                MetodoProcessamento = tipoResiduo.MetodoProcessamento
            };
        }

        // Listar Recicláveis
        [HttpGet("reciclaveis")]
        public async Task<ActionResult<IEnumerable<TipoResiduoViewModel>>> GetReciclaveis()
        {
            var tiposResiduo = await _repository.GetReciclaveisAsync();

            return tiposResiduo.Select(t => new TipoResiduoViewModel
            {
                Id = t.Id,
                Nome = t.Nome,
                Descricao = t.Descricao,
                CorIdentificacao = t.CorIdentificacao,
                Codigo = t.Codigo,
                Reciclavel = t.Reciclavel,
                TaxaReciclagem = t.TaxaReciclagem,
                MetodoProcessamento = t.MetodoProcessamento
            }).ToList();
        }

        // Cadastrar Novo
        [HttpPost]
        public async Task<ActionResult<TipoResiduoViewModel>> PostTipoResiduo(TipoResiduoCreateViewModel viewModel)
        {
            var tipoResiduo = new TipoResiduo
            {
                Nome = viewModel.Nome,
                Descricao = viewModel.Descricao,
                CorIdentificacao = viewModel.CorIdentificacao,
                Codigo = viewModel.Codigo,
                Reciclavel = viewModel.Reciclavel,
                TaxaReciclagem = viewModel.TaxaReciclagem,
                MetodoProcessamento = viewModel.MetodoProcessamento
            };

            await _repository.AddAsync(tipoResiduo);

            var result = new TipoResiduoViewModel
            {
                Id = tipoResiduo.Id,
                Nome = tipoResiduo.Nome,
                Descricao = tipoResiduo.Descricao,
                CorIdentificacao = tipoResiduo.CorIdentificacao,
                Codigo = tipoResiduo.Codigo,
                Reciclavel = tipoResiduo.Reciclavel,
                TaxaReciclagem = tipoResiduo.TaxaReciclagem,
                MetodoProcessamento = tipoResiduo.MetodoProcessamento
            };

            return CreatedAtAction("GetTipoResiduo", new { id = tipoResiduo.Id }, result);
        }

        // Atualizar
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoResiduo(int id, TipoResiduoUpdateViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            var tipoResiduo = await _repository.GetByIdAsync(id);
            if (tipoResiduo == null) return NotFound();

            tipoResiduo.Nome = viewModel.Nome;
            tipoResiduo.Descricao = viewModel.Descricao;
            tipoResiduo.CorIdentificacao = viewModel.CorIdentificacao;
            tipoResiduo.Codigo = viewModel.Codigo;
            tipoResiduo.Reciclavel = viewModel.Reciclavel;
            tipoResiduo.TaxaReciclagem = viewModel.TaxaReciclagem;
            tipoResiduo.MetodoProcessamento = viewModel.MetodoProcessamento;

            await _repository.UpdateAsync(tipoResiduo);

            return NoContent();
        }

        // Remover
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoResiduo(int id)
        {
            var tipoResiduo = await _repository.GetByIdAsync(id);
            if (tipoResiduo == null) return NotFound();

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}