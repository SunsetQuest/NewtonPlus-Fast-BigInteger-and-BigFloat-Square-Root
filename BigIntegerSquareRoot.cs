// Copyright Ryan Scott White. 3/2022
// Released under the MIT License. Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#pragma warning disable IDE0051, IDE0050, CA1050 // Ignore unused code and missing namespace
using System;
using System.Numerics;
using System.Reflection;

public static class BigIntegerSquareRoot
{
    public static BigInteger NewtonPlusSqrt(BigInteger x)
    {
        if (x < 144838757784765629)    // 1.448e17 = ~1<<57
        {
            uint vInt = (uint)Math.Sqrt((ulong)x);
            if ((x >= 4503599761588224) && ((ulong)vInt * vInt > (ulong)x))  // 4.5e15 =  ~1<<52
            {
                vInt--;
            }
            return vInt;
        }

        double xAsDub = (double)x;
        if (xAsDub < 8.5e37)   //  long.max*long.max
        {
            ulong vInt = (ulong)Math.Sqrt(xAsDub);
            BigInteger v = (vInt + ((ulong)(x / vInt))) >> 1;
            return (v * v <= x) ? v : v - 1;
        }

        if (xAsDub < 4.3322e127)
        {
            BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
            v = (v + (x / v)) >> 1;
            if (xAsDub > 2e63)
            {
                v = (v + (x / v)) >> 1;
            }
            return (v * v <= x) ? v : v - 1;
        }

        int xLen = (int)x.GetBitLength();
        int wantedPrecision = (xLen + 1) / 2;
        int xLenMod = xLen + (xLen & 1) + 1;

        //////// Do the first Sqrt on hardware ////////
        long tempX = (long)(x >> (xLenMod - 63));
        double tempSqrt1 = Math.Sqrt(tempX);
        ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
        if (valLong == 0)
        {
            valLong = 1UL << 53;
        }

        ////////  Classic Newton Iterations ////////
        BigInteger val = ((BigInteger)valLong << 52) + (x >> xLenMod - (3 * 53)) / valLong;
        int size = 106;
        for (; size < 256; size <<= 1)
        {
            val = (val << (size - 1)) + (x >> xLenMod - (3 * size)) / val;
        }

        if (xAsDub > 4e254) // 4e254 = 1<<845.76973610139
        {
            int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;

            //////  Apply Starting Size  ////////
            int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
            int needToShiftBy = size - wantedSize;
            val >>= needToShiftBy;
            size = wantedSize;
            do
            {
                ////////  Newton Plus Iterations  ////////
                int shiftX = xLenMod - (3 * size);
                BigInteger valSqrd = (val * val) << (size - 1);
                BigInteger valSU = (x >> shiftX) - valSqrd;
                val = (val << size) + (valSU / val);
                size *= 2;

            } while (size < wantedPrecision);
        }

        /////// There are a few extra digits here, lets save them ///////
        int oversidedBy = size - wantedPrecision;
        BigInteger saveDroppedDigitsBI = val & ((BigInteger.One << oversidedBy) - 1);
        int downby = (oversidedBy < 64) ? (oversidedBy >> 2) + 1 : (oversidedBy - 32);
        ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);


        ////////  Shrink result to wanted Precision  ////////
        val >>= oversidedBy;


        ////////  Detect a round-ups  ////////
        if ((saveDroppedDigits == 0) && (val * val > x))
        {
            val--;
        }

        ////////// Error Detection ////////
        //// I believe the above has no errors but to guarantee the following can be added.
        //// If an error is found, please report it.
        //BigInteger tmp = val * val;
        //if (tmp > x)
        //{
        //    Console.WriteLine($"Missed  , {ToolsForOther.ToBinaryString(saveDroppedDigitsBI, oversidedBy)}, {oversidedBy}, {size}, {wantedPrecision}, {saveDroppedDigitsBI.GetBitLength()}");
        //    if (saveDroppedDigitsBI.GetBitLength() >= 6)
        //        Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
        //    //throw new Exception("Sqrt function had internal error - value too high");
        //}
        //if ((tmp + 2 * val + 1) <= x)
        //{
        //    Console.WriteLine($"(val+1)^2({((val + 1) * (val + 1))}) >= x({x})");
        //    //throw new Exception("Sqrt function had internal error - value too low");
        //}

        return val;
    }


    public static BigInteger SunsetQuestSqrtO2(BigInteger x)
    {
        if (x < 144838757784765629)    // 1.448e17 = ~1<<57
        {
            uint vInt = (uint)Math.Sqrt((ulong)x);
            if ((x >= 4503599761588224) && ((ulong)vInt * vInt > (ulong)x))  // 4.5e15 =  ~1<<52
            {
                vInt--;
            }
            return vInt;
        }

        double xAsDub = (double)x;
        if (xAsDub < 8.5e37)   //  long.max*long.max
        {
            ulong vInt = (ulong)Math.Sqrt(xAsDub);
            BigInteger v = (vInt + ((ulong)(x / vInt))) >> 1;
            return (v * v <= x) ? v : v - 1;
        }

        if (xAsDub < 4.3322e127)
        {
            BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
            v = (v + (x / v)) >> 1;
            if (xAsDub > 2e63)
            {
                v = (v + (x / v)) >> 1;
            }
            //if (v * v > x)
            //    Console.WriteLine($"{(v * v) > x}, {v * v}, {x}, {x - (v * v)}, {(double)(x - (v * v)).GetBitLength() / x.GetBitLength()},  {2 * (x - (v * v)).GetBitLength()}/{x.GetBitLength()}  ");

            return (v * v <= x) ? v : v - 1;
        }

        int xLen = (int)x.GetBitLength();
        int wantedPrecision = (xLen + 1) / 2;
        int xLenMod = xLen + (xLen & 1) + 1;

        //////// Do the first Sqrt on hardware ////////
        long tempX = (long)(x >> (xLenMod - 63));
        double tempSqrt1 = Math.Sqrt(tempX);
        ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
        if (valLong == 0)
        {
            valLong = 1UL << 53;
        }

        //if (((BigInteger)valLong * valLong) > (x >> (xLenMod - 106))) valLong--;

        ////////  Classic Newton Iterations ////////
        BigInteger val = ((BigInteger)valLong << (53 - 1)) + (x >> xLenMod - (3 * 53)) / valLong;
        int size = 106;
        for (; size < 256; size <<= 1)
        {
            val = (val << (size - 1)) + (x >> xLenMod - (3 * size)) / val;
        }

        //BigInteger tempVal = val >> (size - wantedPrecision); if ((tempVal * tempVal) > x) val--;

        if (xAsDub > 4e254) // 4e254 = 1<<845.76973610139
        {
            int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 1; // technically should be "(wantedPrecision-1)/size" but faster without

            //////  Apply Starting Size  ////////
            int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
            int needToShiftBy = size - wantedSize;
            val >>= needToShiftBy;
            size = wantedSize;
            do
            {
                ////////  Newton Plus Iterations  ////////
                int shiftX = xLenMod - (3 * size);
                BigInteger valSqrd = (val * val) << (size - 1);
                BigInteger valSU = (x >> shiftX) - valSqrd;
                val = (val << size) + (valSU / val);
                size *= 2;

            } while (size < wantedPrecision);
        }


        ////////  Shrink result to wanted Precision  ////////
        val >>= size - wantedPrecision;


        ////////  Detect a round-ups  ////////
        // for a ~7% speed bump the following line can be removed but round-ups WILL occur.
        //if ((val * val) > x)
        if (val * val > x)
        {
            val--;
        }


        ////////// Error Detection ////////
        //// I believe the above has no errors but to guarantee the following can be added.
        //// If an error is found, please report it.
        //BigInteger tmp = val * val;
        //if (tmp > x)
        //{
        //    Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
        //    //throw new Exception("Sqrt function had internal error - value too high");
        //}
        //if ((tmp + 2 * val + 1) <= x)
        //{
        //    Console.WriteLine($"(val+1)^2({((val + 1) * (val + 1))}) >= x({x})");
        //    //throw new Exception("Sqrt function had internal error - value too low");
        //}

        return val;
    }












    public static BigInteger SunsetQuestSqrtO(BigInteger x)
    {
        if (x < 144838757784765629)    // 1.448e17 = ~1<<57
        {
            uint vInt = (uint)Math.Sqrt((ulong)x);
            if ((x >= 4503599761588224) && ((ulong)vInt * vInt > (ulong)x))  // 4.5e15 =  ~1<<52
            {
                vInt--;
            }
            return vInt;
        }

        double xAsDub = (double)x;
        if (xAsDub < 8.5e37)   //  long.max*long.max
        {
            ulong vInt = (ulong)Math.Sqrt(xAsDub);
            BigInteger v = (vInt + ((ulong)(x / vInt))) >> 1;
            return (v * v <= x) ? v : v - 1;
        }

        if (xAsDub < 4.3322e127)
        {
            BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
            v = (v + (x / v)) >> 1;
            if (xAsDub > 2e63)
            {
                v = (v + (x / v)) >> 1;
            }
            //if (v * v > x)
            //    Console.WriteLine($"{(v * v) > x}, {v * v}, {x}, {x - (v * v)}, {(double)(x - (v * v)).GetBitLength() / x.GetBitLength()},  {2 * (x - (v * v)).GetBitLength()}/{x.GetBitLength()}  ");

            return (v * v <= x) ? v : v - 1;
        }

        int xLen = (int)x.GetBitLength();
        int wantedPrecision = (xLen + 1) / 2;
        int xLenMod = xLen + (xLen & 1) + 1;

        //////// Do the first Sqrt on hardware ////////
        long tempX = (long)(x >> (xLenMod - 63));
        double tempSqrt1 = Math.Sqrt(tempX);
        ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
        if (valLong == 0)
        {
            valLong = 1UL << 53;
        }

        //if (((BigInteger)valLong * valLong) > (x >> (xLenMod - 106))) valLong--;

        ////////  Classic Newton Iterations ////////
        BigInteger val = ((BigInteger)valLong << (53 - 1)) + (x >> xLenMod - (3 * 53)) / valLong;
        int size = 106;
        for (; size < 256; size <<= 1)
        {
            val = (val << (size - 1)) + (x >> xLenMod - (3 * size)) / val;
        }

        //BigInteger tempVal = val >> (size - wantedPrecision); if ((tempVal * tempVal) > x) val--;

        if (xAsDub > 4e254) // 4e254 = 1<<845.76973610139
        {
            int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 1; // technically should be "(wantedPrecision-1)/size" but faster without

            //////  Apply Starting Size  ////////
            int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
            int needToShiftBy = size - wantedSize;
            val >>= needToShiftBy;
            size = wantedSize;
            do
            {
                ////////  Newton Plus Iterations  ////////
                int shiftX = xLenMod - (3 * size);
                BigInteger valSqrd = (val * val) << (size - 1);
                BigInteger valSU = (x >> shiftX) - valSqrd;
                val = (val << size) + (valSU / val);
                size *= 2;

            } while (size < wantedPrecision);
        }


        ////////  Shrink result to wanted Precision  ////////
        val >>= size - wantedPrecision;


        ////////  Detect a round-ups  ////////
        // for a ~7% speed bump the following line can be removed but round-ups WILL occur.
        // This function can be further optimized - see article. 
        //if ((val * val) > x)
        if (val * val > x)
        {
            val--;
        }


        ////////// Error Detection ////////
        //// I believe the above has no errors but to guarantee the following can be added.
        //// If an error is found, please report it.
        //BigInteger tmp = val * val;
        //if (tmp > x)
        //{
        //    Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
        //    //throw new Exception("Sqrt function had internal error - value too high");
        //}
        //if ((tmp + 2 * val + 1) <= x)
        //{
        //    Console.WriteLine($"(val+1)^2({((val + 1) * (val + 1))}) >= x({x})");
        //    //throw new Exception("Sqrt function had internal error - value too low");
        //}

        return val;
    }

    // NewtonPlus v6 - 1/22/2022 
    //    The instead of doing the full v-squared in "v-squared - x" we could just chop off the front of v-squared
    //    and x since they are the same.
    //    We can just subtract the bottom half of v-squared from some middle portion of x.
    //    The below is an attempt to this however the .net BigInteger class does not have something that will only 
    //    allow us to calculate the bottom half. There is a ModPow that can kind of be used for this but it is not 
    //    super efficient. The below also has an error that needs to be worked out as well.    
    /// <summary>
    /// Performs a Single Newton Plus iteration step.
    /// </summary>
    /// <param name="x">The input. The value we are trying to find the Sqrt of.</param>
    /// <param name="xLenMod">It is Len(X) rounded up to the next even number. It is just calculated once.</param>
    /// <param name="size">The maintained current size of val so it does not need to be recalculated.</param>
    /// <param name="val">The current result. With each call to this function it doubles in size(and precision).</param>
    static void NewtonPlus(BigInteger x, int xLenMod, ref int size, ref BigInteger val)
    {
        int shiftX = xLenMod - (3 * size);
        BigInteger valSqrd = (val * val) << (size - 1);
        BigInteger valSU = (x >> shiftX) - valSqrd;
        val = (val << size) + (valSU / val);
        size *= 2;
    }


    // This was added just to compare with the Newton Plus.
    static void NewtonClassic(BigInteger x, int xLenMod, ref int size, ref BigInteger val)
    {
        BigInteger tempX = x >> xLenMod - (3 * size) + 1;
        val = (val << (size - 1)) + tempX / val;
        size <<= 1;
    }

}
