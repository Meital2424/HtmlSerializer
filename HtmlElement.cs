using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public HtmlElement()
        {
        }

        public HtmlElement(string name)
        {
            Name = name;
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }

        // פונקציה ריצה על כל הצאצאים
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var currentElement = queue.Dequeue();
                yield return currentElement;

                foreach (var child in currentElement.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        // פונקציה ריצה על כל האבות
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        // פונקציה למציאת אלמנטים בעץ לפי סלקטור
        public static HashSet<HtmlElement> FindElementsBySelector(HtmlElement element, Selector selector)
        {
            var results = new HashSet<HtmlElement>();
            FindElementsRecursively(element, selector, results);
            return results;
        }

        // פונקציה ריקורסיבית למציאת אלמנטים
        private static void FindElementsRecursively(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
        {
            // בדיקה אם האלמנט תואם לסלקטור הנוכחי
            if (MatchesSelector(element, selector))
            {
                results.Add(element);  // הוספה ל-HashSet מונעת כפילויות
            }

            // ריצה על כל הצאצאים של האלמנט
            foreach (var child in element.Descendants())
            {
                FindElementsRecursively(child, selector, results);
            }
        }

        // פונקציה לבדוק אם האלמנט תואם לסלקטור
        private static bool MatchesSelector(HtmlElement element, Selector selector)
        {
            if (!string.IsNullOrEmpty(selector.TagName) && !element.Name.Equals(selector.TagName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(selector.Id) && !element.Id.Equals(selector.Id, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (selector.Classes != null && selector.Classes.Count > 0)
            {
                foreach (var className in selector.Classes)
                {
                    if (!element.Classes.Contains(className))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}



