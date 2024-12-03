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

        public static double GetHappinessMultiplier(Happiness happiness) => happiness switch
        {
            Happiness.TremendouslyUnhappy => 0.1,
            Happiness.ConsiderablyUnhappy => 0.5,
            Happiness.ModeratelyUnhappy => 0.7,
            Happiness.SlightlyUnhappy => 0.9,
            Happiness.Neutral => 1,
            Happiness.SlightlyHappy => 1.1,
            Happiness.ModeratelyHappy => 1.2,
            Happiness.ConsiderablyHappy => 1.5,
            Happiness.TremendouslyHappy => 2,
            _ => throw new NotImplementedException(),
        };

        public static double GetRandomHappinessMultiplier(Happiness happiness)
        {
            double multiplier = GetHappinessMultiplier(happiness);

            if (multiplier == 0)
                return multiplier;

            return multiplier + GetRandomVariance(0.1);
        }

        public static double GetRandomVariance(double amount)
        {
            int random = RandomRange(7);

            return random switch
            {
                0 => amount,
                1 => -amount,
                2 => amount * 0.5,
                3 => -amount * 0.5,
                4 => amount * 0.25,
                5 => -amount * 0.25,
                6 => amount * 0.75,
                7 => -amount * 0.75,
                _ => throw new NotImplementedException(),
            };
        }

        public static Happiness DecreaseHappiness(Happiness happiness)
        {
            if (happiness == Happiness.TremendouslyUnhappy)
                return happiness;

            return happiness - 1;
        }

        public static Happiness IncreaseHappiness(Happiness happiness)
        {
            if (happiness == Happiness.TremendouslyHappy)
                return happiness;

            return happiness + 1;
        }
    }
}
