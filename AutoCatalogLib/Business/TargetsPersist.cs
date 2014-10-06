using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business
{
    class TargetsPersist
    {
        private readonly ContextHolder _holder = new ContextHolder();

        public void AddOrUpdate(ISourceProfile target)
        {
            var context = _holder.Context;
            var entity = context.ImportTargets.FirstOrDefault(t => t.Guid == target.Guid);

            if (entity == null)
            {
                Add(target);
            }
            else
            {
                entity.Bridge.FromModel(target);
                context.SaveChanges();   
            }
        }

        private void Add(ISourceProfile target)
        {
            var context = _holder.Context;
            new ImportTargetPoco().Bridge.FromModel(target).AddOrAttach(_holder.Context);
            context.SaveChanges();
        }

        public void Remove(ISourceProfile target)
        {
            Remove(target.Guid);
        }

        public void Remove(Guid guid)
        {
            var context = _holder.Context;
            var entity = context.ImportTargets.First(t => t.Guid == guid);
            context.ImportTargets.Remove(entity);
            context.SaveChanges();
        }

        public IEnumerable<ISourceProfile> GetAll()
        {
            return _holder.Context.ImportTargets.ToArray().Select(p => p.Bridge.GetModel());
        }
    }
}
