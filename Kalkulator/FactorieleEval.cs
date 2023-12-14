namespace FactorieleEval;

public static class FactEval
{
    // TODO (Ian): Implementiraj 3 vrste računanja faktorijela
    public static (double, int) FactorieleRecursive(double a)
    {
        int startTime = DateTime.Now.Millisecond;
        int totalTime;

        if (a < 0) throw new ArgumentException($"Može se izračunati faktorijela samo prirodnih brojeva! {a} nije prirodan");

        if (a == 0)
        {
            totalTime = DateTime.Now.Millisecond - startTime;
            return (1, totalTime);
        }
        
        (double b, _) = FactorieleRecursive(a - 1);

        totalTime = DateTime.Now.Millisecond - startTime;

        Console.WriteLine("Vrijeme: {0}ms", totalTime);

        return (a * b, totalTime);
    }
    public static (double, int) FactorieleOptimized(double a)
    {
        int startTime = DateTime.Now.Millisecond;

        double vrijednost = 1;
        for (int i = 2; i <= a; i++)
        {
            vrijednost *= i;
        }

        int totalTime = DateTime.Now.Millisecond - startTime;

        Console.WriteLine("Vrijeme: {0}ms", totalTime);

        return (vrijednost, totalTime);
    }
    public static (double, int) FactorieleStirling(double a)
    {
        int startTime = DateTime.Now.Millisecond;

        double b = Math.Sqrt(2 * Math.PI * a) * Math.Pow((a / Math.E), a);

        int totalTime = DateTime.Now.Millisecond - startTime;

        Console.WriteLine("Vrijeme: {0}ms", totalTime);

        return (b, totalTime);
    }
}
