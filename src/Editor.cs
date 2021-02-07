using System;
using System.Collections.Generic;
using static WordProcessor.ExtensionsAndHelpers;

namespace WordProcessor
{
    public class Editor
    {
        private int _windowWidth => Console.WindowWidth;
        private int _windowHeight => Console.WindowHeight-1;
        private Document _docToEdit;
        private List<string> _editHistory = new List<string>();
        private int _currentVersion;
        private Position _cursor = null;
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

        private bool bodyContainsCoords(List<string> lines, Position editorCoords)
        {
            return toBodyCoord(editorCoords.Y).IsBetween(-1, lines.Count) && 
                toBodyCoord(editorCoords.X).IsBetween(-1, lines[toBodyCoord(editorCoords.Y)].Length);
        }
        private void printUI(List<string> lines)
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
                else if (x == _cursor.X && y == _cursor.Y)
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
                    setConsoleColor(x, y);
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

        private void setDefaultCursorPosition(List<string> lines)
        {
            bool doesLastLineReachRightMargin = lines[lines.Count-1].Length >= _windowWidth - 4;
            int defaultCursorX = doesLastLineReachRightMargin ? toEditorCoord(0) : toEditorCoord(lines[lines.Count-1].Length);
            int defaultCursorY = doesLastLineReachRightMargin ? toEditorCoord(lines.Count) : toEditorCoord(lines.Count-1);
            _cursor = new Position(defaultCursorX, defaultCursorY);
        }

        private void moveCursor(string direction, List<string> lines)
        {
            var cursorBodyCoords = toBodyCoords(_cursor);
            switch(direction.ToLower())
            {
                case "up":
                    if (cursorBodyCoords.Y == 0) _cursor.X = toEditorCoord(0);
                    else 
                    {
                        if (cursorBodyCoords.X > lines[cursorBodyCoords.Y - 1].Length - 1)
                        {
                            _cursor.X = toEditorCoord(lines[cursorBodyCoords.Y - 1].Length);
                        }
                        _cursor.Y--;
                    }
                    break;
                case "right":
                    // if cursor is at end of line:
                    if (cursorBodyCoords.X == Math.Min(lines[cursorBodyCoords.Y].Length, _windowWidth-5))
                    {
                        // if not last line, move cursor to start of next line:
                        if (cursorBodyCoords.Y < lines.Count-1)
                        {
                            _cursor.X = toEditorCoord(0);
                            _cursor.Y++;
                        }
                        // else, do nothing
                    }
                    else 
                    {
                        _cursor.X++;
                    }
                    break;
                case "down":
                    if (cursorBodyCoords.Y == lines.Count - 1) _cursor.X = toEditorCoord(lines[lines.Count - 1].Length);
                    else 
                    {
                        if (cursorBodyCoords.X > lines[cursorBodyCoords.Y + 1].Length - 1)
                        {
                            _cursor.X = toEditorCoord(lines[cursorBodyCoords.Y + 1].Length);
                        }
                        _cursor.Y++;
                    }
                    break;
                case "left":
                    if (cursorBodyCoords.X == 0)
                    {
                        if (cursorBodyCoords.Y > 0)
                        {
                            _cursor.X = toEditorCoord(Math.Min(lines[cursorBodyCoords.Y-1].Length, _windowWidth-5));
                            _cursor.Y--;
                        }
                    }
                    else 
                    {
                        _cursor.X--;
                    }
                    break;
                default:
                    throw new Exception($"Unable to move cursor due to invalid direction: {direction}");
            }
        }

        private void updateEditHistory()
        {
            if (_currentVersion < _editHistory.Count - 1) 
            {
                _editHistory.RemoveAll(v => _editHistory.IndexOf(v) > _currentVersion);
            }
            _editHistory.Add(_docToEdit.Body);
            _currentVersion = _editHistory.Count - 1;
        }

        private void undo(List<string> lines)
        {
            if (_currentVersion > 0)
            {
                _currentVersion--;
                _docToEdit.Body = _editHistory[_currentVersion];
                lines = _docToEdit.GetLines(_windowWidth - 4);
                setDefaultCursorPosition(lines);
            }
        }

        private void redo(List<string> lines)
        {
            if (_currentVersion < _editHistory.Count - 1)
            {
                _currentVersion++;
                _docToEdit.Body = _editHistory[_currentVersion];
                lines = _docToEdit.GetLines(_windowWidth - 4);
                setDefaultCursorPosition(lines);
            }
        }
        private void addString(string stringToAdd, List<string> lines)
        {
            //Insert added string to lines array at location:
            lines[toBodyCoord(_cursor.Y)] = lines[toBodyCoord(_cursor.Y)].Insert(toBodyCoord(_cursor.X), stringToAdd);
            //Join lines array to string and reassign back to body:
            _docToEdit.LinesToBody(lines, _windowWidth - 4);
            //Recreate lines array from body:
            lines = _docToEdit.GetLines(_windowWidth - 4);
            //Move cursor right based on length of added string:
            for (int i = 0; i < stringToAdd.Length; i++) moveCursor("right", lines);
            updateEditHistory();
        }

        private void removeChar(List<string> lines)
        {
            // If body has at least 1 character:
            if (_docToEdit.Body.Length > 0)
            {
                if (bodyContainsCoords(lines, _cursor))
                {
                    //Remove char at cursor:
                    lines[toBodyCoord(_cursor.Y)] = lines[toBodyCoord(_cursor.Y)].Remove(toBodyCoord(_cursor.X), 1);
                }
                // else if cursor is at end of line and line is not last line:
                else if (toBodyCoord(_cursor.Y).IsBetween(-1, lines.Count - 1) &&
                    toBodyCoord(_cursor.X) == lines[toBodyCoord(_cursor.Y)].Length)
                {
                    lines[toBodyCoord(_cursor.Y)] += lines[toBodyCoord(_cursor.Y) + 1];
                    lines.RemoveAt(toBodyCoord(_cursor.Y) + 1);
                }
                //Join lines array to string and reassign back to body:
                _docToEdit.LinesToBody(lines, _windowWidth - 4);
                //Recreate lines array from body:
                lines = _docToEdit.GetLines(_windowWidth - 4);
                updateEditHistory();
            }
        }

        private void awaitInput(List<string> lines)
        {
            var input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.UpArrow: 
                case ConsoleKey.RightArrow: 
                case ConsoleKey.DownArrow: 
                case ConsoleKey.LeftArrow: 
                    string direction = Enum.GetName(typeof(ConsoleKey), input.Key).Replace("Arrow", "");
                    moveCursor(direction, lines);
                    break;

                case ConsoleKey.Backspace: 
                    moveCursor("left", lines);
                    removeChar(lines); 
                    break;

                case ConsoleKey.Delete:
                    removeChar(lines);
                    break;

                case ConsoleKey.Tab:
                    addString("    ", lines);
                    break;

                case ConsoleKey.Enter: 
                    addString("\n", lines); 
                    break;
                
                case ConsoleKey.O when input.Modifiers == ConsoleModifiers.Control:
                    _docToEdit = FileHandler.Open(PromptLineLoop($"Enter file to open (including file path):", FileHandler.IsValidFilePath));
                    Console.ReadKey();
                    break;

                case ConsoleKey.S when input.Modifiers == (ConsoleModifiers.Shift | ConsoleModifiers.Control):
                    FileHandler.Save(_docToEdit, PromptLineLoop($"Enter directory to save {_docToEdit.Title}.txt to:", FileHandler.IsValidDirectory));
                    break;

                case ConsoleKey.S when input.Modifiers == ConsoleModifiers.Control:
                    FileHandler.Save(_docToEdit);
                    Console.ReadKey();
                    break;
                
                case ConsoleKey.Z when input.Modifiers == ConsoleModifiers.Control:
                    undo(lines);
                    break;

                case ConsoleKey.Y when input.Modifiers == ConsoleModifiers.Control:
                    redo(lines);
                    break;

                default: 
                    addString(input.KeyChar.ToString(), lines); 
                    break;
            }
        }

        public void Run()
        {
            updateEditHistory();
            while (true)
            {
                var lines = _docToEdit.GetLines(_windowWidth - 4);
                if (_cursor == null) setDefaultCursorPosition(lines);
                printUI(lines);
                awaitInput(lines);
                Console.Clear();
            }
        }
    }
}