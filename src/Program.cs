using System;
using System.IO;
// using System.Windows.Forms;

namespace WordProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var newDoc = new Document("Lorem ipsum", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\nUt enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\nDuis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
            var testEditor = new Editor(newDoc);
            testEditor.Run();
        }
    }
}
