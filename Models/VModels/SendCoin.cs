using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VModels
{
    public class SendCoin
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(34)]
        public string Reciver { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(64)]
        public string Memo { get; set; }

    }
}
