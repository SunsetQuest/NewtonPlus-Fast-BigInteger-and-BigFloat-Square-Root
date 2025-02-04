// Copyright Ryan Scott White. 3/2022
// Released under the MIT License. Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Numerics;
using static BigIntegerSquareRoot;

public static class BigFloatingPointSquareRoot
{
    private static readonly int? defaultShift = -200;
    private static readonly int? defaultPrecision = 200;

    // An adapter that turns the floating point square root into a integer.
    public static BigInteger SunsetQuestSqrtFloatUsingIntVersion(BigInteger x)
    {
        int shift = defaultShift ?? 0;
        int wantedPrecision = defaultPrecision ?? (((int)x.GetBitLength() + 1) / 2);
        return SunsetQuestSqrtFloatUsingIntVersion(x, shift, wantedPrecision).val;
    }
    // Wrapper function to allow the use of BigInteger tools for benchmarking/testing.
    public static BigInteger SunsetQuestSqrtFloat5(BigInteger x)
    {
        int shift = defaultShift ?? 0;
        int wantedPrecision = defaultPrecision ?? 0;
        return SunsetQuestSqrtFloat5(x, shift, wantedPrecision).val;
    }
    public static BigInteger SunsetQuestSqrtFloat4(BigInteger x)
    {
        int shift = defaultShift ?? 0;
        int wantedPrecision = defaultPrecision ?? 0;
        return SunsetQuestSqrtFloat4(x, shift, wantedPrecision).val;
    }
    public static BigInteger SetsetQuestSqrtUsingDoubles(BigInteger x)
    {
        int shift = defaultShift ?? 0;
        int wantedPrecision = defaultPrecision ?? 0;
        return SetsetQuestSqrtUsingDoubles(x, shift, wantedPrecision).val;
    }


    // October 12 2021, Updated: 12/24/2021 - A floating point square root that uses the integer square root. This function also allows for flexable precision. Created by Ryan Scott White
    /// <summary>
    /// Calculates the square root of a big floating point number.
    /// </summary>
    /// <param name="x">The input value. This would be the in-precision part of the bits.</param>
    /// <param name="shift">(Optional)The shift that should be applied to the input value. (2 would be shift left by 2)</param>
    /// <param name="wantedPrecision">(Optional)The number of in-precision bits to return.</param>
    /// <returns>(in-precision bits as Big Integer, and the shift to scale it correctly)</returns>
    public static (BigInteger val, int shift) SunsetQuestSqrtFloatUsingIntVersion(BigInteger x, int shift = 0, int wantedPrecision = 0)
    {
        int xLen = (int)x.GetBitLength();
        if (wantedPrecision == 0)
        {
            wantedPrecision = xLen;
        }

        if (x == 0)
        {
            return (0, wantedPrecision);
        }

        int totalLen = shift + xLen;
        int needToShiftInputBy = (2 * wantedPrecision - xLen) - (totalLen & 1);
        BigInteger val = NewtonPlusSqrt(x << needToShiftInputBy);
        int retShift = (totalLen + ((totalLen > 0) ? 1 : 0)) / 2 - wantedPrecision;
        return (val, retShift);
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////            PAST VERSION(S) BELOW          ///////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Summary:Fast Sqrt function that will work with large floating point numbers.
    // Created: By Ryan Scott White around 12-21-2021
    public static (BigInteger val, int shift) SunsetQuestSqrtFloat5(BigInteger x, int shift = 0, int wantedPrecision = 0)
    {
        if (x.Sign <= 0)
        {
            return x.Sign == 0 ? (0, 0) : throw new ArgumentOutOfRangeException(nameof(x));
        }

        bool xIsSmall = x < ((ulong)1 << 57);
        int xLen, totalLen;
        ulong xAsLong;
        int downshiftedX;
        if (xIsSmall)
        {
            xAsLong = (ulong)x;
            xLen = BitOperations.Log2(xAsLong) + 1;
            totalLen = shift + xLen;
            downshiftedX = xLen - 60 - (totalLen & 1);
            xAsLong <<= -downshiftedX;
        }
        else
        {
            xLen = (int)x.GetBitLength();
            totalLen = shift + xLen;
            downshiftedX = xLen - 64 + (totalLen & 1); //  + shift?
            xAsLong = (ulong)(x >> downshiftedX);
        }

        if (wantedPrecision <= 0)
        {
            wantedPrecision = wantedPrecision == 0 ? xLen : throw new ArgumentOutOfRangeException(nameof(wantedPrecision));
        }

        int outputLen = (totalLen + ((totalLen > 0) ? 1 : 0)) / 2;
        int retShift = outputLen - wantedPrecision;

        ulong val2;
        if (wantedPrecision < 27)
        {
            val2 = (ulong)Math.Sqrt(xAsLong);
            return (((BigInteger)val2) << ((downshiftedX + shift) / 2 - retShift), retShift);
        }
        else if (wantedPrecision < 53)
        {
            double val3 = Math.Sqrt(xAsLong);
            ulong val4 = (ulong)((BitConverter.DoubleToInt64Bits(val3) & 0xfffffffffffffL) | 0x10000000000000L);
            int downSize2 = 53 - wantedPrecision;  // 53 bits from double

            BigInteger val5 = val4 >> downSize2;
            BigInteger missFromX = (x << (shift - (retShift * 2))) - (val5 * val5);
            if (missFromX < 0)
            {
                val5--;
            }
            else if (missFromX >= (2 * val5 + 1))
            {
                val5++;
            }

            return (val5, retShift);
        }

        //////// Do H/W Sqrt first to get first 53 bits ////////
        int shiftX = xLen - 105;  //105 = (SqrtHWWidth << 1) - 1
        if ((shift & 1) == 0)
        {
            shiftX &= ~1;
        }
        else if ((xLen & 1) > 0)
        {
            shiftX--;
        }

        BigInteger tempX = x >> shiftX;
        double tempSqrt1 = Math.Sqrt((double)(tempX >> 8)); //todo: remove >> 8? (seems to go faster with 8)
        BigInteger val = (BitConverter.DoubleToInt64Bits(tempSqrt1) & 0xfffffffffffffL) | 0x10000000000000L;
        BigInteger valSq = val * val;
        if (valSq > tempX)
        {
            val--;
        }
        else if ((valSq + (2 * val) + 1) <= tempX)
        {
            val++;
        }


        int size = 53;  // SqrtHWWidth=53

        //////  we can do a few Newtons at this point (The impact is very minor but gives us more starting point options)(For super larger numbers this can even be repeated once more to give us more options but the returns diminish.) //////// 
        for (int i = 0; i < 3; i++)
        {
            if (size >= wantedPrecision)
            {
                return (val >> (size - wantedPrecision), retShift);
            }
            int temp2a = shiftX - size + 1;
            BigInteger temp4a = (x >> temp2a) / val;
            val = (val << (size - 1)) + temp4a;
            size *= 2;
            shiftX -= size;

            if ((val * val) > (x >> shiftX))
            {
                val--;
            }
        }

        ////// Figure out a good starting size //////// 
        int wantedOutputPrecisionMinus1 = wantedPrecision - 1;
        int temp62 = wantedOutputPrecisionMinus1 / size;
        int numOfNewtonSteps = BitOperations.Log2((uint)temp62) + 1;
        int wantedSize = (wantedOutputPrecisionMinus1 >> numOfNewtonSteps) + 1;

        //////  Apply Starting Size  ////////
        int needToShiftBy = size - wantedSize;
        val >>= needToShiftBy;
        size = wantedSize;
        shiftX += needToShiftBy << 1;

        ////////  newton iterations 2+ ////////
        BigInteger topOfX = x >> shiftX;
        BigInteger valSquared = val * val;
        while (size < wantedPrecision)
        {
            BigInteger valSU = (topOfX - valSquared) << (size - 1);
            BigInteger div = valSU / val;
            val = (val << (size + 0)) + div + 1;
            size <<= 1;

            shiftX -= size;
            topOfX = x >> shiftX;

            // Check if val is too large.
            valSquared = val * val;
            if (valSquared > topOfX)
            {
                val--;
                valSquared -= (2 * val) + 1;
                if (valSquared > topOfX)
                {
                    val--;
                    valSquared -= (2 * val) + 1;
                }
            }
        }

        // Verify Results
        //tempX = x << (-retShift * 2 + 0);
        //BigInteger tmp1 = val * val;
        //if ((tmp1) > tempX)
        //    Console.WriteLine($"val^2 ({tmp1}) < x({tempX})  off%:{((double)(tmp1)) / (double)tempX}");
        //else if (((val + 1) * (val + 1)) <= tempX)
        //    Console.WriteLine($"(val+1)^2({((val + 1) * (val + 1))}) >= x({tempX})");

        return (val >> (size - wantedPrecision), retShift);
    }


    // Conv. Math     Func input        Conv. Math Out [Func Out] 
    // Sqrt(2.0)   -> Sqrt(20, 0, 10)-> 1.414213562 [1414213562,0]
    // Sqrt(4.0)   -> Sqrt(40, 0, 10)-> 2.000000000 [2000000000,0]
    // Sqrt(4)     -> Sqrt(4, 0, 10) -> 2,   [2000000000,0]
    // Sqrt(4.0)   -> Sqrt(40, 0)    -> 2.0  [20, 0]
    // Sqrt(40)    -> Sqrt(40, 1)    -> 6.3  [63, 0]
    // Sqrt(400)   -> Sqrt(40, 2)    -> 20   [20, 1]
    // Sqrt(4000)  -> Sqrt(40, 3)    -> 63   [20, 1]
    // Sqrt(4000)  -> Sqrt(4 , 3)    -> 60   [6 , 1]      6.3245553203367586639977870888654
    // Sqrt(400)   -> Sqrt(400, 2)   -> 20.0 [200,1]
    // Sqrt(0.40)  -> Sqrt(40, -1)   -> 0.63 [63,-1]
    // Sqrt(0.040) -> Sqrt(40, -2)   -> 0.20 [20,-1]
    // Sqrt(0.0040)-> Sqrt(40, -3)   -> 0.063[63,-2]

    // another example but in binary
    // x = 1000000, shift = 0, wantedPrecision = 7  --> "1000000"    -->Sqrt--> "1000.000"   --ans--> 1000000,-3
    // x = 1000000, shift = 2, wantedPrecision = 7  --> "1000000"00  -->Sqrt--> "10000.00"   --ans--> 1000000,-2
    // x = 1000000, shift =-2, wantedPrecision = 7  --> "10000.00"   -->Sqrt--> "1000.000"   --ans--> 1000000,-3  <---- double check this row
    // x = 1000000, shift = 0, wantedPrecision = 9  --> "1000000"    -->Sqrt--> "1000.00000" --ans--> 100000000,-5
    // x = 1000000, shift = 2, wantedPrecision = 9  --> "1000000"00  -->Sqrt--> "10000.0000" --ans--> 100000000,-4
    // x = 1000,    shift = 2, wantedPrecision = 2  --> "1000"       -->Sqrt--> "11"111      --ans--> 11,-3
    // x = 100,     shift = 0, wantedPrecision = 4  --> "100"        -->Sqrt--> "10.00"      --ans--> 1000,-2



    // floating poitn Sqrt attempt 4
    // Ryan Scott White 10-16-2021 (some fixes 12/24/2021)
    //Performance idea: when we shift a double up we multiply by some 2^n and when decrease we convert to BigInt then shift.
    public static (BigInteger val, int shift) SunsetQuestSqrtFloat4(BigInteger x, int shift = 0, int wantedPrecision = 0)
    {
        if (x.Sign <= 0)
        {
            return x.Sign == 0 ? (0, 0) : throw new ArgumentOutOfRangeException(nameof(x));
        }

        const int SqrtHWWidth = 53;
        int xLen = (int)x.GetBitLength();
        if (wantedPrecision == 0)
        {
            wantedPrecision = xLen;
        }

        if ((xLen < 58) && (wantedPrecision < 29))  //if (x < 67108864)  // 1.5e17  (ulong)1 << 57  // maybe <30?
        {
            ulong xAsLong = (ulong)x;
            int totalLen2 = shift + xLen;

            int retShift2 = ((totalLen2 + ((totalLen2 > 0) ? 1 : 0)) / 2) - wantedPrecision;

            xAsLong <<= 64 - xLen - (totalLen2 & 1);
            ulong val2 = (ulong)Math.Sqrt(xAsLong);
            if (val2 * val2 > xAsLong)
            {
                val2--;
            }

            int downSize2 = BitOperations.Log2(val2) - wantedPrecision + 1;
            return ((BigInteger)val2 >> downSize2, retShift2);
        }

        int totalLen = shift + xLen;
        //int retShift2 = (totalLen + ((totalLen > 0) ? 1 : 0)) / 2 - wantedPrecision; // we need to floor retShift even when negitive

        int outputLen = (totalLen + ((totalLen > 0) ? 1 : 0)) / 2;
        int retShift = outputLen - wantedPrecision;
        //xxxxxxxxxxxxxxxxxxxxxxxxxxxvvvvvvvvv.    (26 bits + 9 bits = 35 bits
        //                 ooooooooooooooooooo.     outputLen = (xLenPlusShift + 1) / 2 = (35+1)/2 = 18
        //    wwwwwwwwwwwwwwwwwww.wwwwwwwwwwww     (wanted 30 bits)
        //                        >>>>>>>>>>>>     (returnShift to align decimal is -12)  30-18=12

        //////// Do H/W Sqrt first to get first 53 bits ////////
        int size = SqrtHWWidth;
        int shiftX = xLen - ((SqrtHWWidth << 1) - 1);
        if ((shift & 1) == 0)
        {
            //if ((xLen & 1) == 0)
            shiftX &= ~1;
        }
        else if ((xLen & 1) > 0)
        {
            shiftX--;
        }

        BigInteger tempX = x >> shiftX;
        double tempSqrt1 = Math.Sqrt((double)(tempX >> 8)); //todo: remove >> 8? (seems to go faster with 8)
        BigInteger val = (BitConverter.DoubleToInt64Bits(tempSqrt1) & 0xfffffffffffffL) | 0x10000000000000L;
        BigInteger valSq = val * val;
        if (valSq > tempX)
        {
            val--;
        }
        else if ((valSq + (2 * val) + 1) <= tempX)
        {
            val++;
        }

        //////  we can do a few Newtons at this point (The impact is very minor but gives us more starting point options)(For super larger numbers this can even be repeated once more to give us more options but the returns diminish.) //////// 
        for (int i = 0; i < 3; i++)
        {
            if (size >= wantedPrecision)
            {
                return (val >> (size - wantedPrecision), retShift);
            }
            int temp2a = shiftX - size + 1;
            BigInteger temp4a = (x >> temp2a) / val;
            val = (val << (size - 1)) + temp4a;
            size *= 2;
            shiftX -= size;

            if ((val * val) > (x >> shiftX))
            {
                val--;
            }

        }

        ////// Figure out a good starting size //////// 
        int wantedOutputPrecisionMinus1 = wantedPrecision - 1;
        int temp62 = wantedOutputPrecisionMinus1 / size;
        int numOfNewtonSteps = BitOperations.Log2((uint)temp62) + 1;
        int wantedSize = (wantedOutputPrecisionMinus1 >> numOfNewtonSteps) + 1;

        //////  Apply Starting Size  ////////
        int needToShiftBy = size - wantedSize;
        val >>= needToShiftBy;
        size = wantedSize;
        shiftX += needToShiftBy << 1;

        ////////  newton iterations 2+ ////////
        BigInteger topOfX = x >> shiftX;
        BigInteger valSquared = (val * val);
        while (size < wantedPrecision)
        {
            BigInteger valSU = (topOfX - valSquared) << (size - 1);
            BigInteger div = (valSU) / (val);
            val = ((val << (size + 0)) + div) + 1;
            size <<= 1;

            shiftX -= size;
            topOfX = x >> shiftX;

            // Check if val is too large.
            valSquared = (val * val);
            if (valSquared > topOfX)
            {
                val--;
                valSquared -= (2 * val) + 1;
                if (valSquared > topOfX)
                {
                    val--;
                    valSquared -= (2 * val) + 1;
                }
            }
        }

        // Verify Results
        //tempX = x << (-retShift * 2 + 0);
        //BigInteger tmp1 = val * val;
        //if ((tmp1) > tempX)
        //    Console.WriteLine($"val^2 ({tmp1}) < x({tempX})  off%:{((double)(tmp1)) / (double)tempX}");
        //else if (((val + 1) * (val + 1)) <= tempX)
        //    Console.WriteLine($"(val+1)^2({((val + 1) * (val + 1))}) >= x({tempX})");

        return (val >> (size - wantedPrecision), retShift);
    }



    // This Sqrt function uses the double hardware. It is limited to about: x < 57 bits, ShiftAmt < 500, wantedPrecision < 54
    // This function is mostly for verification.
    // Created: Created by Ryan Scott White 12-23-2021
    public static (BigInteger val, int shift) SetsetQuestSqrtUsingDoubles(BigInteger x, int shift, int wantedPrecision)
    {
        if (x.Sign <= 0)
        {
            return x.Sign == 0 ? (0, 0) : throw new ArgumentOutOfRangeException(nameof(x));
        }

        int xLen = (int)x.GetBitLength();

        if (wantedPrecision <= 0)
        {
            wantedPrecision = wantedPrecision == 0 ? xLen : throw new ArgumentOutOfRangeException(nameof(wantedPrecision));
        }

        // apply shift amount
        double xWithShift = (double)x * Math.Pow(2, shift); //todo: use shifts

        double ans = Math.Sqrt(xWithShift);

        // get the length of the answer
        double ansLen = Math.Log2(ans);

        // figure out how much we need to shift the answer by
        int retShift = (int)Math.Round(wantedPrecision - ansLen, 0, MidpointRounding.ToNegativeInfinity);

        //Console.WriteLine($"{totalLen&1} {x&1} {upshift&1} {shift&1} {wantedPrecision&1} {x.Sign} {(x.IsPowerOfTwo?1:0)} {upshift} {upshift+correct}")

        int totalLen = xLen + shift;


        retShift -= totalLen & (x.IsPowerOfTwo ? 1 : 0);

        BigInteger val = (BigInteger)(ans * Math.Pow(2, retShift));//todo: use shifts


        BigInteger tempX = x << (shift + (2 * retShift));
        BigInteger valSq = val * val;
        if (valSq > tempX)
        {
            val--;
        }
        else if ((valSq + (2 * val) + 1) <= tempX)
        {
            val++;
        }


        return (val, -retShift);
    }
}

