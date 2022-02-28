using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public ProductsController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            /*

            // yoksa set et ilk yöntem
            if (String.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            {
                _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            }

            // al ve out ile ver yoksa set et ikinci yöntem
            if (!_memoryCache.TryGetValue("zaman", out string zamanCache))
            {
                _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            }

            ViewBag.zaman = zamanCache;

            // direk kontrol etmeden oluştur
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString());

            */

            if (!_memoryCache.TryGetValue("zaman", out string zamanCache))
            {
                MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions();
                
                //memoryCacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(10);

                memoryCacheEntryOptions.SlidingExpiration = TimeSpan.FromSeconds(10);

                _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), memoryCacheEntryOptions);
            }

            return View();
        }

        public IActionResult Show()
        {
            /*

            // cache ten al
            ViewBag.zaman = _memoryCache.Get<string>("zaman");
            
            // cache ten sil
            _memoryCache.Remove("zaman");

            // varsa al yoksa oluştur
            _memoryCache.GetOrCreate<string>("zaman", entry =>
            {
                return DateTime.Now.ToString();
            });
            
            */

            _memoryCache.TryGetValue("zaman", out string zamanCache);

            ViewBag.zaman = zamanCache;

            return View();
        }
    }
}
