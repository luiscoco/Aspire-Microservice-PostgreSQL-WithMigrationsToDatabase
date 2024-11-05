using AspirePostgreSQL.ApiService.Service;
using AspirePostgreSQL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AspirePostgreSQL.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleService _articleService;

        public ArticlesController(ArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Article article)
        {
            if (article == null)
            {
                return BadRequest();
            }

            try
            {
                var createdArticle = await _articleService.CreateAsync(article);
                return CreatedAtAction(nameof(GetById), new { id = createdArticle.Id }, createdArticle);
            }
            catch (Exception ex)
            {
                // Log the exception or write to the console for debugging
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Article article)
        {
            if (article == null || article.Id != id)
            {
                return BadRequest();
            }

            var success = await _articleService.UpdateAsync(article);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _articleService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
