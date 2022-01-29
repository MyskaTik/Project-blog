using Backend_EF.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace Backend_EF.Services
{
    public class CachingService
    {
        private ApplicationContext db;
        private IMemoryCache cache;
        public CachingService(ApplicationContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
        }
        public void Initialize()
        {
            if (!db.Usersdata.Any())
            {
                var user = new User()
                {
                    Name = "msi",
                    Email = "msi@gmail.com",
                    Password = "msi"
                };
                db.Usersdata.Add(user);
                db.SaveChanges();
                cache.Set(user, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }
    }
}
