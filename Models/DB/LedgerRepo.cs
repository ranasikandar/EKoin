using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DB
{
    public class LedgerRepo: ILedgerRepo
    {
        private readonly AppDbContext context;
        public LedgerRepo(AppDbContext _context)
        {
            context = _context;
        }

        public async Task<ulong> GetMaxTID()
        {
            //var maxValue = await context.Ledger
            //              .DefaultIfEmpty(new Ledger { TID = default }) // Set a default value for your entity type
            //              .Select(Ledger => Ledger.TID)
            //              .MaxAsync();

            try
            {
                return await context.Ledger.DefaultIfEmpty().MaxAsync(x => x.TID);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<Ledger>> GetLedger(ulong fromTID,Int32 take)
        {
            return await context.Ledger.Where(x => x.TID>=fromTID).OrderBy(x => x.TID).Take(take).ToListAsync();
        }

        public async Task<Ledger> UpdateLedger(Ledger LedgerChanges)
        {
            var entity = context.Ledger.Attach(LedgerChanges);
            entity.State = EntityState.Modified;

            await context.SaveChangesAsync();
            return LedgerChanges;

        }

        public async Task<Ledger> AddLedger(Ledger ledger)
        {
            context.Ledger.Add(ledger);

            await context.SaveChangesAsync();
            return ledger;

        }

        public async Task<bool> DeleteLedger(List<Ledger> ledger)
        {
            try
            {
                if (ledger != null && ledger.Any())
                {
                    context.Ledger.RemoveRange(ledger);
                    int effectedRows = await context.SaveChangesAsync();

                    return (effectedRows > 0);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
