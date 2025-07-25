var _activeTab = 1;
var _tabOrders = {
  DEFAULT_TAB: 1,
  TES_TYPE_TAB: 2,
  TEST_SPECIFIC_TAB: 3
}
var _preferenceNames = initPreferenceNames(districtLabel);
var _dependDataChanges = [];

var template =
  '<div class="view-option"> \
  <div class="block-border form"> \
    <div class="block-section-custom"> \
      <div class="d-flex align-items-center justify-content-between mb-4">\
        <h1 class="page-title mb-0">Options </h1>\
        <div>\
          <a v-on:click="toggleModal(true, false)" id="btnChangeOption" v-bind:class="setHeaderStyle(showloading, groupDetails.optionBS)" type="submit">Change options <i class="fa-solid fa-gear ms-2"></i></a> \
          <button v-on:click="toggleModalView(true, false)" class="btn-blue ms-3" v-bind:disabled="showloading || !groupDetails.optionBS" type="submit">Options in Effect</button> \
        </div>\
      </div>\
      <div class="option-container mt-4"> \
        <loading text="Loading" v-bind:showloading="showloading"/> \
        <p v-if="!groupDetails.optionBS" class ="no-data">No data available in table</p> \
        <accordion v-for="item in groupDetails.optionBS" v-bind:item="item" v-bind:modal="false" /> \
      </div> \
    </div> \
  </div> \
  <modal v-bind:show="showModal" v-bind:groupDetails="groupDetails" v-bind:firstData="firstData" v-on:show-modal="toggleModal" v-bind:spectific="this.spectific" /> \
  <view-modal v-bind:showview="showview" v-bind:showloading="showloadingview" v-bind:rawhtml="rawhtml" v-on:show-modal-view="toggleModalView" /> \
</div>';

var modalTemplate =
  '<div class="modal" v-show="show" v-transition="modal"> \
    <div class="modal-dialog modal-xl modal-wrapper modal-dialog-centered modal-dialog-scrollable"> \
      <div class="modal-content modal-container"> \
        <loading text="Saving" v-bind:showloading="showloading"/> \
        <content select="modal-header"> \
          <div class="modal-header"> \
            <h3 class="h3 modal-heading mb-0">Set Student/Parent Portal Options </h3> \
          </div> \
        </content> \
        <div class="modal-body form-preferences"> \
          <accordion v-on:change-value="changeValue" v-bind:modal="true" v-for="item in groupDetails.optionBS" v-bind:item="item" v-bind:spectific="spectific"/> \
        </div> \
        <div class="modal-footer modal-component-footer align-items-center justify-content-around"> \
          <button v-on:click="onCloseModal">Cancel</button> \
          <button v-on:click="submitModal" >Save</button> \
        </div> \
      </div> \
    </div> \
  </div>';

var loadingTemplate =
  '<div v-show="showloading" class="bg-full"> \
  <div class="loading-dot">{{text}}</div> \
</div>';
var loading = Vue.component('loading', {
  template: loadingTemplate,
  props: ['showloading', 'text'],
  data: function () {
    return {
      text: 'Loading',
      showloading: false
    }
  },
});
var modal = Vue.component('modal', {
  template: modalTemplate,
  props: ['show', 'groupDetails', 'firstData', 'showloading', 'spectific'],
  data: function () {
    return {
      show: false,
      groupDetails: {
        default: [],
        optionBS: [],
      },
      active: true,
      showloading: false
    }
  },
  methods: {
    findWithAttr: function (array, attr, value) {
      for (var i = 0; i < array.length; i += 1) {
        if (array[i][attr] === value) {
          return i;
        }
      }
      return -1;
    },
    submitModal: function () {
      var groupTag = [];
      groupTag = this.convertData(this.groupDetails.optionBS);
      var index = this.findWithAttr(this.groupDetails.optionBS, "GroupName", "Item Data");
      if (index > -1) {
        var dependLaunchItemAnalysisItems = this.groupDetails.optionBS[index].dependLaunchItemAnalysisItems;
        var topDependentItems = this.groupDetails.optionBS[index].topDependentItems;
        var dependItemDatas = topDependentItems.concat(dependLaunchItemAnalysisItems);
        groupTag = groupTag.concat(this.convertData(dependItemDatas));
      }
      const model = {
        dataSetCategoryID: _activeTab === _tabOrders.TES_TYPE_TAB ? comboTree1._selectedItem.id : null,
        levelID: this.groupDetails.levelID,
        level: this.groupDetails.level,
        studentPreferenceID: this.groupDetails.studentPreferenceID,
        details: groupTag,
        virtualTestIds: this.groupDetails.virtualTestIds ? this.groupDetails.virtualTestIds : '',
        classIds: this.groupDetails.classIds
      };

      var that = this;
      that.showloading = true;
      setDefaultOption(model, function (res) {
        that.showloading = false;
        that.$emit('show-modal', false);
        that.$emit('show-modal-student');
        that.show = false;
        var elCheckAll = $('input[type="checkbox"]');
        if (elCheckAll.hasClass('input-checked-v2')) {
          elCheckAll.removeClass('input-checked-v2');
        }
        var bodyElement = $('body');
        if (bodyElement) {
          bodyElement.removeClass('modal-open');
        }
        if (window.studentDefaultClass) {
          window.studentDefaultClass.tryGetOptionDefault();
        }
      });
    },
    onCloseModal: function () {
      this.$emit('show-modal', false, true);
      this.$emit('show-modal-student');
      this.show = false;
      var elCheckAll = $('input[type="checkbox"]');
      if (elCheckAll.hasClass('input-checked-v2')) {
        elCheckAll.removeClass('input-checked-v2');
      }
    },
    convertData: function (array) {
      var newArray = [];
      array.forEach(function (element) {
        element.List.forEach(function (item) {
          newArray.push(item);
        });
      });
      return newArray;
    },
    getOptions: function (showModal, groupDetails) {
      this.show = showModal;
      this.groupDetails = groupDetails;
    },
    changeValue: function (name, value) {
      var index = this.findWithAttr(this.groupDetails.optionBS, "GroupName", "Item Data");
      if (index > -1) {
        if (name === 'showItemDetailChart') {
          var dependItemDetailChartItems = this.groupDetails.optionBS[index].dependItemDetailChartItems;
          for (var i = 0; i < dependItemDetailChartItems.length; i++) {
            var group = dependItemDetailChartItems[i];
            for (var j = 0; j < group.List.length; j++) {
              var item = group.List[j];
              if (item.Name === name) continue;

              if (!item.IsDisabledByType) {
                if (!value) {
                  item.Locked = false;
                }
                item.Value = value;
                item.IsDisabled = !value;
                $(item.Name + "checkspectific").prop('disabled', false);
              }
            }
          }
        }

        if (name === 'launchItemAnalysis') {
          var dependLaunchItemAnalysisItems = this.groupDetails.optionBS[index].dependLaunchItemAnalysisItems;
          for (var i = 0; i < dependLaunchItemAnalysisItems.length; i++) {
            var group = dependLaunchItemAnalysisItems[i];
            for (var j = 0; j < group.List.length; j++) {
              var item = group.List[j];
              if (item.Name === name) continue;
              if (!item.IsDisabledByType) {
                if (!value) {
                  item.Locked = false;
                }
                item.Value = value;
                item.IsDisabled = !value;
                $(item.Name + "checkspectific").prop('disabled', false);
              }
            }
          }
        }

        if (name === 'showSummaryDetail') {
          var dependLaunchTopDependentItem = this.groupDetails.optionBS[index].topDependentItems.find(x => x.GroupName === "Tags");
          if (dependLaunchTopDependentItem) {
            for (var j = 0; j < dependLaunchTopDependentItem.List.length; j++) {
              var item = dependLaunchTopDependentItem.List[j];
              if (item.Name === name) continue;
              if (!item.IsDisabledByType) {
                if (!value) {
                  item.Locked = false;
                }
                item.Value = value;
                item.IsDisabled = !value;
                $(item.Name + "checkspectific").prop('disabled', false);
              }
            }
          }
        }

        if (_activeTab === _tabOrders.TES_TYPE_TAB) {
          if (!value)
            $("#" + name + "checkspectific").prop('disabled', true);
          else
            $("#" + name + "checkspectific").prop('disabled', false);
        }
      }
    },
    changeTab: function (value) {
      _activeTab = value;
    }
  }
});

var view = Vue.component('view-modal', {
  template: '<div class="modal" v-show="showview" v-transition="modal"> \
    <div class="modal-dialog modal-xl modal-wrapper matrix modal-dialog-centered modal-dialog-scrollable"> \
      <div class="modal-content modal-container"> \
         <loading text="Loading" v-bind:showloading="showloading"/> \
         <div class="modal-header"> \
            <h3 class="h3 modal-heading mb-0">Options in Effect</h3> \
            <a v-on:click="onCloseModal" class="ui-dialog-titlebar-close ui-corner-all pointer"> \
            </a>\
          </div> \
          <div style="position: relative" class="modal-body"> \
             <div v-html="rawhtml"> \
             </div> \
          </div> \
          <div class="modal-footer form"> \
            <span> \
              <span class="checked option"></span>&nbsp;On (Unlocked)&nbsp; \
              <span class="non-checked option"></span>&nbsp;Off (Unlocked)&nbsp; \
              <span class="locked"><span class="checked option"></span>&nbsp;On and Locked&nbsp;</span> \
              <span class="locked"><span class="non-checked option"></span>&nbsp;Off and Locked&nbsp;</span> \
              <span class="missing"><span class="non-checked option"></span>&nbsp;Not Set&nbsp;</span> \
              <span class="conflict"><span class="option non-checked"></span>&nbsp;Options in Conflict</span> \
            </span> \
          </div> \
      </div> \
    </div>',
  props: ['showview', 'rawhtml', 'showloading'],
  data: function () {
    return {
      rawhtml: ''
    }
  },
  component: {
    loading: loading
  },
  methods: {
    onCloseModal: function () {
      this.$emit('show-modal-view', false);
    },
    getViewOptions: function (showView, rawhtml, showloading) {
      this.showview = showView;
      this.rawhtml = rawhtml;
      this.showloading = showloading;
    }
  }
});

var accordion = Vue.component('accordion', {
  template: '<div class="accordion-container" v-show="item.IsShow"> \
            <a class="accordion-header" @click.prevent="active = !active"> \
              {{item.GroupName}} \
              <i class="fa-solid fa-chevron-down icon-arrow ms-auto"></i> \
            </a> \
            <div v-if="modal"> \
              <div v-bind:class="setRowStyle(index)"  v-show="active" v-for="(subItem, index) in item.List"> \
                <p class="lable">{{reNameItem(subItem.Name)}}</p> \
                <div class="group-input"> \
                  <label class="switch"> \
                      <input :id="renderIdSwitchElement(subItem.Name)" type="checkbox" :name="subItem.Name" v-on:change="onChange(subItem.Name, subItem.Value)" v-model="subItem.Value" :disabled="subItem.IsDisabled"> \
                      <span class="slider round"></span> \
                  </label > \
                  <div class="hide-input"> \
                    <input :id="renderIdElement(subItem.Name, false)" type="radio" :name="subItem.Name" v-model="subItem.Value" :value="false" v-on:click="onChange(subItem.Name, false)" :disabled="subItem.IsDisabled"/> \
                    <label :for="renderIdElement(subItem.Name, false)">Off</label> \
                    <input :id="renderIdElement(subItem.Name, true)" type="radio" :name="subItem.Name" v-model="subItem.Value" :value="true" v-on:click="onChange(subItem.Name, true)" :disabled="subItem.IsDisabled"/> \
                    <label :for="renderIdElement(subItem.Name, true)">On</label> \
                  </div>\
                  <div class="lock-input">\
                    <input v-show="showHideLock(subItem.Name)" type="checkbox" :name="rederIdLockElement(subItem.Name)" :id="rederIdLockElement(subItem.Name)" v-model="subItem.Locked" value="1" :disabled="disabledLock(subItem.IsDisabled, subItem.Value, subItem.Name)"> \
                    <label v-show="showHideLock(subItem.Name)" :for="rederIdLockElement(subItem.Name)">Lock</label> \
                  </div>\
                </div> \
              </div> \
            </div> \
            <div v-else> \
              <div v-bind:class="setRowStyle(index)"  v-show="active" v-for="(subItem, index) in item.List"> \
                <p class="lable">{{reNameItem(subItem.Name)}}</p> \
                <p class="lable">{{subItem.IsConflict ? "Options in Conflict" : (subItem.IsMissing ? "N/A" : (subItem.Value ? "On" : "Off"))}}</p> \
              </div> \
            </div> \
           <div class="group-bs accordion-container" v-show="active" v-for="(dependItem, childIndex) in item.topDependentItems"> \
                <a class="accordion-header" @click.prevent="toggleChild(childIndex)"> \
                  {{dependItem.GroupName}} \
                  <i class="fa-solid fa-chevron-down icon-arrow ms-auto"></i> \
                </a> \
                <div v-if="modal"> \
                  <div v-bind:class="setRowStyle(index)"  v-show="!childrenNotActive[childIndex]" v-for="(dependSubItem, index) in dependItem.List"> \
                    <p class="lable">{{reNameItem(dependSubItem.Name)}}</p> \
                    <div class="group-input"> \
                      <label class="switch"> \
                        <input :id="renderIdSwitchElement(dependSubItem.Name)" type="checkbox" :name="renderSwitchElement(dependSubItem.Name)" v-on:change="onChange(dependSubItem.Name, dependSubItem.Value)" v-model="dependSubItem.Value" :disabled="dependSubItem.IsDisabled"> \
                        <span class="slider round"></span> \
                      </label > \
                      <div class="hide-input"> \
                        <input :id="renderIdElement(dependSubItem.Name, false)" type="radio" :name="dependSubItem.Name" v-model="dependSubItem.Value" :value="false" v-on:click="onChange(dependSubItem.Name, false)" :disabled="dependSubItem.IsDisabled"/> \
                        <label :for="renderIdElement(dependSubItem.Name, false)">Off</label> \
                        <input :id="renderIdElement(dependSubItem.Name, true)" type="radio" :name="dependSubItem.Name" v-model="dependSubItem.Value" :value="true" v-on:click="onChange(dependSubItem.Name, true)" :disabled="dependSubItem.IsDisabled"/> \
                        <label :for="renderIdElement(dependSubItem.Name, true)">On</label> \
                      </div>\
                      <div class="lock-input">\
                        <input v-show="showHideLock(dependSubItem.Name)" type="checkbox" :name="rederIdLockElement(dependSubItem.Name)" :id="rederIdLockElement(dependSubItem.Name)" v-model="dependSubItem.Locked" value="1" :disabled="disabledLock(dependSubItem.IsDisabled, dependSubItem.Value, dependSubItem.Name)"> \
                        <label v-show="showHideLock(dependSubItem.Name)" :for="rederIdLockElement(dependSubItem.Name)">Lock</label> \
                      </div>\
                    </div> \
                  </div> \
                </div> \
                <div v-else> \
                  <div v-bind:class="setRowStyle(index)"  v-show="!childrenNotActive[childIndex]" v-for="(dependSubItem, index) in dependItem.List"> \
                    <p class="lable">{{reNameItem(dependSubItem.Name)}}</p> \
                    <p class="lable">{{dependSubItem.IsMissing ? "N/A" : (dependSubItem.Value ? "On" : "Off")}}</p> \
                  </div> \
                </div> \
            </div> \
           <div class="group-bs accordion-container" v-show="active" v-for="(dependItem, childIndex) in item.botDependentItems"> \
                <a class="accordion-header" @click.prevent="toggleChild(childIndex + 10)"> \
                  {{dependItem.GroupName}} \
                  <i class="fa-solid fa-chevron-down icon-arrow ms-auto"></i> \
                </a> \
                <div v-if="modal"> \
                  <div v-bind:class="setRowStyle(index)"  v-show="!childrenNotActive[childIndex + 10]" v-for="(dependSubItem, index) in dependItem.List"> \
                    <p class="lable">{{reNameItem(dependSubItem.Name)}}</p> \
                    <div class="group-input"> \
                      <label class="switch"> \
                            <input :id="renderIdSwitchElement(dependSubItem.Name)" type="checkbox" :name="renderSwitchElement(dependSubItem.Name)" v-on:change="onChange(dependSubItem.Name, dependSubItem.Value)" v-model="dependSubItem.Value" :disabled="dependSubItem.IsDisabled"> \
                            <span class="slider round"></span> \
                      </label > \
                      <div class="hide-input"> \
                        <input :id="renderIdElement(dependSubItem.Name, false)" type="radio" :name="dependSubItem.Name" v-model="dependSubItem.Value" :value="false" v-on:click="onChange(dependSubItem.Name, false)" :disabled="dependSubItem.IsDisabled"/> \
                        <label :for="renderIdElement(dependSubItem.Name, false)">Off</label> \
                        <input :id="renderIdElement(dependSubItem.Name, true)" type="radio" :name="dependSubItem.Name" v-model="dependSubItem.Value" :value="true" v-on:click="onChange(dependSubItem.Name, true)" :disabled="dependSubItem.IsDisabled"/> \
                        <label :for="renderIdElement(dependSubItem.Name, true)">On</label> \
                      </div>\
                      <div class="lock-input">\
                        <input v-show="showHideLock(dependSubItem.Name)" type="checkbox" :name="rederIdLockElement(dependSubItem.Name)" :id="rederIdLockElement(dependSubItem.Name)" v-model="dependSubItem.Locked" value="1" :disabled="disabledLock(dependSubItem.IsDisabled, dependSubItem.Value, dependSubItem.Name)"> \
                        <label v-show="showHideLock(dependSubItem.Name)" :for="rederIdLockElement(dependSubItem.Name)">Lock</label> \
                      </div> \
                    </div> \
                  </div> \
                </div> \
                <div v-else> \
                  <div v-bind:class="setRowStyle(index)"  v-show="!childrenNotActive[childIndex + 10]" v-for="(dependSubItem, index) in dependItem.List"> \
                    <p class="lable ">{{reNameItem(dependSubItem.Name)}}</p> \
                    <p class="lable">{{dependSubItem.IsMissing ? "N/A" : (dependSubItem.Value ? "On" : "Off")}}</p> \
                  </div> \
                </div> \
                <div class="group-bs accordion-container" v-show="!childrenNotActive[childIndex + 10]" v-for="(dependItemSub, subChildIndex) in dependItem.subDependBotItemDatas"> \
                    <a class="accordion-header" @click.prevent="toggleChild(subChildIndex + 100)"> \
                      {{dependItemSub.GroupName}} \
                      <i class="fa-solid fa-chevron-down icon-arrow ms-auto"></i> \
                    </a> \
                    <div v-if="modal"> \
                      <div v-bind:class="setRowStyle(index)"  v-show="!childrenNotActive[subChildIndex + 100]" v-for="(dependSubItem, index) in dependItemSub.List"> \
                        <p class="lable ">{{reNameItem(dependSubItem.Name)}}</p> \
                        <div class="lable"> \
                        <div class="group-input">\
                          <label class="switch"> \
                            <input :id="renderIdSwitchElement(dependSubItem.Name)" type="checkbox" :name="renderSwitchElement(dependSubItem.Name)" v-on:change="onChange(dependSubItem.Name, dependSubItem.Value)" v-model="dependSubItem.Value" :disabled="dependSubItem.IsDisabled"> \
                            <span class="slider round"></span> \
                          </label > \
                            <div class="hide-input"> \
                              <input :id="renderIdElement(dependSubItem.Name, false)" type="radio" :name="dependSubItem.Name" v-model="dependSubItem.Value" :value="false" v-on:click="onChange(dependSubItem.Name, false)" :disabled="dependSubItem.IsDisabled"/> \
                              <label :for="renderIdElement(dependSubItem.Name, false)">Off</label> \
                              <input :id="renderIdElement(dependSubItem.Name, true)" type="radio" :name="dependSubItem.Name" v-model="dependSubItem.Value" :value="true" v-on:click="onChange(dependSubItem.Name, true)" :disabled="dependSubItem.IsDisabled"/> \
                              <label :for="renderIdElement(dependSubItem.Name, true)">On</label> \
                            </div>\
                            <div class="lock-input"> \
                              <input v-show="showHideLock(dependSubItem.Name)" type="checkbox" :name="rederIdLockElement(dependSubItem.Name)" :id="rederIdLockElement(dependSubItem.Name)" v-model="dependSubItem.Locked" value="1" :disabled="disabledLock(dependSubItem.IsDisabled, dependSubItem.Value, dependSubItem.Name)"> \
                              <label v-show="showHideLock(dependSubItem.Name)" :for="rederIdLockElement(dependSubItem.Name)">Lock</label> \
                            </div>\
                          </div> \
                         </div> \
                      </div> \
                    </div> \
                    <div v-else> \
                      <div v-bind:class="setRowStyle(index)"  v-show="!childrenNotActive[subChildIndex + 100]" v-for="(dependSubItem, index) in dependItemSub.List"> \
                        <p class="lable ">{{reNameItem(dependSubItem.Name)}}</p> \
                        <p class="lable">{{dependSubItem.IsMissing ? "N/A" : (dependSubItem.Value ? "On" : "Off")}}</p> \
                      </div> \
                    </div> \
                </div> \
            </div> \
          </div>',
  props: ['item', 'modal', 'spectific'],
  data: function () {
    return {
      active: true,
      childrenNotActive: [],
      modal: false
    };
  },
  methods: {
    reNameItem: function (key) {
      var name = '';
      _preferenceNames.forEach(function (item) {
        if (item.key === key) {
          name = item.name;
          return;
        }
      });

      return name;
    },
    toggleChild: function (index) {
      Vue.set(this.childrenNotActive, index, !this.childrenNotActive[index]);
    },
    onChange: function (name, value) {
      this.$emit('change-value', name, value);
      //var ckbName = name + 'Ckb';
      //BindNewCkbValue(name, ckbName);
      //console.log('name', BindListCkbValue);
    },
    setRowStyle: function (index) {
      var rowStyle = (index % 2 == 0) ? 'accordion-content' : 'accordion-content bg-gray';
      return rowStyle;
    },
    renderIdElement: function (name, mode) {
      var id = name + (mode == true ? 'ON' : 'OFF') + 'spectific';
      return id;
    },
    renderSwitchElement: function (name) {
      return name + 'Ckb';
    },
    renderIdSwitchElement: function (name) {
      return name + 'CkbV2';
    },
    rederIdLockElement: function (name) {
      return name + 'checkspectific';
    },
    showHideLock: function (name) {
      if (rolesValue === '2' || name === 'visibilityInTestSpecific') {
        return false;
      }
      return true;
    },
    disabledLock: function (disabled, optionValue, name) {
      if (_activeTab === 2 && !optionValue && name !== 'visibilityInTestSpecific')
        return true;
      return disabled;
    }
  }
});

var option = Vue.component('option-default', {
  template: template,
  data: function () {
    return {
      groupDetails: {},
      firstData: {},
      showModal: false,
      active: true,
      view: false,
      showloading: false,
      rawhtml: '',
      showview: false,
      showloadingview: false,
      spectific: '0',
      tabActive: '1'
    };
  },
  component: {
    accordion: accordion,
    modal: modal,
    'view-modal': view
  },
  props: ['view'],
  methods: {
    toggleModal: function (value, isClose) {
      this.showModal = value;
      if (isClose === undefined) {
        this.groupDetails.optionBS.forEach(function (element, index) {
          element.List.forEach(function (optionItem, i) {
            if (optionItem.Name === "visibilityInTestSpecific")
              element.List[i].IsMissing = false;
          });
        });
        this.firstData = this.groupDetails;
      }
      else if (isClose) {
        this.groupDetails.optionBS.forEach(function (group) {
          for (var j = 0; j < group.List.length; j++) {
            var item = group.List[j];
            var itemChange = _dependDataChanges.find(function (_item) {
              return _item.Name === item.Name;
            })
            if (itemChange) {
              item.Value = itemChange.Value;
              item.IsDisabled = itemChange.IsDisabled;
              item.Locked = itemChange.Locked;
              item.IsDisabledByType = itemChange.IsDisabledByType;
            }
          }

          for (var j = 0; j < group.topDependentItems.length; j++) {
            var topDependentItems = group.topDependentItems[j];
            for (var k = 0; k < topDependentItems.List.length; k++) {
              var topDependentItem = topDependentItems.List[k];
              var itemChange = _dependDataChanges.find(function (_item) {
                return _item.Name === topDependentItem.Name;
              })
              if (itemChange) {
                topDependentItem.Value = itemChange.Value;
                topDependentItem.IsDisabled = itemChange.IsDisabled;
                topDependentItem.Locked = itemChange.Locked;
                topDependentItem.IsDisabledByType = itemChange.IsDisabledByType;
              }
            }
          }

          for (var j = 0; j < group.botDependentItems.length; j++) {
            var botDependentItems = group.botDependentItems[j];
            for (var k = 0; k < botDependentItems.List.length; k++) {
              var botDependentItem = botDependentItems.List[k];
              var itemChange = _dependDataChanges.find(function (_item) {
                return _item.Name === botDependentItem.Name;
              })
              if (itemChange) {
                botDependentItem.Value = itemChange.Value;
                botDependentItem.IsDisabled = itemChange.IsDisabled;
                botDependentItem.Locked = itemChange.Locked;
                botDependentItem.IsDisabledByType = itemChange.IsDisabledByType;
              }
            }
          }

          for (var j = 0; j < group.dependItemDetailChartItems.length; j++) {
            var dependItemDetailChartItems = group.dependItemDetailChartItems[j];
            for (var k = 0; k < dependItemDetailChartItems.List.length; k++) {
              var dependItemDetailChartItem = dependItemDetailChartItems.List[k];
              var itemChange = _dependDataChanges.find(function (_item) {
                return _item.Name === dependItemDetailChartItem.Name;
              })
              if (itemChange) {
                dependItemDetailChartItem.Value = itemChange.Value;
                dependItemDetailChartItem.IsDisabled = itemChange.IsDisabled;
                dependItemDetailChartItem.Locked = itemChange.Locked;
                dependItemDetailChartItem.IsDisabledByType = itemChange.IsDisabledByType;
              }
            }
          }

          for (var j = 0; j < group.dependLaunchItemAnalysisItems.length; j++) {
            var dependLaunchItemAnalysisItems = group.dependLaunchItemAnalysisItems[j];
            for (var k = 0; k < dependLaunchItemAnalysisItems.List.length; k++) {
              var dependLaunchItemAnalysisItem = dependLaunchItemAnalysisItems.List[k];
              var itemChange = _dependDataChanges.find(function (_item) {
                return _item.Name === dependLaunchItemAnalysisItem.Name;
              })
              if (itemChange) {
                dependLaunchItemAnalysisItem.Value = itemChange.Value;
                dependLaunchItemAnalysisItem.IsDisabled = itemChange.IsDisabled;
                dependLaunchItemAnalysisItem.Locked = itemChange.Locked;
                dependLaunchItemAnalysisItem.IsDisabledByType = itemChange.IsDisabledByType;
              }
            }
          }
        });
        var bodyElement = $('body');
        if (bodyElement) {
          bodyElement.removeClass('modal-open');
        }
      }
      else {
        var itemDataBS = this.groupDetails.optionBS.find(x => x.GroupName === "Item Data")
        if (itemDataBS && itemDataBS.List.some(x => !x.Value)) {
          var dependLaunchTopDependentItem = itemDataBS.topDependentItems.find(x => x.GroupName === "Tags");
          if (dependLaunchTopDependentItem) {
            for (var j = 0; j < dependLaunchTopDependentItem.List.length; j++) {
              dependLaunchTopDependentItem.List[j].IsDisabled = true;
            }
          }
        }
        this.firstData = this.groupDetails;
        var bodyElement = $('body');
        if (bodyElement) {
          bodyElement.addClass('modal-open');
        }
      }
    },
    toggleModalView: function (value) {
      this.showview = value;
      this.rawhtml = '';
      that = this;
      if (value) {
        this.showloadingview = true;
        var dataSetCategoryID = this.tabActive == _tabOrders.TES_TYPE_TAB ? comboTree1._selectedItem.id : 0;
        getMatrix({ level: this.groupDetails.level, levelID: this.groupDetails.levelID, dataSetCategoryID: dataSetCategoryID, virtualTestId: 0, tabActive: this.tabActive }, function (res) {
          that.rawhtml = res;
          that.showloadingview = false;
        });
      }
    },
    defaultOption: function (showViewOption, obj, tabActive) {
      var that = this;
      if (showViewOption) {
        that.showloading = true;
        getDefaultOption({ level: obj.level, levelID: obj.levelID, dataSetCategoryID: obj.dataSetCategoryID }, function (res) {
          _dependDataChanges = res.Details;
          if (res) {
            that.showloading = false;
          }
          var groupDetails = {
            optionBS: [],
            default: [],
            dataSetCategoryID: res.DataSetCategoryID,
            levelID: res.LevelID,
            level: res.Level,
            studentPreferenceID: res.StudentPreferenceID
          };
          var topDependItemDatas = ['Tags', 'Averages'];
          var botDependItemDatas = ['Item Analysis'];
          var subDependBotItemDatas = ['Time Spent', 'Item Detail Chart'];
          var dependLaunchItemAnalysisItems = [];
          var dependItemDetailChartItems = [];
          var dependItemDatas = topDependItemDatas.concat(botDependItemDatas).concat(subDependBotItemDatas);
          var dependGroups = [];
          res.GroupDetails.forEach(function (element, index) {
            if (dependItemDatas.indexOf(element.GroupName) > -1) {
              dependGroups.push(element);
              if (element.GroupName === subDependBotItemDatas[1]) {
                dependItemDetailChartItems.push(element);
              }
              if (element.GroupName === botDependItemDatas[0] || element.GroupName === subDependBotItemDatas[0]) {
                dependLaunchItemAnalysisItems.push(element);
              }
            }
            else {
              var isShow = (index == 0 && tabActive == 1) ? false : true;

              element.List.forEach(function (optionItem, i) {
                var itemIndex = tabActive == 1 ? optionItem.Name.indexOf('visibilityInTestSpecific') : optionItem.Name.indexOf('showTest');
                if (itemIndex > -1)
                  element.List.splice(itemIndex, 1);
              });

              var group = {
                GroupName: element.GroupName,
                List: element.List,
                topDependentItems: [],
                botDependentItems: [],
                dependItemDetailChartItems: [],
                dependLaunchItemAnalysisItems: [],
                IsShow: isShow
              }
              groupDetails['optionBS'].push(group);
            }
          });

          var index = 0;
          for (var i = 0; i < groupDetails.optionBS.length; i += 1) {
            if (groupDetails.optionBS[i]["GroupName"] === "Item Data") {
              index = i;
            }
          }
          groupDetails['optionBS'][index].topDependentItems = dependGroups.filter(function (group) {
              return topDependItemDatas.indexOf(group.GroupName) != -1;
          });
          groupDetails['optionBS'][index].botDependentItems = dependGroups.filter(function (group) {
            return botDependItemDatas.indexOf(group.GroupName) != -1;
          });
          groupDetails['optionBS'][index].botDependentItems[0]['subDependBotItemDatas'] = dependGroups.filter(function (group) {
            return subDependBotItemDatas.indexOf(group.GroupName) != -1;
          });
          groupDetails['optionBS'][index].dependItemDetailChartItems = dependItemDetailChartItems;
          groupDetails['optionBS'][index].dependLaunchItemAnalysisItems = dependLaunchItemAnalysisItems.concat(dependItemDetailChartItems);

          that.firstData = groupDetails;
          that.groupDetails = groupDetails;
        });
      } else {
        this.groupDetails = {};
      }
    },
    setHeaderStyle: function (showLoading, options) {
      var headerStyle = showLoading || !options ? 'btn btn-link pe-0 disabled ms-3' : 'btn btn-link pe-0 ms-3';
      return headerStyle;
    },
    changeTabOption: function (value) {
      this.tabActive = value;
    }
  }
});
