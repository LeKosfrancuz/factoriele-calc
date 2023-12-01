using System.Diagnostics;


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
    public static Varijabla? FindPerName(this List<Varijabla> Varijable, string itemName)
    {
        for (int i = 0; i < Varijable.Count; i++)
        {
            if (Varijable[i].ime == itemName)
            {
                return Varijable[i];
            }
        }

        return null;
    }
}

public class UserInputParser
{
    public const string exitFromCalc = "Q";
    public const string helpMeni = "HELP";
    public const string defineVariable = "DEF";
    public const string clScr = "CLS";

    private static void PrintHelpMeni()
    {
        //Za Kalkulator
        Console.WriteLine("\nHELP MENI ZA KALKULATOR FAKTORIELA\n\n" +
                          "od verzije 2.5 sljedece komande su dostupne: \n\n\n" +
                          "\"{0} [ImeVarijable]\" \n" +
                          "\t- Kreira varijablu [ImeVarijable]\n\n" +
                          "\"{1}\", \"?\"\n" +
                          "\t- Pokaže ovaj meni\n\n" +
                          "\"{2}\", \"ESC\"\n" +
                          "\t- Izađe iz programa\n\n" +
                          "\"{3}\" \n" +
                          "\t- Očisti ekran\n\n",
                          UserInputParser.defineVariable, UserInputParser.helpMeni, UserInputParser.exitFromCalc, UserInputParser.clScr);
        Console.WriteLine("Operacije \"A + B\", \"A - B\", \"A * B\", \"A = B\"");
        Console.WriteLine("\t- Moraju imati 2 ili više operanda npr (A = B, A + B, ...)");
        Console.WriteLine("\t- Operandi su imena varijabli i brojevi (numericke vrjednosti)");
        //Console.WriteLine("\t- Ove operacije MORAJU se pisati sa razmakom oko njih npr (A_=_B) gdje je \"_\" razmak\n");

        Console.Write("Pritisni neki gumb za nastavak...");
        Console.ReadKey();
        Console.WriteLine("                                 \n");

        Console.WriteLine("Operacije potenciranja: \"A^B\"");
        Console.WriteLine("\t- Moraju imati 2 operanda");
        Console.WriteLine("\t- Pišu se BEZ razmaka npr (2^3)");
        Console.WriteLine("\t- A - broj ili varijabla");
        Console.WriteLine("\t- B - broj ili varijabla\n");
        return;
    }


    public static int StringToKalkulatorParser(string userInput, List<Varijabla> Varijable, int tempInt = 0)
    {
        bool print = true;
        if (userInput == null) return (int)returnFlagsParser.err;
        if (userInput.Length == 0) return (int)returnFlagsParser.err;

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

        int prioritetEQU = 9;
        int prioritetPL = 5;
        int prioritetMIN = 5;
        int prioritetMUL = 4;
        // TODO: Zagrade
        int prioritetFAC = 2;
        int prioritetPOT = 1;



        List<string> userInputOperacije = new List<string>(userInput.Split(" "));
        if (userInputOperacije.Count == 1 && (!userInputOperacije[0].Contains("^") && !userInputOperacije[0].Contains("!")))
        {
            Console.WriteLine($"{userInput}");
            return (int)returnFlagsParser.softExit;
        }
        if (userInputOperacije[0].ToUpper() == defineVariable)
        {
            string imeVar = userInputOperacije[1];
            Console.Write($"Upiši vrijednost nove varijable \"{imeVar}\": ");
            Varijabla noviVar = new Varijabla(imeVar, double.Parse(Console.ReadLine() + ""));
            Varijable.Add(noviVar);

            return (int)returnFlagsParser.softExit;
        }
        int maxIndex = userInputOperacije.Count();
        int brojOperacija = maxIndex / 2;
        int brojOperanda = maxIndex / 2 + 1;

        if (brojOperacija + brojOperanda != userInputOperacije.Count()) throw new InvalidOperationException("Zbroj operanda i operacija nije jednak broju svih elemenata!");

        List<PrioritetOperacija> Operacije = new List<PrioritetOperacija>();

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
                        if (operacija.Contains("^"))
                        {
                            Operacije.Add(IspuniListuPrioriteta(i, prioritetPOT, '^'));
                        }
                        else if (operacija.Contains("!"))
                        {
                            ChangeColor.RedBGLine("Računanje faktorijela nije implementirano!\n");
                            Debug.Assert(false);
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

        List<Varijabla> VarijableCopy = new List<Varijabla>(Varijable);

        if (Operacije[0].operacija == '+')
        {
            string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
            string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
            bool numOnly = false;

            var A = VarijableCopy.FindPerName(prijeOperacije + "");
            var B = VarijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                // TODO: Istražit zašto koristim index a ne `prijeOperacije`
                if (IsNumber(userInputOperacije[Operacije[0].index - 1]) && IsNumber(posljeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) + double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    numOnly = true;
                    print = false;
                }
                else if (IsNumber(prijeOperacije) || IsNumber(posljeOperacije))
                {
                    double rj = (A == null ? double.Parse(prijeOperacije) : A.element) + (B == null ? double.Parse(posljeOperacije) : B.element);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    numOnly = true;
                    print = false;
                }
                else
                {
                    throw new ArgumentException($"Jedna od varijabli nije definirana (\"{prijeOperacije} + {posljeOperacije}\")");
                }
            }


            if (!numOnly)
            {
                Varijabla tempVar = new Varijabla($"TMP{tempInt}", A.element + B.element);
                VarijableCopy.Add(tempVar);

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
            bool numOnly = false;

            var A = VarijableCopy.FindPerName(prijeOperacije + "");
            var B = VarijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                // TODO: Istražit zašto koristim index a ne `prijeOperacije`
                if (IsNumber(userInputOperacije[Operacije[0].index - 1]) && IsNumber(posljeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) - double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    numOnly = true;
                    print = false;
                }
                else if (IsNumber(prijeOperacije) || IsNumber(posljeOperacije))
                {
                    double rj = (A == null ? double.Parse(prijeOperacije) : A.element) - (B == null ? double.Parse(posljeOperacije) : B.element);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    numOnly = true;
                    print = false;
                }
                else
                {
                    throw new ArgumentException($"Jedna od varijabli nije definirana (\"{prijeOperacije} - {posljeOperacije}\")");
                }
            }


            if (!numOnly)
            {
                Varijabla tempVar = new Varijabla($"TMP{tempInt}", A.element - B.element);
                VarijableCopy.Add(tempVar);

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

            var A = VarijableCopy.FindPerName(prijeOperacije + "");
            var B = VarijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    double rj = double.Parse(prijeOperacije) * double.Parse(posljeOperacije);
                    userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                    numOnly = true;
                    print = false;

                }
                else if (A == null && !IsNumber(prijeOperacije) || B == null && !IsNumber(posljeOperacije))
                    throw new ArgumentException($"Jedna od varijabli nije definirana (\"{prijeOperacije} + {posljeOperacije}\")");
            }


            if (!numOnly)
            {
                Varijabla tempVar;

                if (IsNumber(posljeOperacije))
                    tempVar = new Varijabla($"TMP{tempInt}", A.element * double.Parse(posljeOperacije));
                else if (IsNumber(prijeOperacije))
                    tempVar = new Varijabla($"TMP{tempInt}", double.Parse(prijeOperacije) * B.element);
                else
                    tempVar = new Varijabla($"TMP{tempInt}", A.element * B.element);

                VarijableCopy.Add(tempVar);

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
            bool numOnly = false;

            if (!IsNumber(posljeOperacije) && VarijableCopy.FindPerName(posljeOperacije + "") == null)
            {
                throw new ArgumentException("Nakon znaka \"^\" mora biti broj ili ime varijable");
            }

            var A = VarijableCopy.FindPerName(prijeOperacije + "");
            var B = VarijableCopy.FindPerName(posljeOperacije + "");
            if (A == null || B == null)
            {
                if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                {
                    double rj;
                    numOnly = true;
                    rj = Math.Pow(double.Parse(prijeOperacije), double.Parse(posljeOperacije));

                    userInputOperacije[Operacije[0].index] = $"{rj}";
                    print = false;

                }
                else if (IsNumber(posljeOperacije))
                {
                    Varijabla tempVar = new Varijabla($"TMP{tempInt}", Math.Pow(A.element, double.Parse(posljeOperacije)));
                    VarijableCopy.Add(tempVar);

                    userInputOperacije[Operacije[0].index] = $"TMP{tempInt}";
                    numOnly = true;
                }
                else if (IsNumber(prijeOperacije))
                {
                    Varijabla tempVar = new Varijabla($"TMP{tempInt}", Math.Pow(double.Parse(prijeOperacije), B.element));
                    VarijableCopy.Add(tempVar);

                    userInputOperacije[Operacije[0].index] = $"TMP{tempInt}";
                    numOnly = true;
                }
                else throw new ArgumentException("A^B -> A i B moraju biti brojevi ili imena varijabli");
            }

            if (!numOnly)
            {
                Varijabla tempVar;

                {
                    tempVar = new Varijabla($"TMP{tempInt}", Math.Pow(A.element, B.element));
                }

                VarijableCopy.Add(tempVar);

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
        else if (Operacije[0].operacija == '=')
        {
            string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
            string posljeOperacije = userInputOperacije[Operacije[0].index + 1];

            var A = VarijableCopy.FindPerName(prijeOperacije + "");
            var B = VarijableCopy.FindPerName(posljeOperacije + "");
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
                    Varijabla tempVar = new Varijabla(prijeOperacije, double.Parse(posljeOperacije));

                    Varijable.Add(tempVar);
                    Operacije.RemoveAt(0);
                    return (int)returnFlagsParser.exitResultStored;
                }
                else if (B == null)
                    throw new ArgumentException($"Nije pronađena varijabla {posljeOperacije}!");
                else if (A == null)
                {
                    Varijabla tempVar = new Varijabla(prijeOperacije, B.element);
                    Varijable.Add(tempVar);

                    Operacije.RemoveAt(0);
                    return (int)returnFlagsParser.exitResultStored;
                }
            }


            if (A != null)
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
                        if (ReDefinirajVarijablu(A, Varijable))
                        {
                            Varijabla Rj = new Varijabla(imeNoveVar, B.element);
                            Varijable.Remove(A);
                            Varijable.Add(Rj);
                            return (int)returnFlagsParser.exitResultStored;
                        }
                    return (int)returnFlagsParser.normal;
                }
            }


        }

        //Console.WriteLine(userInput);
        int rfll = (int)returnFlagsParser.normal;
        if (Operacije.Count() > 0)
        {
            rfll = StringToKalkulatorParser(userInput, VarijableCopy, tempInt + 1); // Return from lower layer
        }

        if ((int)returnFlagsParser.JednadzbaFalse == rfll || (int)returnFlagsParser.JednadzbaTrue == rfll) return (int)returnFlagsParser.softExit;
        if ((int)returnFlagsParser.softExit == rfll) return rfll;

        if (rfll == (int)returnFlagsParser.normal)
        {
            if (VarijableCopy.Count > 0)
            {
                Varijable.Add(VarijableCopy[VarijableCopy.Count - 1]);
            }
        }

        if (rfll == (int)returnFlagsParser.exitResultStored)
        {
            var NameColTest = Varijable.FindPerName(VarijableCopy[VarijableCopy.Count - 1].ime);
            if (NameColTest != null) Varijable.Remove(NameColTest);
            Varijable.Add(VarijableCopy[VarijableCopy.Count - 1]);
        }

        if (IsNumber(userInput)) Console.WriteLine(userInput);

        if (rfll == (int)returnFlagsParser.err)
        {
            return rfll;
        }

        if (tempInt == 0 && !IsNumber(userInput) && print)
        {
            Varijabla A = VarijableCopy[VarijableCopy.Count - 1];
            if (Varijable.FindPerName(A.ime) != null)
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
            if (VarijableCopy.Count > 0)
            {
                Varijable.Remove(VarijableCopy[VarijableCopy.Count - 1]);
            }
        }

        return rfll;
    }


    private static PrioritetOperacija IspuniListuPrioriteta(int index, int prioritet, char operacija)
    {
        PrioritetOperacija prioritetOperacija = new PrioritetOperacija();
        prioritetOperacija.prioritet = prioritet;
        prioritetOperacija.index = index;
        prioritetOperacija.operacija = operacija;

        return prioritetOperacija;
    }

    public static bool ReDefinirajVarijablu(Varijabla VarijablaKojaSeBrise, List<Varijabla> Varijable)
    {
        Console.Write("\nMatrica {0} već postoji! Želite li ju redefinirati? [Y/n]: ", VarijablaKojaSeBrise.ime);
        string YNreDef = Console.ReadLine() + "";
        if (YNreDef.ToUpper() != "Y")
            return false;
        Varijable.Remove(VarijablaKojaSeBrise);
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

public static class CommandColor
{
    public static string LineColoring(string userInput, List<Varijabla> Varijable)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("                                \r"); //Da očisti help poruku
        if (userInput == "") Console.Write("Upiši \"?\" za pomoc\r");

        (int x_pomak, int y_pomak) = Console.GetCursorPosition();
        x_pomak = Console.CursorLeft;
        Console.SetCursorPosition(x_pomak, y_pomak - 1);

        if (userInput == "") return userInput;

        Console.Write(">");

        //Console.Write("\x1b[1A\r\x1b[2K\r>");

        List<BojaOperacija> userInputOperandi = new List<BojaOperacija> { AddOperacija(0, "0") };
        userInputOperandi.RemoveAt(0);
        if (userInputOperandi.Count > 0) throw new InvalidProgramException("Prazno polje operanada nije prazno!");

        string tempInput = "";
        int brojOperanada = 0;


        for (int i = 0; i < userInput.Length; i++)
        {
            tempInput += userInput[i];

            if (tempInput == "+" || tempInput == "-" || tempInput == "*" || tempInput == "/" || tempInput == "^" || tempInput == "!" || tempInput == "=")
            {
                userInputOperandi.Add(AddOperacija((int)ConsoleColor.White, tempInput));
                brojOperanada++;
                tempInput = "";
                continue;
            }

            if (tempInput == " ")
            {
                tempInput = "";
                continue;
            }

            if (userInput[(i + 1) % userInput.Length] == ' ' || i == userInput.Length - 1 || char.IsAscii(userInput[(i + 1) % userInput.Length])
                && !(char.IsLetterOrDigit(userInput[(i + 1) % userInput.Length]) || userInput[(i + 1) % userInput.Length] == '.'))
            {
                //Ako je ključna riječ -> obojaj u ljubičasto
                if (tempInput.ToUpper() == UserInputParser.defineVariable || tempInput.ToUpper() == UserInputParser.helpMeni ||
                    tempInput.ToUpper() == UserInputParser.exitFromCalc || tempInput.ToUpper() == UserInputParser.clScr ||
                    tempInput == "?" || tempInput.ToUpper() == "ESC")
                {
                    userInputOperandi.Add(AddOperacija((int)ConsoleColor.Magenta, tempInput));
                }
                else
                {
                    //Ako varijabla ne postoji, ovisi o mjestu u komandi obojaj u zeleno(spremanje u novu) ili crveno(računanje s novom)
                    var tempVar = Varijable.FindPerName(tempInput);
                    if (tempVar == null || UserInputParser.IsNumber(tempInput))
                        if (UserInputParser.IsNumber(tempInput))
                            //Ako je broj -> obojaj u cyane
                            userInputOperandi.Add(AddOperacija((int)ConsoleColor.Cyan, tempInput));
                        else if (tempInput == userInput.Split("=")[0].Split(" ")[0])
                            userInputOperandi.Add(AddOperacija((int)ConsoleColor.Green, tempInput));
                        else
                            userInputOperandi.Add(AddOperacija((int)ConsoleColor.Red, tempInput));
                    else
                        //Ime postojece matrice obojaj u žutu
                        userInputOperandi.Add(AddOperacija((int)ConsoleColor.Yellow, tempInput));
                }

                brojOperanada++;
                tempInput = "";
            }

        }

        List<BojaOperacija> outputString = new List<BojaOperacija> { AddOperacija(0, "0") };
        outputString.RemoveAt(0);

        for (int i = 0; i < userInputOperandi.Count; i++)
        {
            if (userInputOperandi[(i + 1) % userInputOperandi.Count].operacija.Contains('^')
                || userInputOperandi[i].operacija.Contains('^')
                || userInputOperandi[(i + 1) % userInputOperandi.Count].operacija.Contains('!')
                || userInputOperandi[i].operacija.Contains('!')
                || i == userInputOperandi.Count - 1)
                outputString.Add(userInputOperandi[i]);
            else
            {
                outputString.Add(userInputOperandi[i]);
                outputString[i] = AddOperacija(outputString[i].boja, outputString[i].operacija + " ");
            }
        }

        for (int i = 0; i < outputString.Count; i++)
        {
            Console.ForegroundColor = (ConsoleColor)outputString[i].boja;
            Console.Write(outputString[i].operacija);
        }

        int brojZnakovaOperanada = 0;
        for (int i = 0; i < outputString.Count; i++)
            brojZnakovaOperanada += outputString[i].operacija.Length;

        Console.WriteLine(string.Empty.PadRight(Math.Abs(userInput.Length - brojZnakovaOperanada)));

        string outputStringUnEscaped = "";
        for (int i = 0; i < outputString.Count; i++)
        {
            outputStringUnEscaped += outputString[i].operacija;
        }

        return outputStringUnEscaped;

    }

    private static BojaOperacija AddOperacija(int bojaOperacije, string tekstOperacije)
    {
        BojaOperacija bojaOperacija;
        bojaOperacija.boja = bojaOperacije;
        bojaOperacija.operacija = tekstOperacije;

        return bojaOperacija;
    }

    private struct BojaOperacija
    {
        public int boja;
        public string operacija;
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