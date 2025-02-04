// most of the code below if from others and some has been modified by me (see note for source locaiton)


#pragma warning disable IDE0051, IDE0050, CA1050 // Ignore unused code and missing namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


public static class BigIntegerSquareRootOthers
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // MaxKlaxx, 2020, https://stackoverflow.com/a/63909229/2352507
    //public static BigInteger FastSqrtSmallNumber = 4503599761588223UL; // as static readonly = reduce compare overhead
    public static BigInteger MaxKlaxxSqrt(BigInteger value) // MaxKlaxxSqrt
    {
        if (value <= 4503599761588223UL) // small enough for Math.Sqrt() or negative?
        {
            if (value.Sign < 0)
            {
                throw new ArgumentException("Negative argument.");
            }

            return (ulong)Math.Sqrt((ulong)value);
        }

        BigInteger root; // now filled with an approximate value
        int byteLen = value.ToByteArray().Length;
        if (byteLen < 128) // small enough for direct double conversion?
        {
            root = (BigInteger)Math.Sqrt((double)value);
        }
        else // large: reduce with bit-shifting, then convert to double (and back)
        {
            root = (BigInteger)Math.Sqrt((double)(value >> (byteLen - 127) * 8)) << (byteLen - 127) * 4;
        }

        for (; ; )
        {
            BigInteger root2 = value / root + root >> 1;
            if ((root2 == root || root2 == root + 1) && IsSqrt1(value, root))
            {
                return root;
            }

            root = value / root2 + root2 >> 1;
            if ((root == root2 || root == root2 + 1) && IsSqrt1(value, root2))
            {
                return root2;
            }
        }
    }
    private static bool IsSqrt1(BigInteger value, BigInteger root)
    {
        BigInteger lowerBound = root * root;
        return value >= lowerBound && value <= lowerBound + (root << 1);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Converted from Java to C# by Ryan Scott White
    // added code to handle negative numbers.

    //https://code.yawk.at/java/13/java.base/java/math/MutableBigInteger.java#java.math.MutableBigInteger%23sqrt()  6/9/2021
    /* Calculate the integer square root {@code floor(sqrt(this))} where
     * {@code sqrt(.)} denotes the mathematical square root. The contents of
     * {@code this} are<b> not</b> changed.The value of {@code this} is assumed
     * to be non-negative.
     *
     * @implNote The implementation is based on the material in Henry S. Warren,
     * Jr., <i>Hacker's Delight (2nd ed.)</i> (Addison Wesley, 2013), 279-282.
     *
     * @throws ArithmeticException if the value returned by {@code bitLength()}
     * overflows the range of {@code int}.
     * @return the integer square root of {@code this}
     * @since 9
    */
    public static BigInteger JavaSqrt(BigInteger n)
    {
        // Special cases.
        if (n <= 0)                                             // if (this.isZero())
        {
            if (n == 0)
            {
                return 0;                                       // return new MutableBigInteger(0);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(n));
            }
        }

        long bitLengthLong = n.GetBitLength();

        if (bitLengthLong == 1 && n < 4)                        // if (this.value.length == 1 && (this.value[0] & LONG_MASK) < 4)
        { // result is unity
            return BigInteger.One;
        }

        if (bitLengthLong <= 63)                                // if (bitLength() <= 63)
        {
            // Initial estimate is the square root of the positive long value.
            long v = (long)n;                                   // long v = new BigInteger(this.value, 1).longValueExact();
            long xk = (long)Math.Sqrt(v);                       // long xk = (long)Math.floor(Math.sqrt(v));

            // Refine the estimate.
            do
            {
                long xk1 = (xk + v / xk) / 2;                   // [No Change]

                // Terminate when non-decreasing.
                if (xk1 >= xk)                                  // [No Change]
                {
                    return xk | (xk & 0xffffffffL);             // new MutableBigInteger(new int[] { (int)(xk >>> 32), (int)(xk & LONG_MASK)    });
                }

                xk = xk1;                                       // [No Change]
            } while (true);                                     // [No Change]
        }
        else
        {
            // Set up the initial estimate of the iteration.

            // Obtain the bitLength > 63.
            int bitLength = (int)bitLengthLong;                 // int bitLength = (int)this.bitLength();
            if (bitLength != bitLengthLong)                     // if (bitLength != this.bitLength())
            {
                throw new ArithmeticException("bitLength() integer overflow");
            }

            // Determine an even valued right shift into positive long range.
            int shift = bitLength - 63;                         // [No Change]
            if (shift % 2 == 1)                                 // [No Change]
            {
                shift++;                                        // [No Change]
            }

            // Shift the value into positive long range.        
            BigInteger xk = n >> shift;                         // MutableBigInteger xk = new MutableBigInteger(this);
                                                                // xk.rightShift(shift);
                                                                // xk.normalize();


            // Use the SqRt of the shifted value as an approx.  
            double d = (double)xk;                              // double d = new BigInteger(xk.value, 1).doubleValue();
            xk = (long)Math.Ceiling(Math.Sqrt(d));              // BigInteger bi = BigInteger.valueOf((long)Math.ceil(Math.sqrt(d)));
                                                                // [Not needed] xk = new MutableBigInteger(bi.mag);

            // Shift the approximate sqrt back into the original range.
            xk <<= (shift / 2);                                 // xk.leftShift(shift / 2);

            // Refine the estimate. 
            // [Not needed] MutableBigInteger xk1 = new MutableBigInteger();
            do
            {
                BigInteger xk1 = (xk + n / xk) >> 1;                   // this.divide(xk, xk1, false);
                                                                       // [Not needed] xk1.add(xk);
                                                                       // [Not needed] xk1.rightShift(1);
                                                                       // Terminate when non-decreasing.
                if (xk1 >= xk)                                  // if (xk1.compare(xk) >= 0)
                {
                    return xk;                                  // [No Change]
                }
                xk = xk1;                                       // xk.copyValue(xk1);
                                                                // [Not needed] xk1.reset();
            } while (true);                                     // [No Change]
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // RedGreenCode, 2011 https://stackoverflow.com/a/6084813/2352507
    public static BigInteger RedGreenCodeSqrt(BigInteger n) //RedGreenCodeSqrt
    {
        if (n == 0)
        {
            return 0;
        }

        if (n > 0)
        {
            int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
            BigInteger root = BigInteger.One << (bitLength / 2);

            while (!IsSqrt2(n, root))
            {
                root += n / root;
                root /= 2;
            }

            return root;
        }

        throw new ArithmeticException("NaN");
    }
    public static bool IsSqrt2(BigInteger n, BigInteger root)
    {
        BigInteger lowerBound = root * root;
        BigInteger upperBound = (root + 1) * (root + 1);

        return (n >= lowerBound && n < upperBound);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Nordic Mainframe 2010   https://stackoverflow.com/a/3432579
    // Note: Updated by Ryan S. White(sunsetquest) to work with BigInteger and also added a negitive number guard.
    public static BigInteger NordicMainframeSqrt(BigInteger x) //NordicMainframeSqrt
    {
        if (x.Sign < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        int b = (int)(x.GetBitLength() + 1) / 2;    // USE FOR VERSIONS .NET 5   & UP
        //int b = (GetBitLength(x) + 1) / 2;    // USE FOR VERSIONS .NET 4.8 & UNDER

        BigInteger r = 0; // r will contain the result
        BigInteger r2 = 0; // here we maintain r squared
        while (b >= 0)
        {
            BigInteger sr2 = r2;
            BigInteger sr = r;
            // compute (r+(1<<b))**2, we have r**2 already.
            r2 += ((r << (1 + b)) + (BigInteger.One << (b + b)));
            r += (BigInteger.One << b);
            if (r2 > x)
            {
                r = sr;
                r2 = sr2;
            }
            b--;
        }

        return r;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Nordic Mainframe 2010   https://stackoverflow.com/a/3432579  [Optimized by Ryan Scott White in Nov-Dec 2021]
    // Note: Updated by Ryan White to work with BigInteger and also add negative number guard clause.
    // Updated to be initialized with HW sqrt. 
    public static BigInteger NordicMainframeOptimizedSqrt1(BigInteger x) //NordicMainframeOptimizedSqrt
    {
        const long Bit53 = (1L << 52);
        // This first bit is from Max Klaxx (casting to a long speeds up the Sqrt)
        if (x < Bit53)
        {
            if (x.Sign >= 0)
            {
                return (int)Math.Sqrt((long)x);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
        }

        double asDouble = (double)x;
        int exp;
        long mantissa53;

        if (double.IsFinite(asDouble))
        {
            if (asDouble <= 2e31)
            {
                long v = (long)(Math.Sqrt(BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(asDouble) + 1)));
                return (v + x / v) >> 1;
            }
            if (asDouble <= 4e127)
            {
                BigInteger v = (BigInteger)(Math.Sqrt(asDouble));
                if (asDouble > 2e63)
                {
                    v = (v + x / v) >> 1;
                }

                v = (v + x / v) >> 1;
                return (v * v <= x) ? v : --v;
            }

            long bits = BitConverter.DoubleToInt64Bits(Math.Sqrt(asDouble));
            exp = ((int)(bits >> 52) & 0x3ff) + 2;
            mantissa53 = (bits & (Bit53 - 1)) | Bit53;
        }
        else
        {
            // ~1 to make sure we shift by an even count - this makes sure we know the output size. (if it were odd it would be much more complicated to calc.)
            int shiftAmt = ((int)x.GetBitLength() - 105) & ~1;   // USE FOR VERSIONS .NET 5   & UP
            //int shiftAmt = (GetBitLength(x) - 105) & ~1;       // USE FOR VERSIONS .NET 4.8 & UNDER

            mantissa53 = (long)Math.Sqrt((double)(x >> shiftAmt));
            exp = (shiftAmt >> 1) + 53;
        }

        int b = exp - 53;
        BigInteger r = ((BigInteger)mantissa53 << b); // r will contain the result
        BigInteger r2 = r * r; // here we maintain r squared
        Console.WriteLine($"r {r} b:{b}");
        while (b >= 0)
        {
            BigInteger sr2 = r2;
            BigInteger sr = r;
            // compute (r+(1<<b))**2, we have r**2 already.
            r2 += ((r << (1 + b)) + (BigInteger.One << (b + b)));
            r += (BigInteger.One << b);
            if (r2 > x)
            {
                r = sr;
                r2 = sr2;
            }
            b--;
        }



        // I believe the above has no errors but to guarantee the following can be added.
        // If an error is found, please report it.
        BigInteger tmp = r * r;
        if (tmp > x)
        {
            Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
            //throw new Exception("Sqrt function had internal error - value too high");
        }
        else if ((tmp + 2 * r + 1) <= x)
        {
            Console.WriteLine($"(val+1)^2({((r + 1) * (r + 1))}) >= x({x})");
            //throw new Exception("Sqrt function had internal error - value too low");
        }
        else
        { }

        return r;
    }


    // Another method inspired by Nordic Mainframe's answer in 2010   https://stackoverflow.com/a/3432579 
    // Modified by Ryan Scott White on 1/22/22 to see if we can improve it.
    public static BigInteger AnotherOptimizedSqrt(BigInteger x)
    {
        if (x < 144838757784765629)    //known bad at 144838757784765629 = 1<<57=1.448e17
        {
            uint vInt = (uint)Math.Sqrt((ulong)x);
            if ((x >= 4503599761588224) && ((ulong)vInt * vInt > (ulong)x))  // known bad at 4503599761588224 = 4.5e15 =  ~1<<52
            {
                vInt--;
            }

            return vInt;
        }

        double xAsDub = (double)x;
        if (xAsDub < 8e37)   //update 8e37 seems to be stable here with 9 hours @ [2-8]e37 //maybe up to 3.40282e38 (aka ulong.max*ulong.max)?
        {
            ulong vInt = (ulong)Math.Sqrt(xAsDub);
            BigInteger v = (vInt + ((ulong)(x / vInt))) >> 1;
            return (v * v <= x) ? v : v - 1;
        }

        if (xAsDub < 4.3322e127)  // 4e127 = 1<<423.88  can we go to  4.3322E+127? Tested 12H between 4.29845e127-4.33229e127 and no errors (lowest found 4332296397072526994426)
        {
            BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
            v = (v + x / v) >> 1;
            if (xAsDub > 2e63)
            {
                v = (v + (x / v)) >> 1;
            }

            return (v * v <= x) ? v : v - 1;
        }

        int xLen = (int)x.GetBitLength();
        int outLen = ((xLen + 1) / 2);
        int xLenMod = xLen + (xLen & 1) + 1;

        //////// Do the first Sqrt on hardware ////////
        long tempX = (long)(x >> (xLenMod - 63));
        double tempSqrt1 = Math.Sqrt(tempX);
        ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;

        if ((valLong & 0x10000000000000L) == 0)
        {
            valLong = (valLong | 0x10000000000000L) << 1;
        }

        //if (((BigInteger)valLong * valLong) > (x >> (xLenMod - 106))) valLong--;

        ////////  Classic Newton Iterations ////////
        //BigInteger val = valLong;
        //int size = 53;
        BigInteger val = ((BigInteger)valLong << (53 - 1)) + (x >> xLenMod - (3 * 53)) / valLong;
        int size = 106;
        //for (; size < 256; size <<= 1)
        //    val = (val << (size - 1)) + (x >> xLenMod - (3 * size)) / val;

        //if (xAsDub > 4e254) // 4e254 = 1<<845.76973610139
        //{
        //    do
        //    {
        //        //  Newton Plus Iterations ////////
        //        int shiftX = xLenMod - (3 * size);
        //        BigInteger valSqrd = (val * val) << (size - 1);
        //        BigInteger valSU = (x >> shiftX) - valSqrd;
        //        val = (val << size) + (valSU / val);
        //        size *= 2;

        //    } while ((size << 1) < outLen);
        //}

        BigInteger tempVal = val >> (size - outLen);
        if ((tempVal * tempVal) > x)
        {
            val--;
        }
        //tempVal = val >> (size - outLen);
        //if ((tempVal * tempVal) > x)
        //    val--;

        if (size > outLen)
        {
            val >>= size - outLen;
            if ((val * val) > x)
            {
                val--;
            }

            return val;
        }

        // Partial source for below code: Nordic Mainframe 2010   https://stackoverflow.com/a/3432579
        // Modified by Ryan Scott White on 1/22/22 to see if we can improve it. 
        // I noticed Nordic's function was the only other BigO(n^2 log(n) but was starting out behind.  I wanted to see
        // if I gave it a good start using the hardware Sqrt and even doing a couple newton Rounds would help but it did not.
        // It is still BigO(n^2 log(n) but it is still n^2 slower. I also tried to increase the precision as it ignores but
        // that did not help much.
        int b = outLen - size - 1;
        BigInteger r = val; // r will contain the result
        BigInteger r2 = r * r; // here we maintain r squared
        while (b >= 0)
        {
            r <<= 1;
            r2 <<= 2;
            BigInteger sr2 = r2;
            r2 = r2 + (r << 1) + 1;
            bool overflow = r2 > (x >> (b << 1));
            if (overflow)
            {
                r2 = sr2;
            }
            else
            {
                r++;
            }

            b--;
        }

        // Below was another attempt that I gave up on.
        //const int SET_SZ = 8;
        //const int BYTES_IN_SET = SET_SZ / 8;
        //int sets = (x.GetByteCount() + (BYTES_IN_SET - 1)) / BYTES_IN_SET;
        //byte[] X = new byte[sets];
        //byte[] V = new byte[(sets + 1) / 2];
        //byte[] V2 = new byte[sets];
        //Buffer.BlockCopy(x.ToByteArray(), 0, X, 0, x.GetByteCount());
        //Buffer.BlockCopy((val << ((outLen - size) % BYTES_IN_SET)).ToByteArray(), 0, V, (outLen - size) / BYTES_IN_SET, val.GetByteCount());
        //BigInteger vsqrd = (val * val);
        //Buffer.BlockCopy((vsqrd << ((2 * (outLen - size)) % 8)).ToByteArray(), 0, V2, (2 * (outLen - size)) / BYTES_IN_SET, vsqrd.GetByteCount());
        //BigInteger v = val << 9999;  // 9999 is todo item
        //BigInteger v2 = v * v;
        //for (int b = outLen - size - 1; b >= 0; b--)
        //{
        //    BigInteger v2Temp = v2 + ((v << (b + 1)) + (1 << (2 * b)));
        //    bool overflow = v2Temp > x;
        //    if (!overflow)
        //    {
        //        v2 = v2Temp;
        //        v += (BigInteger.One << b);
        //    }
        //}


        //// I believe the above has no errors but to guarantee the following can be added.
        //// If an error is found, please report it.
        //BigInteger tmp = r * r;
        //if (tmp > x)
        //{
        //    Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
        //    //throw new Exception("Sqrt function had internal error - value too high");
        //}
        //else if ((tmp + 2 * r + 1) <= x)
        //{
        //    Console.WriteLine($"(val+1)^2({((r + 1) * (r + 1))}) >= x({x})");
        //    //throw new Exception("Sqrt function had internal error - value too low");
        //}
        //else
        //{ }

        return r;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Source: Michael Steiner, Jan 2016 http://mjs5.com/2016/01/20/c-biginteger-square-root-function/
    // Slightly modified by Sunsetquest to correct an error below 6. (thank you M Ktsis D) 
    public static BigInteger MichaelSteinerSqrt(BigInteger number)  //MichaelSteinerSqrt
    {
        if (number < 9)
        {
            if (number == 0)
            {
                return 0;
            }

            if (number < 4)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        BigInteger n = 0, p = 0;
        BigInteger high = number >> 1;
        BigInteger low = BigInteger.Zero;

        while (high > low + 1)
        {
            n = (high + low) >> 1;
            p = n * n;
            if (number < p)
            {
                high = n;
            }
            else if (number > p)
            {
                low = n;
            }
            else
            {
                break;
            }
        }
        return number == p ? n : low;
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Edward Falk 2013  https://stackoverflow.com/a/16804098/2352507
    // Converted from Java to C# by Ryan Scott White
    // - Edward's code was designed to work well in Java (not C#)
    // - performance might be impacted because of this
    // - added code to handle negative numbers
    public static BigInteger EdwardFalkSqrt(BigInteger x) //EdwardFalkSqrt
    {
        if (x <= 0)
        {
            if (x == 0)
            {
                return 0;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }
        }

        BigInteger div = BigInteger.One << (int)(x.GetBitLength() >> 1 - 1);  // USE FOR VERSIONS .NET 5   & UP
        //BigInteger div = BigInteger.One << (GetBitLength(x) >> 1 - 1);      // USE FOR VERSIONS .NET 4.8 & UNDER
        BigInteger div2 = div;
        // Loop until we hit the same value twice in a row, or wind
        // up alternating.
        for (; ; )
        {
            BigInteger y = (div + (x / div)) >> 1;
            if ((y == div) || (y == div2))
            {
                return BigInteger.Min(div, div2);
            }

            div2 = div;
            div = y;
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Jim 2012 https://stackoverflow.com/a/11962756/2352507
    // Converted from Java to C# by Ryan Scott White
    // - Jim's code was designed to work well in Java (not C#)
    // - performance might be impacted because of this
    public static BigInteger JimSqrt(BigInteger x)
    {
        if (x.CompareTo(0) < 0)
        {
            throw new ArgumentException("Negative argument.");
        }
        // square roots of 0 and 1 are trivial and
        // y == 0 will cause a divide-by-zero exception
        if (x.Equals(0) || x.Equals(BigInteger.One))
        {
            return x;
        } // end if
        BigInteger y;
        // starting with y = x / 2 avoids magnitude issues with x squared
        for (y = x / 2;
            y.CompareTo(x / (y)) > 0;
            y = (x / y + y) / 2)
        { }

        return y;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Jeremy Kahan - Sept. 22 '16 at 5:37 https://stackoverflow.com/a/39631028
    public static BigInteger KahanSqrt(BigInteger n) //KahanSqrt
    {
        BigInteger oddNumber = 1;
        BigInteger result = 0;
        while (n >= oddNumber)
        {
            n -= oddNumber;
            oddNumber += 2;
            result++;
        }
        return result;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // source: Jan Schultke's post https://stackoverflow.com/a/63457507/2352507 Aug. 2020
    // converted from C++ to C# by Ryan Scott White
    // - Jan's code was designed to work well in C++ (not C#), performance will go down.
    // - added code to handle 0 and -1
    public static BigInteger SchultkeSqrt(BigInteger n)
    {
        int shift = (int)n.GetBitLength();
        shift += shift & 1; // round up to next multiple of 2

        BigInteger result = 0;

        if (n <= 0)
        {
            if (n < 0)
            {
                throw new ArgumentException("Negative argument.");
            }
            return 0;
        }

        do
        {
            shift -= 2;
            result <<= 1; // make space for the next guessed bit
            result |= 1;  // guess that the next bit is 1
            result ^= (result * result > (n >> shift)) ? 1 : 0; // revert if guess too high
        }
        while (shift != 0);

        return result;
    }
}
