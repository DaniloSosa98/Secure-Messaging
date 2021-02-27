using System.IO;
using Newtonsoft.Json;

namespace Messenger
{
    /// <summary>
    /// Class for the format of the public key
    /// </summary>
    public class PubK
    {
        public string email{ get; set; }
        public string key{ get; set; }
        
        //This method saves the public key as a .key file
        public void saveK() {
            string path = @"./public.key";
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (StreamWriter streamWriter = File.CreateText(path)) {
                streamWriter.Write(json);
            }
        }
    }
}