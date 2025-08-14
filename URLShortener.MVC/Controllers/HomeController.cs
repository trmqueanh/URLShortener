using Microsoft.AspNetCore.Mvc;
using URLShortener.MVC.Models;
using URLShortener.MVC.Data;
using URLShortener.MVC.Services;

namespace URLShortener.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DataContext context, IUrlShortenerService urlShortenerService, ILogger<HomeController> logger)
        {
            _context = context;
            _urlShortenerService = urlShortenerService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(HomeModel model)
        {
            _logger.LogInformation("Received form submission: Description={Description}, OriginalUrl={OriginalUrl}, CharacterCount={CharacterCount}", 
                model.Description, model.OriginalUrl, model.CharacterCount);

            // Remove ShortenedUrl from ModelState validation since it's auto-generated
            ModelState.Remove("ShortenedUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    string shortenedUrl;
                    int maxAttempts = 10;
                    int attempts = 0;

                    // Generate unique shortened URL
                    do
                    {
                        shortenedUrl = _urlShortenerService.GenerateShortenedUrl(model.CharacterCount);
                        attempts++;
                        _logger.LogInformation("Generated shortened URL attempt {Attempt}: {ShortenedUrl}", attempts, shortenedUrl);
                    } while (!_urlShortenerService.IsShortenedUrlUnique(shortenedUrl, _context) && attempts < maxAttempts);

                    if (attempts >= maxAttempts)
                    {
                        _logger.LogWarning("Failed to generate unique shortened URL after {MaxAttempts} attempts", maxAttempts);
                        ModelState.AddModelError("", "Unable to generate unique shortened URL. Please try again.");
                        return View(model);
                    }

                    // Set the generated shortened URL
                    model.ShortenedUrl = shortenedUrl;
                    
                    _logger.LogInformation("Saving to database: {ShortenedUrl} -> {OriginalUrl}", shortenedUrl, model.OriginalUrl);
                    
                    _context.HomeModels.Add(model);
                    _context.SaveChanges();
                    
                    var host = $"{Request.Scheme}://{Request.Host}/";
                    ViewBag.Message = $"Your shortened URL is: {host}s/{model.ShortenedUrl}";
                    ViewBag.ShortenedUrl = model.ShortenedUrl;
                    ViewBag.OriginalUrl = model.OriginalUrl;
                    
                    _logger.LogInformation("Successfully created shortened URL: {ShortenedUrl}", model.ShortenedUrl);
                    
                    return View(new HomeModel());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating shortened URL");
                    ModelState.AddModelError("", "An error occurred while creating the shortened URL. Please try again.");
                    return View(model);
                }
            }
            else
            {
                _logger.LogWarning("Model validation failed: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete([FromBody] DeleteRequest request)
        {
            _logger.LogInformation("Delete request received for ID: {Id}", request?.Id);
            
            try
            {
                if (request == null || request.Id <= 0)
                {
                    _logger.LogWarning("Invalid delete request: ID is null or invalid");
                    return Json(new { success = false, message = "Invalid request." });
                }

                var urlToDelete = _context.HomeModels.Find(request.Id);
                if (urlToDelete == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent URL with ID: {Id}", request.Id);
                    return Json(new { success = false, message = "URL not found." });
                }

                _logger.LogInformation("Deleting URL: ID={Id}, ShortenedUrl={ShortenedUrl}", request.Id, urlToDelete.ShortenedUrl);
                
                _context.HomeModels.Remove(urlToDelete);
                _context.SaveChanges();
                
                _logger.LogInformation("Successfully deleted URL with ID: {Id}", request.Id);
                
                return Json(new { success = true, message = "URL deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting URL with ID: {Id}", request?.Id);
                return Json(new { success = false, message = "An error occurred while deleting the URL." });
            }
        }
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }
}
