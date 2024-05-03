using NBitcoin;
using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Wallet;

namespace Library
{
    public class Wallet:ILibraryWallet
    {
        public Key_Pair GenPubPk()
        {
            try
            {
                Key_Pair key_Pair = new Key_Pair();

                // Generate keys from mnemonic
                Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
                ExtKey extendedKey = mnemonic.DeriveExtKey();
                Key privateKey = extendedKey.PrivateKey;
                PubKey publicKey = privateKey.PubKey;

                // Generate address
                BitcoinAddress address = publicKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);

                // Saving and loading keys
                string privateKeyHex = privateKey.ToHex();
                string publicKeyHex = publicKey.ToHex();
                string addressString = address.ToString();

                key_Pair.Private_Key = privateKey;
                key_Pair.Public_Key = publicKey;
                key_Pair.Btc_Address = address;
                key_Pair.PrivateKey_Hex = privateKeyHex;
                key_Pair.PublicKey_Hex = publicKeyHex;
                key_Pair.Address_String = addressString;
                key_Pair.Mnemonic_12_Words = mnemonic.ToString();

                return key_Pair;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Key_Pair GenPubPkFromMnemonic(string mnemonic)
        {
            try
            {
                Key_Pair key_Pair = new Key_Pair();

                // Generate keys from mnemonic
                Mnemonic _mnemonic = new Mnemonic(mnemonic, Wordlist.English);
                ExtKey extendedKey = _mnemonic.DeriveExtKey();
                Key privateKey = extendedKey.PrivateKey;
                PubKey publicKey = privateKey.PubKey;

                // Generate address
                //BitcoinAddress address = publicKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main);
                BitcoinAddress address = publicKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);

                // Saving and loading keys
                string privateKeyHex = privateKey.ToHex();
                string publicKeyHex = publicKey.ToHex();
                string addressString = address.ToString();

                key_Pair.Private_Key = privateKey;
                key_Pair.Public_Key = publicKey;
                key_Pair.Btc_Address = address;
                key_Pair.PrivateKey_Hex = privateKeyHex;
                key_Pair.PublicKey_Hex = publicKeyHex;
                key_Pair.Address_String = addressString;
                key_Pair.Mnemonic_12_Words = _mnemonic.ToString();

                return key_Pair;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Signature_Data_Hash SignData(bool isBase58OtherwiseHEX, string PK, string @data)
        {
            string base58PrivateKey = string.Empty;

            try
            {
                if (!isBase58OtherwiseHEX)
                {
                    // Parse the hexadecimal string into a byte array
                    byte[] privateKeyBytes = NBitcoin.DataEncoders.Encoders.Hex.DecodeData(PK);

                    // Create a key object from the byte array
                    Key privateKey = new Key(privateKeyBytes);

                    // Convert the key to a base58-encoded string
                    base58PrivateKey = privateKey.GetWif(Network.Main).ToString();

                }

                // Convert the message to a byte array
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(@data);

                // Compute the hash of the message
                uint256 messageHash = Hashes.DoubleSHA256(messageBytes);
                string _key = (isBase58OtherwiseHEX) ? PK : base58PrivateKey;
                BitcoinSecret bitcoinSecret = new BitcoinSecret(_key, Network.Main);
                ECDSASignature signature = bitcoinSecret.PrivateKey.Sign(messageHash);

                Signature_Data_Hash signature_Hash = new Signature_Data_Hash
                {
                    D_Hash = messageHash.ToBytes(true),
                    DerSign = signature.ToDER(),
                    Data = data
                };

                return signature_Hash;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Signature_Data_Hash SignData(bool isBase58OtherwiseHEX, string key, byte[] data)
        {
            string base58PrivateKey = string.Empty;

            try
            {
                if (!isBase58OtherwiseHEX)
                {
                    // Parse the hexadecimal string into a byte array
                    byte[] privateKeyBytes = NBitcoin.DataEncoders.Encoders.Hex.DecodeData(key);

                    // Create a key object from the byte array
                    Key privateKey = new Key(privateKeyBytes);

                    // Convert the key to a base58-encoded string
                    base58PrivateKey = privateKey.GetWif(Network.Main).ToString();

                }

                // Compute the hash of the message
                uint256 messageHash = Hashes.DoubleSHA256(data);
                string _key = (isBase58OtherwiseHEX) ? key : base58PrivateKey;
                BitcoinSecret bitcoinSecret = new BitcoinSecret(_key, Network.Main);
                ECDSASignature signature = bitcoinSecret.PrivateKey.Sign(messageHash);

                Signature_Data_Hash signature_Hash = new Signature_Data_Hash
                {
                    D_Hash = messageHash.ToBytes(true),
                    DerSign = signature.ToDER(),
                    Data = System.Text.Encoding.UTF8.GetString(data)
                };

                return signature_Hash;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Signature_Data_Hash SignData(Key key, string @data)
        {
            try
            {
                // Convert the message to a byte array
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(@data);

                // Compute the hash of the message
                uint256 messageHash = Hashes.DoubleSHA256(messageBytes);

                BitcoinSecret bitcoinSecret = new BitcoinSecret(key, Network.Main);
                ECDSASignature signature = bitcoinSecret.PrivateKey.Sign(messageHash);

                Signature_Data_Hash signature_Hash = new Signature_Data_Hash
                {
                    D_Hash = messageHash.ToBytes(),
                    DerSign = signature.ToDER(),
                    Data = data
                };

                return signature_Hash;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Signature_Data_Hash SignData(Key key, byte[] data)
        {
            try
            {
                // Compute the hash of the message
                uint256 messageHash = Hashes.DoubleSHA256(data);

                BitcoinSecret bitcoinSecret = new BitcoinSecret(key, Network.Main);
                ECDSASignature signature = bitcoinSecret.PrivateKey.Sign(messageHash);

                Signature_Data_Hash signature_Hash = new Signature_Data_Hash
                {
                    D_Hash = messageHash.ToBytes(),
                    DerSign = signature.ToDER(),
                    Data = System.Text.Encoding.UTF8.GetString(data)
                };

                return signature_Hash;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool VerifyData(PubKey pubKey, uint256 messageHash, ECDSASignature signature)
        {
            try
            {
                //verify the signature to ensure it's correct
                return pubKey.Verify(messageHash, signature);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public uint256 GenSHA256Hash(string @data)
        {
            try
            {
                // Convert the message to a byte array
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(@data);

                // Compute the hash of the message
                uint256 messageHash = Hashes.DoubleSHA256(messageBytes);

                return messageHash;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
