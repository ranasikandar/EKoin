using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKoin.Utility
{
    public class Misc
    {
        private readonly IConfiguration configuration;

        public Misc(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        
    }
}
