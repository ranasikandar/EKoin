using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VModels
{
    public class AddressBalance
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(34)]
        public string Address { get; set; }
    }
}
