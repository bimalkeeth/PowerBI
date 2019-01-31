using System.IO;
using System.Net;
using System.Net.Http;

namespace PowerBIService.Helpers
{
    public static class PowerBIHelper
    {
        
        public static string StreamToString(this Stream stream)
        {
            
            using (StreamReader reader = new StreamReader(stream))
            {
                string text = reader.ReadToEnd();
                return text;
            }
        }
        public static string ResponseToString(this HttpWebResponse webResponse)
        {
            using (Stream data = webResponse.GetResponseStream())
            {
                return data.StreamToString();
            }
        }
        public static string ResponseToString(this HttpResponseMessage webResponse)
        {
            using (Stream data = webResponse.Content.ReadAsStreamAsync().Result)
            {
                return data.StreamToString();
            }
        }
        
        public static long CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }

            return input.Length;
        }
        
    }
}