using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Wallet
    {
        public class Key_Pair
        {
            public Key Private_Key { get; set; }
            public PubKey Public_Key { get; set; }
            public BitcoinAddress Btc_Address { get; set; }
            public string PrivateKey_Hex { get; set; }
            public string PublicKey_Hex { get; set; }
            public string Address_String { get; set; }
            public string Mnemonic_12_Words { get; set; }
        }

        public class Signature_Data_Hash
        {
            public string Data { get; set; }
            public byte[] D_Hash { get; set; }
            public byte[] DerSign { get; set; }

        }
    }
}
