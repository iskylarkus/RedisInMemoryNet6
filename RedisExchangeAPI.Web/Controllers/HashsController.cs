using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;

namespace RedisExchangeAPI.Web.Controllers
{
    public class HashsController : BaseController
    {
        private readonly string hashKey = "sozluk";

        public HashsController(RedisService redisService) : base(redisService)
        {
        }

        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> sozluk = new Dictionary<string, string>();

            if (await _db.KeyExistsAsync(hashKey))
            {
                _db.HashGetAll(hashKey).ToList().ForEach(x => sozluk.Add(x.Name.ToString(), x.Value.ToString()));
            }

            return View(sozluk);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name, string value)
        {
            if (!await _db.KeyExistsAsync(hashKey))
            {
                await _db.KeyExpireAsync(hashKey, DateTime.Now.AddMinutes(5)); // absolute expire
            }

            // await _db.KeyExpireAsync(hashKey, DateTime.Now.AddMinutes(5)); // sliding expire

            await _db.HashSetAsync(hashKey, name, value);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItem(string name)
        {
            await _db.HashDeleteAsync(hashKey, name);
            return RedirectToAction(nameof(Index));
        }
    }
}
