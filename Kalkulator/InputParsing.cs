using System.Diagnostics;
using System.Runtime;
using FactorieleEval;

namespace InputParsing;

public class Varijabla
{
    public double element { get; set; }
    public string ime { get; set; }


    public Varijabla(string ime, double element)
    {
        this.ime = ime;
        this.element = element;

    }
}

public static class VarijablaListExtention
{
    public static Varijabla? FindPerName(this List<Varijabla> varijable, string itemName)
    {
        for (int i = 0; i < varijable.Count; i++)
        {
            if (varijable[i].ime == itemName)
            {
                return varijable[i];
            }
        }

        return null;
    }
}

public class UserInputParser
{
    // Mora biti definirano VELIKIM slovima!
    public const string exitFromCalc = "Q";
    public const string helpMeni = "HELP";
    public const string defineVariable = "DEF";
    public const string clScr = "CLS";
    public const string measureTime = "TIME";

    public const char factorieleModeRecursive = 'r';
    public const char factorieleModeOptimized = 'o';
    public const char factorieleModeStirling  = 's';
    public const char factorieleModeBigInt    = 'b';

    public static bool IsFactMode(char mode)
    {
        return (mode == factorieleModeRecursive) || (mode == factorieleModeOptimized) || (mode == factorieleModeBigInt)
                || (mode == factorieleModeStirling) || (Char.IsWhiteSpace(mode));
    }

    private static void PrintHelpMeni()
    {
        //Za Kalkulator
        Console.WriteLine("\nHELP MENI ZA KALKULATOR FAKTORIELA\n");
        Console.WriteLine("od verzije 2.5 sljedece komande su dostupne: \n\n");

        Console.WriteLine($"\"{defineVariable} [ImeVarijable]\" ");
        Console.WriteLine("\t- Kreira varijablu [ImeVarijable]\n");

        Console.WriteLine($"\"{helpMeni}\", \"?\"");
        Console.WriteLine("\t- Pokaže ovaj meni\n");

        Console.WriteLine($"\"{exitFromCalc}\", \"ESC\"");
        Console.WriteLine("\t- Izađe iz programa\n");

        Console.WriteLine($"\"{clScr}\" ");
        Console.WriteLine("\t- Očisti ekran\n");

        Console.WriteLine("Operacije \"A + B\", \"A - B\", \"A * B\", \"A / B\", \"A = B\"");
        Console.WriteLine("\t- Moraju imati 2 ili više operanda npr (A = B, A + B, ...)");
        Console.WriteLine("\t- Operandi su imena varijabli i brojevi (numericke vrjednosti)");

        Console.Write("Pritisni neki gumb za nastavak...");
        Console.ReadKey();
        Console.WriteLine("                                 \n");

        Console.WriteLine("Operacije potenciranja: \"A^B\"");
        Console.WriteLine("\t- Moraju imati 2 operanda");
        Console.WriteLine("\t- A - broj ili varijabla");
        Console.WriteLine("\t- B - broj ili varijabla\n");

        Console.WriteLine("Operacija faktorijel: \"A![mode]\"");
        Console.WriteLine("\t- mode default: o");
        Console.WriteLine("\t- Modovi:");
        Console.WriteLine($"\t  {factorieleModeRecursive} - evaluacija rekurzijom");
        Console.WriteLine($"\t  {factorieleModeOptimized} - optimizirana evaluacija (trenutno koristi for petlju)");
        Console.WriteLine($"\t  {factorieleModeBigInt} - optimizirana evaluacija za jako velike brojeve");
        Console.WriteLine($"\t  {factorieleModeStirling} - evaluacija Stirlingovom aproksimacijom\n");
        Console.WriteLine($"\"{measureTime}\" [matematički izraz za evaluirati]");
        Console.WriteLine($"\t- Mjerenji vrijeme izvođenja matematičkog izraza ");
        Console.WriteLine( "i omogućuje dodatnu statistiku(specifično funkcije operacija faktorijela)\n");
        return;
    }

    public static bool ShouldExit(int exitCode)
    {
        return exitCode != (int)returnFlagsParser.SaveToFile && exitCode != (int)returnFlagsParser.exitProgram;
    }

    public static int StringToKalkulatorParser(string userInput, List<Varijabla> varijable, int tempInt = 0)
    {
        bool print = true;
        if (userInput == null) return (int)returnFlagsParser.err;
        if (userInput.Length == 0) return (int)returnFlagsParser.err;

        bool measureFactExecTime = tempInt == -1 ? true : false;

        switch (userInput.ToUpper())
        {
            case "ESC": /* Fallthrough */
            case exitFromCalc:
                {
                    return (int)returnFlagsParser.exitProgram;
                }
            case clScr:
                {
                    Console.Clear();
                    return (int)returnFlagsParser.softExit;
                }
            case "?": /* Fallthrough */
            case helpMeni:
                {
                    PrintHelpMeni();
                    return (int)returnFlagsParser.softExit;
                }
            default: break;
        }

        if (userInput.ToUpper().StartsWith(measureTime))
        {
            return (int)StringToKalkulatorParser(userInput[(measureTime.Length + 1)..userInput.Length], varijable, -1);
        }

        int prioritetEQU = 9;
        int prioritetPL  = 5;
        int prioritetMIN = 5;
        int prioritetMUL = 4;
        int prioritetDIV = 4;
        int prioritetFAC = 2;
        int prioritetPOT = 1;


        while (userInput.Contains('('))
        {
            int first = userInput.IndexOf('(');
            int last = userInput.LastIndexOf(')');

            int zagrade = 0;
            for (int i = first; i < last; i++)
            {
                if (userInput[i] == '(') zagrade++;
                if (userInput[i] == ')') zagrade--;

                if (zagrade == 0)
                {
                    last = i;
                    break;
                }
            }
            if (first < 0) throw new ArgumentException($"Nedostaje otvarajuća zagrada! [>(< {userInput}]");
            if (last <= 0) throw new ArgumentException($"Nedostaje zatvarajuća zagrada! [{userInput} >)<]");

            List<Varijabla> _varijableCopy = new(varijable);

            UserInputParser.StringToKalkulatorParser($"__PAREN{tempInt} = " + userInput[(first + 1)..last], _varijableCopy, tempInt + 1);
            Varijabla? result = _varijableCopy.FindPerName($"__PAREN{tempInt}");
            if (result == null) return (int)returnFlagsParser.err;

            userInput = userInput[0 .. first] + result.element.ToString() + userInput[(last + 1) .. userInput.Length];

            if (userInput == "") return (int)returnFlagsParser.err;
        }

        List<string> userInputOperacije = new(userInput.Split(" "));

        if (userInputOperacije.Count == 1 && (!userInputOperacije[0].Contains('^') && !userInputOperacije[0].Contains('!')))
        {
            if (tempInt == 0 && !IsNumber(userInput))
            {
                var A = varijable.FindPerName(userInput) ?? throw new ArgumentException($"Nije pronađena varijabla {userInput}!");
                Console.WriteLine($"{A.element}");
                return (int)returnFlagsParser.normal;
            }
            else
            {
                Console.WriteLine($"{userInput}");
                return (int)returnFlagsParser.softExit;
            }
        }
        if (userInputOperacije[0].ToUpper() == defineVariable)
        {
            string imeVar = userInputOperacije[1];
            Console.Write($"Upiši vrijednost nove varijable \"{imeVar}\": ");
            Varijabla noviVar = new(imeVar, double.Parse(Console.ReadLine() + ""));
            varijable.Add(noviVar);

            return (int)returnFlagsParser.softExit;
        }
        int maxIndex = userInputOperacije.Count;
        int brojOperacija = maxIndex / 2;
        int brojOperanda = maxIndex / 2 + 1;

        if (brojOperacija + brojOperanda != userInputOperacije.Count) throw new InvalidOperationException("Zbroj operanda i operacija nije jednak broju svih elemenata!");

        List<PrioritetOperacija> Operacije = new();

        for (int i = 0; i < maxIndex; i++)
        {
            var operacija = userInputOperacije[i];
            switch (operacija)
            {
                case "=":
                    {
                        Operacije.Add(IspuniListuPrioriteta(i, prioritetEQU, '='));
                    }
                    break;
                case "*":
                    {
                        Operacije.Add(IspuniListuPrioriteta(i, prioritetMUL, '*'));
                    }
                    break;
                case "/":
                    {
                        Operacije.Add(IspuniListuPrioriteta(i, prioritetDIV, '/'));
                    } break;
                case "+":
                    {
                        Operacije.Add(IspuniListuPrioriteta(i, prioritetPL, '+'));
                    }
                    break;
                case "-":
                    {
                        Operacije.Add(IspuniListuPrioriteta(i, prioritetMIN, '-'));
                    }
                    break;
                default:
                    {
                        if (operacija.Contains('^'))
                        {
                            Operacije.Add(IspuniListuPrioriteta(i, prioritetPOT, '^'));
                        }
                        else if (operacija.Contains('!'))
                        {
                            Operacije.Add(IspuniListuPrioriteta(i, prioritetFAC, '!'));
                        }
                    }
                    break;
            }
        }

        if (Operacije.Count > 0)
        {
            var sorter = new KComparerPrioriteta();
            Operacije.Sort(sorter);
        }
        else return (int)returnFlagsParser.err;

        List<Varijabla> varijableCopy = new(varijable);

        if (Operacije[0].operacija == '+')
        {
            string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
            string posljeOperacije = userInputOperacije[Operacije[0].index + 1];

            var A = varijableCopy.FindPerName(prijeOperacije + "");
            var B = varijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) + double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                }
                else if (IsNumber(posljeOperacije))
                {
                    double rj = (A == null ? throw new ArgumentException($"Varijabla \"{prijeOperacije}\" nije definirana") : A.element) + double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                }
                else if (IsNumber(prijeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) + (B == null ? throw new ArgumentException($"Varijabla \"{posljeOperacije}\" nije definirana") : B.element);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                }
                else
                {
                    throw new ArgumentException($"Jedna od varijabli nije definirana (\"{prijeOperacije} + {posljeOperacije}\")");
                }
            }
            else // var-only
            {
                Varijabla tempVar = new($"TMP{tempInt}", A.element + B.element);
                varijableCopy.Add(tempVar);

                userInputOperacije[Operacije[0].index + 1] = $"TMP{tempInt}"; //Sprema ime TMP varijable za kasniju upotrebu
                userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                userInputOperacije.RemoveAt(Operacije[0].index - 1);
            }

            userInput = "";

            for (int i = 0; i < maxIndex - 2; i++)      //Rekonstruira Input s imenima novih varijabli
            {
                if (i == maxIndex - 3) userInput += userInputOperacije[i];
                else userInput += userInputOperacije[i] + " ";
            }
            Operacije.RemoveAt(0);

        }
        else if (Operacije[0].operacija == '-')
        {
            string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
            string posljeOperacije = userInputOperacije[Operacije[0].index + 1];

            var A = varijableCopy.FindPerName(prijeOperacije + "");
            var B = varijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) - double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                }
                else if (IsNumber(posljeOperacije))
                {
                    double rj = (A == null ? throw new ArgumentException($"Varijabla \"{prijeOperacije}\" nije definirana") : A.element) - double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                }
                else if (IsNumber(prijeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) - (B == null ? throw new ArgumentException($"Varijabla \"{posljeOperacije}\" nije definirana") : B.element);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                }
                else
                {
                    throw new ArgumentException($"Jedna od varijabli nije definirana (\"{prijeOperacije} - {posljeOperacije}\")");
                }
            }
            else // var-only
            {
                Varijabla tempVar = new($"TMP{tempInt}", A.element - B.element);
                varijableCopy.Add(tempVar);

                userInputOperacije[Operacije[0].index + 1] = $"TMP{tempInt}"; //Sprema ime TMP varijable za kasniju upotrebu
                userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                userInputOperacije.RemoveAt(Operacije[0].index - 1);
            }

            userInput = "";

            for (int i = 0; i < maxIndex - 2; i++)      //Rekonstruira Input s imenima novih varijabli
            {
                if (i == maxIndex - 3) userInput += userInputOperacije[i];
                else userInput += userInputOperacije[i] + " ";
            }
            Operacije.RemoveAt(0);

        }
        else if (Operacije[0].operacija == '*')
        {
            string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
            string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
            bool numOnly = false;

            var A = varijableCopy.FindPerName(prijeOperacije + "");
            var B = varijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) * double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                    numOnly = true;

                }
                else if (A == null && !IsNumber(prijeOperacije) || B == null && !IsNumber(posljeOperacije))
                    throw new ArgumentException($"Jedna od varijabli nije definirana (\"{prijeOperacije} * {posljeOperacije}\")");
            }

            if (!numOnly)// var-only
            {
                Varijabla tempVar;

                if (IsNumber(posljeOperacije) && A != null)
                    tempVar = new Varijabla($"TMP{tempInt}", A.element * double.Parse(posljeOperacije));
                else if (IsNumber(prijeOperacije) && B != null)
                    tempVar = new Varijabla($"TMP{tempInt}", double.Parse(prijeOperacije) * B.element);
                else if (A != null && B != null)
                    tempVar = new Varijabla($"TMP{tempInt}", A.element * B.element);
                else
                    throw new ArgumentException($"Niti jedna od varijabli nije definirana (\"{prijeOperacije} * {posljeOperacije}\")");

                varijableCopy.Add(tempVar);

                userInputOperacije[Operacije[0].index + 1] = $"TMP{tempInt}"; //Sprema ime TMP varijable za kasniju upotrebu
                userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                userInputOperacije.RemoveAt(Operacije[0].index - 1);
            }

            userInput = "";

            for (int i = 0; i < maxIndex - 2; i++)      //Rekonstruira Input s imenima novih varijabli
            {
                if (i == maxIndex - 3) userInput += userInputOperacije[i];
                else userInput += userInputOperacije[i] + " ";
            }
            Operacije.RemoveAt(0);

        }
        else if (Operacije[0].operacija == '/')
        {
            string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
            string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
            bool numOnly = false;

            var A = varijableCopy.FindPerName(prijeOperacije + "");
            var B = varijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) / double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    print = false;
                    numOnly = true;

                }
                else if (A == null && !IsNumber(prijeOperacije) || B == null && !IsNumber(posljeOperacije))
                    throw new ArgumentException($"Jedna od varijabli nije definirana (\"{prijeOperacije} / {posljeOperacije}\")");
            }

            if (!numOnly)// var-only
            {
                Varijabla tempVar;

                if (IsNumber(posljeOperacije) && A != null)
                    tempVar = new Varijabla($"TMP{tempInt}", A.element / double.Parse(posljeOperacije));
                else if (IsNumber(prijeOperacije) && B != null)
                    tempVar = new Varijabla($"TMP{tempInt}", double.Parse(prijeOperacije) / B.element);
                else if (A != null && B != null)
                    tempVar = new Varijabla($"TMP{tempInt}", A.element / B.element);
                else
                    throw new ArgumentException($"Niti jedna od varijabli nije definirana (\"{prijeOperacije} / {posljeOperacije}\")");

                varijableCopy.Add(tempVar);

                userInputOperacije[Operacije[0].index + 1] = $"TMP{tempInt}"; //Sprema ime TMP varijable za kasniju upotrebu
                userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                userInputOperacije.RemoveAt(Operacije[0].index - 1);
            }

            userInput = "";

            for (int i = 0; i < maxIndex - 2; i++)      //Rekonstruira Input s imenima novih varijabli
            {
                if (i == maxIndex - 3) userInput += userInputOperacije[i];
                else userInput += userInputOperacije[i] + " ";
            }
            Operacije.RemoveAt(0);

        }
        else if (Operacije[0].operacija == '^')
        {
            string[] razdvajanjeNaBazuIEksponent = userInputOperacije[Operacije[0].index].Split('^');
            string prijeOperacije = razdvajanjeNaBazuIEksponent[0];
            string posljeOperacije = razdvajanjeNaBazuIEksponent[1];

            if (!IsNumber(posljeOperacije) && varijableCopy.FindPerName(posljeOperacije + "") == null)
            {
                throw new ArgumentException("Nakon znaka \"^\" mora biti broj ili ime varijable");
            }

            var A = varijableCopy.FindPerName(prijeOperacije + "");
            var B = varijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    double rj;
                    rj = Math.Pow(double.Parse(prijeOperacije), double.Parse(posljeOperacije));

                    userInputOperacije[Operacije[0].index] = $"{rj}";
                    print = false;

                }
                else if (IsNumber(posljeOperacije) && A != null)
                {
                    Varijabla tempVar = new($"TMP{tempInt}", Math.Pow(A.element, double.Parse(posljeOperacije)));
                    varijableCopy.Add(tempVar);

                    userInputOperacije[Operacije[0].index] = $"TMP{tempInt}";
                }
                else if (IsNumber(prijeOperacije) && B != null)
                {
                    Varijabla tempVar = new($"TMP{tempInt}", Math.Pow(double.Parse(prijeOperacije), B.element));
                    varijableCopy.Add(tempVar);

                    userInputOperacije[Operacije[0].index] = $"TMP{tempInt}";
                }
                else throw new ArgumentException("A^B -> A i B moraju biti brojevi ili imena varijabli");
            } else
            {
                Varijabla tempVar;

                {
                    tempVar = new Varijabla($"TMP{tempInt}", Math.Pow(A.element, B.element));
                }

                varijableCopy.Add(tempVar);

                userInputOperacije[Operacije[0].index] = $"TMP{tempInt}";
            }
            userInput = "";

            if (razdvajanjeNaBazuIEksponent.Count() > 2)
            {
                for (int i = 2; i < razdvajanjeNaBazuIEksponent.Count(); i++)
                {
                    userInputOperacije[Operacije[0].index] += $"^{razdvajanjeNaBazuIEksponent[i]}";
                }
            }

            for (int i = 0; i < maxIndex; i++)      //Rekonstruira Input s imenima novih varijabli
            {
                if (i == maxIndex - 1) userInput += userInputOperacije[i];
                else userInput += userInputOperacije[i] + " ";
            }
            if (razdvajanjeNaBazuIEksponent.Count() == 2)
            {
                Operacije.RemoveAt(0);
            }
        }
        else if (Operacije[0].operacija == '!')
        {
            string[] operandi = userInputOperacije[Operacije[0].index].Split('!');
            string prijeOperacije = operandi[0];
            char mode = operandi.Length > 1 && operandi[1].Length > 0 ? operandi[1][0] : ' ';

            if (!IsFactMode(mode))
            {
                throw new ArgumentException("Nakon znaka \"!\" mora biti metoda evaluacije ili prazno! \n"
                                         + $" Primjer korištenja druge metode je: {prijeOperacije}!{factorieleModeStirling}. "
                                         + $"(pogledaj {helpMeni} za bolje objašnjenje)");
            }

            Func<double, double> fact;

            switch (mode)
            {
                case factorieleModeRecursive:
                    {
                        fact = FactEval.FactorieleRecursive;
                    }
                    break;
                case factorieleModeStirling:
                    {
                        fact = FactEval.FactorieleStirling;
                    } break;
                case factorieleModeOptimized: /* Fallthrough */
                default:
                    {
                        fact = FactEval.FactorieleOptimized;
                    } break;
            }

            var A = varijableCopy.FindPerName(prijeOperacije + "");

            if (A == null)
            {
                if (IsNumber(prijeOperacije))
                {
                    userInputOperacije[Operacije[0].index] = $"{fact(double.Parse(prijeOperacije))}";

                    if (mode == factorieleModeStirling && measureFactExecTime && double.Parse(prijeOperacije) <= 170)
                    {
                        double a = FactEval.FactorieleOptimized(double.Parse(prijeOperacije));
                        double b = FactEval.FactorieleStirling(double.Parse(prijeOperacije));
                        Console.WriteLine("Apsolutna greška: " + Math.Abs(b - a));
                        Console.WriteLine("Relativna greška: " + Math.Abs(b - a) / a + "\n");
                    }

                    if (double.Parse(prijeOperacije) > 170 || mode == factorieleModeBigInt)
                    {
                        Console.WriteLine("Full number: " + FactEval.FactorieleBigInt(double.Parse(prijeOperacije)));
                    }

                    if (measureFactExecTime)
                    {
                        FactEval.MessureExecTimeMs(fact, double.Parse(prijeOperacije));
                    }
                    print = false;

                }
                else throw new ArgumentException($"A!{mode} -> A moraja biti broj ili imene varijable");
            }
            else // var-only
            {
                Varijabla tempVar;

                {
                    double rj = fact(A.element);
                    tempVar = new Varijabla($"TMP{tempInt}", rj);
                }

                varijableCopy.Add(tempVar);

                userInputOperacije[Operacije[0].index] = $"TMP{tempInt}";
            }
            userInput = "";

            if (operandi.Length > 2)
            {
                for (int i = 2; i < operandi.Length; i++)
                {
                    userInputOperacije[Operacije[0].index] += $"!{operandi[i]}";
                }
            }

            for (int i = 0; i < maxIndex; i++)      //Rekonstruira Input s imenima novih varijabli
            {
                if (i == maxIndex - 1) userInput += userInputOperacije[i];
                else userInput += userInputOperacije[i] + " ";
            }
            if (operandi.Length <= 2)
            {
                Operacije.RemoveAt(0);
            }

        }
        else if (Operacije[0].operacija == '=')
        {
            string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
            string posljeOperacije = userInputOperacije[Operacije[0].index + 1];

            var A = varijableCopy.FindPerName(prijeOperacije + "");
            var B = varijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    if (double.Parse(prijeOperacije) == double.Parse(posljeOperacije))
                    {
                        Console.Write("Lijeva i desna strana su jednake! ");
                        ChangeColor.GreenBGLine("TRUE");
                        return (int)returnFlagsParser.JednadzbaTrue;
                    }
                    Console.Write("Lijeva i desna strana nije jednaka! ");
                    ChangeColor.RedBGLine("FALSE");
                    return (int)returnFlagsParser.JednadzbaFalse;
                }
                else if (A == null && IsNumber(posljeOperacije))
                {
                    Varijabla tempVar = new(prijeOperacije, double.Parse(posljeOperacije));

                    varijable.Add(tempVar);
                    Operacije.RemoveAt(0);
                    return (int)returnFlagsParser.exitResultStored;
                }
                else if (A != null && IsNumber(posljeOperacije))
                {
                    if (A.element == double.Parse(posljeOperacije))
                    {
                        Console.Write("Lijeva i desna strana su jednake! ");
                        ChangeColor.GreenBGLine("TRUE");
                        Operacije.RemoveAt(0);
                        return (int)returnFlagsParser.JednadzbaTrue;
                    }

                    Console.Write("Lijeva i desna strana nisu jednake! ");
                    ChangeColor.RedBGLine("FALSE");
                    Operacije.RemoveAt(0);
                    string imeNoveVar = A.ime;
                    if (!A.ime.Contains("TMP"))
                        if (ReDefinirajVarijablu(A, varijable))
                        {
                            Varijabla Rj = new(imeNoveVar, double.Parse(posljeOperacije));
                            varijable.Remove(A);
                            varijable.Add(Rj);
                            return (int)returnFlagsParser.exitResultStored;
                        }
                    return (int)returnFlagsParser.normal;
                }
                else if (B == null)
                    throw new ArgumentException($"Nije pronađena varijabla {posljeOperacije}!");
                else if (A == null)
                {
                    Varijabla tempVar = new(prijeOperacije, B.element);
                    varijable.Add(tempVar);

                    Operacije.RemoveAt(0);
                    return (int)returnFlagsParser.exitResultStored;
                }
            }


            if (A != null && B != null)
            {
                if (A.element == B.element)
                {
                    Console.Write("Lijeva i desna strana su jednake! ");
                    ChangeColor.GreenBGLine("TRUE");
                    Operacije.RemoveAt(0);
                    return (int)returnFlagsParser.JednadzbaTrue;
                }
                else
                {
                    Console.Write("Lijeva i desna strana nisu jednake! ");
                    ChangeColor.RedBGLine("FALSE");
                    Operacije.RemoveAt(0);
                    string imeNoveVar = A.ime;
                    if (!A.ime.Contains("TMP"))
                        if (ReDefinirajVarijablu(A, varijable))
                        {
                            Varijabla Rj = new(imeNoveVar, B.element);
                            varijable.Remove(A);
                            varijable.Add(Rj);
                            return (int)returnFlagsParser.exitResultStored;
                        }
                    return (int)returnFlagsParser.normal;
                }
            }


        }

        //Console.WriteLine(userInput);
        int rfll = (int)returnFlagsParser.normal;
        if (Operacije.Count > 0)
        {
            rfll = StringToKalkulatorParser(userInput, varijableCopy, tempInt + 1); // Return from lower layer
        }

        if ((int)returnFlagsParser.JednadzbaFalse == rfll || (int)returnFlagsParser.JednadzbaTrue == rfll) return (int)returnFlagsParser.softExit;
        if ((int)returnFlagsParser.softExit == rfll) return rfll;

        if (rfll == (int)returnFlagsParser.normal)
        {
            if (varijableCopy.Count > 0)
            {
                varijable.Add(varijableCopy[^1]);
            }
        }

        if (rfll == (int)returnFlagsParser.exitResultStored)
        {
            var NameColTest = varijable.FindPerName(varijableCopy[^1].ime);
            if (NameColTest != null) varijable.Remove(NameColTest);
            varijable.Add(varijableCopy[^1]);
        }

        if (IsNumber(userInput)) Console.WriteLine(userInput);

        if (rfll == (int)returnFlagsParser.err)
        {
            return rfll;
        }

        if (tempInt == 0 && !IsNumber(userInput) && print)
        {
            Varijabla A = varijableCopy[^1];
            if (varijable.FindPerName(A.ime) != null)
            {
                Console.WriteLine($"{A.element}");
            }
            else
            {
                Console.WriteLine($"Varijabla s imenom {A.ime} nije pronađena!\n");
            }
        }

        if (tempInt == 0 && rfll == (int)returnFlagsParser.normal)
        {
            if (varijableCopy.Count > 0)
            {
                varijable.Remove(varijableCopy[^1]);
            }
        }

        return rfll;
    }


    private static PrioritetOperacija IspuniListuPrioriteta(int index, int prioritet, char operacija)
    {
        PrioritetOperacija prioritetOperacija = new()
        {
            prioritet = prioritet,
            index = index,
            operacija = operacija
        };

        return prioritetOperacija;
    }

    public static bool ReDefinirajVarijablu(Varijabla VarijablaKojaSeBrise, List<Varijabla> varijable)
    {
        Console.Write("\nVarijabla {0} već postoji! Želite li ju redefinirati? [Y/n]: ", VarijablaKojaSeBrise.ime);
        string YNreDef = Console.ReadLine() + "";
        if (YNreDef.ToUpper() != "Y")
            return false;
        varijable.Remove(VarijablaKojaSeBrise);
        return true;
    }


    public static bool IsNumber(string input)
    {
        if (input == null) return false;

        try
        {
            double.Parse(input);
        }
        catch (FormatException)
        {
            return false;
        }

        return true;

    }
}

public enum returnFlagsParser
{
    err = -1, normal, exitResultStored, SaveToFile, JednadzbaTrue, JednadzbaFalse, exitToConsole, exitProgram, softExit
}

struct PrioritetOperacija
{
    public int prioritet;
    public int index;
    public char operacija;
}

class KComparerPrioriteta : IComparer<PrioritetOperacija>
{
    public int Compare(PrioritetOperacija x, PrioritetOperacija y)
    {
        if (x.prioritet < y.prioritet)                               //sortira prioritet rastuci
            return -1;
        else if (x.prioritet > y.prioritet)
            return 1;
        else if (x.index < y.index)
            return -1;
        else
            return 1;
    }
}