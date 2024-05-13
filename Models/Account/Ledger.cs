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
        [Key]
        [Required]
        [Column(TypeName = "bigint")]
        public ulong TID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(32)]
        [Column(TypeName = "varchar(32)")]
        //[Display(Name = "MD5 Last Hash")]
        public string LHash { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(34)]
        [Column(TypeName = "varchar(34)")]
        public string Sender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(34)]
        [Column(TypeName = "varchar(34)")]
        public string Reciver { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }//0.00000001-99999999

        [DataType(DataType.Text)]
        [MaxLength(64)]
        [Column(TypeName = "varchar(64)")]
        public string Memo { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime TransactionTime { get; set; }

        [Required]
        [Column(TypeName = "varbinary(70)")] //bytes
        public byte[] Signature { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(32)]
        [Column(TypeName = "varchar(32)")]
        //[Display(Name = "MD5 Hash")]
        public string Hash { get; set; }
    }

}
