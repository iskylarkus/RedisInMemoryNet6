using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class ListsController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private readonly string listKey = "listnames";

        public ListsController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(1);
        }

        public async Task<IActionResult> Index()
        {
            List<string> namesList = new List<string>();

            if (await _db.KeyExistsAsync(listKey))
            {
                _db.ListRange(listKey).ToList().ForEach(x => namesList.Add(x.ToString()));
            }

            return View(namesList);
        }

        [HttpPost]
        public async Task<IActionResult> LeftPush(string name)
        {
            await _db.ListLeftPushAsync(listKey, name);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RightPush(string name)
        {
            await _db.ListRightPushAsync(listKey, name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItem(string name)
        {
            await _db.ListRemoveAsync(listKey, name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LeftPop()
        {
            await _db.ListLeftPopAsync(listKey);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RightPop()
        {
            await _db.ListRightPopAsync(listKey);
            return RedirectToAction(nameof(Index));
        }
    }
}
