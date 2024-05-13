using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VModels.Wallet
{
    public class VerifyData
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(66)]
        public string pubx { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(32)]
        public string dHash { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(96)]
        public string derSign { get; set; }
    }
}
