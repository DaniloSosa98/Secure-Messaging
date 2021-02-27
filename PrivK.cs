using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Messenger
{
    /// <summary>
    /// Class for the format of the private key
    /// </summary>
    public class PrivK
    {
        public List<string> email{ get; set; }
        public string key{ get; set; }
        
        //This method saves the private key as a .key file
        public void saveK() {
            string path = @"./private.key";
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (StreamWriter streamWriter = File.CreateText(path)) {
                streamWriter.Write(json);
            }
        }
    }
}