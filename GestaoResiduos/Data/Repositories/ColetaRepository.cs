using GestaoResiduos.Models;
using GestaoResiduos.Repositories;
using GestaoResiduos.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;

namespace GestaoResiduos.Data.Repositories
{
    public class ColetaRepository : IRepository<Coleta>
    {
        private readonly DatabaseContext _context;

        public ColetaRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coleta>> GetAllAsync()
        {
            return await _context.Coletas
                .Include(c => c.Usuario)
                .Include(c => c.TipoResiduo)
                .Include(c => c.Processamento)
                .ToListAsync();
        }

        public async Task<Coleta> GetByIdAsync(int id)
        {
            return await _context.Coletas
                .Include(c => c.Usuario)
                .Include(c => c.TipoResiduo)
                .Include(c => c.Processamento)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Coleta>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Coletas
                .Where(c => c.UsuarioId == usuarioId)
                .Include(c => c.TipoResiduo)
                .Include(c => c.Processamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coleta>> GetByStatusAsync(string status)
        {
            return await _context.Coletas
                .Where(c => c.Status == status)
                .Include(c => c.Usuario)
                .Include(c => c.TipoResiduo)
                .Include(c => c.Processamento)
                .ToListAsync();
        }

        public async Task AddAsync(Coleta coleta)
        {
            var coletaToAdd = new Coleta
            {
                UsuarioId = coleta.UsuarioId,
                TipoResiduoId = coleta.TipoResiduoId,
                Quantidade = coleta.Quantidade,
                DataColeta = coleta.DataColeta,
                Local = coleta.Local,
                Status = coleta.Status,
                Observacoes = coleta.Observacoes
            };

            await _context.Coletas.AddAsync(coletaToAdd);
            await _context.SaveChangesAsync();

            // Atualizar o objeto original com o ID gerado
            coleta.Id = coletaToAdd.Id;
        }

        public async Task UpdateAsync(Coleta coleta)
        {
            _context.Entry(coleta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var coleta = await GetByIdAsync(id);
            if (coleta != null)
            {
                _context.Coletas.Remove(coleta);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            // Usar CountAsync em vez de AnyAsync para evitar problemas com Oracle
            return await _context.Coletas.CountAsync(e => e.Id == id) > 0;
        }

        public async Task<bool> HasProcessamentoAsync(int id)
        {
            // Usar CountAsync em vez de AnyAsync para evitar problemas com Oracle
            return await _context.Processamentos.CountAsync(p => p.ColetaId == id) > 0;
        }

        // Métodos específicos para estatísticas
        public async Task<int> GetTotalColetasAsync()
        {
            return await _context.Coletas.CountAsync();
        }

        public async Task<decimal> GetTotalQuantidadeAsync()
        {
            return await _context.Coletas.SumAsync(c => c.Quantidade);
        }

        public async Task<DateTime?> GetDataMaisRecenteAsync()
        {
            return await _context.Coletas.MaxAsync(c => (DateTime?)c.DataColeta);
        }
    }
}