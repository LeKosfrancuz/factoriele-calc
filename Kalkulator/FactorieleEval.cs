namespace FactorieleEval;

public static class FactEval
{
    // TODO (Ian): Implementiraj 3 vrste računanja faktorijela
    public static double FactorieleRecursive(double a)
    {
        int startTime = DateTime.Now.Millisecond;

        if (a == 0)
        {
            int totalTime = DateTime.Now.Millisecond - startTime;
            return 1;
        }
        else
        {
            int totalTime = DateTime.Now.Millisecond - startTime;
            return a * FactorieleRecursive(a - 1);
        }
    }
    public static (double, int) FactorieleOptimized(double a)
    {
        int startTime = DateTime.Now.Millisecond;

        int vrijednost = 1;
        for (int i = 2; i <= a; i++)
        {
            vrijednost *= i;
        }

        int totalTime = DateTime.Now.Millisecond - startTime;


        return (vrijednost, totalTime);
    }
    public static (double, int) FactorieleStirling(double a)
    {
        int startTime = DateTime.Now.Millisecond;

        double b = Math.Sqrt(2 * Math.PI * a) * Math.Pow((a / Math.E), a);

        int totalTime = DateTime.Now.Millisecond - startTime;


        return (b, totalTime);
    }
}
