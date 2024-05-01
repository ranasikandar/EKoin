using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DB
{
    public interface INodeRepo
    {
        Task<List<NetworkNode>> GetNetworkNodes(int Id_Local, bool? tp = null);
        Task<List<NetworkNode>> GetNetworkNodes(string pubkx, string ip);
        Task<NetworkNode> AddUpdateNetworkNode(NetworkNode NetworkNodeChanges);
        Task<bool> DeleteNetworkNode(int Id_Local);
        Task<bool> DeleteNetworkNode(List<NetworkNode> network_Nodes);
    }
}
