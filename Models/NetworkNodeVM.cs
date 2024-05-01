using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NetworkNodeVM
    {
        public NetworkNodeVM()
        {
            Nodes = new();
        }
        public List<NetNodes> Nodes { get; set; }
        [Required]
        public byte[] DerSign { get; set; }
    }

    public class NetNodes
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(66)]
        public string pubkx { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(15)]//127.000.000.100
        public string ip { get; set; }

        [Required]
        //[MaxLength(5)]//65535
        public int port { get; set; }


        [Required]
        public bool isTP { get; set; }
    }
}
