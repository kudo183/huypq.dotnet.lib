using huypq.QueryBuilder;
using SimpleDataGrid.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer
{
    class MainWindowViewModel : EditableGridViewModel<LogMessage>
    {
        DataManager _dataManager;

        HeaderFilterBaseModel _tFilter;
        HeaderFilterBaseModel _aFilter;
        HeaderFilterBaseModel _bFilter;
        HeaderFilterBaseModel _cFilter;
        HeaderFilterBaseModel _dFilter;
        HeaderFilterBaseModel _eFilter;
        HeaderFilterBaseModel _fFilter;

        public MainWindowViewModel(DataManager dataManager)
        {
            _dataManager = dataManager;

            _tFilter = new HeaderTextFilterModel("Timestamp", nameof(LogMessage.T), typeof(string));
            _aFilter = new HeaderTextFilterModel("Level", nameof(LogMessage.A), typeof(string));
            _bFilter = new HeaderTextFilterModel("Category", nameof(LogMessage.B), typeof(string));
            _cFilter = new HeaderTextFilterModel("EventID", nameof(LogMessage.C), typeof(string));
            _dFilter = new HeaderTextFilterModel("Scope", nameof(LogMessage.D), typeof(string));
            _eFilter = new HeaderTextFilterModel("Message", nameof(LogMessage.E), typeof(string));
            _fFilter = new HeaderTextFilterModel("Exception", nameof(LogMessage.F), typeof(string));

            AddHeaderFilter(_tFilter);
            AddHeaderFilter(_aFilter);
            AddHeaderFilter(_bFilter);
            AddHeaderFilter(_cFilter);
            AddHeaderFilter(_dFilter);
            AddHeaderFilter(_eFilter);
            AddHeaderFilter(_fFilter);
        }

        public override void Load()
        {
            var qe = new QueryExpression()
            {
                PageIndex = PagerViewModel.CurrentPageIndex,
                PageSize = PagerViewModel.PageSize,
                WhereOptions = WhereOptionsFromHeaderFilter(HeaderFilters),
                OrderOptions = OrderOptionsFromHeaderFilter(HeaderFilters)
            };

            var data = _dataManager.GetData(ref qe, out int pageCount);

            Entities.Reset(data);

            PagerViewModel.ItemCount = Entities.Count;
            PagerViewModel.PageCount = pageCount;
            PagerViewModel.SetCurrentPageIndexWithoutAction(qe.PageIndex);

            SysMsg = "OK";
        }
    }
}
