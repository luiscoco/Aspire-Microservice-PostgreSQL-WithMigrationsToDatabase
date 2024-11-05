using AspirePostgreSQL.Database;
using AspirePostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspirePostgreSQL.ApiService.Service
{
    public class ArticleService
    {
        private readonly ApplicationDbContext _context;

        public ArticleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(Guid id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<Article> CreateAsync(Article article)
        {
            article.Id = Guid.NewGuid();

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return article;
        }

        public async Task<bool> UpdateAsync(Article updatedArticle)
        {
            Article? existingArticle = new Article();
            existingArticle = await _context.Articles.FindAsync(updatedArticle.Id);
            if (existingArticle == null)
            {
                return false;
            }

            existingArticle.Title = updatedArticle.Title;
            existingArticle.Content = updatedArticle.Content;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return false;
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
