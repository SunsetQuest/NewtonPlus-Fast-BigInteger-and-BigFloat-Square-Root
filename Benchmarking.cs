// Copyright Ryan Scott White. 2022, 2025
// Released under the MIT License. Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

internal static class Benchmarking
{
    /// <summary>
    /// We will do several tests on each increment size. This is the percentage to keep.
    /// A value us 0.3 would keep the top 30% for each test.  We keep a top percentage because 
    /// windows processes are interrupted by context switches and we want the tests with the least 
    /// number of interruptions for the most accurate true score. 
    /// </summary>
    private const double KEEP_TOP_PROPORTION = 0.2; //0.333334;

    /// <summary>
    /// The reporting granularity is at what sizes to we want to report.
    /// Use 1.0442737824274138403219664787399 [4^(1/32)] for more detail or use "2" for 2-bit 4-bit, 8-bit, etc sizes.
    /// </summary>
    private const double REPORT_INCREMENT_SIZE = 2;

    /// <summary>
    /// This is the number of sub-tests to do for each step size.
    /// </summary>
    private const int SUBDIVISIONS_STEP_SIZE = 11;

    public static void Benchmark(Func<BigInteger, BigInteger> Sqrt, int TrialCount = 1, int Seconds = 600, bool skipSmallNumbers = true, bool displayOutput = true, bool constantTestSize = true, int maxScale = int.MaxValue)
    {
        string methodName = Sqrt.Method.Name;

        double SUB_DIV_MAGNITUDE = Math.Pow(REPORT_INCREMENT_SIZE, 1.0 / SUBDIVISIONS_STEP_SIZE);

        //IntPtr before = Process.GetCurrentProcess().ProcessorAffinity;
        //Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1 <<30); // assign to a non-busy core
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        //Process.GetCurrentProcess().PriorityBoostEnabled = true;

        Random r = new(2344218);

        Stopwatch perfTimer = new();

        BigInteger junk = new();

        Stopwatch maxTime = Stopwatch.StartNew();

        List<long> trailTimes = new();


        for (int scaleIndex = 3; scaleIndex < maxScale; scaleIndex++) //3
        {
            trailTimes.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            maxTime.Restart();

            // use for decreasing test count with number size
            int thisTrialCt = constantTestSize ? TrialCount : (int)(TrialCount / Math.Pow(scaleIndex - 2, 2.5));

            for (int trail = 0; trail < thisTrialCt; trail++)
            {
                if (maxTime.ElapsedMilliseconds > (Seconds * 1000))
                {
                    goto skip;
                }

                perfTimer.Restart();

                for (int subDivNumber = -(SUBDIVISIONS_STEP_SIZE / 2); subDivNumber <= (SUBDIVISIONS_STEP_SIZE / 2); subDivNumber++)
                {
                    int itemNumber = (scaleIndex * SUBDIVISIONS_STEP_SIZE) + subDivNumber;
                    int bitCount = (int)Math.Round(Math.Pow(SUB_DIV_MAGNITUDE, itemNumber));
                    int byteCt = 1 + (bitCount - 1) / 8;
                    int topBitPosInTopByte = ((bitCount - 1) % 8); // 0-7
                    byte[] bytes = new byte[byteCt];

                    r.NextBytes(bytes);
                    bytes[byteCt - 1] = (byte)(bytes[byteCt - 1] & ((1 << topBitPosInTopByte) - 1) | (1 << topBitPosInTopByte));
                    BigInteger x = new(bytes, true, false);

                    Thread.Sleep(0);
                    perfTimer.Start();
                    BigInteger root = Sqrt(x);
                    perfTimer.Stop();

                    //BigInteger tmp = root * root;
                    //if (tmp > x)
                    //    Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
                    //if ((tmp + 2 * root + 1) <= x)
                    //    Console.WriteLine($"(val+1)^2({((root + 1) * (root + 1))}) >= x({x})");

                    junk ^= root;

                    if (skipSmallNumbers && perfTimer.ElapsedMilliseconds < (Seconds / 2))
                    {
                        goto skip2;
                    }
                }

                trailTimes.Add(perfTimer.ElapsedTicks);
            }
        skip:
            //remove the slowest values and also removes first time JIT times
            int keepCt = (int)Math.Ceiling(trailTimes.Count * KEEP_TOP_PROPORTION);
            double avg = trailTimes.OrderBy(x => x).Take(keepCt).Average() / SUBDIVISIONS_STEP_SIZE;  // was
            // Console.WriteLine(i1.GetBitLength() + " \t" + (double)i1 + "\tR:" + sw0 + " M:" + sw1 + "  (" + totalTicks0.ToString() + " vs " + totalTicks1.ToString() + ")   Diff: " + (b21 - a01).ToString() + "," + (b22 - a02).ToString() + " j" + junk.ToString()[0]);
            int bitCount2 = (int)Math.Round(Math.Pow(Math.Pow(2, 1.0 / SUBDIVISIONS_STEP_SIZE), scaleIndex * SUBDIVISIONS_STEP_SIZE));

            if (displayOutput)
            {
                Console.WriteLine(methodName + "\t" + keepCt + "\t" + trailTimes.Count + "\t" + bitCount2 + "\t " + Math.Round(avg, 2));
            }

            if (TrialCount < 4)
            {
                if (TrialCount == trailTimes.Count && (maxTime.ElapsedMilliseconds > (Seconds * 1000)))
                {
                    break;
                }
            }
            else if (trailTimes.Count < 3)
            {
                break;
            }

        skip2:;
        }

        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
        Thread.CurrentThread.Priority = ThreadPriority.Normal;
        //Process.GetCurrentProcess().ProcessorAffinity = before;
    }


    public static void BenchmarkMathSqrt()
    {
        int TrialCount = 4000;

        const double KEEP_TOP_PROPORTION = 0.2; //0.333334;
        //IntPtr before = Process.GetCurrentProcess().ProcessorAffinity;
        //Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1 << 12);
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        //Process.GetCurrentProcess().PriorityBoostEnabled = true;


        Stopwatch perfTimer = new();

#pragma warning disable CS0219 // The variable 'funcOut' is assigned but its value is never used
        double funcOut = 0;
#pragma warning restore CS0219 // The variable 'funcOut' is assigned but its value is never used
        double junk = 123456;

        Stopwatch maxTime = Stopwatch.StartNew();

        List<long> trailTimes = new();


        for (long setIndex = 0; /*setIndex<64*/ ; setIndex++)
        {
            trailTimes.Clear();
            double funcIn = 0 + BitConverter.Int64BitsToDouble(setIndex | (long)1 << 62);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            maxTime.Restart();

            for (int trail = 0; trail < TrialCount; trail++)
            {
                perfTimer.Restart();
                //funcIn *= 0.99999;

                Thread.Sleep(0);
                perfTimer.Start();
                for (int trail2 = 0; trail2 < 11; trail2++)
                {
                    junk += Math.Sqrt(funcIn);
                }
                perfTimer.Stop();

                trailTimes.Add(perfTimer.ElapsedTicks);

            }
            //remove the slowest values and also removes first time JIT times
            int keepCt = (int)Math.Ceiling(trailTimes.Count * KEEP_TOP_PROPORTION);
            double avg = trailTimes.OrderBy(x => x).Take(keepCt).Average();
            int bitCount2 = (int)Math.Log2(funcIn);

            string binary = Convert.ToString(BitConverter.DoubleToInt64Bits(funcIn), 2).PadLeft(64, '0').Insert(12, ":").Insert(1, ":");
            Console.WriteLine("bits:" + bitCount2 + "\t" + Math.Round(avg, 2) + "\t" + binary + " " + funcIn);

            if (trailTimes.Count < 3)
            {
                break;
            }
        }

        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
        Thread.CurrentThread.Priority = ThreadPriority.Normal;
        //Process.GetCurrentProcess().ProcessorAffinity = before;
    }
}
