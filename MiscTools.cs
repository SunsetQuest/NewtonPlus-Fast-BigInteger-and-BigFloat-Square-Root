// some of the below if from others, some is modified by me, and some is created by me(see notes)

using System;
using System.Numerics;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////           OTHER STUFF                    ///////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////
///
public static class MiscTools
{
    /// <summary>
    /// Creates a string of binary shades that is easier to identify patterns.
    /// </summary>
    public static string ToBinaryShades(BigInteger bigInteger, int padZeros = 0)
    {
        return ToBinaryString(bigInteger, padZeros).Replace('1', '█').Replace('0', '·'); // · ░
    }

    /// <summary>
    /// Converts a BigInteger to a string in base 2 for display.
    /// source: https://stackoverflow.com/a/15447131/2352507  Kevin P. Rice  2013 (modified by Ryan Scott White)
    /// </summary>
    public static string ToBinaryString(BigInteger bigInteger, int padZeros = 0)
    {
        byte[] bytes = bigInteger.ToByteArray();
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

        return base2.ToString().TrimStart('0').PadLeft(padZeros, '0');
    }

    /// <summary>
    /// Converts a BigInteger to a string in base 2 for display with two's complement support.
    /// source: ChatGPT 01-preview on 10-6-2024
    /// </summary>
    public static string ToBinaryString2(BigInteger bigInt, bool useTwoComplement = false)
    {
        if (bigInt.IsZero)
            return "0";

        if (useTwoComplement)
        {
            // Get the two's complement byte array (little-endian order)
            byte[] bytes = bigInt.ToByteArray();

            StringBuilder sb = new StringBuilder();

            // Process bytes from most significant to least significant
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                // Convert each byte to its binary representation, padded to 8 bits
                sb.Append(Convert.ToString(bytes[i], 2).PadLeft(8, '0'));
            }

            // Remove leading zeros
            string result = sb.ToString().TrimStart('0');

            // Ensure at least one zero is returned for zero values
            return string.IsNullOrEmpty(result) ? "0" : result;
        }
        else
        {
            bool isNegative = bigInt.Sign < 0;
            if (isNegative)
                bigInt = BigInteger.Abs(bigInt);

            StringBuilder sb = new StringBuilder();

            while (bigInt > 0)
            {
                BigInteger remainder;
                bigInt = BigInteger.DivRem(bigInt, 2, out remainder);
                sb.Insert(0, remainder.ToString());
            }

            if (isNegative)
                sb.Insert(0, "-");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Concatenates two BigIntegers into a single BigInteger.
    /// source: ChatGPT 10-6-2024 o1
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static BigInteger ConcatenateBigIntegers(BigInteger a, BigInteger b)
    {
        // Assume both BigIntegers are positive (ignore negative numbers)
        // Get the byte counts needed for 'a' and 'b'
        int byteCountA = a.GetByteCount();//a.GetByteCount(isUnsigned: true);
        int byteCountB = b.GetByteCount();//.GetByteCount(isUnsigned: true);

        // Allocate a single buffer for the concatenated bytes
        byte[] concatenatedBytes = new byte[byteCountA + byteCountB];
        Span<byte> concatenatedSpan = concatenatedBytes.AsSpan();

        // Write the bytes of 'a' into the first part of the span
        if (!a.TryWriteBytes(concatenatedSpan[..byteCountA], out int bytesWrittenA, isUnsigned: false, isBigEndian: true) ||
            bytesWrittenA != byteCountA)
        {
            throw new InvalidOperationException("Failed to write bytes of BigInteger 'a'");
        }

        // Write the bytes of 'b' into the second part of the span
        if (!b.TryWriteBytes(concatenatedSpan.Slice(byteCountA, byteCountB), out int bytesWrittenB, isUnsigned: false, isBigEndian: true) ||
            bytesWrittenB != byteCountB)
        {
            throw new InvalidOperationException("Failed to write bytes of BigInteger 'b'");
        }

        // Create a new BigInteger from the concatenated bytes
        BigInteger result = new(concatenatedSpan, isUnsigned: true, isBigEndian: true);
        return result;
    }

    public static BigInteger ConcatenateBigIntegers2(BigInteger a, BigInteger b)
    {
        // Assume both BigIntegers are positive (ignore negative numbers)
        // Get the byte counts needed for 'a' and 'b'
        int byteCountA = a.GetByteCount(isUnsigned: true);
        int byteCountB = b.GetByteCount(isUnsigned: true);

        // Allocate a single buffer for the concatenated bytes
        byte[] concatenatedBytes = new byte[byteCountA + byteCountB];
        Span<byte> concatenatedSpan = concatenatedBytes.AsSpan();

        a.TryWriteBytes(concatenatedSpan.Slice(0), out int _, isUnsigned: false, isBigEndian: true);
        b.TryWriteBytes(concatenatedSpan.Slice(byteCountA), out int _, isUnsigned: false, isBigEndian: true);

        // Create a new BigInteger from the concatenated bytes
        BigInteger result = new BigInteger(concatenatedSpan, isUnsigned: true, isBigEndian: true);
        return result;
    }

    /// <summary>
    /// A high performance BigInteger to binary string converter
    /// that supports 0 and negative numbers.
    /// License: MIT / Created by Ryan Scott White, 7/16/2022;
    /// https://stackoverflow.com/a/73009264/2352507
    /// </summary>
    public static string ToBinaryString3(BigInteger x)
    {
        // Setup source
        ReadOnlySpan<byte> srcBytes = x.ToByteArray();
        int srcLoc = srcBytes.Length - 1;

        // Find the first bit set in the first byte so we don't print extra zeros.
        int msb = BitOperations.Log2(srcBytes[srcLoc]);

        // Setup Target
        Span<char> dstBytes = stackalloc char[srcLoc * 8 + msb + 2];
        int dstLoc = 0;

        // Add leading '-' sign if negative.
        if (x.Sign < 0)
        {
            dstBytes[dstLoc++] = '-';
        }
        //else if (!x.IsZero) dstBytes[dstLoc++] = '0'; // add adding leading '0' (optional)

        // The first byte is special because we don't want to print leading zeros.
        byte b = srcBytes[srcLoc--];
        for (int j = msb; j >= 0; j--)
        {
            dstBytes[dstLoc++] = (char)('0' + ((b >> j) & 1));
        }

        // Add the remaining bits.
        for (; srcLoc >= 0; srcLoc--)
        {
            byte b2 = srcBytes[srcLoc];
            for (int j = 7; j >= 0; j--)
            {
                dstBytes[dstLoc++] = (char)('0' + ((b2 >> j) & 1));
            }
        }

        return dstBytes.ToString();
    }


    // Converts a double value to a string in base 2 for display.
    // Example: 123.5 --> "0:10000000101:1110111000000000000000000000000000000000000000000000"
    // Created by Ryan S. White in 2020
    // Released under the MIT license (should contain author somewhere)
    // https://stackoverflow.com/a/68052530/2352507
    public static string DoubleToBinaryString(double val)
    {
        long v = BitConverter.DoubleToInt64Bits(val);
        string binary = Convert.ToString(v, 2);
        return binary.PadLeft(64, '0').Insert(12, ":").Insert(1, ":");
    }

    // Converts a double value in Int64 format to a string in base 2 for display.
    // Created by Ryan S. White in 2020
    // Released under the MIT license (should contain author somewhere)
    static string DoubleToBinaryString(long doubleInInt64Format)
    {
        string binary = Convert.ToString(doubleInInt64Format, 2);
        binary = binary.PadLeft(64, '0').Insert(12, ":").Insert(1, ":");
        return binary;
    }
}



