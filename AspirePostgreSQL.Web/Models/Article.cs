namespace AspirePostgreSQL.Web.Models;

public class Article
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public List<string> Tags{ get; set; } = new();

    public DateTime? CreatedOnUtc { get; set; } = new();

    public DateTime? PublishedOnUtc { get; set; } = new();
}
