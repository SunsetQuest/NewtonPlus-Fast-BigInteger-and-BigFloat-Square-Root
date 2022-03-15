// Copyright Ryan Scott White. 3/2022
// Relased under the MIT License. Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.



using System;
using System.Numerics;
using System.Threading.Tasks;
using static BigFloatingPointSquareRoot;

public static class Test_BigFloat
{
    public static void TestBigFloatSqrt()
    {
        int count = 0;
        _ = Parallel.For(0, 4, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (p, s) =>
        {
            Random random = new(p + 4); // new(3);
            int pass = 0;

            if (p == 0)
            {
                if ((0, 5) != SunsetQuestSqrtFloatUsingIntVersion(0, 5, 5))
                {
                    Console.WriteLine("!!!!!! Failed with X being 0 !!!!!!");
                }

                if ((6, 1) != SunsetQuestSqrtFloatUsingIntVersion(5, 5, 0))
                {
                    Console.WriteLine("!!!!!! Failed with WantedPrecision being 0 !!!!!!");
                }
            }

            for (long thisCount = 0; thisCount < long.MaxValue; thisCount++)
            {
                count++;
                //int shiftAmt = random.Next(-random.Next(2, random.Next(2, 1000)), random.Next(2, random.Next(2, 1000)));
                //int wantedPrecision = random.Next(0, random.Next(2, random.Next(2, 1000)));
                //var buffer = new byte[random.Next(1, random.Next(2, random.Next(2, 1000)))];

                int shiftAmt = random.Next(870, 900);
                int wantedPrecision = random.Next(50, 55);
                byte[] buffer = new byte[random.Next(3, 8)];

                random.NextBytes(buffer);
                BigInteger x = BigInteger.Abs(new BigInteger(buffer));

                if (random.Next() < 2000)
                {
                    Console.WriteLine($"Thread:{p} Random sample: ({x.GetBitLength()}, {shiftAmt}, {wantedPrecision}) Tests Performed:{count}");
                }

                (BigInteger val, int shift) ver5 = SunsetQuestSqrtFloat5(x, shiftAmt, wantedPrecision);
                (BigInteger val, int shift) ver4 = SunsetQuestSqrtFloat4(x, shiftAmt, wantedPrecision);
                (BigInteger val, int shift) org1 = SunsetQuestSqrtFloatUsingIntVersion(x, shiftAmt, wantedPrecision);
                (BigInteger val, int shift) dbl1 = ver5;
                if (x.GetBitLength() < 55 && Math.Abs(shiftAmt) < 900 && wantedPrecision < 55)
                {
                    dbl1 = SetsetQuestSqrtUsingDoubles(x, shiftAmt, wantedPrecision); //Limited to about: x < 55 bits, abs(ShiftAmt) < 900, wantedPrecision < 55, 
                }

                if (ver5 == ver4 && ver5 == org1 && ver5 == dbl1)
                {
                    pass++;
                }
                else
                {
                    Console.WriteLine($"{x}, {shiftAmt}, {wantedPrecision} [xLenOdd:{x.GetBitLength() & 1}  shiftOdd:{shiftAmt & 1}] --> {pass / (double)count}");
                    Console.WriteLine($"V5:       {ver5}\r\nV4:       {ver4}\r\nOriginal: {org1}\r\nDoubles:  {dbl1}");
                }
            }
        });
    }
}
