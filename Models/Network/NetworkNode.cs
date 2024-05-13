using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NetworkNode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Local { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(66)]
        [Column(TypeName = "varchar(66)")]
        public string Pubkx { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(15)]//127.000.000.100
        [Column(TypeName = "varchar(15)")]
        public string IP { get; set; }

        [Required]
        //[MaxLength(5)]//65535
        public int Port { get; set; }


        [Required]
        public bool IsTP { get; set; }
    }
}
