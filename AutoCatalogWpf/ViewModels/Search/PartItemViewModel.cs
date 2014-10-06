using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using AutoCatalogLib;
using AutoCatalogLib.Business;
using AutoCatalogLib.Business.CatalogSearch;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogWpf.Views;
using AutoCatalogWpf.Views.Search;

namespace AutoCatalogWpf.ViewModels.Search
{
	public class PartItemViewModel : ViewModel
	{
		private IEnumerable<PartItemViewModel> _analogs;

		public PartItemViewModel()
		{
			Name = "Название детальки, много буков";
			PartNumber = "PARTNUM123";
			Price = 100.50;
			Balance = 10;
			Brand = "Brand Co";
			Commentary = "Комментарий, но его не должно быть видно, ибо его нет.";
		}

		public PartItemViewModel(PartItem item)
		{
			Name = item.Name;
			Balance = item.Balance;
			Brand = item.Brand;
			Commentary = item.Commentary;
			PartNumber = item.PartNumber;
			Price = item.Price;

			PartItem = item;
		}

		public string Name { get; set; }
		public string PartNumber { get; set; }
		public string Commentary { get; set; }
		public double Price { get; set; }
		public double Balance { get; set; }
		public string Brand { get; set; }
		public PartItem PartItem { get; private set; }

		public string Distributor { get; set; }
		public bool HasDistributor { get; set; }
		public bool HasSource { get; set; }

		private Command _invokeSource;
		public Command InvokeSource
		{
			get
			{
				return _invokeSource ?? (_invokeSource = new Command());
			}
		}

		public bool IsAvailablePart
		{
			get { return PartItem != null; }
		}

		public bool IsSelected { get; set; }

		public bool HasCommentary
		{
			get { return !String.IsNullOrEmpty(Commentary); }
		}

		public bool HasRealAnalogs
		{
			get
			{
				return
					!IsSearchingAnalogs && RealAnalogs.Any(a => a.IsAvailablePart);
			}
		}

		public bool HasVirtualAnalogs
		{
			get
			{
				return
					!IsSearchingAnalogs && VirtualAnalogs.Any(a => !a.IsAvailablePart);
			}
		}

		public bool HasNoAnalogs
		{
			get { return !IsSearchingAnalogs && (_analogs == null || !_analogs.Any()); }
		}

		public bool IsSearchingAnalogs { get; set; }

		public Command ClosePage
		{
			get
			{
				return new Command
				{
					ExecuteAction = () => App.Window.NavigateModal(null)
				};
			}
		}

		public PartItemViewModel SelectedAnalog { get; set; }

		public IEnumerable<PartItemViewModel> RealAnalogs
		{
			get
			{
				if (_analogs == null)
					return new PartItemViewModel[0];

				return _analogs.Where(a => a.IsAvailablePart);
			}
		}

		public IEnumerable<PartItemViewModel> VirtualAnalogs
		{
			get
			{
				if (_analogs == null)
					return new PartItemViewModel[0];

				return _analogs.Where(a => !a.IsAvailablePart);
			}
		}

		public void FetchDetails()
		{
			Task.Factory.StartNew(FetchAnalogsDetails);
			Task.Factory.StartNew(FetchSourceDetails);
		}

		private void FetchSourceDetails()
		{
            var importId = ImportToolkit.SearchImportInfo(PartItem);
            if (importId == null || String.IsNullOrEmpty(importId.Distributor))
		    {
		        HasDistributor = false;
                return;
		    }

		    HasDistributor = true;
            Distributor = importId.Distributor;

		    if (!Config.Manager.EnableViewItemSource)
		    {
		        HasSource = false;
                return;
		    }

            var source = SourceLocator.GetSourceByLocation(importId.SourceLocation);
            HasSource = SourceInvoker.IsInvokable(source, PartItem.SourceArgument);
            if (HasSource)
		        InvokeSource.ExecuteAction = () => SourceInvoker.Invoke(source, PartItem.SourceArgument);
		}

        private async void FetchAnalogsDetails()
		{
			if (_analogs != null && _analogs.Any())
				return;

			if (!IsAvailablePart)
				return;

			if (PartItem.AnalogsPartNumbers.Length == 0)
				return;

			IsSearchingAnalogs = true;

			var search = Config.Manager.GetAnalogsSearchEngine();
			var realAnalogs = (await search.SearchAnyAsync(PartItem.AnalogsPartNumbers)).Items
				.Select(a => new PartItemViewModel(a)).ToArray();

			var virtualAnalogsPnStrings =
				PartItem.AnalogsPartNumbers.Except(realAnalogs.Select(a => a.PartNumber)).ToArray();

			var virtualAnalogs =
				virtualAnalogsPnStrings.Select(a => new PartItemViewModel { PartNumber = a }).ToArray();

			_analogs = realAnalogs.Concat(virtualAnalogs).ToArray();

			IsSearchingAnalogs = false;

			RaisePropertyChanged("RealAnalogs");
			RaisePropertyChanged("VirtualAnalogs");
			RaisePropertyChanged("HasRealAnalogs");
			RaisePropertyChanged("HasVirtualAnalogs");
			RaisePropertyChanged("HasNoAnalogs");
		}

		internal void OnItemAction()
		{
			if (SelectedAnalog != null)
				App.Window.NavigateModal(new PartItemDetailsPage(SelectedAnalog));
		}
	}
}