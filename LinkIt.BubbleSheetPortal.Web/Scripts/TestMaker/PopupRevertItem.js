var domain = window.location.protocol + "//" + window.location.host;
var PopupRevertItem = Vue.extend({
  template: `
      <modal-component :show.sync="isShowPopupRevertItem" :width="width" id="popupRevertItem">
        <div slot="header">
          <h1 style="font-size: 1.375rem;margin-bottom: 1.5rem;color: var(--navyColor);box-shadow: none;text-shadow: none;:none;font-weight: bold;">
            Revert Item
          </h1>
        </div>
        <div slot="body"  id="dataTableRevertItem">
          <table
            class="datatable table no-margin mb-4"
            width="100%"
          >
            <colgroup>
              <col span="1" style="width: 10%" />
              <col span="1" style="width: 25%" />
              <col span="1" style="width: 45%" />
              <col span="1" style="width: 20%" />
            </colgroup>
            <thead>
              <tr>
                <th></th>
                <th>Date</th>
                <th>Content</th>
                <th>Author</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in listVersionsItem" :key="item.QTIItemHistoryID">
                <td>
                  <div class="d-flex justify-content-around"  style="column-gap: 0.25rem;">
                    <a
                      :style="{ 'visibility': !IsCurrentVersion(item)  ? 'visible' : 'hidden' }"
                      href="javascript:void(0);"
                      title="Revert"
                      @click="RevertItem(item)"
                      class="with-tip"
                      v-html="revertIcon"
                    >
                    </a>
                    <a
                      href="javascript:void(0);"
                      title="Review"
                      @click="ReviewItem(item)"
                      class="with-tip"
                      v-html="reviewIcon"
                    >
                    </a>
                  </div>
                </td>
                <td>
                  <div>
                    <div style="font-size: 14px;">
                      {{ FormatDateTime(item.ChangedDate) }}
                    </div>
                    <div style="font-size: small;" v-html="CurrentVersionText(item)">
                    </div>
                  </div>
                </td>
                <td v-html="DisplayQTIItemTile(item.XmlContent)"></td>
                <td>{{ item.AuthorFullName }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div slot="footer">
          <button
            type="button"
            class="btn-red"
            v-on:click="isShowPopupRevertItem = false"
            style="z-index: inherit;"
          >
            Close
          </button>
        </div>
      </modal-component>`,
  components: {
    "modal-component": ModalComponent,
  },
  props: {
    width: {
      type: Number,
      default: 800,
    },
    isShowPopupRevertItem: {
      type: Boolean,
      required: true,
      default: false,
      twoWay: true,
    },
    qtiItemId: {
      type: Number,
      required: true,
    },
  },
  computed: {},
  watch: {
    isShowPopupRevertItem: function (val) {
      if (val) {
        this.init();
      }
    },
  },

  data: function () {
    return {
      listVersionsItem: [],
      reviewIcon:
        '<span class="custom-icon custom-icon fa-solid fa-eye icon-grey"></span>',
      revertIcon:
        '<span class="custom-icon custom-icon fa-solid fa-clock-rotate-left icon-grey"></span>',
      URL: {
        getListVersionsItem: domain + "/TestMaker/GetMostRecentItemVersions",
        checkQtiItemExists: domain + "/ItemBank/CheckQtiItemExists",
        getEditQtiItemItem: domain + "/ItemBank/ShowEditQtiItemItem",
      },
    };
  },
  created: function () {
    this.GetMostRecentItemVersions();
  },
  methods: {
    GetMostRecentItemVersions: function () {
      var self = this;
      if (this.qtiItemId !== 0) {
        $.ajax({
          url: self.URL.getListVersionsItem,
          data: { qtiItemId: this.qtiItemId },
          type: "GET",
          success: function (data) {
            Vue.set(self, "listVersionsItem", data.versions || []);
          },
        });
      }
    },
    DisplayQTIItemTile: function (xmlContent) {
      var title = "";
      xmlContent = correctInlineChoice(xmlContent);
      $(xmlContent)
        .find(".itemBody, itemBody, itembody")
        .each(function () {
          var itemBody = $(this);
          itemBody.find("videolinkit").replaceWith(function () {
            return $("");
          });
          if (
            $(xmlContent).find("responsedeclaration").attr("partialgrading") ==
            "1"
          ) {
            itemBody.find("sourcetext").each(function () {
              if ($(this).attr("pointvalue") > 0) {
                $(this).addClass("marker-correct");
              }
            });
          } else {
            $(xmlContent)
              .find("correctResponse")
              .each(function () {
                var id = $(this).attr("identifier");
                itemBody
                  .find('sourcetext[identifier="' + id + '"]')
                  .addClass("marker-correct");
              });
          }
          title = itemBody.html();
        });
      title = title.replaceAll("<object", "<object style='display: none;'");
      var divTitle =
        '<div class="content-item-revert">' +
        title +
        "</div>";

      return divTitle;
    },
    FormatDateTime: function (dateString) {
      if (!dateString) {
        return "";
      }
      return moment(dateString).format("MMM DD, YYYY hh:mmA") || "";
    },
    IsCurrentVersion: function (item) {
      return item.QTIItemHistoryID === 0;
    },
    CurrentVersionText: function (item) {
      if (!this.IsCurrentVersion(item)) {
        return "";
      }
      const dateString = this.FormatDateTime(item.RevertedFromDate);
      return dateString
        ? `(Current Version<br>Reverted from ${dateString})`
        : "(Current Version)";
    },
    ReviewItem: function (item) {
      ShowBlock($("#popupRevertItem"), "Loading");
      this.showEditQtiItemPopupIndex(item.QTIItemID, item.QTIItemHistoryID, 1);
    },
    RevertItem: function (item) {
      this.isShowPopupRevertItem = false;
      this.$emit("on-revert-item", item);
    },

    showEditQtiItemPopupIndex: function (
      qtiItemId,
      qtiItemHistoryId,
      showPassage
    ) {
      var self = this;
      qtiItemId = qtiItemId || 0;
      qtiItemHistoryId = qtiItemHistoryId || 0;
      //Check if this item is existing or not
      $.ajax({
        url: self.URL.checkQtiItemExists,
        data: { qtiItemId: qtiItemId },
        type: "get",
        cache: false,
      })
        .done(function (response) {
          if (response.Exists == "False") {
            $("#popupRevertItem").unblock();
            customAlert(response.errorMessage, { ZIndex: 9999 });
            return;
          } else {
            var worker = $("<div />");
            let url = `${self.URL.getEditQtiItemItem}?qtiItemId=${qtiItemId}&showPassage=${showPassage}`;
            if (qtiItemHistoryId > 0) {
              url += `&qtiItemHistoryId=${qtiItemHistoryId}`;
            }
            worker
              .addClass("dialog EditQtiItemPopUpDialogCSS")
              .attr("id", "editQtiItemDialog")
              .appendTo("body")
              .load(url, function () {
                worker.dialog({
                  title: $(this).attr("Standard"),
                  open: function () {
                    AdjustQtiItemDetail(); //declared in _QtiItemDetail.cshtml
                    var qtiItemHTML = $("#divQtiItemDetail").html();
                    qtiItemHTML = qtiItemHTML
                      .replace(/<videolinkit/g, "<video")
                      .replace(/<\/videolinkit>/g, "</video>")
                      .replace(/<sourcelinkit /g, "<source ")
                      .replace(/<\/sourcelinkit>/g, "</source>");
                    $("#divQtiItemDetail").html(qtiItemHTML);
                    $("#divQtiItemDetail").find("video").trigger("play");
                    // Load content in file qtiItemLoadMedia.js
                    loadContentImageHotSpot("#divQtiItemDetail");
                    loadContentDragAndDrop("#divQtiItemDetail");
                    loadContentNumberLineHotspot("#divQtiItemDetail");
                    loadContentGlossary(
                      "#divQtiItemDetail",
                      "#glossaryMessage"
                    );
                    $("#popupRevertItem").unblock();
                    self.isShowPopupRevertItem = false;
                  },
                  close: function () {
                    $(".ui-widget-overlay:last").remove();
                    $(this).remove();
                    $("#tips").html("");
                    // Stop text to speech
                    if (!!responsiveVoice) {
                      responsiveVoice.cancel();
                    }
                    self.isShowPopupRevertItem = true;
                  },
                  modal: false,
                  width: 780,
                  resizable: false,
                });
              });
            showModalDialogBG();
            $("#btnCloseUserClick").live("click", function (e) {
              $(".dialog").dialog("close");
            });
          }
        })
        .error(function (request) {});
    },
    init() {
      $(".with-tip").tip();
      if (window.MathJax) {
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, "dataTableRevertItem"]);
      }
    },
  },
});

Vue.component("popup-revert-item", PopupRevertItem);
