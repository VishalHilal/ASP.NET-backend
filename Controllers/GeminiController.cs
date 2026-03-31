using Microsoft.AspNetCore.Mvc;
using AIImageGeneratorBackend.Services;

namespace AIImageGeneratorBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public GeminiController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        public async Task<IActionResult> Generate(string prompt)
        {
            var result = await _geminiService.GenerateText(prompt);
            return Ok(new { response = result });
        }
    }
}
