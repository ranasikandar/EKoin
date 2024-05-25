using Models.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EKoin.Utility
{
    public interface IHttpRequest
    {
        public Task<string> SubmitTransaction(SubmitTransaction submitTransaction, string hostAddress);
        Task<string> PostData(StringContent content, string hostAddress);
        Task<string> PostData<T>(T t, string hostAddress);
    }
}
