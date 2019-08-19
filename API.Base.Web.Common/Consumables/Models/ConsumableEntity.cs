using System.Collections.Generic;
using API.Base.Web.Base.Models.Entities;

namespace API.Base.Web.Common.Consumables.Models
{
    public class ConsumableEntity : Entity
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public ICollection<ConsumedRecordEntity> ConsumedRecords { get; set; }

        public int TotalConsumed => ConsumedRecords?.Count ?? 0;
    }
}