using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using nacl;

namespace NaCl.PerformanceTests
{
  class Program
  {
    static void Main(string[] args)
    {
      //ScalarMultiplication64Test();

      ScalarMultiplication32Test();

      Console.ReadLine();     
    }

    static private void ScalarMultiplication32Test()
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
        ScalarMultiplication32.MultiplyBase(result, key);
      }

      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = 0; i < iterations; i++)
      {
        ScalarMultiplication32.MultiplyBase(result, key);
      }

      stopwatch.Stop();

      Console.WriteLine("32bit avg {0} microseconds", stopwatch.ElapsedMilliseconds * 1000.0 / iterations);
    }

    static private void ScalarMultiplication64Test()
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
        ScalarMultiplication64.MultiplyBase(result, key);
      }

      Stopwatch stopwatch = Stopwatch.StartNew();

      for (int i = 0; i < iterations; i++)
      {
        ScalarMultiplication64.MultiplyBase(result, key);  
      }
      
      stopwatch.Stop();

      Console.WriteLine("64bit avg {0} microseconds", stopwatch.ElapsedMilliseconds * 1000.0 / iterations);
    }
  }
}
