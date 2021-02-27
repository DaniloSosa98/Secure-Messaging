using System;
using System.Collections.Generic;
using System.Numerics;

namespace Messenger
{
    /// <summary>
    /// This class contains the operations to
    /// generate the public and private keys
    /// </summary>
    public class KeyGenerator {
        public int sizeK;
        public PubK pubK{ get; private set; }
        public PrivK privK{ get; private set; }
        
        //Constructor that receives the size of the keys
        public KeyGenerator(int sizeK) {
            this.sizeK = sizeK;
        }
        
        //Function to generate both keys, it basically generates
        //a byte array for each e E/d D, n N and appends them to generate the key
        public void genKeys() {
            var prime = new Prime(sizeK/2);
            prime.genPrime();
            var p = prime.primeN;
            prime.genPrime();
            var q = prime.primeN;
            var N = p * q;
            var r = (p - 1) * (q - 1);
            BigInteger E = 65536;
            var D = modInverse(E, r);
            int e = E.ToByteArray().Length;
            int n = N.ToByteArray().Length;
            
            byte[] b1 = BitConverter.GetBytes(e);
            byte[] b2 = E.ToByteArray();
            byte[] b3 = BitConverter.GetBytes(n);
            byte[] b4 = N.ToByteArray();
            byte[] arrayEK = new byte[b1.Length + b2.Length + b3.Length+ b4.Length];
            Buffer.BlockCopy(b1, 0, arrayEK, 0, b1.Length);
            Buffer.BlockCopy(b2, 0, arrayEK, b1.Length, b2.Length);
            Buffer.BlockCopy(b3, 0, arrayEK, b1.Length + b2.Length, b3.Length);
            Buffer.BlockCopy(b4, 0, arrayEK, b1.Length + b2.Length+b3.Length, b4.Length);
            
            string EK = Convert.ToBase64String(arrayEK);
            pubK = new PubK();
            pubK.key = EK;
            pubK.email = "";
            pubK.saveK();

            int d = D.ToByteArray().Length;
            byte[] b5 = BitConverter.GetBytes(d);
            byte[] b6 = D.ToByteArray();
            byte[] arrayDK = new byte[b5.Length + b6.Length + b3.Length+ b4.Length];
            Buffer.BlockCopy(b5, 0, arrayDK, 0, b5.Length);
            Buffer.BlockCopy(b6, 0, arrayDK, b5.Length, b6.Length);
            Buffer.BlockCopy(b3, 0, arrayDK, b5.Length + b6.Length, b3.Length);
            Buffer.BlockCopy(b4, 0, arrayDK, b5.Length + b6.Length+b3.Length, b4.Length);

            string DK = Convert.ToBase64String(arrayDK);
            privK = new PrivK();
            privK.key = DK;
            privK.email = new List<string>();
            privK.saveK();
        }
        
        //Function given by the professor to calculate D
        public static BigInteger modInverse(BigInteger a, BigInteger n) {
            BigInteger i = n, v = 0, d = 1;
            while (a>0) {
                BigInteger t = i/a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t*x;
                v = x;
            }
            v %= n;
            if (v<0) v = (v+n)%n;
            return v;
        }
    }
}