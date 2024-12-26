public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; } = new List<string>();
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    public Selector()
    {
        Classes = new List<string>();
    }

    // פונקציה סטטית שממירה מחרוזת של Selector לאובייקט Selector
    public static Selector Parse(string selectorString)
    {
        var selector = new Selector();
        var parts = selectorString.Split(' ');

        Selector currentSelector = selector;

        foreach (var part in parts)
        {
            if (part.StartsWith("#"))
            {
                currentSelector.Id = part.Substring(1);
            }
            else if (part.StartsWith("."))
            {
                currentSelector.Classes.Add(part.Substring(1));
            }
            else
            {
                currentSelector.TagName = part;
            }
        }

        return selector;
    }
}
