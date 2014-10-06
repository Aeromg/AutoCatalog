using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogLib.Persist.Generic;
using NLog.Targets.Wrappers;

namespace AutoCatalogLib.Persist.BusinessModels.Catalog
{
    class ImportTargetBridge : Bridge<ISourceProfile, ImportTargetPoco>
    {
        public ImportTargetBridge(ImportTargetPoco entity) : base(entity) { }

        protected override ISourceProfile GetModel(ImportTargetPoco entity)
        {
            switch (entity.TargetType)
            {
                case TargetType.File:
                    return new FileSourceProfile
                    {
                        FilePath = entity.Target,
                        RuleIdentificatorString = entity.RuleIdentificator,
                        Guid = entity.Guid,
                        TransactionIdentificator = entity.TransactionIdentificator,
                        CleanBeforeImport = entity.CleanBeforeImport,
                        EmbedSource = entity.EmbedSource,
                        Distributor = entity.Distributor
                    };

                case TargetType.Web:
                    return new WebSourceProfile
                    {
                        Url = entity.Target,
                        RuleIdentificatorString = entity.RuleIdentificator,
                        Guid = entity.Guid,
                        TransactionIdentificator = entity.TransactionIdentificator,
                        CleanBeforeImport = entity.CleanBeforeImport,
                        EmbedSource = entity.EmbedSource,
                        Distributor = entity.Distributor
                    };

                default:
                    throw new NotImplementedException("TargetType " + entity.TargetType + " не реализован");
            }
        }

        protected override void UpdateEntity(ImportTargetPoco entity, ISourceProfile model)
        {
            if(model == null || entity == null)
                throw new ArgumentNullException();

            if (model is FileSourceProfile)
            {
                var fileTarget = (FileSourceProfile)model;
                entity.TargetType = TargetType.File;
                entity.Guid = fileTarget.Guid;
                entity.Target = fileTarget.FilePath;
                entity.RuleIdentificator = fileTarget.Rule.Identificator;
                entity.TransactionIdentificator = fileTarget.TransactionIdentificator;
                entity.CleanBeforeImport = fileTarget.CleanBeforeImport;
                entity.EmbedSource = fileTarget.EmbedSource;
                entity.Distributor = fileTarget.Distributor;

                return;
            }

            if (model is WebSourceProfile)
            {
                var webTarget = (WebSourceProfile)model;
                entity.TargetType = TargetType.Web;
                entity.Guid = webTarget.Guid;
                entity.Target = webTarget.Url;
                entity.RuleIdentificator = webTarget.Rule.Identificator;
                entity.TransactionIdentificator = webTarget.TransactionIdentificator;
                entity.CleanBeforeImport = webTarget.CleanBeforeImport;
                entity.EmbedSource = webTarget.EmbedSource;
                entity.Distributor = webTarget.Distributor;

                return;
            }

            throw new NotImplementedException("Хранение типа " + model.GetType().FullName + " не реализовано");
        }
    }
}
