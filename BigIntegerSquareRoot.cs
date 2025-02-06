// Copyright Ryan Scott White. 2022, 2025
// Released under the MIT License. Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#pragma warning disable IDE0051, IDE0050, CA1050 // Ignore unused code and missing namespace
using System;
using System.Numerics;

public static class BigIntegerSquareRoot
{
    public static BigInteger SunsetQuestSqrt(BigInteger x)
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

        val = (val << (106 - 1)) + (x >> xLenMod - (3 * 106)) / val;
        val = (val << (212 - 1)) + (x >> xLenMod - (3 * 212)) / val;
        int size = 424;

        if (xAsDub < 4e254)
        {
            /////// There are a few extra digits here, lets save them ///////
            int oversidedBy = size - wantedPrecision;
            //ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);

            uint simple = (uint)((val >> (oversidedBy - 8)) & byte.MaxValue);

            ////////  Shrink result to wanted Precision  ////////
            val >>= oversidedBy;

            ////////  Detect a round-ups  ////////
            if ((simple == 0) && (val * val > x))
            {

                //Console.WriteLine(MiscTools.ToBinaryString2(simple));
                val--;
            }
        }
        else if (xLen > 1 << 15)
        {
            val = (val << (424 - 1)) + (x >> xLenMod - (3 * 424)) / val;
            size <<= 1;

            const int EXTRA_BITS_TO_REMOVE = 2; // 0=fails, 1=SLOW!!! 2=OK (14 hours of testing)

            ////////  Apply Starting Size  ////////
            //int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;
            int startingSize = wantedPrecision + 0; //6
            int numOfNewtonSteps = 0;
            while (startingSize > size)
            {
                startingSize = (startingSize >> 1) + EXTRA_BITS_TO_REMOVE;
                numOfNewtonSteps++;
            }

            int needToShiftBy = size - startingSize;
            val >>= needToShiftBy;
            size = startingSize;
            int finalSize = wantedPrecision + (4 << (numOfNewtonSteps)) + 4;
            int xInv2Shift = finalSize - xLen % 2 - 64;

            BigInteger xInv = InverseForLargeNumbersOnly(x, xLen, finalSize - 64);

            ////////  Divide-less Iterations  ////////
            BigInteger THREE = ((BigInteger)3);
            do
            {
                size <<= 1;
                BigInteger xInvShift2 = xInv >> (xInv2Shift - size);
                //if (xInv2Shift - size< 67) Console.WriteLine(xInv2Shift - size);
                BigInteger valSqrd = BigInteger.Pow(val, 2);
                BigInteger rightSide = (THREE << size) - ((xInvShift2 * valSqrd) >> size);
                val = (val * rightSide) >> ((size >> 1) + 1 + EXTRA_BITS_TO_REMOVE);

                size -= EXTRA_BITS_TO_REMOVE;
                //needToShiftBy = EXTRA_BITS_TO_REMOVE;
                //val >>= needToShiftBy;
            } while (size < wantedPrecision);

            val >>= size - wantedPrecision /*+ needToShiftBy*/;
        }
        else
        {
            int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;

            //////  Apply Starting Size  ////////
            int startingSize = (wantedPrecision >> numOfNewtonSteps) + 2;
            int needToShiftBy = size - startingSize;
            val >>= needToShiftBy;
            size = startingSize;
            do
            {
                ////////  Newton Plus Iterations  ////////
                int shiftX = xLenMod - (3 * size);
                BigInteger valSqrd = (val * val) << (size - 1);
                BigInteger valSU = (x >> shiftX) - valSqrd;
                val = (val << size) + (valSU / val);
                size <<= 1;

            } while (size < wantedPrecision);

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






    private static BigInteger InverseForLargeNumbersOnly(BigInteger x, int xLen, int finalSize)
    {
        //////// Get the Inverse of X ////////
        BigInteger xInv;
        // Tuning constants                          
        const int EXTRA_START = 2; //seems faster with 2 vs 0
        const int START_CUTOFF = 400;
        const int NEWTON_CUTOFF = 800;
        const int EXTRA_TO_REMOVE1 = 2; // error defected in sqrt if 0
        const int EXTRA_TO_REMOVE2 = 1; // Errors on large numbers if removed!
        const int BOOST_LARGER_NUMS = 2;
        
        ////////  Get Starting Size  ////////
        int desiredStartSize = finalSize + 1 + (EXTRA_START * 2);
        while (desiredStartSize > START_CUTOFF)
        {
            desiredStartSize = ((desiredStartSize + 1) >> 1) + BOOST_LARGER_NUMS;
        }
        int curSize = desiredStartSize;

        BigInteger scaledOne2 = BigInteger.One << ((curSize << 1) + (EXTRA_START * 2));
        BigInteger result = scaledOne2 / (x >> (xLen - curSize - 1 - EXTRA_START));
        curSize += EXTRA_START;

        ////////////////////// Newton Inverse  //////////////////////
        do
        {
            int doubleCurSize = curSize << 1;

            BigInteger scalingFactor = BigInteger.One << (doubleCurSize + 1);
            BigInteger xTimesY = ((x >> (xLen - doubleCurSize)) * result) >> (curSize - 1);
            // future: we only need the bottom half of xTimesY.
            BigInteger twoMinusXy = scalingFactor - xTimesY;
            result = (result * twoMinusXy) >> (curSize + EXTRA_TO_REMOVE1);
            curSize = doubleCurSize - EXTRA_TO_REMOVE1;

        } while (curSize <= NEWTON_CUTOFF);

        // Lets make sure we are 100% accurate at this point - back off until we see both a 0 and 1
        int reduceBy2 = (int)BigInteger.TrailingZeroCount(result.IsEven ? result : (~result)) + 1;
        if (reduceBy2 < 32) // 32 is flexible
        {
            result >>= reduceBy2;
            curSize -= reduceBy2;
        }
        else
        {
            // if we have something with many trailing zeros or ones, lets fallback to the safe classic
            // method to ensure correctness.
            xInv = (BigInteger.One << (xLen + ((finalSize + 1 == 0) ? xLen : finalSize + 1) - 1)) / x;
            return xInv;
        }

        ////////////////////// NewtonPlus Inverse  //////////////////////
        while (true)
        {
            int doubleCurSize = curSize << 1;

            // We need insert our "1" in the middle, we do this by incrementing the upper half with a 1
            result++; // we could just do a add a "(1 << doublecurSize)"
            BigInteger mask = (BigInteger.One << (curSize + 1)) - 1;
            BigInteger xTimesY = ((x >> (xLen - doubleCurSize)) * result) >> (curSize - 1); // future: we only need the bottom half of this.

            if (doubleCurSize - EXTRA_TO_REMOVE2 > finalSize + 1) // maybe remove EXTRA_TO_REMOVE2
            {
                xInv = ((result << (2 * curSize)) - (result * (xTimesY & mask))) >> (3 * curSize - finalSize - 1);  // can we subtract out a 'curSize'
                return xInv;
            }
            result = ((result << (doubleCurSize)) - (result * (xTimesY & mask))) >> (curSize + EXTRA_TO_REMOVE2);
            curSize = doubleCurSize - EXTRA_TO_REMOVE2;

            //// back off until we see both a zero and one
            int reduceBy = (int)BigInteger.TrailingZeroCount(result.IsEven ? result : ~result) + 1;
            if (reduceBy < 100)
            {
                result >>= reduceBy;
                curSize -= reduceBy;
            }
        }
    }

    private static string Bits(BigInteger val)
    {
        return $"{MiscTools.ToBinaryString(val, 0)} [{val.GetBitLength()}]";
    }


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
        //int xLen = (int)Math.Ceiling(BigInteger.Log(x, 2));

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
            //BigInteger temp1 = (val << (size - 1));
            //int temp2 = (xLenMod - (3 * size));
            //BigInteger temp3 = (x >> temp2);
            //BigInteger temp4 = temp3 / val;
            //val = temp1 + temp4;
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
