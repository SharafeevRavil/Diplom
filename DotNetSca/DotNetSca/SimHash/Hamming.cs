namespace DotNetSca.SimHash;

public static class Hamming
{
    public static int Distance(byte[] left, byte[] right)
    {
        var distance = 0;
        for (var i = 0; i < left.Length; i++)
        {
            var diff = left[i] ^ right[i];
            for (var j = 0; j < 8; j++)
            {
                if ((diff & 1) == 1) distance++;
                diff >>= 1;
            }
        }
        return distance;
    }
}