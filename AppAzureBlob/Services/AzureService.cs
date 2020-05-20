using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAzureBlob.Services
{
    public enum AzureContainer
    {
        Image,
        Text
    }

    public class AzureService
    {
        public static CloudBlobContainer GetContainer(AzureContainer containerType)
        {
            var account = CloudStorageAccount.Parse(Config.Constants.StorageConection);
            //Tener el vinvulo
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerType.ToString().ToLower());
        }

        //Nos lista todos los archivos de un contenedor
        public static async Task<IList<string>> GetFilesListAsync(AzureContainer containerType)
        {
            //Obtiene todos los archivos blob del contenedor especifico  en un containerType
            var container = GetContainer(containerType);
            var list = new List<string>();
            BlobContinuationToken token = null;
            do
            {
                var result = await container.ListBlobsSegmentedAsync(token);
                if(result.Results.Count() > 0)
                {
                    var blobs = result.Results.Cast<CloudBlockBlob>().Select(b => b.Name);
                    list.AddRange(blobs);
                }
                token = result.ContinuationToken;
            } while (token != null);
            return list;
        }


        public static async Task<byte[]> GetFileAsync(AzureContainer containerType, string name)
        {
            //Descarga el archivo con el nombre name
            var container = GetContainer(containerType);
            var blob = container.GetBlobReference(name);
            if(await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                byte[] blobBytes = new byte[blob.Properties.Length];
                await blob.DownloadToByteArrayAsync(blobBytes, 0);
                return blobBytes;
            }
            return null;
        }

        public static async Task<string> UploadFileAsync(AzureContainer containerType, Stream stream)
        {
            var container = GetContainer(containerType);
            await container.CreateIfNotExistsAsync();

            //Identificador unico (UUID)
            var name = Guid.NewGuid().ToString();
            var fileBlob = container.GetBlockBlobReference(name);
            await fileBlob.UploadFromStreamAsync(stream);

            return name;
        }

        public static async Task<bool> DeleteFyleAsync(AzureContainer containerType, string name)
        {
            //Borra un archivo blob "name" de un contenedor 
            var container = GetContainer(containerType);
            var blob = container.GetBlockBlobReference(name);
            return await blob.DeleteIfExistsAsync();
        }

        public static async Task<bool> DeleteContainerAsync(AzureContainer containerType)
        {
            //Borra un contenedor 
            var container = GetContainer(containerType);
            return await container.DeleteIfExistsAsync();
        }
    }

}

