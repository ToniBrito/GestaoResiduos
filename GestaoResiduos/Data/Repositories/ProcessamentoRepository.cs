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
    public class ProcessamentoRepository : IRepository<Processamento>
    {
        private readonly DatabaseContext _context;

        public ProcessamentoRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Processamento>> GetAllAsync()
        {
            return await _context.Processamentos
                .Include(p => p.Coleta)
                .Include(p => p.Usuario)
                .Include(p => p.TipoResiduo)
                .ToListAsync();
        }

        public async Task<Processamento> GetByIdAsync(int id)
        {
            return await _context.Processamentos
                .Include(p => p.Coleta)
                .Include(p => p.Usuario)
                .Include(p => p.TipoResiduo)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Processamento> GetByColetaIdAsync(int coletaId)
        {
            return await _context.Processamentos
                .Include(p => p.Coleta)
                .Include(p => p.Usuario)
                .Include(p => p.TipoResiduo)
                .FirstOrDefaultAsync(p => p.ColetaId == coletaId);
        }

        public async Task<IEnumerable<Processamento>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Processamentos
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Coleta)
                .Include(p => p.TipoResiduo)
                .ToListAsync();
        }

        public async Task AddAsync(Processamento processamento)
        {
            var processamentoToAdd = new Processamento
            {
                ColetaId = processamento.ColetaId,
                UsuarioId = processamento.UsuarioId,
                TipoResiduoId = processamento.TipoResiduoId,
                DataProcessamento = processamento.DataProcessamento,
                Metodo = processamento.Metodo,
                Eficiencia = processamento.Eficiencia,
                QuantidadeProcessada = processamento.QuantidadeProcessada,
                QuantidadeProduzida = processamento.QuantidadeProduzida,
                Status = processamento.Status,
                Resultado = processamento.Resultado
            };

            await _context.Processamentos.AddAsync(processamentoToAdd);
            await _context.SaveChangesAsync();

            // Atualizar o objeto original com o ID gerado
            processamento.Id = processamentoToAdd.Id;
        }

        public async Task UpdateAsync(Processamento processamento)
        {
            _context.Entry(processamento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var processamento = await GetByIdAsync(id);
            if (processamento != null)
            {
                _context.Processamentos.Remove(processamento);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            // Usar CountAsync em vez de AnyAsync para evitar problemas com Oracle
            return await _context.Processamentos.CountAsync(e => e.Id == id) > 0;
        }

        public async Task<bool> ColetaHasProcessamentoAsync(int coletaId)
        {
            // Usar CountAsync em vez de AnyAsync para evitar problemas com Oracle
            return await _context.Processamentos.CountAsync(p => p.ColetaId == coletaId) > 0;
        }

        // Métodos específicos para estatísticas
        public async Task<int> GetTotalProcessamentosAsync()
        {
            return await _context.Processamentos.CountAsync();
        }

        public async Task<decimal> GetMediaEficienciaAsync()
        {
            return await _context.Processamentos.AverageAsync(p => p.Eficiencia);
        }

        public async Task<decimal> GetTotalProcessadoAsync()
        {
            return await _context.Processamentos.SumAsync(p => p.QuantidadeProcessada);
        }

        public async Task<decimal> GetTotalProduzidoAsync()
        {
            return await _context.Processamentos.SumAsync(p => p.QuantidadeProduzida);
        }
    }
}

