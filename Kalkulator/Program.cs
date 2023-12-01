using System.Diagnostics;
using InputParsing;

internal class Kalkulator
{
    static void Main()
    {
        List<Varijabla> Varijable = new();
        

        ///                         KOMANDNA LINIJA

        int exitProgram = 1;

        
        ChangeColor.GrayTX("Dobro došli u ");
        ChangeColor.WhiteTX("Kalulator za faktoriele\n");
        ChangeColor.GrayTX("Upišite HELP za listu komandi\n");
       
        Console.CancelKeyPress += new ConsoleCancelEventHandler(ctrlCRukovanje);

        while (exitProgram != (int)returnFlagsParser.SaveToFile && exitProgram != (int)returnFlagsParser.exitProgram)
        {
            ChangeColor.WhiteTX(">"); // prompt
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string userInput = Console.ReadLine() + "";

                userInput = CommandColor.LineColoring(userInput, Varijable);

                Console.ForegroundColor = ConsoleColor.White;
                exitProgram = UserInputParser.StringToKalkulatorParser(userInput, Varijable);
            }
            catch (ArgumentException e) { ChangeColor.RedTX("Greška: "); Console.WriteLine(e.Message); }
            catch (InvalidOperationException e) { ChangeColor.RedTX("Greška: "); Console.WriteLine(e.Message); }
            catch (FormatException e) { ChangeColor.RedTX("Greška: "); Console.WriteLine(e.Message); }
        }

        Credits();

    }

    private static void Credits()
    {
        char cj = '\u0107';
        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine($"\nNapravili Ian Klari{cj}, Mateo Kos i Ozana Tomljanovi{cj}");
        Console.Write("Kalkulator za faktoriele ");
        
        ChangeColor.YellowTX("v1.0");

        Console.WriteLine(", Prosinac 2023.");
        Thread.Sleep(500);
    }

    protected static void ctrlCRukovanje(object sender, ConsoleCancelEventArgs args)
    {

        Console.WriteLine("\nProgram je prisilno zaustavljen");

        Console.WriteLine($"\n  Pritisnut: {args.SpecialKey}");

        Console.WriteLine(" ");

        Credits();

        args.Cancel = false;

    }
}

