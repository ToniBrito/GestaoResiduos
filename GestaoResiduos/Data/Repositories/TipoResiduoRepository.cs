using GestaoResiduos.Models;
using GestaoResiduos.Repositories;
using GestaoResiduos.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoResiduos.Data.Repositories
{
    public class TipoResiduoRepository : IRepository<TipoResiduo>
    {
        private readonly DatabaseContext _context;

        public TipoResiduoRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoResiduo>> GetAllAsync()
        {
            return await _context.TipoResiduos.ToListAsync();
        }

        public async Task<TipoResiduo> GetByIdAsync(int id)
        {
            return await _context.TipoResiduos.FindAsync(id);
        }

        public async Task<IEnumerable<TipoResiduo>> GetReciclaveisAsync()
        {
            return await _context.TipoResiduos
                .Where(t => t.Reciclavel == "S")
                .ToListAsync();
        }

        public async Task AddAsync(TipoResiduo tipoResiduo)
        {
            await _context.TipoResiduos.AddAsync(tipoResiduo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TipoResiduo tipoResiduo)
        {
            _context.Entry(tipoResiduo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tipoResiduo = await GetByIdAsync(id);
            if (tipoResiduo != null)
            {
                _context.TipoResiduos.Remove(tipoResiduo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            // Usar CountAsync em vez de AnyAsync para evitar problemas com Oracle
            return await _context.TipoResiduos.CountAsync(e => e.Id == id) > 0;
        }

        public async Task<bool> HasColetasAsync(int id)
        {
            return await _context.Coletas.AnyAsync(c => c.TipoResiduoId == id);
        }
    }
}