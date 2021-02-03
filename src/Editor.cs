using System;
using System.Collections.Generic;

namespace WordProcessor
{
    public class Editor
    {
        private int _windowWidth => Console.WindowWidth;
        private int _windowHeight => Console.WindowHeight-2;
        private Document _docToEdit;

        public Editor(Document documentToEdit = null) 
        {
            if (documentToEdit != null)
            {
                _docToEdit = documentToEdit;
            }
            else 
            {
                _docToEdit = Document.Init();
            }
        }

        private string[] bodyToLines()
        {
            int maxLineLength = _windowWidth - 4;
            string[] paragraphs = _docToEdit.Body.Split('\n');
            var lines = new List<string>();
            foreach (var p in paragraphs)
            {
                if (p.Length > maxLineLength)
                {
                    for (int i = 0; i < p.Length; i+= maxLineLength)
                    {
                        lines.Add(p.Substring(i, Math.Min(maxLineLength, p.Length-i)));
                    }
                }
                else
                {
                    lines.Add(p);
                }
            }
            return lines.ToArray();
        }

        private void printInterface()
        {
            void setConsoleColor(int x, int y)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                if (y == 0)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (x == 0 || x == _windowWidth-1 ||
                        y == 1 || y == _windowHeight-1)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                }
            }

            var windowTitle = $"  {_docToEdit.Title} - C# Word Processor";
            string[] bodyLines = bodyToLines();
            for (int y = 0; y < _windowHeight; y++)
            {
                for (int x = 0; x < _windowWidth; x++)
                {
                    setConsoleColor(x, y);
                    // Body is printed starting at [2, 2]:
                    bool bodyArrayContainsCoords = y - 2 >= 0 && x - 2 >= 0 &&
                        bodyLines.Length > y - 2 && bodyLines[y - 2].Length > x - 2;

                    if (y == 0 && x < windowTitle.Length)
                    {
                        Console.Write(windowTitle[x]);
                    }
                    else if (bodyArrayContainsCoords)
                    {
                        Console.Write(bodyLines[y - 2][x - 2]);
                    }
                    else 
                    {
                        Console.Write(' ');
                    }

                    Console.ResetColor();
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                printInterface();
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}