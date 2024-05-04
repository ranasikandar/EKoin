using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Ledger
    {
        //[Key]
        [Required]
        public Int64 TID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(32)]
        //[Display(Name = "MD5 Last Hash")]
        public string LHash { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(32)]
        public string Sender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(32)]
        public string Reciver { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(64)]
        public string Memo { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TransactionTime { get; set; }

        [Required]
        [Column(TypeName = "varbinary(4096)")] //4096=4KB //max
        public byte[] Signature { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(32)]
        //[Display(Name = "MD5 Hash")]
        public string Hash { get; set; }
    }

}
