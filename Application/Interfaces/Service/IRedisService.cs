using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IRedisService
    {
        Task StoreDataAsync(string key, string keyString, TimeSpan expiration);
        Task<long> StoreCountAsync(string key, long count, TimeSpan expiration);
        Task<bool> VerifyDataAsync(string key, string dataString);
        Task<bool> IsExistKeyAsync(string key);
        Task DeleteDataAsync(string key);
        Task<string> GetValueAsync(string key);
    }
}
