using GestaoResiduos.Models;
using GestaoResiduos.Repositories;
using GestaoResiduos.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoResiduos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _repository;

        public UsuarioController(UsuarioRepository repository)
        {
            _repository = repository;
        }

        // Listar Usuarios
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UsuarioViewModel>>> GetUsuarios()
        {
            var usuarios = await _repository.GetAllAsync();

            return usuarios.Select(u => new UsuarioViewModel
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Tipo = u.Tipo,
                Endereco = u.Endereco,
                Telefone = u.Telefone,
                DataCadastro = u.DataCadastro
            }).ToList();
        }

        // Listar usuários por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioViewModel>> GetUsuario(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return new UsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Tipo = usuario.Tipo,
                Endereco = usuario.Endereco,
                Telefone = usuario.Telefone,
                DataCadastro = usuario.DataCadastro
            };
        }

        // Listar usuários por tipo
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<UsuarioViewModel>>> GetUsuariosPorTipo(string tipo)
        {
            var usuarios = await _repository.GetByTipoAsync(tipo);

            return usuarios.Select(u => new UsuarioViewModel
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Tipo = u.Tipo,
                Endereco = u.Endereco,
                Telefone = u.Telefone,
                DataCadastro = u.DataCadastro
            }).ToList();
        }

        // Cadastrar Usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioViewModel>> PostUsuario(UsuarioCreateViewModel viewModel)
        {
            var usuario = new Usuario
            {
                Nome = viewModel.Nome,
                Email = viewModel.Email,
                Tipo = viewModel.Tipo,
                Endereco = viewModel.Endereco,
                Telefone = viewModel.Telefone,
                DataCadastro = DateTime.Now
            };

            await _repository.AddAsync(usuario);

            var result = new UsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Tipo = usuario.Tipo,
                Endereco = usuario.Endereco,
                Telefone = usuario.Telefone,
                DataCadastro = usuario.DataCadastro
            };

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, result);
        }

        // Atualizar Usuário
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return BadRequest();
            }

            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            usuario.Nome = viewModel.Nome;
            usuario.Email = viewModel.Email;
            usuario.Tipo = viewModel.Tipo;
            usuario.Endereco = viewModel.Endereco;
            usuario.Telefone = viewModel.Telefone;

            await _repository.UpdateAsync(usuario);

            return NoContent();
        }

        // Deletar Usuario
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
