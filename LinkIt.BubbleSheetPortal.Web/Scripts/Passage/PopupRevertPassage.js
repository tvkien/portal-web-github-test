var domain = window.location.protocol + "//" + window.location.host;
var PopupRevertPassage = Vue.extend({
  template: `
      <modal-component :show.sync="isShowPopupRevertPassage" :width="width" id="popupRevertPassage">
        <div slot="header">
          <h1 style="font-size: 1.375rem;margin-bottom: 1.5rem;color: var(--navyColor);box-shadow: none;text-shadow: none;:none;font-weight: bold;">
            Revert Passage
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
              <tr v-for="item in listVersionsItem" :key="item.QTIRefObjectHistoryId">
                <td>
                  <div class="d-flex justify-content-around" style="column-gap: 0.25rem;">
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
                <td>
                  <div v-html="item.XmlContent" class="content-passage-revert"></div>
                  </div>
                </td>
                <td>{{ item.AuthorFullName }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div slot="footer">
          <button
            type="button"
            class="btn-red"
            v-on:click="isShowPopupRevertPassage = false"
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
    isShowPopupRevertPassage: {
      type: Boolean,
      required: true,
      default: false,
      twoWay: true,
    },
    qtiRefObjectId: {
      type: Number,
      required: true,
    },
  },
  computed: {},
  watch: {
    isShowPopupRevertPassage: function (val) {
      if (val) {
        this.$nextTick(function () {
          $(".with-tip").tip();
        });
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
        getListVersionsPassage:
          domain + "/PassageEditor/GetMostRecentPassageVersions",
        getPassageDetail: domain + "/QTIItem/ShowPassageDetail",
      },
    };
  },
  created: function () {
    this.GetMostRecentItemVersions();
  },
  ready: function () {},
  methods: {
    GetMostRecentItemVersions: function () {
      var self = this;
      if (this.qtiRefObjectId !== 0) {
        $.ajax({
          url: self.URL.getListVersionsPassage,
          data: { qtiRefObjectId: this.qtiRefObjectId },
          type: "GET",
          success: function (data) {
            Vue.set(self, "listVersionsItem", data.versions || []);
          },
        });
      }
    },
    FormatDateTime: function (dateString) {
      if (!dateString) {
        return "";
      }
      return moment(dateString).format("MMM DD, YYYY hh:mmA") || "";
    },
    IsCurrentVersion: function (item) {
      return item.QTIRefObjectHistoryId === 0;
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
      var self = this;
      ShowBlock($("#popupRevertPassage"), "Loading");
      var worker = $("<div />");
      let url = `${self.URL.getPassageDetail}?refObjectID=${item.QTIRefObjectId}`;
      if (item.QTIRefObjectHistoryId > 0) {
        url += `&qtiRefObjectHistoryId=${item.QTIRefObjectHistoryId}`;
      }
      worker
        .addClass("dialog PassageDetailPopupOnQtiItemDetailCSS")
        .attr("id", "PassageDetailDialog")
        .appendTo("body")
        .load(url, function () {
          worker.dialog({
            title: $(this).attr("PassageDetail"),
            open: function () {
              showModalDialogBG();
              $("#popupRevertPassage").unblock();
              $("#divPassageDetail").find("video").trigger("play");
              $("#btnEditPassageDetail").remove();
              self.isShowPopupRevertPassage = false;
            },
            close: function () {
              $(".ui-widget-overlay.overlay-revert-passage").remove();
              $(this).remove();
              $("#tips").html("");
              // Stop text to speech
              if (window.playsound != null) {
                window.playsound.pause();
              }
              self.isShowPopupRevertPassage = true;
            },
            modal: false,
            width: 780,
            resizable: false,
          });
        });
      $("#btnCloseUserClickPassageDetail").live("click", function (e) {
        $(".dialog .PassageDetailPopupOnQtiItemDetailCSS").dialog("close");
      });
    },
    RevertItem: function (item) {
      this.isShowPopupRevertPassage = false;
      this.$emit("on-revert-passage", item);
    },
  },
});
Vue.component("popup-revert-passage", PopupRevertPassage);
function showModalDialogBG() {
  var $uidialog = $(".ui-dialog");
  var zIndexArr = [];
  var maxIndex = 0;
  // Push all zIndex to array
  $uidialog.each(function () {
    var $self = $(this);
    var zIndex = $self.css("z-index").replace("px", "");
    zIndexArr.push(zIndex);
  });
  // Get max value in array
  maxIndex = Math.max.apply(Math, zIndexArr);
  var win = $("body");

  $("body").prepend(
    '<div class="ui-widget-overlay overlay-revert-passage" style="width: ' +
      win.width() +
      "px; height: " +
      win.height() +
      "px; z-index:" +
      maxIndex +
      ';"></div>'
  );
}
