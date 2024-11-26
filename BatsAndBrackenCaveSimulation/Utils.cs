using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using static Library.Core.Methods;

namespace BatsAndBrackenCaveSimulation
{
    public static class Utils
    {
        public static string HappinessToString(Happiness happiness)
        {
            string happinessString = happiness.ToString();

            if (happiness == Happiness.Neutral)
                return happinessString;

            if (happinessString.Contains('H'))
            {
                string[] strings = happinessString.Split('H');

                return strings[0] + " H" + strings[1];
            }

            if (happinessString.Contains('U'))
            {
                string[] strings = happinessString.Split('U');

                return strings[0] + " U" + strings[1];
            }

            return happinessString;
        }
    }
}
