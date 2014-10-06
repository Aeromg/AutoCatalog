using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ValueFormatters;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Dynamic.Formatters;

namespace AutoCatalogLib.Business
{
    class FormattersPersistService : IDisposable
    {
        private readonly ContextHolder _contextHolder = new ContextHolder();

        public IFormatter SearchFormatter(Guid guid)
        {
            var fixd = _contextHolder.Context.FixedValueFormatters.FirstOrDefault(f => f.Guid == guid);
            if (fixd != null)
                return fixd.Bridge.GetModel();
            
            var scrp = _contextHolder.Context.ScriptedValueFormatters.FirstOrDefault(f => f.Guid == guid);
            if (scrp != null)
                return scrp.Bridge.GetModel();

            return null;
        }

        public void AddOrUpdate(IFormatter formatter)
        {
            if (formatter is JavaScriptValueFormatter)
                AddOrUpdateScripted((JavaScriptValueFormatter) formatter);
            else
                AddOrUpdateFixed(formatter);
        }

        public void Remove(IFormatter formatter)
        {
            if (formatter is JavaScriptValueFormatter)
                RemoveScripted((JavaScriptValueFormatter) formatter);
        }

        private void AddOrUpdateScripted(IFormatter formatter)
        {
            var context = _contextHolder.Context;
            var entity = context.ScriptedValueFormatters.FirstOrDefault(f => f.Guid == formatter.Guid);

            if (entity == null)
                new CustomFormatterReference().Bridge.FromModel(formatter).AddOrAttach(context);
            else
            {
                entity.Bridge.FromModel(formatter);
            }

            context.SaveChanges();
        }

        private void AddOrUpdateFixed(IFormatter formatter)
        {
            var context = _contextHolder.Context;
            var entity = context.FixedValueFormatters.FirstOrDefault(f => f.Guid == formatter.Guid);

            if (entity == null)
                new FixedFormatterReference().Bridge.FromModel(formatter).AddOrAttach(context);
            else
            {
                entity.Bridge.FromModel(formatter);
                context.SaveChanges();
            }
        }

        private void RemoveScripted(IFormatter formatter)
        {
            var context = _contextHolder.Context;
            var entity = context.ScriptedValueFormatters.FirstOrDefault(f => f.Guid == formatter.Guid);

            if (entity == null) return;

            entity.Remove(context);
            context.SaveChanges();
        }

        public IEnumerable<IFormatter> EnumerateFormatters()
        {
            return _contextHolder.Context.ScriptedValueFormatters.ToArray().Select(s => s.Bridge.GetModel());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FormattersPersistService()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _contextHolder.Dispose();
        }
    }
}