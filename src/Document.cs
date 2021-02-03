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
        public Dictionary<DateTime, string> EditHistory {get; protected set;} = new Dictionary<DateTime, string>();
        public string Body {get; private set;}

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

        public void AddChar(char charToAdd)
        {
            EditHistory.Add(DateTime.Now, Body);
            Body += charToAdd;
        }

        public void RemoveChar()
        {
            EditHistory.Add(DateTime.Now, Body);
            Body = Body.Remove(Body.Length-1, 1);
        }
    }
}