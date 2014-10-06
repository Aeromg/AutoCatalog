using System;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ExcelFormat;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogLib.Persist;

namespace AutoCatalogLib
{
    public class TestUtils
    {
        public static void SimpleStupidLocker()
        {
            /* var theEndOfWorld = new DateTime(2014, 6, 1);
            if (theEndOfWorld < DateTime.Now)
                throw new Exception(@"Приложение сломалось. Позвоните по номеру (342) 2046054"); */
        }

        public static void DefaultRules()
        {
            Context.Drop();

            ImportRulesLocator.SetBehavior(BuildMasumaRules());
            ImportRulesLocator.SetBehavior(BuildTissRules());

            TargetsLocator.Set(new FileSourceProfile
            {
                FilePath = @"C:\Temp\masuma_bigdata.xls",
                Guid = Guid.NewGuid(),
                RuleIdentificatorString = "masumaexcel",
                TransactionIdentificator = "masuma"
            });

            TargetsLocator.Set(new FileSourceProfile
            {
                FilePath = @"C:\Temp\tiss.xls",
                Guid = Guid.NewGuid(),
                RuleIdentificatorString = "tissexcel",
                TransactionIdentificator = "tiss"
            });
        }

        public static ExcelImportRule BuildMasumaRules()
        {
            return new ExcelImportRule()
            {
                ColumnOffset = 0,
                RowOffset = 7,
                WorksheetIndex = 1,
                CsvMode = true,
                Identificator = "masumaexcel",
                Name = "Masuma Excel",
                Description = @"Файл прайс-листов Excel поставщика Masuma",
                Columns = new[]
                {
                    new ExcelImportRule.Column()
                    {
                        Name = @"Brand",
                        Formatter = FormattersLocator.Search<string>().First(),
                        Index = 1,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"PartNumber",
                        Formatter = FormattersLocator.Search<string>().First(),
                        Index = 3,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Name",
                        Formatter = FormattersLocator.Search<string>().First(),
                        Index = 5,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Analogs",
                        Formatter = FormattersLocator.Search<string[]>().First(),
                        Index = 4,
                        Active = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Balance",
                        Formatter = FormattersLocator.Search<double>().First(),
                        Index = 8,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Price",
                        Formatter = FormattersLocator.Search<double>().First(),
                        Index = 9,
                        Active = true,
                        Required = true
                    }
                }
            };
        }

        public static ExcelImportRule BuildTissRules()
        {
            return new ExcelImportRule()
            {
                ColumnOffset = 1,
                RowOffset = 8,
                WorksheetIndex = 1,
                CsvMode = true,
                Identificator = "tissexcel",
                Name = "Tiss Excel",
                Description = @"Файл прайс-листов Excel поставщика Tiss",
                Columns = new[]
                {
                    new ExcelImportRule.Column()
                    {
                        Name = @"Brand",
                        Formatter = FormattersLocator.Search<string>().First(),
                        Index = 1,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"PartNumber",
                        Formatter = FormattersLocator.Search<string>().First(),
                        Index = 3,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Name",
                        Formatter = FormattersLocator.Search<string>().First(),
                        Index = 2,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Analogs",
                        Formatter = FormattersLocator.Search<string[]>().First(),
                        Index = 5,
                        Active = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Balance",
                        Formatter = FormattersLocator.Search<double>().First(),
                        Index = 14,
                        Active = true,
                        Required = true
                    },
                    new ExcelImportRule.Column()
                    {
                        Name = @"Price",
                        Formatter = FormattersLocator.Search<double>().First(),
                        Index = 8,
                        Active = true,
                        Required = true
                    }
                }
            };
        }
    }
}