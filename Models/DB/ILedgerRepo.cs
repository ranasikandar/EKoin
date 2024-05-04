using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DB
{
    public interface ILedgerRepo
    {
        Task<Int64> GetMaxTID();
        Task<List<Ledger>> GetLedger(Int64 fromTID, Int32 take);
        Task<Ledger> AddUpdateLedger(Ledger LedgerChanges);
        Task<bool> DeleteLedger(List<Ledger> ledger);
    }
}
