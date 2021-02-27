using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Messenger
{
    public class Prime
    {
        public byte by{ get; private set; }
        public BigInteger primeN{ get; private set; }

        /// <summary>
        /// Prime constructor receives bytes and count
        /// </summary>
        /// <param name="by"></param>
        /// <param name="count"></param>
        public Prime(int b) {
            this.@by = (byte) (b/8);
        }
        
        /// <summary>
        /// Function with a Parallel loop to get the Prime
        /// numbers
        /// </summary>
        public void genPrime() {
            BigInteger bi = 0;
            //Parallel For loop that runs until we get a 
            //prime number
            Parallel.For(0, Int32.MaxValue, (i, state) => {
                BigInteger bi = this.genRand();
                if (bi.IsProbablyPrime()) {
                    this.primeN = bi;
                    state.Stop();
                }
            });
        }
    }
    public static class ExtensionMethods {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        
        /// <summary>
        /// Provided Extension method that checks if a BigInteger
        /// is prime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="witnesses"></param>
        /// <returns></returns>
        public static Boolean IsProbablyPrime(this BigInteger value, int witnesses = 10) {
            if (value <= 1) return false;
            if (witnesses <= 0) witnesses = 10;
            BigInteger d = value - 1;
            int s = 0;
            while (d % 2 == 0) {
                d /= 2;
                s += 1;
            }
            Byte[] bytes = new Byte[value.ToByteArray().LongLength];
            BigInteger a;
            for (int i = 0; i < witnesses; i++) {
                do {
                    var Gen = new Random();
                    Gen.NextBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= value - 2);
                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1) continue;
                for (int r = 1; r < s; r++) {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == 1) return false;
                    if (x == value - 1) break;
                }
                if (x != value - 1) return false;
            }
            return true;
        }
        
        /// <summary>
        /// Function that generates a random
        /// set of bytes that are later used
        /// in the BigInteger class
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static BigInteger genRand(this Prime val) {
            //Byte set with of size 'by'
            byte[] set = new byte[val.by];
            //Generate the random set of bytes
            rng.GetBytes(set);
            //Send bytes to BigInteger class
            BigInteger value = new BigInteger(set);
            //Return BigInteger
            return value;
        }
    }
}