using System.Diagnostics;
using System.Numerics;

namespace FactorieleEval;

public static class FactEval
{
    public static double FactorieleRecursive(double a)
    {
        if (a < 0) throw new ArgumentException($"Može se izračunati faktorijela samo prirodnih brojeva! {a} nije prirodan");

        if (a == 0) return 1;
        if (a > 5000) return 1;


        return a * FactorieleRecursive(a - 1);
    }

    public static double FactorieleOptimized(double a)
    {
        double vrijednost = 1;
        for (ulong i = 2; i <= a; i++)
        {
            vrijednost *= i;
        }

        return vrijednost;
    }

    public static double FactorieleStirling(double a)
    {

        double b = Math.Sqrt(2 * Math.PI * a) * Math.Pow((a / Math.E), a);

        return b;
    }

    public static BigInteger FactorieleBigInt(double a)
    {
        BigInteger vrijednost = 1;
        for (ulong i = 2; i <= a; i++)
        {
            vrijednost *= i;
        }
        return vrijednost;
    }

    public static (double, double) MessureExecTimeMs(Func<double, double> fact, double a)
    {
        Stopwatch sw = Stopwatch.StartNew();

        double factResult = 0;

        int iters = 100*1000;
        for (int i = 0; i < iters; i++)
        {
            factResult = fact(a);
        }

        sw.Stop();

        long nanoSec = ((1000L * 1000L * 1000L) / Stopwatch.Frequency) * sw.ElapsedTicks / iters;

        Console.WriteLine($"Vrijeme: {nanoSec * 1e-3:N3} µs");

        return (nanoSec * 1e-6, factResult);
    }
}
