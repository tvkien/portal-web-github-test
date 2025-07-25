var isPageLoad = true;

function BuildSearchResultDataTable(tableSelector) {
    var sAjaxSource = $(tableSelector).attr('url');
    var options = {
        bServerSide: true,
        sServerMethod: "POST",
        bDestroy: true,
        bProcessing: false,
        bFilter: false,
        sAjaxSource: sAjaxSource,
        fnServerParams: function (aoData) {
            aoData.push(
                { name: "PageLoad", value: isPageLoad },
                { name: "SelectedCategories", value: viewModel.SelectedCategories() },
                { name: "SearchText", value: viewModel.SearchText() }
            );

            isPageLoad = false;
        },
        iDisplayLength: 25,
        aoColumns: [
            { mData: 'ID', sName: 'ID', bSearchable: true, bSortable: false, bVisible: true, sWidth: '90px' },
            { mData: 'HelpResourceTypeID', sName: 'hrt.DisplayText', bSearchable: false, bSortable: false, bVisible: false},
            { mData: 'FileType', sName: 'hft.Name', bSearchable: true, bSortable: true, bVisible: true, sWidth: '80px' },
            { mData: 'Category', sName: 'hc.DisplayText', bSearchable: true, bSortable: true, bVisible: true, sWidth: '78px' },
            { mData: 'Topic', sName: 'Topic', bSearchable: true, bSortable: true, bVisible: true, sWidth: '150px' },
            { mData: 'Description', sName: 'Description', bSearchable: true, bSortable: true, bVisible: true },
            { mData: 'UpdatedDateStr', sName: 'DateUpdated', bSearchable: true, bSortable: true, bVisible: false, sWidth: '70px', sClass: 'css-align-right' },
        ],
        aaSorting: [[0, "asc"]],
        oLanguage: { "sEmptyTable": "No data available in table" },
        fnRowCallback: function (nRow, aData) {
            if (RowCallbackAdmin != null && typeof (RowCallbackAdmin) === 'function'){
                RowCallbackAdmin(nRow, aData);
            }
            
            return nRow;
        },
        fnPreDrawCallback: function () {
            ShowBlock($(tableSelector), 'Loading');
            $('#filterSheets').attr("disabled", "disabled");

            return true;
        },
        fnDrawCallback: function () {
            $(tableSelector).unblock();
            $('#filterSheets').removeAttr("disabled");
            $('.tooltipBox').tip();
            return true;
        }
    };

    return options;
}
