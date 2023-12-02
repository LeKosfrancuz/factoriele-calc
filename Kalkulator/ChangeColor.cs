namespace InputParsing;


public static class ChangeColor
{
    public static void WhiteTX(string s)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(s);
        Console.ResetColor();
    }

    public static void GrayTX(string s)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(s);
        Console.ResetColor();
    }

    public static void RedBG(string s)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write(s);
        Console.ResetColor();
    }

    public static void RedTX(string s)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(s);
        Console.ResetColor();
    }

    public static void GreenBG(string s)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.Write(s);
        Console.ResetColor();
    }

    public static string YellowTX(string s)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(s);
        Console.ResetColor();
        return "";
    }

    public static string GreenBGLine(string s)
    {
        GreenBG(s);
        Console.WriteLine();
        return "";
    }

    public static string RedBGLine(string s)
    {
        RedBG(s);
        Console.WriteLine();
        return "";
    }
}
