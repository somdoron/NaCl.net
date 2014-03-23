using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nacl;

namespace NaCl.PerformanceTests
{
  class Program
  {
    static void Main(string[] args)
    {
      ScalarMultiplicationTest();

      Console.ReadLine();
    }

    static private void ScalarMultiplicationTest()
    {
      byte[] key = new byte[32];      

      for (int i = 0; i < 32; i++)
      {
        key[i] = 42;
      }

      byte[] result = new byte[32];

      const int iterations = 100;

      // load the caches
      for (int i = 0; i < 100; i++)
      {
        ScalarMultiplication.MultiplyBase(result, key);
      }

      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = 0; i < iterations; i++)
      {
        ScalarMultiplication.MultiplyBase(result, key);  
      }
      
      stopwatch.Stop();

      Console.WriteLine("{0} took {1} ms avg or {2} microseconds", iterations, 
        stopwatch.ElapsedMilliseconds, stopwatch.ElapsedMilliseconds * 1000.0  / iterations);      
    }
  }
}
