using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SetsController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private readonly string setKey = "setnames";

        public SetsController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(2);
        }

        public async Task<IActionResult> Index()
        {
            HashSet<string> namesList = new HashSet<string>();

            if (await _db.KeyExistsAsync(setKey))
            {
                _db.SetMembers(setKey).ToList().ForEach(x => namesList.Add(x.ToString()));
            }

            return View(namesList);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            if (! await _db.KeyExistsAsync(setKey))
            {
                await _db.KeyExpireAsync(setKey, DateTime.Now.AddMinutes(5)); // absolute expire
            }

            // await _db.KeyExpireAsync(setKey, DateTime.Now.AddMinutes(5)); // sliding expire

            await _db.SetAddAsync(setKey, name);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItem(string name)
        {
            await _db.SetRemoveAsync(setKey, name);
            return RedirectToAction(nameof(Index));
        }
    }
}
