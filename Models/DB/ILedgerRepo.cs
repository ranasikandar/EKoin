using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DB
{
    public interface ILedgerRepo
    {
        Task<ulong> GetMaxTID();
        Task<List<Ledger>> GetLedger(ulong fromTID, Int32 take=500);
        Task<Ledger> AddLedger(Ledger LedgerChanges);
        Task<Ledger> UpdateLedger(Ledger LedgerChanges);
        Task<bool> DeleteLedger(List<Ledger> ledger);
    }
}
