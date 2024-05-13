using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Reciver { get; set; }//include data in sign

        [Required]
        [DataType(DataType.Currency)]
        [Range(typeof(Decimal), "0.00000001", "99999999")]
        [Column(TypeName = "decimal(16, 8)")]
        public decimal Amount { get; set; }//include data in sign

        [DataType(DataType.Text)]
        [MaxLength(64)]
        public string Memo { get; set; }//include data in sign

    }
}
