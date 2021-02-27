using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Messenger
{
    class Program {
        /// @author Danilo Sosa, CS @ RIT
        /// The main function manages and evaluates the arguments
        /// and also provides help for the user.
        static async Task Main(string[] args) {
            //Help message that shows usage
            if (args.Length== 1 && args[0].Equals("help")) {
                Console.WriteLine("Usage: dotnet run <option> <other arguments>" +
                                  "\n- keyGen keysize" +
                                  "\n- sendKey email" +
                                  "\n- getKey email" +
                                  "\n- sendMsg email plaintext" +
                                  "\n- getMsg email");
                Environment.Exit(1);
            //keyGen option stores the key size and generated the keys    
            }else if (args.Length == 2 && args[0].Equals("keyGen")) {
                var keySize = intParse(args[1]);
                var gk = new KeyGenerator(keySize);
                gk.genKeys();
                Console.WriteLine("Keys generated successfully");
            //sendKey option checks if the keys exist and 
            //proceeds to send the public key
            }else if (args.Length == 2 && args[0].Equals("sendKey")) {
                if (!File.Exists(@"./public.key") || !File.Exists(@"./private.key")) {
                    Console.WriteLine("Error: You need to generate keys before sending");
                    Environment.Exit(1);
                }
                string email = args[1];
                await sendKey(email);
                Console.WriteLine("Key send successful");
            //getKey option stores the email of the key requested    
            }else if (args.Length == 2 && args[0].Equals("getKey")) {
                string email = args[1];
                await getKey(email);
                Console.WriteLine("Key stored successfully");
            }else if (args.Length == 3 && args[0].Equals("sendMsg")) {
                string email = args[1];
                string plaintext = args[2];
            }else if (args.Length == 2 && args[0].Equals("getMsg")) {
                string email = args[1];
            }else {
                Console.WriteLine("Error!" +
                                  "\nUsage: dotnet run <option> <other arguments>" +
                                  "\n- keyGen keysize" +
                                  "\n- sendKey email" +
                                  "\n- getKey email" +
                                  "\n- sendMsg email plaintext" +
                                  "\n- getMsg email");
                Environment.Exit(1);
            }
        }
        /// <summary>
        /// This method sends a request to the server and validates
        /// if the key exists in the server.
        /// If the key doesn't exists it exits.
        /// If it does exists it uses JSON to store it and
        /// save it as a .key file
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task getKey(string email) {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync( "http://kayrun.cs.rit.edu:5000/Key/"+email);
            
            if (string.IsNullOrEmpty(response)) {
                Console.WriteLine("Error: There is no key for that email");
                Environment.Exit(1);
            }
            
            PubK pubK = JsonConvert.DeserializeObject<PubK>(response);
            string path = @"./"+email+".key";
            string json = JsonConvert.SerializeObject(pubK, Formatting.Indented);
            using (StreamWriter streamWriter = File.CreateText(path)) {
                streamWriter.Write(json);
            }
        }
        /// <summary>
        /// This function updates both keys with the email and proceeds
        /// to send the public key to the server.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async Task sendKey(string email) {

            var file = readJSON(@"./public.key");
            PubK pubK = JsonConvert.DeserializeObject<PubK>(file);
            pubK.email = email;
            pubK.saveK();
            file = readJSON(@"./public.key");
            var content = new StringContent (file, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PutAsync("http://kayrun.cs.rit.edu:5000/Key/"+email, content);
            if (!response.IsSuccessStatusCode) {
                Console.WriteLine("There was an error sending the key");
                Environment.Exit(1);
            }
            file = readJSON(@"./private.key");
            PrivK privK = JsonConvert.DeserializeObject<PrivK>(file);
            privK.email.Add(email);
            privK.saveK();
        }
        
        /// <summary>
        /// This short function stores all the .key
        /// file in a string and returns it
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string readJSON(string path) {
            return File.ReadAllText(path);
        }
        
        /// <summary>
        /// This function decodes a key from a .key file
        /// of a public key and gets N and E
        /// </summary>
        /// <param name="key"></param>
        public static void decodePubK(string key) {
            PubK pubK = JsonConvert.DeserializeObject<PubK>(key);
            byte[] EK = Convert.FromBase64String(pubK.key);
            byte[] b1 = EK.Take(4).ToArray();
            int e = BitConverter.ToInt32(b1);
            byte[] b2 = new byte[e];
            Buffer.BlockCopy(EK, 4, b2, 0, e);
            BigInteger E = new BigInteger(b2);
            byte[] b3 = new byte[4];
            Buffer.BlockCopy(EK, 4+e, b3, 0, 4);
            int n = BitConverter.ToInt32(b3);
            byte[] b4 = new byte[n];
            Buffer.BlockCopy(EK, 4+e+4, b4, 0, n);
            BigInteger N = new BigInteger(b4);
            
        }
        
        /// <summary>
        /// This function decodes a key from a .key file
        /// of a private key and gets N and D
        /// </summary>
        /// <param name="key"></param>
        public static void decodePrivK(string key) {
            PrivK privK = JsonConvert.DeserializeObject<PrivK>(key);
            byte[] DK = Convert.FromBase64String(privK.key);
            byte[] b1 = DK.Take(4).ToArray();
            int d = BitConverter.ToInt32(b1);
            byte[] b2 = new byte[d];
            Buffer.BlockCopy(DK, 4, b2, 0, d);
            BigInteger D = new BigInteger(b2);
            byte[] b3 = new byte[4];
            Buffer.BlockCopy(DK, 4+d, b3, 0, 4);
            int n = BitConverter.ToInt32(b3);
            byte[] b4 = new byte[n];
            Buffer.BlockCopy(DK, 4+d+4, b4, 0, n);
            BigInteger N = new BigInteger(b4);
        }
        
        /// <summary>
        /// This function validates if the key size
        /// is a number
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int intParse(string b) {
            int x;
            if (!Int32.TryParse(b, out x)) {
                Console.WriteLine("Error: '{0}' is not a number", b);
                Environment.Exit(1);
            }
            return x;
        }
    }
}