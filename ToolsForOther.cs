// some ofthe below if from others, some is modified by me, and some is created by me(see notes)

using System;
using System.Numerics;
using System.Text;

/////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////           OTHER STUFF                    ///////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////
///
public static class ToolsForOther
{

    //static int FirstNonMatch(string a, string b)
    //{
    //    int length = Math.Min(a.Length, b.Length);
    //    for (int i = 0; i < length; i++)
    //    {
    //        if (a[i] != b[i])
    //            return i;
    //    }
    //    return length;
    //}


    // Created by Ryan S. White for testing.
    public static double SqrtSimple(double x)
    {
        return Math.Sqrt(x);
    }


    //source: https://stackoverflow.com/a/15447131/2352507  Kevin P. Rice  2013 (modified by Ryan Scott White)
    public static string ToBinaryString(BigInteger bigint, int padZeros = 0)
    {
        byte[] bytes = bigint.ToByteArray();
        int idx = bytes.Length - 1;

        // Create a StringBuilder having appropriate capacity.
        StringBuilder base2 = new StringBuilder(bytes.Length * 8);

        // Convert first byte to binary.
        string binary = Convert.ToString(bytes[idx], 2);

        //// Ensure leading zero exists if value is positive.
        //if (binary[0] != '0' && bigint.Sign == 1)
        //{
        //    base2.Append('0');
        //}

        // Append binary string to StringBuilder.
        base2.Append(binary);

        // Convert remaining bytes adding leading zeros.
        for (idx--; idx >= 0; idx--)
        {
            base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
        }

        return base2.ToString().PadLeft(padZeros, '0');
    }

    //// Converts a double value to a string in base 2 for display.
    //// Example: 123.5 --> "0:10000000101:1110111000000000000000000000000000000000000000000000"
    //// Created by Ryan S. White in 2020
    //// Released under the MIT license (should contain author somewhere)
    //// https://stackoverflow.com/a/68052530/2352507
    //public static string DoubleToBinaryString(double val)
    //{
    //    long v = BitConverter.DoubleToInt64Bits(val);
    //    string binary = Convert.ToString(v, 2);
    //    return binary.PadLeft(64, '0').Insert(12, ":").Insert(1, ":");
    //}

    //// Converts a double value in Int64 format to a string in base 2 for display.
    //// Created by Ryan S. White in 2020
    //// Released under the MIT license (should contain author somewhere)
    //static string DoubleToBinaryString(long doubleInInt64Format)
    //{
    //    string binary = Convert.ToString(doubleInInt64Format, 2);
    //    binary = binary.PadLeft(64, '0').Insert(12, ":").Insert(1, ":");
    //    return binary;
    //}
}



