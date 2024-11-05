namespace AspirePostgreSQL.Web.Models;

public class Article
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}
