using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;

namespace CmdImg
{
    class Program
    {
        private static Dictionary<ConsoleColor, Color> _colorMap = new Dictionary<ConsoleColor, Color>()
        {
            { ConsoleColor.Blue, Color.Blue },
            { ConsoleColor.Cyan, Color.Cyan },
            { ConsoleColor.DarkBlue, Color.FromArgb(0, 0, 128) },
            { ConsoleColor.DarkCyan, Color.FromArgb(0, 128, 128) },
            { ConsoleColor.DarkGreen, Color.FromArgb(0, 128, 0) },
            { ConsoleColor.DarkMagenta, Color.FromArgb(128, 0, 128) },
            { ConsoleColor.DarkRed, Color.FromArgb(128, 0, 0) },
            { ConsoleColor.DarkYellow, Color.FromArgb(128, 128, 0) },
            { ConsoleColor.Green, Color.FromArgb(0, 255, 0) },
            { ConsoleColor.Magenta, Color.Magenta },
            { ConsoleColor.Red, Color.Red },
            { ConsoleColor.Yellow, Color.Yellow },
            { ConsoleColor.Black, Color.Black },
            { ConsoleColor.DarkGray, Color.FromArgb(128, 128, 128) },
            { ConsoleColor.Gray, Color.FromArgb(192, 192, 192) },
            { ConsoleColor.White, Color.White }
        }; 

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please supply a single parameter - an image file");
                return;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("The sepcified file does not exist");
                return;
            }
            var image = new Bitmap(args[0]);
            var width = Math.Min(Console.BufferWidth - 3, image.Width);
            var height = (int)(image.Height / (double)image.Width * width);
            var resized = new Bitmap(image, width, height);
            var horizontalBorder = String.Join(String.Empty, Enumerable.Range(0, width).Select(n => "═"));
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔" + horizontalBorder + "╗");
            for (var i = 0; i < resized.Height; i += 2)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║");
                for (var j = 0; j < resized.Width; j++)
                {
                    Console.BackgroundColor = GetConsoleColor(resized.GetPixel(j, i));
                    Console.ForegroundColor = i == resized.Height - 1 ? ConsoleColor.Black : GetConsoleColor(resized.GetPixel(j, i + 1));
                    Console.Write("▄");
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("║");
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╚" + horizontalBorder + "╝");
            Console.ResetColor();
        }

        private static ConsoleColor GetConsoleColor(Color color)
        {
            var bestChoice = ConsoleColor.Black;
            if (color.A < 50)
            {
                return bestChoice;
            }

            var comparer = new CieDe2000Comparison();
            var labColor = new Rgb() { R = color.R, G = color.G, B = color.B }.To<Lab>();

            var minDiff = Double.MaxValue;
            foreach (var kvp in _colorMap)
            {
                var testColor = new Rgb() { R = kvp.Value.R, G = kvp.Value.G, B = kvp.Value.B }.To<Lab>();
                var diff = labColor.Compare(testColor, comparer);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    bestChoice = kvp.Key;
                }
            } 
            return bestChoice;
        }
    }
}
