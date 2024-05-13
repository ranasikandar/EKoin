using Models.VModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Network
{
    public class SubmitTransaction:SendCoin
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(66)]
        public string SenderPubkx { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TransactionInitTime { get; set; }//include data in sign

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(96)]
        public string DerSign { get; set; }

    }
}
