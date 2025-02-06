// Copyright Ryan Scott White. 3/2022
// Released under the MIT License. Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


#pragma warning disable IDE0051, IDE0050 // Ignore unused code and missing namespace
using System;
using static Benchmarking;
using static BigIntegerSquareRoot;
using static BigIntegerSquareRootOthers;
using static Test_BigFloat;
using static Test_BigIntegers;

internal static class Program
{
    private static void Main()
    {
#if !BENCHMARK1CORE
        //////////////////////  Play Test Area  //////////////////////

        // Testing...
        TestBigIntegerSqrt(SunsetQuestSqrt, testTimeInSeconds: 10, randomMaxBitSize: 40000, runIndefinitely: true);
        //Console.WriteLine("Any key to continue or exit."); Console.ReadLine();
#endif

#if BENCHMARK1CORE
        ////////////////////  Play Benchmark Area  //////////////////////
        Console.WriteLine($"methodName\tkeepCt\ttrails\tBits\tNanoseconds");

        //Quick compare benchmark between past and new versions

        while (true)
        {
            Benchmark(SunsetQuestSqrt, trialCount: 20, seconds: 2, skipSmallNumbers: false);
            Benchmark(NewtonPlusSqrt, trialCount: 20, seconds: 2, skipSmallNumbers: false);
            Benchmark(SunsetQuestSqrt, trialCount: 20, seconds: 2, skipSmallNumbers: false);
            Benchmark(NewtonPlusSqrt, trialCount: 20, seconds: 2, skipSmallNumbers: false);
        }
        Console.ReadLine();
        // Full compare benchmark between past and new versions
        for (int i = 0; i < int.MaxValue; i++)
        {
            Benchmark(SunsetQuestSqrt);
            Benchmark(NewtonPlusSqrt);
            Benchmark(MaxKlaxxSqrt);
            Benchmark(JimSqrt);
            Benchmark(RedGreenCodeSqrt);
            Benchmark(NordicMainframeSqrt);
            //Benchmark(NordicMainframeOptimizedSqrt);
            Benchmark(MichaelSteinerSqrt);
            Benchmark(SchultkeSqrt);
            Benchmark(JavaSqrt);
            //Benchmark(KahanSqrt);
            Benchmark(EdwardFalkSqrt);
        }
#endif
    }
}
