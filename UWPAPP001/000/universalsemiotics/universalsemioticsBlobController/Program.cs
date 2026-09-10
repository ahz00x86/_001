using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace universalsemioticsBlobController
{
    class Program
    {
        static void Main(string[] args)
        {
            removeBlobsFreeConversion();
            removeBlobsProConversion();
        }

        private static void removeBlobsFreeConversion()
        {
            Console.WriteLine("BORRANDO FREE INI");

            List<Mp3Free> mp3s = new List<Mp3Free>();
            DateTime dt16 = DateTime.Now.AddDays(-3);
            using (universalsemioticsEntities entities = new universalsemioticsEntities())
            {
                mp3s = entities.Mp3Free.Where(x => (!x.estado.HasValue || x.estado.Value) && x.fechaIni < dt16).ToList();
                Console.WriteLine(mp3s.Count.ToString());

                CloudBlobClient blobClient;
                CloudBlobContainer blobContainerMp3;
                CloudBlobContainer blobContainerTxt;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=universalsemiotics;AccountKey=V/6Vo6+nJKM5/xQDCqABNrmu9c0jvwG/a2GpHyrTLQvNNFyXjzzB95ObwNdCnVYQjJkZaZJcCgYY4ZZ5v5ozDA==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainerMp3 = blobClient.GetContainerReference("mp3free");
                blobContainerTxt = blobClient.GetContainerReference("docsfree");
                foreach (Mp3Free mp3 in mp3s)
                {
                    CloudBlockBlob blobMp3 = blobContainerMp3.GetBlockBlobReference(mp3.Id.ToString() + ".mp3");
                    CloudBlockBlob blobTxt = blobContainerTxt.GetBlockBlobReference(mp3.Id.ToString() + ".us");
                    try
                    {
                        blobMp3.DeleteIfExists();
                        blobTxt.DeleteIfExists();
                    }
                    catch (Exception yaBorrado)
                    {
                        Console.WriteLine(yaBorrado.ToString());
                    }
                    mp3.estado = false;
                }
                entities.SaveChanges();
                Console.WriteLine("BORRANDO FREE FIN");
            }
        }

        private static void removeBlobsProConversion()
        {
            Console.WriteLine("BORRANDO PRO INI");

            List<Mp3Pro> mp3s = new List<Mp3Pro>();
            DateTime dt16 = DateTime.Now.AddDays(-8);
            using (universalsemioticsEntities entities = new universalsemioticsEntities())
            {
                mp3s = entities.Mp3Pro.Where(x => (!x.estado.HasValue || x.estado.Value) && x.fechaIni < dt16).ToList();
                Console.WriteLine(mp3s.Count.ToString());

                CloudBlobClient blobClient;
                CloudBlobContainer blobContainerMp3;
                CloudBlobContainer blobContainerTxt;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=universalsemiotics;AccountKey=V/6Vo6+nJKM5/xQDCqABNrmu9c0jvwG/a2GpHyrTLQvNNFyXjzzB95ObwNdCnVYQjJkZaZJcCgYY4ZZ5v5ozDA==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainerMp3 = blobClient.GetContainerReference("mp3pro");
                blobContainerTxt = blobClient.GetContainerReference("docspro");
                foreach (Mp3Pro mp3 in mp3s)
                {
                    CloudBlockBlob blobMp3 = blobContainerMp3.GetBlockBlobReference(mp3.Id.ToString() + ".mp3");
                    CloudBlockBlob blobTxt = blobContainerTxt.GetBlockBlobReference(mp3.Id.ToString() + ".us");
                    try
                    {
                        blobMp3.DeleteIfExists();
                        blobTxt.DeleteIfExists();
                    }
                    catch (Exception yaBorrado)
                    {
                        Console.WriteLine(yaBorrado.ToString());
                    }
                    mp3.estado = false;
                }
                entities.SaveChanges();

                Console.WriteLine("BORRANDO PRO FIN");
            }
        }

    }
}
