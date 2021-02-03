using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordProcessor
{
    static class ExtensionsAndHelpers
    {   
        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            var rand = new Random();
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        public static double RandomDouble(double min, double max)
        {
            var rand = new Random();
            return rand.NextDouble() * (max - min) + min;
        }
        
        public static bool RandomBool()
        {
            var rand = new Random();
            return rand.Next() > (Int32.MaxValue / 2);
        }

        public static List<T> RandomSample<T>(this IEnumerable<T> enumerable, int sampleSize)
        {
            if (sampleSize < enumerable.Count())
            {
                var sample = new List<T>();
                int i = 0;
                while (i < sampleSize)
                {
                    var potentialRandomElement = enumerable.RandomElement();
                    if (!sample.Contains(potentialRandomElement))
                    {
                        sample.Add(potentialRandomElement);
                        i++;
                    }
                }
                return sample;
            }
            else
            {
                return enumerable.ToList();
            }
        }
        
        public static bool IsBetween(this IComparable num, IComparable min, IComparable max)
        {
            return num.CompareTo(min) > 0 && num.CompareTo(max) < 0;
        }
        
        public static void MoveElement<T>(this List<T> list, T TElement, int distance)
        {
            if (list.Contains(TElement))
            {
                int oldIndex = list.IndexOf(TElement);
                int newIndex = oldIndex + distance;
                if (newIndex < 0) 
                {
                    newIndex = 0;
                }
                else if (newIndex > list.Count - 1) 
                {
                    newIndex = list.Count -1;
                }
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, TElement);
            }
            else
            {
                throw new Exception("List does not contain element.");
            }
        }
        
        public static string ToString(this IEnumerable<string> source, string conjunction)
        {
            if (source == null || !source.Any()) return null;
            var sourceArr = source.ToArray();
            string output = "";
            for (int i = 0; i < sourceArr.Length; i++) 
            {
                output += sourceArr[i];
                if (i != sourceArr.Length-1) output += (sourceArr.Length == 2 ? " " : ", ");
                if (i == sourceArr.Length-2) output += $"{conjunction} ";
            }
            return output;
        }
        
        public static string FromTitleOrCamelCase(this string source)
        {
            string output = Regex.Replace(source, @"([A-Z])", " " + "$1").ToLower();
            output = Regex.Replace(output, @"_", "");
            if (output[0] == ' ') output.ToCharArray().ToList().RemoveAt(0);
            return output;
        }

        public static string IndefiniteArticle(this string noun)
        {
            return "AEIOUaeiou".IndexOf(noun[0])
            >= 0 ? "an" : "a";
        }
    
        public static string PromptLine(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        public static ConsoleKey PromptKey(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadKey(true).Key;
        }

        public static string PromptLineLoop(string prompt, Func<string, bool> validator)
        {
            string output = null;
            while (output == null)
            {
                var input = PromptLine(prompt);
                if (validator(input)) output = input;
                Console.Clear();
            }
            return output;
        }
    }
}