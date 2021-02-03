using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordProcessor;

namespace WordProcessorTests
{
    [TestClass]
    public class DocumentTests
    {
        Document docToAddChar;
        Document docToModify;
        Document docToRemoveChar;

        [TestInitialize]
        public void AddCharSetup() 
        {
            docToAddChar = new Document(title:"The Title", author:"The Author", body:"The Bod");
            docToAddChar.AddChar('y');

            docToModify = new Document(title:"The Title", author:"The Author");
            docToModify.AddChar('a');
            docToModify.AddChar('b');
            docToModify.AddChar('c');
            docToModify.AddChar('d');
            docToModify.AddChar('e');

            docToRemoveChar = new Document(title:"The Title", author:"The Author", body:"The Body");
            docToRemoveChar.RemoveChar();
        }

        [TestMethod]
        public void AddCharAppendsCharToBody()
        {
            Assert.IsTrue(docToAddChar.Body == "The Body");
        }

        [TestMethod]
        public void AddCharAddsCopyToHistory()
        {
            Assert.IsTrue(docToAddChar.EditHistory.Count == 1);
        }

        [TestMethod]
        public void EditHistoryCopiesAreDistinct()
        {
            Assert.IsTrue(docToModify.EditHistory.Distinct().Count() == 5);
        }

        [TestMethod]
        public void RemoveCharRemovesCharFromBody()
        {
            Assert.IsTrue(docToRemoveChar.Body == "The Bod");
        }

        [TestMethod]
        public void RemoveCharAddsCopyToHistory()
        {
            Assert.IsTrue(docToRemoveChar.EditHistory.Count == 1);
        }
    }
}
