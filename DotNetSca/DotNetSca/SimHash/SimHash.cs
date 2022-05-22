using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace DotNetSca.SimHash;

public class SimHash
{
    public const int HashLength = 128;

    public static (Func<string, byte[]> hashFunc, Action disposeAction) GetMd5HashFunc()
    {
        var md5 = MD5.Create();
        var hashFunc = new Func<string, byte[]>(
            str =>
            {
                var strBytes = Encoding.UTF8.GetBytes(str);
                return md5.ComputeHash(strBytes);
            });
        var disposeAction = new Action(() => md5.Dispose());
        return (hashFunc, disposeAction);
    }

    public static byte[] GenerateSimHash(IEnumerable<string> features,
        (Func<string, byte[]> hashFunc, Action disposeAction) hashFunc)
    {
        var sh = new int[HashLength];
        foreach (var feature in features)
        {
            var hash = hashFunc.hashFunc(feature);
            for (var i = 0; i < hash.Length; i++)
            {
                //    128  ,    64   ,    32   ,    16   ,     8   ,     4   ,    2    ,    1
                // 10000000, 01000000, 00100000, 00010000, 00001000, 00000100, 00000010, 00000001
                //j in the following for loop serves as a mask to check if hash[index] is 1 or 0
                var br = 0;
                for (var j = HashLength; j > 0; j /= 2)
                {
                    var index = i * 8 + br;
                    br++;

                    sh[index] = (hash[i] & j) != 0
                        ? sh[index] + 1
                        : sh[index] - 1;
                }
            }
        }

        /*var hashBytes = new byte[HashLength / 8];
        for (var i = 0; i < HashLength; i += 8)
        {
            byte a = 0;
            for (var j = 0; j < 8; j++)
            {
                a <<= 1;
                if (sh[i + j] >= 0) a++;
            }
            hashBytes[i / 8] = a;
        }
        
        hashFunc.disposeAction();
        return hashBytes;*/
            
        var hashBits = new BitArray(HashLength);
        for (var i = 0; i < HashLength; i += 8)
        for (var j = 7; j >= 0; j--)
            hashBits[i + j] = sh[i + 7 - j] >= 0;
        var hashBytes = new byte[HashLength / 8];
        hashBits.CopyTo(hashBytes, 0);
        
        hashFunc.disposeAction();
        return hashBytes;
    }

    public static string HashBytesToString(IEnumerable<byte> hashBytes) =>
        string.Join("", hashBytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
}