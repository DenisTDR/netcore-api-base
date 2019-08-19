using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.Consumables.Models
{
    public class ConsumedRecordEntity : Entity
    {
        public ConsumableEntity Consumable { get; set; }
        
        public User User { get; set; }
    }
}