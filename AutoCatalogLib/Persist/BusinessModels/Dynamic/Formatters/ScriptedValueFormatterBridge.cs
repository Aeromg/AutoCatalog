using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ValueFormatters;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Dynamic.Formatters
{
    class ScriptedValueFormatterBridge : Bridge<IFormatter, CustomFormatterReference>
    {
        public ScriptedValueFormatterBridge(CustomFormatterReference entity) : base(entity) { }

        protected override IFormatter GetModel(CustomFormatterReference entity)
        {
            return new JavaScriptValueFormatter()
            {
                Name = entity.Name,
                Description = entity.Description,
                Guid = entity.Guid,
                Type = GeneralizedTypes.GetConcreteType(entity.TargetValueType),
                Script = entity.Script
            };
        }

        protected override void UpdateEntity(CustomFormatterReference entity, IFormatter model)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Guid = model.Guid;
            entity.TargetValueType = GeneralizedTypes.GetGeneralizedTypeValue(model.Type);
            entity.Script = ((JavaScriptValueFormatter) model).Script;
        }
    }
}