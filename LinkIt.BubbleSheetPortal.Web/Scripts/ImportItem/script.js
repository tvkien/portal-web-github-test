//=============TestDesign.js==========================================
(function ($) {
  'use strict';

  setTimeout(function () {
    // Tab Pane
    var $itemLibraryTab = $('.itemLibraryTab');

    if ($itemLibraryTab.length) {
      var $itemLibraryTabListItem = $itemLibraryTab.find('.itemLibraryTab-list-item');
      var $itemLibraryTabPane = $itemLibraryTab.find('.itemLibraryTab-pane');

      $itemLibraryTabListItem.on('click', function () {
        var $self = $(this);
        var dataTab = $self.data('tab');

        $itemLibraryTabListItem.removeClass('active');
        $self.addClass('active');

        $itemLibraryTabPane.hide();
        $itemLibraryTab.find('.itemLibraryTab-pane[id="' + dataTab + '"]').show();
      });
    }
  }, 3000);
}(jQuery));

//==========="/Scripts/QtiItem/qtiitem.js"===========================
function parseXmlContentQtiItem(xmlContent) {
  var title = '';
  xmlContent = correctInlineChoice(xmlContent);
  $(xmlContent).find('.itemBody, itemBody, itembody').each(function () {
    var itemBody = $(this);
    itemBody.find("videolinkit").replaceWith(function () {
      return $('');
    });

    if ($(xmlContent).find("responsedeclaration").attr("partialgrading") == "1") {
      itemBody.find("sourcetext").each(function () {
        if ($(this).attr("pointvalue") > 0) {
          $(this).addClass("marker-correct");
        }
      });
    } else {
      $(xmlContent).find("correctResponse").each(function () {
        var id = $(this).attr("identifier");
        itemBody.find("sourcetext[identifier=\"" + id + "\"]").addClass("marker-correct");
      });
    }

    title = itemBody.html();
  });
  title = title.replaceAll("<object", "<object style='display: none;'");
  var divTitle = '<div style="max-height:60px; overflow:hidden;max-width:346px">' + title + '</div>';
  return divTitle;
}

//==================== HANDLING====================================
var firstLoad = true;
var firstLoadListItemsFromLibrary = true;
var firstLoadListItemsFromLibraryNew = true;

$(document).ready(function () {
  window.onload = function () {
    loadPassageCriteriaStaticData();
    loadItemCriteriaStaticData();
    InItDatatableQti3pItemPassage();
    InitDataTableQtiItemPassage();
    $("#selectDistrictCategory").change();
  };

  function getAjaxSource1(isLoad = false) {
    $('#chkAllItem').removeAttr('checked');
    if (isLoad) {
      return getAjaxLoadItemFromLibraryByFilter();
    }
  }

  function inItDataNWEATable() {
    var options1 = {
      bServerSide: true,
      bDestroy: true,
      bFilter: false,
      sAjaxSource: getAjaxSource1(true),
      fnServerParams: function (aoData) {
        var item = null;
        for (var i = 0; i < aoData.length; i++) {
          item = aoData[i];
          if (item.name == 'sSearch') {
            do {
              item.value = item.value.replace('""', '"');
            } while (item.value.indexOf('""') >= 0)

            if (item.value == '"') {
              item.value = item.value.replace('"', "''");
            } else {
              item.value = encodeURI(item.value);
            }
            break;
          }
        }
      },
      bStateSave: false,
      bAutoWidth: false,
      iDisplayLength: 100,
      aaSorting: [
        [0, "asc"]
      ],
      aoColumns: [{
        sType: 'integer',
        sName: 'QTI3pItemID',
        bSearchable: false,
        bSortable: false,
        sWidth: "10px"
      },
      {
        sType: 'string',
        sName: 'Name',
        bSearchable: false,
        bSortable: false,
        sWidth: "400px"
      },
      {
        sType: 'string',
        sName: 'ToolTip',
        bSearchable: false,
        bSortable: false,
        bVisible: false
      },
      {
        sType: 'int',
        sName: 'MaxItemTooltipLength',
        bSearchable: false,
        bSortable: false,
        bVisible: false
      },
      {
        sType: 'string',
        sName: 'From3pUpload',
        bSearchable: false,
        bSortable: false,
        bVisible: false
      }
      ],
      fnRowCallback: function (nRow, aData) {
        $('td:eq(0)', nRow).html(setCheckBoxItem(aData[0]));
        if (aData[4] == 'True' || aData[4] == 'true') {
          var title = '';
          aData[1] = correctInlineChoice(aData[1]);
          $(aData[1]).find('.itemBody, itemBody, itembody').each(function () {
            var itemBody = $(this);
            itemBody.find("videolinkit").replaceWith(function () {
              return $('');
            });
            if ($(aData[1]).find("responsedeclaration").attr("partialgrading") == "1") {
              itemBody.find("sourcetext").each(function () {
                if ($(this).attr("pointvalue") > 0) {
                  $(this).addClass("marker-correct");
                }
              });
            } else {
              $(aData[1]).find("correctResponse").each(function () {
                var id = $(this).attr("identifier");
                itemBody.find("sourcetext[identifier=\"" + id + "\"]").addClass("marker-correct");
              });
            }

            title = itemBody.html();
          });
          title = title.replaceAll("<object", "<object style='display: none;'");
          var divTitle = '<div style="max-height:62px; overflow:hidden;max-width:346px">' + title + '</div>';
          $('td:eq(1)', nRow).html(divTitle);

          $('td:eq(1)', nRow).attr("onclick", 'showEditQtiItemPopupUpload(' + aData[0] + ',1)');
        } else {
          $('td:eq(1)', nRow).html(parseXmlContent1(aData[1]));
          $('td:eq(1)', nRow).attr("onclick", 'showEditQti3pItemPopup(' + aData[0] + ',1)');
        }
        $('td:eq(1)', nRow).addClass('with-tip');
        $('td:eq(1)', nRow).bind({
          mouseenter: function () {
            displayItemTooltip($(this), aData[2], aData[3]);
          },
          mouseleave: function () {
            $(this).removeClass('with-tip');
            $('#tips div:last-child').html('');
          }
        });

        addWordBreakToTableCellItem($('td:eq(1)', nRow), 35, aData[1]);
        $('td:eq(1)', nRow).addClass("cursor-pointer");
        $('#showPassagesForFoundItem').prop('disabled', false);
      },
      fnPreDrawCallback: function () {
        ShowBlock($('#dataTable1'), 'Loading');
        return true;
      },
      fnDrawCallback: function () {
        firstLoadListItemsFromLibrary = false;
        LoadImages('#dataTable1');
        $('.with-tip').tip();
        $('#dataTable1').unblock();
        formatTableForAddingVertialScrollBar('dataTable1', 'scrollItemDataTable1', 'noscrollItemDataTable1', 'scrollItemDataTable1IE9', 'noscrollItemDataTable1IE9');
      }
    };
    $("#dataTable1").data("options", options1);
  }

  $('input[name="ItemLibrarySelection"]').click(function () {
    var isNWEAItemTab = false;
    $('#showPassagesForFoundItem').prop('disabled', true);
    if ($(this).attr('id') == 'radioNWEAItemLibrary') {
      $('.linkit-tab').hide();
      isNWEAItemTab = true;
      $("#lblItemType").text('All');
      $("#lblItemTypeNotThird").text('All');
      $('#divItemCriteria #selectItemType').val(-1);
      $('#radioPersonalItemLibrary').removeAttr('checked', 'checked');
      $('#radioDistrictItemLibrary').removeAttr('checked', 'checked');
      if ($(this).is(':checked')) {
        firstLoadListItemsFromLibrary = true;
        $('#selectSource').show();
        showNWEAItemLibraryFilter();
        if ($('#selectSource').val() == 'select') {
          $('#searchByPassage').prop('disabled', true);
        } else if ($('#selectSource').val() == 1) {
          $("#selectSource").trigger("change");
        }

        if ($('#selectSource option').length === 0) {
          InitDataQti3pSource();
        }

        inItDataNWEATable();
      }

      firstLoadListItemsFromLibrary = false;
    } else {
      $('.linkit-tab').show();

      if (!firstLoad) {
        $('#isSearched').val(!firstLoad);
      }
      $('#selectSource').hide();
      if ($(this).attr('id') == 'radioPersonalItemLibrary' || $(this).attr('id') == 'radioDistrictItemLibrary') {
        $('#radioNWEAItemLibrary').removeAttr('checked', 'checked');
        if ($('#radioPersonalItemLibrary').is(':checked') || $('#radioDistrictItemLibrary').is(':checked')) {
          firstLoadListItemsFromLibraryNew = true;
          if (!firstLoad) {
            ReloadItem2();
          }
          $('#searchByPassage').prop('disabled', false);
        }
      }

      updateFilterUI();
      setDisabledFillterButton(isNWEAItemTab);
      $('#idKeyword').val('');
      $("select").not('#selectSource').not('.block-pagination select').prop('selectedIndex', 0);
      $('#txtKeyword').val('');
      $('#txtStandard').val('');
      $('#txtTopic').val('');
      $('#txtSkill').val('');
      $('#txtOther').val('');
      $('#txtDistrictTag').val('');
      $('#dvStandardTreeView').html('<span>Please use the filters to search</span>');
      $('#txtRefObjectTitle').val('');
      $('#txtItemTitle').val('');
      $('#txtDescription').val('');
      $('#txt3pItemTitle').val('');
      $('#txt3pDescription').val('');
      $('.txtKeyword').val('');
      getCriteriaSchema();
    }
    ShowItemGridViewByActiveTab();
  });

  updateFilterUI();
});

function showNWEAItemLibraryFilter() {
  $('#divStandardContainer').parent().show();
  $('#divStandardContainerNew').parent().hide();
  $('#divItemCriteria').parent().show();
  $('#divItemCriteriaNew').parent().hide();

  $('#qti3pItemResult').show();
  $('#qtiItemResult').hide();
  $("#setFilterNWEA").show();
  $("#setFilter").hide();

  $('#selectPassageGrade').show();
  $('#selectPassageGradeNew').hide();

  $('#selectPassageSubject').show();
  $('#selectPassageSubjectNew').hide();

  if (!$('#qti3pItemResult').html() || $('#qti3pItemResult').html().trim().length == 0) {
    var url = virtualTestUrl.loadListItemsFromLibrary;
    $.get(url,
      function (data) {
        $('showNWEAItemLibraryFilter => #qti3pItemResult').html(data);
      });
  } else {
    if (isSearchedItem()) {
      ReloadItem1();
    }
  }
  $("#fsNonNWEACriteria").hide();
  $("#fsNWEACriteria").show();
  //load standart for filter
  $('#divStandardContainer').html('');
  $('#divStandardContainerNew').html('');
  var urlLoadStandard = virtualTestUrl.loadStandardFilter;
  $.ajax({
    url: urlLoadStandard,
    cache: false
  })
    .done(function (html) {
      $('#divStandardContainer').html(html);
      //ToggleStandardCriteria();
    });

  $('#lblPassageNumberFilter').show();
  $('#selectPassageNumber').val('');
  $('#selectPassageNumber').show();
  $('#txtRefObjectTitle').val('');

  $('#lblWordCountFilter').show();
  $('#selectWordCount').show();
  $('#selectWordCount').val(0);
  $('#selectPassageTitle').show();
  $('#txtRefObjectTitle').hide();
}

$('#fsItemCriteria').coolfieldset();
$('#fsItemCriteriaNew').coolfieldset({
  animation: false
});
$('#fsStateStandards').coolfieldset();
$('#fsStateStandardsNew').coolfieldset();
$('#fsTagFilter').coolfieldset();
$('#fsPassageCriteria').coolfieldset({
  animation: false
});
$('#fsNWEACriteria').coolfieldset();
$('#fsNonNWEACriteria').coolfieldset();

var viewModelItemLibrary;
var itemBanksPersonalData = null;

$(function () {
  $('.write').addClass('current');
  $('#manageTests').addClass('current');
  viewModelItemLibrary = new CreateItemFromLibraryViewModel();
  ko.applyBindings(viewModelItemLibrary, document.getElementById('divItemFromLibrary'));

  $('#fsItemCriteria').coolfieldset({
    collapsed: false
  });
  $('#fsItemCriteriaNew').coolfieldset({
    collapsed: false
  });
  $('#fsStateStandards').coolfieldset({
    collapsed: true
  });
  $('#fsStateStandardsNew').coolfieldset({
    collapsed: true,
    animation: false
  });
  $('#fsPassageCriteria').coolfieldset({
    collapsed: true,
    animation: false
  });

  bindEvents();
  $('#fsNWEACriteria').coolfieldset({
    collapsed: true
  });
  $('#fsNonNWEACriteria').coolfieldset({
    collapsed: true
  });

  $("#searchByPassage").die("click");
  $("#searchByPassage").click(function () {
    firstLoadListItemsFromLibrary = false;
    firstLoadListItemsFromLibraryNew = false;
    $('#isSearched').val(true);
    if (isCheckedNWEAItemLibrary()) {
      $('#dataTablePassageItem3p').css('width', 'auto');
      
      ReloadPassageItem3p();
    } else {
      $('#dataTablePassageItem').css('width', 'auto');
      ReloadPassageItem();
    }
    showItemFilterResult(false);
    showPassageResult(true);
  });

  $("#showPassagesForFoundItem").die("click");
  $("#showPassagesForFoundItem").click(function () {
     firstLoadListItemsFromLibrary = false;
     firstLoadListItemsFromLibraryNew = false;
      $('#isSearched').val(true);
      
      if (isCheckedNWEAItemLibrary()) {
          $("#dataTablePassageItem3p").dataTable().fnReloadAjax(showPassageForFoundItem(false));
          $('#dataTablePassageItem3p').removeAttr('width');
      }
      else {
          $('#dataTablePassageItem').removeAttr('style');
          $("#dataTablePassageItem").dataTable().fnReloadAjax(showPassageForFoundItem(true));
      }
      $('li[data-tab="data-passage-criteria"]').click();
      $("#clearFilterPassage").click();
      showItemFilterResult(false);
      showPassageResult(true);
  });

  $("#selectSource").change(function () {
    firstLoadListItemsFromLibrary = true;
    setDisableFilterNWEA();
    updateFilterUI();
    if (PopulateSate != undefined) {
      ShowBlock($('#divStateStandards'), 'Loading');
      PopulateSate();
    }
    if ($(this).val() == 1) // 1 = Navigate
    {
      $('#divItemCriteria label#selectItemTypeLabel').show();
      $('#divItemCriteria select#selectItemType').show();
      $("#lblItemType").text('All');
    } else {
      $('#divItemCriteria label#selectItemTypeLabel').hide();
      $('#divItemCriteria select#selectItemType').hide();
    }
    if ($('#radioNWEAItemLibrary').is(':checked')) {
      $('#divListItem').show();
      $('#qti3pItemResult').show();
      $('#qtiItemResult').hide();
    }
    ShowItemGridViewByActiveTab();
  });

  $("#setFilter").die("click");
  $("#setFilter").click(function () {
    $('#showPassagesForFoundItem').prop('disabled', true);
    firstLoadListItemsFromLibraryNew = false;
    $('#isSearched').val(true);
    showItemFilterResult(true);
    showPassageResult(false);
    if (!$('#radioNWEAItemLibrary').is(':checked')) {
      ReloadItem2();
    }
  });

  $("#clearFilter").click(function () {
    $('#idKeyword').val('');
    $("select").not('#selectSource').not('.block-pagination select').prop('selectedIndex', 0);
    $('#txtKeyword').val('');
    $('#txtStandard').val('');
    $('#txtTopic').val('');
    $('#txtSkill').val('');
    $('#txtOther').val('');
    $('#txtDistrictTag').val('');
    $('#dvStandardTreeView').html('<span>Please use the filters to search</span>');
    $('#txtRefObjectTitle').val('');
    setDisableFilterNWEA();
  });

  $("#clearFilterPassage").click(function () {
    $("#data-passage-criteria select").prop('selectedIndex', 0);
    $('#txtRefObjectTitle').val('');
  });
});

function load3pItem() {
  if ($('#radioNWEAItemLibrary').is(':checked')) {
    if (!is3pUpload()) {
      $('#qti3pItemResult').show();
      $('#qtiItemResult').hide();
      $('#qti3pItemUploadResult').hide();
      if (ReloadItem1 != undefined) {
        ReloadItem1();
      }
    } else { //load 3p from upload (Progress, Certica,..)
      $('#qti3pItemResult').hide();
      $('#qtiItemResult').hide();
      $('#qti3pItemUploadResult').show();
      if ($('#qti3pItemUploadResult').html().trim().length == 0) {
        var url = virtualTestUrl.loadListItemsFromLibraryUpload + '?virtualTestId=' + '@ViewBag.VirtualTestId';
        $.get(url,
          function (data) {
            $('#qti3pItemUploadResult').html(data);
          });
      } else {
        ReloadItemUpload();
      }
    }
  }
}

function bindEvents() {
  $('button[data-dialog="close"]').die('click');
  $(document).on('click', 'button[data-dialog="close"]', function (e) {
    var self = $(e.target);
    self.closest('.dialog').dialog('close');
  });
}
var isShowQtiItem = false;

function showEditQtiItem(qtiItemId, showPassage) {
  $.ajax({
    url: virtualTestUrl.checkQtiItemExists,
    data: {
      qtiItemId: qtiItemId
    },
    type: 'get',
    cache: false
  }).done(function (response) {
    if (response.Exists == 'False') {
      alert(response.errorMessage);
      return;
    } else {
      if (isShowQtiItem == true) return;
      isShowQtiItem = true;

      if (showPassage == null) {
        showPassage = 0;
      }
      //Show popup
      var worker = $('<div />');
      worker
        .addClass("dialog EditQtiItemPopUpDialogCSS")
        .attr("id", "editQtiItemDialog")
        .appendTo("body")
        .load(showEditQtiItemItem + '?qtiItemId=' + qtiItemId + '&showPassage=' + showPassage, function () {
          worker.dialog({
            title: $(this).attr("Standard"),
            open: function () {
              //a new overlay will be generated when opening an dialog
              //set zindex of new overlay to make it cover Filter Item From Library Popup
              var z_index = $("#addNewItemFromLibraryPopup").parent().css('z-index');
              $('.ui-widget-overlay:last').css('z-index', parseInt(z_index) + 1);
              $("#editQtiItemDialog").parent().css('z-index', parseInt(z_index) + 2);
              $("#editQtiItemDialog").prev().css('top', '30px');
              $('.ui-widget-overlay:last').height(2000);
              if ($('#qtiItemDataTablePopup').length > 0) {
                ui.qtiItemDataTablePopup.fnDraw();
              }
              AdjustQtiItemDetail(); //declared in _QtiItemDetail.cshtml

              var qtiItemHTML = $('#divQtiItemDetail').html();
              qtiItemHTML = qtiItemHTML.replace(/<videolinkit/g, "<video").replace(/<\/videolinkit>/g, "</video>").replace(/<sourcelinkit /g, "<source ").replace(/<\/sourcelinkit>/g, "</source>");
              $('#divQtiItemDetail').html(qtiItemHTML);

              $('#divQtiItemDetail').find('video').trigger('play');
              // Load content in file qtiItemLoadMedia.js
              loadContentImageHotSpot('#divQtiItemDetail');
              loadContentDragAndDrop('#divQtiItemDetail');
              loadContentNumberLineHotspot('#divQtiItemDetail');
              loadContentGlossary('#divQtiItemDetail', '#glossaryMessage');
            },
            close: function () {
              $('.ui-widget-overlay:last').remove();
              $(this).remove();
              $('#tips').html('');
              isShowQtiItem = false;
            },
            modal: false,
            width: 480,
            resizable: false,
            position: {
              my: "center",
              at: "center",
              of: window
            }
          });
        });
      showModalDialogBG();
    }
  }).error(function (request) { });
}

function showModalDialogBG() {
  var win = $('body');
  $('body').prepend('<div class="ui-widget-overlay" style="width: ' + win.width() + 'px; height: ' + win.height() + 'px; z-index: 1001;"></div>');
}

function AddSelectListItems(selectList, results, defaultValue) {
  if (results == null) {
    return;
  }
  if (results.length == 0) {
    return;
  }
  selectList.empty();
  selectList.append($("<option></option>").attr("value", "-1").text(defaultValue));
  $.each(results, function (i, value) {
    selectList
      .append($("<option></option>")
        .attr("value", value.Id)
        .text(value.Name));
  });
}

var binding = false;

function reloadItemBanks() {
  $('#selectItemsBankId').empty();

  if (itemBanksPersonalData == null) {
    binding = true;
    var url = '';
    if ($('#radioPersonalItemLibrary').is(':checked') && $('#radioDistrictItemLibrary').is(':checked')) {
      url = virtualTestUrl.getItemBanksPersonalAndDistrict;
    } else {
      if ($('#radioPersonalItemLibrary').is(':checked')) {
        url = virtualTestUrl.itemBanksPersonalDataUrl;
      }
      if ($('#radioDistrictItemLibrary').is(':checked')) {
        url = virtualTestUrl.getItemDistrict;
      }
    }
    if (url.length > 0 && binding == true) {
      $('#selectItemsBankId').empty();
      binding = false;
      $.get(url, function (data) {
        AddSelectListItems($('#selectItemsBankId'), data, 'All');
      });
    }
  } else {
    AddSelectListItems($('#selectItemsBankId'), itemBanksPersonalData, 'All');
    itemBanksPersonalData = null; //clear after first use
  }
}

function LoadImages(containerSelector) {
  $(containerSelector).find("img").each(function () {
    var image = $(this);
    var imageUrl = image.attr("src");
    if (IsNullOrEmpty(imageUrl)) {
      imageUrl = image.attr("source");
    }

    var path = window.location.protocol + '//' + window.location.host;
    if (IsNullOrEmpty(imageUrl)) imageUrl = path + '/Content/images/emptybg.png';

    var testItemMediaPath = $('#hidTestItemMediaPath').val();
    var isLoadImage = imageUrl.indexOf(testItemMediaPath) != -1;

    if (isLoadImage) imageUrl = imageUrl.replace(testItemMediaPath, '');

    if (imageUrl.charAt(0) == '/') imageUrl = imageUrl.substring(1);

    image.attr("source", '');
    image.attr("src", imageUrl);
    if (imageUrl.toLowerCase().indexOf("http") == 0) return;
    if (((imageUrl && imageUrl.toLowerCase().indexOf("itemset") >= 0) || isLoadImage) &&
      imageUrl.toLowerCase().indexOf("getviewreferenceimg") < 0) {
      imageUrl = '/TestAssignmentRegrader/GetViewReferenceImg?imgPath=' + imageUrl;
      imageUrl = imageUrl + "&timestamp=" + new Date().getTime();
      image.attr("src", imageUrl);
    }
  });
  ResizeImagesBaseOnPercent('#qtiItemDataTable');
}

function IsNullOrEmpty(value) {
  return typeof (value) === "undefined" || value == null || $.trim(value) == '';
}

function ShowItemGridViewByActiveTab() {
  var activeTab = $('.itemLibraryTab').find('.itemLibraryTab-list-item.active').data('tab');
  if(activeTab == 'data-item-criteria-new'){
    showItemFilterResult(true);
    showPassageResult(false);
  }

  if(activeTab == 'data-passage-criteria'){
    showItemFilterResult(false);
    showPassageResult(true);
  }
}
