using AutoCatalogLib.Exchange;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Dynamic.Formatters
{
    public class FixedFormatterReference : PersistValueFormatter<FixedFormatterReference>
    {
        protected override Bridge<IFormatter, FixedFormatterReference> BuildBridge()
        {
            return new FixedValueFormatterBridge(this);
        }
    }
}