using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DB
{
    public interface IBalanceRepo
    {
        Task<decimal> GetBalance(string address);
        Task<List<Balance>> GetBalances(string address);
        Task<Balance> GetBalanceEntity(string address);
        Task<Balance> AddUpdateBalance(Balance balance);
        Task<bool> DeleteBalance(Balance balance);
        Task<bool> DeleteBalance(string address);

    }
}
