using System;
using System.Collections.Generic;
using System.Linq;
using static WordProcessor.ExtensionsAndHelpers;

namespace WordProcessor
{
    public class Document
    {
        public string Title {get; private set;}
        public string Body {get; set;}

        public Document(string title, string body = "")
        {
            Title = title;
            Body = body;
        }

        public Document(Document documentToClone)
        {
            Title = documentToClone.Title;
            Body = documentToClone.Body;
        }

        public static Document Init()
        {
            bool InputIsNotNullOrEmpty(string input) { return !String.IsNullOrEmpty(input); }

            string title = PromptLineLoop("Enter a title for this document:", InputIsNotNullOrEmpty);
            
            return new Document(title);
        }

        public List<string> GetLines(int lineLength)
        {
            string[] paragraphs = Body.Split('\n');
            var lines = new List<string>();
            foreach (var p in paragraphs)
            {
                if (p.Length > lineLength)
                {
                    for (int i = 0; i < p.Length; i+= lineLength)
                    {
                        lines.Add(p.Substring(i, Math.Min(lineLength, p.Length-i)));
                    }
                }
                else
                {
                    lines.Add(p);
                }
            }
            return lines;
        }

        public void LinesToBody(List<string> lines, int lineLength)
        {
            var linesWithNewLineChars = lines.Select(l => l.Length < lineLength && lines.IndexOf(l) < lines.Count - 1 ? l + '\n' : l);
            Body = String.Join("", linesWithNewLineChars);
        }
    }
}