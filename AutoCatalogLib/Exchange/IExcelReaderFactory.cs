using AutoCatalogLib.Exchange.ExcelFormat.Impl;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.Exchange
{
    interface IExcelReaderFactory : IExternalReaderFactory<ExcelReader>, IModule
    {
    }
}
