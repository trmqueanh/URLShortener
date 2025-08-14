using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using URLShortener.MVC.Models;
using URLShortener.MVC.Data;

namespace URLShortener.MVC.Controllers
{
    public class HistoryController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<HistoryController> _logger;

        public HistoryController(DataContext context, ILogger<HistoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var allUrls = _context.HomeModels
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
            return View(allUrls);
        }
    }
}
