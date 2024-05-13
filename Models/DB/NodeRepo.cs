using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DB
{
    public class NodeRepo: INodeRepo
    {
        #region CTOR
        private readonly AppDbContext context;
        public NodeRepo(AppDbContext _context)
        {
            context = _context;
        }
        #endregion


        public async Task<List<NetworkNode>> GetNetworkNodes(int Id_Local, bool? isTP = null)
        {
            if (Id_Local > 0)
            {
                if (isTP == null)
                {
                    return await context.NetworkNodes.Where(x => x.Id_Local == Id_Local).ToListAsync();
                }
                else
                {
                    return await context.NetworkNodes.Where(x => x.Id_Local == Id_Local && x.IsTP == isTP).ToListAsync();
                }
            }
            else
            {
                if (isTP == null)
                {
                    //Random random = new Random();
                    //int skipper = random.Next(0, context.NetworkNodes.Count());
                    //return await context.NetworkNodes.OrderBy(x=> x.Pubkx).Skip(skipper).Take(3).ToListAsync();

                    ///ef core 6 required
                    //return await context.NetworkNodes.OrderBy(x => EF.Functions.Random()).Take(3).ToListAsync();

                    return await context.NetworkNodes.OrderBy(x => Guid.NewGuid()).Take(3).ToListAsync();
                }
                else
                {
                    return await context.NetworkNodes.Where(x => x.IsTP == isTP).OrderBy(x => Guid.NewGuid()).Take(3).ToListAsync();
                }
            }
        }

        public async Task<List<NetworkNode>> GetNetworkNodes(string pubkx, string ip)
        {
            if (!string.IsNullOrEmpty(pubkx) && !string.IsNullOrEmpty(ip))
            {
                return await context.NetworkNodes.Where(x => x.Pubkx == pubkx || x.IP == ip).ToListAsync();
            }
            else
            {
                if (!string.IsNullOrEmpty(pubkx))
                {
                    return await context.NetworkNodes.Where(x => x.Pubkx == pubkx).ToListAsync();
                }
                if (!string.IsNullOrEmpty(ip))
                {
                    return await context.NetworkNodes.Where(x => x.IP == ip).ToListAsync();
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<NetworkNode> AddUpdateNetworkNode(NetworkNode NetworkNodeChanges)
        {
            //chk wather update or add new
            if (NetworkNodeChanges.Id_Local > 0)
            {
                var entity = context.NetworkNodes.Attach(NetworkNodeChanges);
                entity.State = EntityState.Modified;
            }
            else
            {
                context.NetworkNodes.Add(NetworkNodeChanges);
            }

            await context.SaveChangesAsync();
            return NetworkNodeChanges;

        }

        public async Task<bool> DeleteNetworkNode(int Id_Local)
        {
            try
            {
                if (Id_Local < 1)
                {
                    return false;
                }

                List<NetworkNode> networkNodes = await GetNetworkNodes(Id_Local);

                if (networkNodes.Any())
                {
                    context.NetworkNodes.RemoveRange(networkNodes);
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

        public async Task<bool> DeleteNetworkNode(List<NetworkNode> networkNodes)
        {
            try
            {
                if (networkNodes != null && networkNodes.Any())
                {
                    context.NetworkNodes.RemoveRange(networkNodes);
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
