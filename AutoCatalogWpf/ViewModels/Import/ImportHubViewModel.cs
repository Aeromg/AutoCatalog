using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogWpf.Views.Import;

namespace AutoCatalogWpf.ViewModels.Import
{
    public class ImportHubViewModel : ViewModel
    {
        public IEnumerable<ImportTargetPreset> PresetTargets { get; private set; }
        public ObservableCollection<CustomImportTargetViewModel> CustomTargets { get; set; }

        private CustomImportTargetViewModel _currentCustomTarget;
        public CustomImportTargetViewModel CurrentCustomTarget
        {
            get
            {
                return _currentCustomTarget;
            }
            set
            {
                if(_currentCustomTarget == value)
                    return;

                _currentCustomTarget = value;
                RaisePropertyChanged("IsCustomTargetSelected");
                RaisePropertyChanged("EditTargetEnabled");
                RemoveCustom.OnCanExecuteChanged();
                EditCustom.OnCanExecuteChanged();

                if (IsCustomTargetEditor)
                {
                    EditCurrentCustomTarget();
                }
            }
        }

        private CustomImportTargetViewModel _editCustomTarget;
        public CustomImportTargetViewModel EditCustomTarget
        {
            get
            {
                return _editCustomTarget;
            }
            set
            {
                if (_editCustomTarget == value)
                    return;

                if (_editCustomTarget != null)
                    _editCustomTarget.PropertyChanged -= EditCustomTarget_PropertyChanged;
                
                _editCustomTarget = value;

                if(_editCustomTarget != null)
                    _editCustomTarget.PropertyChanged += EditCustomTarget_PropertyChanged;

                RaisePropertyChanged("IsCustomTargetSelected");
                RaisePropertyChanged("EditTargetEnabled");

                if (IsCustomTargetEditor)
                {
                    EditCurrentCustomTarget();
                }
            }
        }

        public Command ClosePage
        {
            get
            {
                return new Command
                {
                    ExecuteAction = App.Window.GoBack
                };
            }
        }

        private Command _addCustomCommand;
        public Command AddCustom
        {
            get { return _addCustomCommand ?? (_addCustomCommand = new Command
            {
                ExecuteAction = AddCustomTarget
            }); }
        }

        private Command _removeCustomCommand;
        public Command RemoveCustom
        {
            get { return _removeCustomCommand ?? (_removeCustomCommand = new Command
            {
                ExecuteAction = RemoveCurrentCustomTarget,
                CanExecuteFunc = () => IsCustomTargetSelected
            }); }
        }

        private Command _editCustomCommand;
        public Command EditCustom
        {
            get
            {
                return _editCustomCommand ?? (_editCustomCommand = new Command
                    {
                        ExecuteAction = EditCurrentCustomTarget,
                        CanExecuteFunc = () => IsCustomTargetSelected
                    });
            }
        }

        private Command _saveCustomCommand;
        public Command SaveCustom
        {
            get
            {
                return _saveCustomCommand ?? (_saveCustomCommand = new Command
                    {
                        ExecuteAction = SaveEditCurrentCustomTarget,
                        CanExecuteFunc = () => EditCustomTarget != null && EditCustomTarget.CheckForm()
                    });
            }
        }

        private Command _startCommand;
        public Command Start
        {
            get
            {
                return _startCommand ?? (_startCommand = new Command
                {
                    ExecuteAction = StartImpl,
                    CanExecuteFunc = () => !IsCustomTargetEditor && (PresetTargets.Any(t => t.IsSelected) || CustomTargets.Any())
                });
            }
        }

        public Command Cancel
        {
            get
            {
                return new Command()
                {
                    ExecuteAction = App.Window.GoBack
                };
            }
        }

        public bool IsCustomTargetSelected
        {
            get { return CurrentCustomTarget != null; }
        }

        public bool IsSummary
        {
            get { return !IsCustomTargetEditor; }
        }

        private bool _isCustomTargetEditor;
        public bool IsCustomTargetEditor
        {
            get { return _isCustomTargetEditor; }
            set
            {
                if(_isCustomTargetEditor == value)
                    return;

                _isCustomTargetEditor = value;
                RaisePropertyChanged("IsSummary");
            }
        }

        public bool EditTargetEnabled
        {
            get { return IsCustomTargetSelected && IsSummary; }
        }

        public ImportHubViewModel()
        {
            BuildPresetTargets();
            CustomTargets = new ObservableCollection<CustomImportTargetViewModel>();
            CurrentCustomTarget = CustomTargets.FirstOrDefault();
            //Demo();
        }

        private void Demo()
        {
            PresetTargets = new ImportTargetPreset[]
            {
                new ImportTargetPreset
                {
                    IsSelected = false,
                    Target = new FileSourceProfile()
                    {
                        FilePath = @"C:\Test\test.xls"
                    }
                },
                new ImportTargetPreset
                {
                    IsSelected = false,
                    Target = new FileSourceProfile()
                    {
                        FilePath = @"D:\Data\test.txt"
                    }
                },
                new ImportTargetPreset
                {
                    IsSelected = false,
                    Target = new FileSourceProfile()
                    {
                        FilePath = @"D:\some other directory\*.xml"
                    }
                }
            };
            
            CustomTargets.Add(new CustomImportTargetViewModel
            {
                FilePath = @"c:\Some\1.xls"
            });

            CustomTargets.Add(new CustomImportTargetViewModel
            {
                FilePath = @"c:\Some\1.xls"
            });
        }

        private void BuildPresetTargets()
        {
            PresetTargets = TargetsLocator.Targets.Select(t => new ImportTargetPreset {Target = t}).ToArray();
            foreach (var target in PresetTargets)
                target.PropertyChanged += (sender, args) => PresetTargets_Selected((ImportTargetPreset) sender);
        }

        private void PresetTargets_Selected(ImportTargetPreset target)
        {
            Start.OnCanExecuteChanged();
        }

        private void AddCustomTarget()
        {
            EditCustomTarget = new CustomImportTargetViewModel();
            EditCustomTarget.PropertyChanged += EditCustomTarget_PropertyChanged;
            IsCustomTargetEditor = true;
        }

        void EditCustomTarget_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveCustom.OnCanExecuteChanged();
        }

        private void RemoveCurrentCustomTarget()
        {
            CustomTargets.Remove(CurrentCustomTarget);
            if (EditCustomTarget == CurrentCustomTarget)
                CancelEditCurrentCustomTarget();

            CurrentCustomTarget = null;

            Start.OnCanExecuteChanged();
        }

        private void EditCurrentCustomTarget()
        {
            IsCustomTargetEditor = true;
            EditCustomTarget = CurrentCustomTarget;
            EditCustomTarget.RollBack();
        }

        private void CancelEditCurrentCustomTarget()
        {
            if (EditCustomTarget != null)
            {
                EditCustomTarget.RollBack();
                IsCustomTargetEditor = false;
                EditCustomTarget = null;
            }

            CustomTargets.Remove(CurrentCustomTarget);
        }

        private void SaveEditCurrentCustomTarget()
        {
            EditCustomTarget.Save();
            if (!CustomTargets.Contains(EditCustomTarget))
                CustomTargets.Add(EditCustomTarget);

            CurrentCustomTarget = EditCustomTarget;
            IsCustomTargetEditor = false;
            EditCustomTarget = null;

            Start.OnCanExecuteChanged();
        }

        private void StartImpl()
        {
            var presets = PresetTargets.Where(t => t.IsSelected).Select(t => t.Target).Cast<FileSourceProfile>();
            var customs = CustomTargets.Select(t => new FileSourceProfile()
            {
                FilePath = t.FilePath,
                RuleIdentificatorString = t.Rule.Identificator
            });

            var task = ImportToolkit.CreateImportTask(presets.Concat(customs));
            var vm = new ImportProgressViewModel(task);
            App.Window.NavigateModal(new ImportProgressPage(vm));
            vm.Start();
        }
    }
}
