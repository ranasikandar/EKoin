using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VModels.Network
{
    public class RememberMe
    {
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(66)]
        public string pubkx { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(96)]
        public string derSign { get; set; }

        public bool isTP { get; set; }
    }
}
