using System;
using System.Collections.Generic;
using static WordProcessor.ExtensionsAndHelpers;

namespace WordProcessor
{
    public class Document
    {
        public string Title {get; private set;}
        public string Author {get; private set;}
        public DateTime CreationDate {get; private set;}
        public string Body {get; set;}

        public Document(string title, string author, DateTime? creationDate = null, string body = "")
        {
            Title = title;
            Author = author;
            CreationDate = creationDate != null ? (DateTime)creationDate : DateTime.Now;
            Body = body;
        }

        public Document(Document documentToClone)
        {
            Title = documentToClone.Title;
            Author = documentToClone.Author;
            CreationDate = documentToClone.CreationDate;
            Body = documentToClone.Body;
        }

        public static Document Init()
        {
            bool InputIsNotNullOrEmpty(string input) { return !String.IsNullOrEmpty(input); }

            string title = PromptLineLoop("Enter a title for this document:", InputIsNotNullOrEmpty);
            string author = PromptLineLoop("Enter an author for this document:", InputIsNotNullOrEmpty);
            
            return new Document(title, author);
        }

        public string[] GetLines(int lineLength)
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
            return lines.ToArray();
        }
    }
}