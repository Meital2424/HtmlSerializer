//אתר עזר: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_expressions/Cheatsheet

using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlSerializer;
using System.Text.Json;


namespace HtmlSerializer
{
    public class Program
    {
        static void Main(string[] args)
        {
            // קריאת קובצי JSON
            string jsonContent1 = File.ReadAllText("HtmlTags.json");
            string jsonContent2 = File.ReadAllText("HtmlVoidTags.json");

            // המרת JSON למערך מחרוזות
            string[] htmlTags = JsonSerializer.Deserialize<string[]>(jsonContent1) ?? Array.Empty<string>();
            string[] htmlVoidTags = JsonSerializer.Deserialize<string[]>(jsonContent2) ?? Array.Empty<string>();

            Console.WriteLine("Printing the files HtmlTags after conversion:");

            // הדפסת הנתונים
            foreach (var e in htmlTags)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Printing the files HtmlVoidTags after conversion:");
            foreach (var e in htmlVoidTags)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("------------------------------------------------------------------");

            // יצירת עץ HTML
            HtmlElement root = new HtmlElement("html");
            HtmlElement currentElement = root;

            foreach (string line in htmlTags)
            {
                ProcessLine(line, ref currentElement, htmlVoidTags);
            }

            // הצגת מבנה העץ שנוצר
            PrintHtmlTree(root, 0);
        }

        static void ProcessLine(string line, ref HtmlElement currentElement, string[] voidTags)
        {
            // חתוך את המילה הראשונה
            string[] parts = line.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string firstWord = parts[0];
            string remainingContent = parts.Length > 1 ? parts[1] : string.Empty;

            if (firstWord == "html/")
            {
                Console.WriteLine("Reached the end of the HTML document.");
                return;
            }

            if (firstWord.StartsWith("/"))
            {
                // תגית סוגרת - עלה לרמת האב
                currentElement = currentElement.Parent ?? currentElement;
                return;
            }

            // תגית חדשה - צור אובייקט HtmlElement
            HtmlElement newElement = new HtmlElement(firstWord);
            ParseAttributes(remainingContent, newElement);

            // עדכון מאפיינים חשובים
            if (newElement.Attributes.ContainsKey("id"))
            {
                newElement.Id = newElement.Attributes["id"];
            }

            if (newElement.Attributes.ContainsKey("class"))
            {
                newElement.Classes.AddRange(newElement.Attributes["class"].Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }

            // בדיקה אם תגית סוגרת את עצמה
            if (line.EndsWith("/") || voidTags.Contains(firstWord))
            {
                currentElement.Children.Add(newElement);
                newElement.Parent = currentElement;
                return;
            }

            // עדכון הורה והוספת תגיות ילדים
            currentElement.Children.Add(newElement);
            newElement.Parent = currentElement;
            currentElement = newElement;

        }

        static void ParseAttributes(string content, HtmlElement element)
        {
            // ביטוי רגולרי לפירוק Attributes
            Regex regex = new Regex(@"(\w+)=[""']([^""']+)[""']");
            MatchCollection matches = regex.Matches(content);

            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                element.Attributes[key] = value;
            }
        }

        static void PrintHtmlTree(HtmlElement element, int level)
        {
            Console.WriteLine(new string(' ', level * 2) + $"<{element.Name}>");

            if (!string.IsNullOrEmpty(element.InnerHtml))
            {
                Console.WriteLine(new string(' ', (level + 1) * 2) + element.InnerHtml);
            }

            foreach (var child in element.Children)
            {
                PrintHtmlTree(child, level + 1);
            }

            Console.WriteLine(new string(' ', level * 2) + $"</{element.Name}>");
        }
    }

}


