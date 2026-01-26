using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Shared.Entity.Ads;
using UnityEngine;

namespace Shared.Repository.Ads
{
    [Repository]
    public class MrecPositionRepository : JsonPlayerPrefsRepository<MrecPositionEntity>
    {
        public void Add(string placement, Vector2 position)
        {
            var entity = Get();
            if (entity == null)
                entity = new MrecPositionEntity();
            entity.AddPosition(placement, position);
            Save(entity);
        }
    }
}