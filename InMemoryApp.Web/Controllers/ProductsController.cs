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

            return View();
        }

        public IActionResult Show()
        {
            // cache ten al
            ViewBag.zaman = _memoryCache.Get<string>("zaman");
            
            // cache ten sil
            _memoryCache.Remove("zaman");

            // varsa al yoksa oluştur
            _memoryCache.GetOrCreate<string>("zaman", entry =>
            {
                return DateTime.Now.ToString();
            });
            
            return View();
        }
    }
}
