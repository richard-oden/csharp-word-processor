using System;
using System.Collections.Generic;

namespace WordProcessor
{
    public class Editor
    {
        private int _windowWidth => Console.WindowWidth;
        private int _windowHeight => Console.WindowHeight-2;
        private Document _docToEdit;
        private Dictionary<DateTime, string> _editHistory = new Dictionary<DateTime, string>();

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

        private int toBodyCoord(int editorCoord)
        {
            return editorCoord - 2;
        }

        private Position toBodyCoords(Position editorCoords)
        {
            return new Position(editorCoords.X - 2, editorCoords.Y - 2);
        }

        private int toEditorCoord(int bodyCoord)
        {
            return bodyCoord + 2;
        }

        private Position toEditorCoords(Position bodyCoords)
        {
            return new Position(bodyCoords.X + 2, bodyCoords.Y + 2);
        }

        private bool bodyContainsCoords(string[] lines, Position editorCoords)
        {
            return toBodyCoord(editorCoords.Y) >= 0 && toBodyCoord(editorCoords.Y) < lines.Length && 
                toBodyCoord(editorCoords.X) >= 0 && toBodyCoord(editorCoords.X) < lines[toBodyCoord(editorCoords.Y)].Length;
        }
        private void printInterface(string[] lines, Position cursor)
        {
            void setConsoleColor(int x, int y, Position cursor)
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
                else if (x == cursor.X && y == cursor.Y)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            var windowTitle = $"  {_docToEdit.Title} - C# Word Processor";
            for (int y = 0; y < _windowHeight; y++)
            {
                for (int x = 0; x < _windowWidth; x++)
                {
                    setConsoleColor(x, y, cursor);
                    if (y == 0 && x < windowTitle.Length) 
                    {
                        Console.Write(windowTitle[x]);
                    }
                    else if (bodyContainsCoords(lines, new Position(x, y)))
                    {
                        Console.Write(lines[toBodyCoord(y)][toBodyCoord(x)]);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
            }
        }

        private Position getDefaultCursorPosition(string[] lines)
        {
            bool doesLastLineReachRightMargin = lines[lines.Length-1].Length > _windowWidth - 4;
            int defaultCursorX = doesLastLineReachRightMargin ? toEditorCoord(0) : toEditorCoord(lines[lines.Length-1].Length);
            int defaultCursorY = doesLastLineReachRightMargin ? toEditorCoord(lines.Length) : toEditorCoord(lines.Length-1);
            return new Position(defaultCursorX, defaultCursorY);
        }

        private void moveCursor(Position cursor, string direction, string[] lines)
        {
            var cursorBodyCoords = toBodyCoords(cursor);
            switch(direction.ToLower())
            {
                case "up":
                    if (cursorBodyCoords.Y == 0) cursor.X = toEditorCoord(0);
                    else 
                    {
                        if (cursorBodyCoords.X > lines[cursorBodyCoords.Y - 1].Length - 1)
                        {
                            cursor.X = toEditorCoord(lines[cursorBodyCoords.Y - 1].Length);
                        }
                        cursor.Y--;
                    }
                    break;
                case "right":
                    if (cursorBodyCoords.X == Math.Min(lines[cursorBodyCoords.Y].Length, _windowWidth-5))
                    {
                        if (cursorBodyCoords.Y < lines.Length-1)
                        {
                            cursor.X = toEditorCoord(0);
                            cursor.Y++;
                        }
                    }
                    else 
                    {
                        cursor.X++;
                    }
                    break;
                case "down":
                    if (cursorBodyCoords.Y == lines.Length - 1) cursor.X = toEditorCoord(lines[lines.Length - 1].Length);
                    else 
                    {
                        if (cursorBodyCoords.X > lines[cursorBodyCoords.Y + 1].Length - 1)
                        {
                            cursor.X = toEditorCoord(lines[cursorBodyCoords.Y + 1].Length);
                        }
                        cursor.Y++;
                    }
                    break;
                case "left":
                    if (cursorBodyCoords.X == 0)
                    {
                        if (cursorBodyCoords.Y > 0)
                        {
                            cursor.X = toEditorCoord(Math.Min(lines[cursorBodyCoords.Y-1].Length, _windowWidth-5));
                            cursor.Y--;
                        }
                    }
                    else 
                    {
                        cursor.X--;
                    }
                    break;
                default:
                    throw new Exception($"Unable to move cursor due to invalid direction: {direction}");
            }
        }

        private void addChar(char charToAdd, Position cursor)
        {
            _editHistory.Add(DateTime.Now, _docToEdit.Body);
            _docToEdit.Body += charToAdd;
        }

        private void removeChar(Position cursor)
        {
            if (_docToEdit.Body.Length > 1)
            {
                _editHistory.Add(DateTime.Now, _docToEdit.Body);
                _docToEdit.Body = _docToEdit.Body.Remove(_docToEdit.Body.Length-1, 1);
            }
        }

        private void awaitInput(string[] lines, Position cursor)
        {
            var input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.UpArrow: 
                case ConsoleKey.RightArrow: 
                case ConsoleKey.DownArrow: 
                case ConsoleKey.LeftArrow: 
                    string direction = Enum.GetName(typeof(ConsoleKey), input.Key).Replace("Arrow", "");
                    moveCursor(cursor, direction, lines);
                    break;

                case ConsoleKey.Backspace: 
                    removeChar(cursor); 
                    break;

                case ConsoleKey.Enter: 
                    addChar('\n', cursor); 
                    break;

                default: 
                    addChar(input.KeyChar, cursor); 
                    break;
            }
        }

        public void Run()
        {
            Position cursor = null;
            while (true)
            {
                var lines = _docToEdit.GetLines(_windowWidth - 4);
                if (cursor == null) cursor = getDefaultCursorPosition(lines);
                printInterface(lines, cursor);
                awaitInput(lines, cursor);
                Console.Clear();
            }
        }
    }
}