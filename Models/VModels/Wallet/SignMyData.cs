﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VModels.Wallet
{
    public class SignMyData
    {
        [Required]
        [DataType(DataType.Text)]
        public string data { get; set; }
    }
}
