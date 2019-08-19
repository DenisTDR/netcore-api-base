using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Web.Base.Auth.Models.Entities;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Extensions;
using API.Base.Web.Base.Misc.PatchBag;
using API.Base.Web.Base.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Base.Web.Base.Database.Repository.Helpers
{
    public class EntityUpdateHelper<T> where T : class, IEntity
    {
        private readonly IDataLayer _dataLayer;

        public EntityUpdateHelper(IDataLayer dataLayer, DbSet<T> dbSet)
        {
            _dataLayer = dataLayer;
//            _dbSet = dbSet;
        }

        public async Task TakeCareOf(EntityPatchBag<T> eub, T existing)
        {
            var updatablePrimitiveProperties =
                EntityTypeHelper.GetUpdatablePrimitveProperties<T>(
                    EntityTypeHelper.GetPropertiesNames(eub.PropertiesToUpdate));

            foreach (var upp in updatablePrimitiveProperties)
            {
                upp.SetValue(existing, upp.GetValue(eub.Model));
            }


            var updateableEntityProperties =
                EntityTypeHelper.GetUpdateableEntityProperties<T>(
                    EntityTypeHelper.GetPropertiesNames(eub.PropertiesToUpdate));


            foreach (var uep in updateableEntityProperties)
            {
                var e = (IEntity) uep.GetValue(eub.Model);
                if (e == null)
                {
                    uep.SetValue(existing, null);
                }
                else
                {
                    var propRepo = _dataLayer.Repository(uep.PropertyType);
                    var e2 = await propRepo.GetOneEntity(e.Selector);
                    if (e2 == null)
                    {
                        e2 = await propRepo.GetOneEntity(e.Id);
                    }

                    uep.SetValue(existing, e2);
                }
            }
        }

        public static EntityPatchBag<T> GetEpbFor(T current, T existing)
        {
            var epb = new EntityPatchBag<T>
            {
                Id = existing.Id,
                Model = current,
                PropertiesToUpdate = new Dictionary<string, bool>()
            };
            var updateablePrimitiveProperties =
                EntityTypeHelper.GetUpdatablePrimitveProperties<T>();

            foreach (var upp in updateablePrimitiveProperties)
            {
                var crtValue = upp.GetValue(current);
                if (crtValue == null && upp.GetValue(existing) != null ||
                    crtValue != null && !crtValue.Equals(upp.GetValue(existing)))
                {
                    epb.PropertiesToUpdate.Add(upp.Name, true);
                }
            }

            var updateableEntityProperties =
                EntityTypeHelper.GetUpdateableEntityProperties<T>();
            foreach (var uep in updateableEntityProperties)
            {
                var crtValue = (IEntity) uep.GetValue(current);
                var existingValue = (IEntity) uep.GetValue(existing);
                if (crtValue == null || existingValue == null
                                     || (!string.IsNullOrEmpty(crtValue.Id) && crtValue.Id != existingValue.Id)
                                     || (!string.IsNullOrEmpty(crtValue.Selector) &&
                                         crtValue.Selector != existingValue.Selector))
                {
                    epb.PropertiesToUpdate.Add(uep.Name, true);
                }
            }

            return epb;
        }

        public async Task BindRelatedEntities(T e)
        {
            var updateableEntityProperties =
                EntityTypeHelper.GetUpdateableEntityProperties<T>();
            if (e == null)
            {
                return;
            }

            foreach (var uep in updateableEntityProperties)
            {
                var value = (IEntity) uep.GetValue(e);
                if (value == null || string.IsNullOrEmpty(value.Id))
                {
                    continue;
                }

                var existingEntity = await _dataLayer.Repository(uep.PropertyType).GetOneEntity(value.Id);
                if (existingEntity == null)
                {
                    continue;
                }

                uep.SetValue(e, existingEntity);
            }
        }

        public static void ClearNullRelatedEntities(T e)
        {
            if (e == null)
            {
                return;
            }

            var updateableEntityProperties =
                EntityTypeHelper.GetUpdateableEntityProperties<T>();

            foreach (var uep in updateableEntityProperties)
            {
                var value = (IEntity) uep.GetValue(e);
                if (value != null && string.IsNullOrEmpty(value.Id))
                {
                    uep.SetValue(e, null);
                }
            }
        }
    }
}