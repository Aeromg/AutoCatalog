using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetOffice.ExcelApi;

namespace AutoCatalogLib.Utils.Cli
{
    public class CliApplication
    {
        private readonly ArgumentsSet _arguments;
        private readonly IDictionary<ArgumentHeader, Type> _routines = new Dictionary<ArgumentHeader, Type>();

        public CliApplication(string arguments)
        {
            _arguments = Argument.Parse(arguments);
        }

        public void Main()
        {
            RegisterRoutines();
            var argumentHeader = _arguments.All().Select(a => a.Header).FirstOrDefault(h => !h.IsSubArgument);
            if (argumentHeader == null)
            {
                argumentHeader = ArgumentsDictionary.Help;
            }

            var routine = BuildRoutine(argumentHeader);
            routine.Run();
        }

        private Routine BuildRoutine(ArgumentHeader header)
        {
            Type routineType;
            if(!_routines.TryGetValue(header, out routineType))
                throw new NotImplementedException();

            var routine = Activator.CreateInstance(routineType) as Routine;
            routine.SetArguments(_arguments);

            return routine;
        }

        private void RegisterRoutines()
        {
            _routines.Add(ArgumentsDictionary.Help, typeof(HelpRoutine));

            _routines.Add(ArgumentsDictionary.DropEverything, typeof (DropEverythingRoutine));
            _routines.Add(ArgumentsDictionary.DropImport, typeof(DropImportRoutine));
            _routines.Add(ArgumentsDictionary.DropRecords, typeof(DropRecordsRoutine));

            _routines.Add(ArgumentsDictionary.Import, typeof(ImportRoutine));
            _routines.Add(ArgumentsDictionary.ImportSource, typeof(ImportSourceRoutine));

            _routines.Add(ArgumentsDictionary.ImportDb, typeof(ImportDbRoutine));
            _routines.Add(ArgumentsDictionary.ExportDb, typeof(ExportDbDbRoutine));

            _routines.Add(ArgumentsDictionary.ShowImports, typeof(ShowImportsRoutine));
            _routines.Add(ArgumentsDictionary.ShowRules, typeof(ShowRulesRoutine));
            _routines.Add(ArgumentsDictionary.ShowSources, typeof(ShowSourcesRoutine));

            _routines.Add(ArgumentsDictionary.Batch, typeof(BatchRoutine));
        }
    }
}
