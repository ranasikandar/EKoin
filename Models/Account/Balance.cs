using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Balance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Local { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(34)]
        [Column(TypeName = "varchar(34)")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(16, 8)")]
        public decimal Amount { get; set; }
    }
}
