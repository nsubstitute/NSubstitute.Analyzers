public class TableOfContentsEntry {

    public string CheckId { get; }
    public string Category { get; }
    public string Description { get; }
    public FilePath FilePath { get; }

    private static string CheckIdPattern = @"<td>CheckId<\/td>\s*<td>(?<value>[\w\s]+)<\/td>";
    private static string CategoryPattern = @"<td>Category<\/td>\s*<td>(?<value>[\w\s\-]+)<\/td>";
    private static string DescriptionPattern = @"## Cause\s+(?<value>[^#]+)\s+##";

    private TableOfContentsEntry(string checkId, string category, string description, FilePath file)
    {
        CheckId = checkId;
        Category = category;
        Description = description;
        FilePath = file;
    }

    public static TableOfContentsEntry Parse(FilePath file)
    {
        var s = System.IO.File.ReadAllText(file.ToString());
        var checkId = Regex.Match(s, CheckIdPattern).Groups["value"].Value.Trim();
        var category = Regex.Match(s, CategoryPattern).Groups["value"].Value.Trim();
        var description = Regex.Match(s, DescriptionPattern).Groups["value"].Value.Trim();
        return new TableOfContentsEntry(checkId, category, description, file);
    }

    public override string ToString() => $"{CheckId}: {Category}\n{Description}";
}