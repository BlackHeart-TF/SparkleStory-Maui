using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SparkleServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IMemoryCache _cache;

        public ItemController(ILogger<ItemController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }
        public byte[] GetThumbnail(string cacheKey)
        {
            // Attempt to get the cached thumbnail
            if (_cache.TryGetValue(cacheKey, out byte[] cachedThumbnail))
            {
                // Cache hit: return the cached thumbnail
                return cachedThumbnail;
            }

            // Cache miss: return null (or you could generate the thumbnail here)
            return null;
        }

        [HttpGet("{ItemType}/{ItemID}")]
        public ClientObject? Get([FromRoute] string ItemType, [FromRoute] long ItemID)
        {
            if (!Enum.TryParse(typeof(CustomizationOption), ItemType, out var result))
            {
                _logger.LogError("Item Stype not found: " + ItemType);
                return null;
            }
            string cacheKey = $"{(CustomizationOption?)result}:{ItemID}";
            // Return the cached thumbnail
            var item = new ClientObject()
            {
                ItemID = ItemID,
                ItemType = (CustomizationOption)result,
            };
            var cachedThumbnail = GetThumbnail(cacheKey);
            if (cachedThumbnail != null)
            {
                // Return the cached thumbnail
                item.Content = cachedThumbnail;
                return item;
            }

            
            //Get Content


            return item;
        }
    }
}
