using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

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

        public async Task<IActionResult> Create()
        {
            DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions();

            distributedCacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(10);

            distributedCacheEntryOptions.SlidingExpiration = TimeSpan.FromSeconds(100);

            Product product1 = new Product { Id = 1, Name = "Kalem", Price = 38 };
            Product product2 = new Product { Id = 2, Name = "Silgi", Price = 19 };
            Product product3 = new Product { Id = 3, Name = "Kitap", Price = 95 };

            string jsonProduct1 = JsonSerializer.Serialize(product1);
            string jsonProduct2 = JsonSerializer.Serialize(product2);
            string jsonProduct3 = JsonSerializer.Serialize(product3);

            await _distributedCache.SetStringAsync($"{nameof(Product)}:{product1.Id}", jsonProduct1, distributedCacheEntryOptions);
            await _distributedCache.SetStringAsync($"{nameof(Product)}:{product2.Id}", jsonProduct2, distributedCacheEntryOptions);
            await _distributedCache.SetStringAsync($"{nameof(Product)}:{product3.Id}", jsonProduct3, distributedCacheEntryOptions);



            byte[] byteProduct1 = Encoding.UTF8.GetBytes(jsonProduct1);
            byte[] byteProduct2 = Encoding.UTF8.GetBytes(jsonProduct2);
            byte[] byteProduct3 = Encoding.UTF8.GetBytes(jsonProduct3);

            await _distributedCache.SetAsync($"b{nameof(Product)}:{product1.Id}", byteProduct1, distributedCacheEntryOptions);
            await _distributedCache.SetAsync($"b{nameof(Product)}:{product2.Id}", byteProduct2, distributedCacheEntryOptions);
            await _distributedCache.SetAsync($"b{nameof(Product)}:{product3.Id}", byteProduct3, distributedCacheEntryOptions);



            return View();
        }

        public async Task<IActionResult> List()
        {
            string jsonProduct1 = await _distributedCache.GetStringAsync("Product:1");
            string jsonProduct2 = await _distributedCache.GetStringAsync("Product:2");
            string jsonProduct3 = await _distributedCache.GetStringAsync("Product:3");

            Product product1 = JsonSerializer.Deserialize<Product>(jsonProduct1);
            Product product2 = JsonSerializer.Deserialize<Product>(jsonProduct2);
            Product product3 = JsonSerializer.Deserialize<Product>(jsonProduct3);

            ViewBag.product1 = product1;
            ViewBag.product2 = product2;
            ViewBag.product3 = product3;



            byte[] byteProduct11 = await _distributedCache.GetAsync("bProduct:1");
            byte[] byteProduct22 = await _distributedCache.GetAsync("bProduct:2");
            byte[] byteProduct33 = await _distributedCache.GetAsync("bProduct:3");

            string jsonProduct11 = Encoding.UTF8.GetString(byteProduct11);
            string jsonProduct22 = Encoding.UTF8.GetString(byteProduct22);
            string jsonProduct33 = Encoding.UTF8.GetString(byteProduct33);

            Product product11 = JsonSerializer.Deserialize<Product>(jsonProduct11);
            Product product22 = JsonSerializer.Deserialize<Product>(jsonProduct22);
            Product product33 = JsonSerializer.Deserialize<Product>(jsonProduct33);

            ViewBag.product11 = product11;
            ViewBag.product22 = product22;
            ViewBag.product33 = product33;



            return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/kelebek.png");

            byte[] imageByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("image", imageByte);

            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] imageByte = _distributedCache.Get("image");

            return File(imageByte, "image/png");
        }
    }
}
