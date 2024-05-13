using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DB
{
    public class BalanceRepo:IBalanceRepo
    {
        #region ctor
        private readonly AppDbContext context;
        public BalanceRepo(AppDbContext _context)
        {
            context = _context;
        }
        #endregion

        public async Task<decimal> GetBalance(string address)
        {
            return await context.Balances.Where(x => x.Address == address).Select(x => x.Amount).FirstOrDefaultAsync();
        }

        public async Task<List<Balance>> GetBalances(string address)
        {
            return await context.Balances.Where(x => x.Address == address).ToListAsync();
        }

        public async Task<Balance> GetBalanceEntity(string address)
        {
            return await context.Balances.Where(x => x.Address == address).FirstOrDefaultAsync();
        }

        public async Task<Balance> AddUpdateBalance(Balance balance)
        {
            //chk wather update or add new
            if (balance.Id_Local > 0)
            {
                var entity = context.Balances.Attach(balance);
                entity.State = EntityState.Modified;
            }
            else
            {
                context.Balances.Add(balance);
            }

            await context.SaveChangesAsync();
            return balance;

        }

        public async Task<bool> DeleteBalance(Balance balance)
        {
            try
            {
                if (balance!=null)
                {
                    context.Balances.Remove(balance);
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

        public async Task<bool> DeleteBalance(string address)
        {
            try
            {
                List<Balance> balances = await GetBalances(address);

                if (balances.Any())
                {
                    context.Balances.RemoveRange(balances);
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
