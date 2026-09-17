using GestaoResiduos.Models;
using GestaoResiduos.Data.Repositories;
using GestaoResiduos.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoResiduos.Repositories
{
    public class UsuarioRepository : IRepository<Usuario>
    {
        private readonly DatabaseContext _context;

        public UsuarioRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<IEnumerable<Usuario>> GetByTipoAsync(string tipo)
        {
            return await _context.Usuarios
                .Where(u => u.Tipo == tipo)
                .ToListAsync();
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await GetByIdAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> HasColetasAsync(int id)
        {
            return await _context.Coletas.AnyAsync(c => c.UsuarioId == id);
        }
    }
}
