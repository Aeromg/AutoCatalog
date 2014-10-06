using System;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Dynamic.Formatters
{
    public class CustomFormatterReference : PersistValueFormatter<CustomFormatterReference>
    {
        public string Script { get; set; }

        internal override void BeforeStateChanged(Context context, EntityChangeState state)
        {
            if (state == EntityChangeState.Add)
                Guid = Guid.NewGuid();
        }

        protected override Bridge<IFormatter, CustomFormatterReference> BuildBridge()
        {
            return new ScriptedValueFormatterBridge(this);
        }
    }
}