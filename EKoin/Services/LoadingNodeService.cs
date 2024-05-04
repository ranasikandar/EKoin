using Library;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Models.Wallet;

namespace EKoin.Services
{
    public class LoadingNodeService: IHostedService
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IMySettings mySettings;
        private readonly ILibraryWallet libraryWallet;

        public LoadingNodeService(IMySettings _mySettings, ILibraryWallet _libraryWallet)
        {
            mySettings = _mySettings;
            libraryWallet = _libraryWallet;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                #region CHECK IF WALLET FILE IS NOT PRESENT CREATE NEW 
                if (!File.Exists(Path.Combine(System.AppContext.BaseDirectory, "myWallet.json")))
                {
                    logger.Info("No myWallet.json file, creating new");
                    try
                    {
                        Key_Pair wallet = libraryWallet.GenPubPk();

                        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(@"{'my_pkx':'" + wallet.PrivateKey_Hex
                            + "','my_pubx':'" + wallet.PublicKey_Hex + "','my_addr':'" + wallet.Address_String + "','my_mnem':'" + wallet.Mnemonic_12_Words + "'}");
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(Path.Combine(System.AppContext.BaseDirectory, "myWallet.json"), output);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
                #endregion


                #region setCache

                string mypk = mySettings.GetValue("my_pkx", "myWallet.json");
                mySettings.GetValue("my_pubx", "myWallet.json");
                mySettings.GetValue("my_addr", "myWallet.json");

                // Parse the hexadecimal string into a byte array
                byte[] privateKeyBytes = NBitcoin.DataEncoders.Encoders.Hex.DecodeData(mypk);
                // Create a key object from the byte array
                Key privateKey = new Key(privateKeyBytes);

                mySettings.GetSetCache("myPrivateKey", privateKey);
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Task.FromException(ex);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
