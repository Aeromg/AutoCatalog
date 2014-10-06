using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Dynamic.Formatters
{
    class FixedValueFormatterBridge : Bridge<IFormatter, FixedFormatterReference>
    {
        public FixedValueFormatterBridge(FixedFormatterReference entity) : base(entity) { }


        protected override IFormatter GetModel(FixedFormatterReference entity)
        {
            return FormattersLocator.Get(entity.Guid);
        }

        protected override void UpdateEntity(FixedFormatterReference entity, IFormatter model)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Guid = model.Guid;
            entity.TargetValueType = GeneralizedTypes.GetGeneralizedTypeValue(model.Type);
        }
    }
}