using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VModels.Wallet
{
    public class Mnemonic12Words
    {
        [Required]
        [DataType(DataType.Text)]
        public string mnemonic12Words { get; set; }
    }
}
