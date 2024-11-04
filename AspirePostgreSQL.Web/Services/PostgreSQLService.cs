using AspirePostgreSQL.Web.Models;

namespace AspirePostgreSQL.Web.Services
{
    public class ArticleModelService
    {
        private readonly HttpClient _httpClient;

        public ArticleModelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all ArticleModels
        public async Task<List<Article>> GetAllArticleModels()
        {
            return await _httpClient.GetFromJsonAsync<List<Article>>("api/Articles");
        }

        // Get ArticleModel by ID
        public async Task<Article> GetArticleModelById(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Article>($"api/Articles/{id}");
        }

        // Add new ArticleModel
        public async Task<HttpResponseMessage> AddArticleModel(Article model)
        {
            return await _httpClient.PostAsJsonAsync("api/Articles", model);
        }

        // Update ArticleModel
        public async Task<HttpResponseMessage> UpdateArticleModel(Guid id, Article model)
        {
            return await _httpClient.PutAsJsonAsync($"api/Articles/{id}", model);
        }

        // Delete ArticleModel
        public async Task<HttpResponseMessage> DeleteArticleModel(Guid id)
        {
            return await _httpClient.DeleteAsync($"api/Articles/{id}");
        }
    }
}
