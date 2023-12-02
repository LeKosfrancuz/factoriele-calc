using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputParsing;

public static class CommandColor
{
    public static string LineColoring(string userInput, List<Varijabla> varijable)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write("                                \r"); //Da očisti help poruku
        if (userInput == "") Console.Write("Upiši \"?\" za pomoc\r");

        (int x_pomak, int y_pomak) = Console.GetCursorPosition();
        //int x_pomak = Console.CursorLeft;
        Console.SetCursorPosition(x_pomak, y_pomak - 1);

        if (userInput == "") return userInput;

        Console.Write(">");

        //Console.Write("\x1b[1A\r\x1b[2K\r>");

        List<BojaOperacija> userInputOperandi = new();

        string tempInput = "";
        int brojOperanada = 0;


        for (int i = 0; i < userInput.Length; i++)
        {
            tempInput += userInput[i];

            if (tempInput == "+" || tempInput == "*" || tempInput == "/" || tempInput == "^" || tempInput == "!" || tempInput == "=")
            {
                userInputOperandi.Add(AddOperacija((int)ConsoleColor.White, tempInput));
                brojOperanada++;
                tempInput = "";
                continue;
            } else if (tempInput == "-")
            {
                for (int j = 1; j < userInput.Length && j <= i; j++)
                {
                    char prevOperator = userInput[i - j];
                    if (prevOperator == ' ') continue;
                    
                    if (prevOperator == '+' || prevOperator == '*' || prevOperator == '/' || prevOperator == '-'
                        || prevOperator == '^' || prevOperator == '!' || prevOperator == '=' || userInput[(i - j)..i].Length == 0)
                    {
                        int k = i+1;
                        if (i + 1 < userInput.Length)
                        {
                            while (userInput[(i + 1)..k].Replace(" ", "").Equals("") && k < userInput.Length) k++;
                            i = k - 2;
                        }
                        break;
                    }

                    
                    userInputOperandi.Add(AddOperacija((int)ConsoleColor.White, tempInput));
                    brojOperanada++;
                    tempInput = "";
                    break;
                }
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
                    var tempVar = varijable.FindPerName(tempInput);
                    if (tempVar == null || UserInputParser.IsNumber(tempInput))
                    {
                        if (UserInputParser.IsNumber(tempInput))
                            //Ako je broj -> obojaj u cyane
                            userInputOperandi.Add(AddOperacija((int)ConsoleColor.Cyan, tempInput));
                        else if (userInput.Split("=").Length > 1 && tempInput == userInput.Split("=")[0].Split(" ")[0])
                            userInputOperandi.Add(AddOperacija((int)ConsoleColor.Green, tempInput));
                        else
                            userInputOperandi.Add(AddOperacija((int)ConsoleColor.Red, tempInput));
                    }
                    else
                    {
                        //Ime postojece matrice obojaj u žutu
                        userInputOperandi.Add(AddOperacija((int)ConsoleColor.Yellow, tempInput));
                    }
                }

                brojOperanada++;
                tempInput = "";
            }

        }

        List<BojaOperacija> outputString = new();

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
