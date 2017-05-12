using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Interfaces
{
    public interface IUserMessageRepository : IRepository<UserMessage,long>
    {
        IQueryable<UserMessage> GetAllSent(string userId);
    }
}
