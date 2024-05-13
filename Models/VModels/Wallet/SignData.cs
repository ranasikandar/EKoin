using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VModels.Wallet
{
    public class SignData
    {
        public bool isBase58OtherwiseHEX { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(64)]
        public string privateKey { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string data { get; set; }
    }
}
