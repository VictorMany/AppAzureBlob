using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AppAzureBlob.Helpers
{
        public class Converters
        {
            public async static Task<byte[]> FileToByteArray(string filePath)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    FileStream stream = File.Open(filePath, FileMode.Open);
                    byte[] bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    return bytes;
                }
                else
                {
                    return null;
                }
            }
        }
    
}
