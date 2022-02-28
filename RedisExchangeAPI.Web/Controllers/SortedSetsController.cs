using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SortedSetsController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private readonly string sortedsetKey = "sortedsetnames";

        public SortedSetsController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(3);
        }

        public async Task<IActionResult> Index()
        {
            HashSet<string> namesList = new HashSet<string>();

            if (await _db.KeyExistsAsync(sortedsetKey))
            {
                _db.SortedSetScan(sortedsetKey).ToList().ForEach(x => namesList.Add(x.ToString()));
            }

            return View(namesList);
        }

        public async Task<IActionResult> Reverse()
        {
            HashSet<string> namesList = new HashSet<string>();

            if (await _db.KeyExistsAsync(sortedsetKey))
            {
                _db.SortedSetRangeByRank(sortedsetKey, order: Order.Descending).ToList().ForEach(x => namesList.Add(x.ToString()));
            }

            return View(namesList);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name, int score)
        {
            if (!await _db.KeyExistsAsync(sortedsetKey))
            {
                await _db.KeyExpireAsync(sortedsetKey, DateTime.Now.AddMinutes(5)); // absolute expire
            }

            // await _db.KeyExpireAsync(sortedsetKey, DateTime.Now.AddMinutes(5)); // sliding expire

            await _db.SortedSetAddAsync(sortedsetKey, name, score);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItem(string name)
        {
            await _db.SortedSetRemoveAsync(sortedsetKey, name.Split(":")[0]);
            return RedirectToAction(nameof(Index));
        }
    }
}
