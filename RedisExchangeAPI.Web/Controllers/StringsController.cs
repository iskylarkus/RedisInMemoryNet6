using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class StringsController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;

        public StringsController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(0);
        }

        public async Task<IActionResult> Index()
        {
            await _db.StringSetAsync("name", "ilker");
            await _db.StringSetAsync("visitor", 100);

            return View();
        }

        public async Task<IActionResult> Show()
        {
            var name = await _db.StringGetAsync("name");

            if (!name.HasValue)
            {
                name = "NONE";
            }

            ViewBag.name = name.ToString();



            var lenght = await _db.StringLengthAsync("name");

            ViewBag.lenght = lenght;



            var visitor = await _db.StringGetAsync("visitor");

            if (!visitor.HasValue)
            {
                visitor = "0";
            }

            ViewBag.visitor = visitor.ToString();



            return View();
        }

        public async Task<IActionResult> Increment()
        {
            ViewBag.visitor = await _db.StringIncrementAsync("visitor", 1);

            return View();
        }

        public async Task<IActionResult> Decrement()
        {
            ViewBag.visitor = await _db.StringDecrementAsync("visitor", 1);

            return View();
        }

        public async Task<IActionResult> Append()
        {
            await _db.StringAppendAsync("name", "selvi");

            return View();
        }

        public async Task<IActionResult> Range()
        {
            var name = await _db.StringGetRangeAsync("name", 0, 3);

            if (!name.HasValue)
            {
                name = "NONE";
            }

            ViewBag.name = name.ToString();

            return View();
        }
    }
}
