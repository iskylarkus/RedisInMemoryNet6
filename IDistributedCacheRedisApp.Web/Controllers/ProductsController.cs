using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions();
            
            distributedCacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            distributedCacheEntryOptions.SlidingExpiration = TimeSpan.FromSeconds(20);

            _distributedCache.SetString("name", "ilker", distributedCacheEntryOptions);

            await _distributedCache.SetStringAsync("surname", "selvi", distributedCacheEntryOptions);

            return View();
        }

        public async Task<IActionResult> Show()
        {
            string name = _distributedCache.GetString("name");

            if (String.IsNullOrEmpty(name))
            {
                name = "NONE";
            }

            ViewBag.name = name;

            string surname = await _distributedCache.GetStringAsync("surname");

            if (String.IsNullOrEmpty(surname))
            {
                surname = "NONE";
            }

            ViewBag.surname = surname;

            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("name");

            return View();
        }
    }
}
