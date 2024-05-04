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

        public async Task<Int64> GetMaxTID()
        {
            return await context.Ledger.MaxAsync(x => x.TID);
        }

        public async Task<List<Ledger>> GetLedger(Int64 fromTID,Int32 take)
        {
            return await context.Ledger.Where(x => x.TID>=fromTID).OrderBy(x => x.TID).Take(take).ToListAsync();
        }

        public async Task<Ledger> AddUpdateLedger(Ledger LedgerChanges)
        {
            //chk wather update or add new
            if (LedgerChanges.TID > 0)
            {
                var couple = context.Ledger.Attach(LedgerChanges);
                couple.State = EntityState.Modified;
            }
            else
            {
                context.Ledger.Add(LedgerChanges);
            }

            await context.SaveChangesAsync();
            return LedgerChanges;

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
