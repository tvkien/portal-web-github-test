var renderdefaultSectionLvlSettings = (ArrSectionsChecked) => {
  defaultSectionLvlSettings.list_setting_checked = ArrSectionsChecked
}

var DISPLAY_SetAtSectionLevels = (resetSectionsLvlSetting = false) => {
  $('.setAtSectionLvl').addClass('hide');
  $('.setAtSectionLvl').parent().css('display', 'none');
  $('#lbltimeLimit').show();

  defaultSectionLvlSettings.show_time_limit_warning_readonly = testPreferenceModel.OptionTags.filter(o => o.Key == 'showTimeLimitWarning')[0].Value

  sectionsOptionsToolsTagsDefaultSettings.map(v => v.Key).forEach(sectionItem => {
    $(`#${sectionItem}1`).removeClass('hide');
    $(`#${sectionItem}1`).parent().css('display', 'block');

    if (sectionItem == "timeLimitSectionItems") {
      if ($('#lbltimeLimit').text().toLowerCase() != 'off') {
        $('#lbltimeLimit').text('On')
      } else {
        $('#lbltimeLimit').text('Off')
      }
    }
  })
  if (resetSectionsLvlSetting) {
    sectionsOptionsToolsTagsDefaultSettings = []
  }
  renderdefaultSectionLvlSettings(sectionsOptionsToolsTagsDefaultSettings)
}


function DISPLAY_sectionAvailability() {
  let sectionAvailability = testPreferenceModel.OptionTags.filter(o => o.Key == 'sectionAvailability') || []
  if (sectionAvailability.length && sectionAvailability[0].Attributes[0].Value == 1) {
    let sectionList = sectionAvailability[0].SectionItems
    $('#lblsectionAvailability').text('On')
    $(`input[name='sectionAvailability_Edit'][value='1']`).prop("checked", true);
    $('.sectionsAvailabilityList1_Edit').show();
    $('.sectionsAvailabilityList1').parent().show();
    $('.sectionsAvailabilityList1').removeClass('hide');
    for (var i = 0; i < defaultSectionLvlSettings.list_sections.length; i++) {
      var sectionInList = sectionList.find(function(item) {
        return item.Attributes[0].Value == defaultSectionLvlSettings.list_sections[i].VirtualSectionId;
      })
      if (sectionInList) {
        if (sectionInList.Attributes[1].Value == '1') {
          $(`#lbl_section_${sectionInList.Attributes[0].Value}_close1`).addClass('hide')
          $(`#lbl_section_${sectionInList.Attributes[0].Value}_open1`).removeClass('hide')

          $(`#section_${sectionInList.Attributes[0].Value}_open`).attr('checked', 'checked');

        } else {
          $(`#lbl_section_${sectionInList.Attributes[0].Value}_close1`).removeClass('hide')
          $(`#lbl_section_${sectionInList.Attributes[0].Value}_open1`).addClass('hide')

          $(`#section_${sectionInList.Attributes[0].Value}_close`).attr('checked', 'checked');
        }
      } else {
        var editSection = $('#btnEnableEditSectionAvailability');
        var section = defaultSectionLvlSettings.list_sections[i];
        if (editSection.length) {
          $(`#lbl_section_${section.VirtualSectionId}_close1`).removeClass('hide')
          $(`#lbl_section_${section.VirtualSectionId}_open1`).addClass('hide')
          $(`#section_${section.VirtualSectionId}_open`).prop('checked', false);
        } else {
          $(`#lbl_section_${section.VirtualSectionId}_close1`).addClass('hide')
          $(`#lbl_section_${section.VirtualSectionId}_open1`).removeClass('hide')
          $(`#section_${section.VirtualSectionId}_open`).prop('checked', true);
        }

      }
    }
  } else {
    $('#lblsectionAvailability').text('Off')
    $(`input[name='sectionAvailability_Edit']`).prop("checked", false);
    $('.sectionsAvailabilityList1_Edit').hide();
    $('.sectionsAvailabilityList1').addClass('hide')
  }
}

function reLoadBlockDefaultSettings(reRenderEl = false, latestCustomSectionsSetttingDefaultSettings) {
  var getSettingsItems = (obj) => {
    return obj.filter(a => a.Key.includes("SectionItems"))
  }
  sectionsOptionsToolsTagsDefaultSettings = [
    ...getSettingsItems(testPreferenceModel.OptionTags),
    ...getSettingsItems(testPreferenceModel.ToolTags)
  ]

  if (reRenderEl) {
    defaultSectionLvlSettings.list_sections = latestCustomSectionsSetttingDefaultSettings
    renderdefaultSectionLvlSettings(sectionsOptionsToolsTagsDefaultSettings)
    DISPLAY_SetAtSectionLevels()
    DISPLAY_sectionAvailability()
  }
}

var section__template = null;

var defaultSectionLvlSettings = null;

function initVueInstant() {

  section__template = {
      props: ['active_section_option_tags', 'active_section_tool_tags', 'list_setting_checked', "show_time_limit_warning_readonly"],
      watch: {
        list_setting_checked(val) {
          if (val.length > 0) {
            this.fields_display = val.map(a => a.Key)
          } else {
            this.fields_display = []
          }
        }
      },
      methods: {
        tools_handle() {
          var FIELD_NAME_TOOL = [
            'supportCalculatorSectionItems',
            'mathPaletteSectionItems',
            'spanishPaletteSectionItems',
            'simplePaletteSectionItems',
            'frenchPaletteSectionItems',
            'protractorSectionItems',
            'scientificCalculatorSectionItems',
            'supportHighlightTextSectionItems',
            'flagItemToolSectionItems',
            'eliminateChoiceToolSectionItems',
            'equationeditorSectionItems',
            'drawingToolSectionItems',
            'ruler6inchSectionItems',
            'ruler12inchSectionItems',
            'ruler15cmSectionItems',
            'ruler30cmSectionItems'
          ]
          return this.fields_display.find(function (item) {
            return FIELD_NAME_TOOL.includes(item)
          })
        },
        isNumber: function (evt) {
          evt = (evt) ? evt : window.event;
          var charCode = (evt.which) ? evt.which : evt.keyCode;

          if (
            charCode !== 46 &&
            (charCode > 31 && (charCode < 48 || charCode > 57)) ||
            (this.active_section_option_tags.duration_timeLimitSectionItems.includes('.') && charCode == 46)
          ) {
            evt.preventDefault();
          } else {
            return true;
          }
        },
        disabledBaseInKeyLockField(lockFieldChecked) {
          let flagDisabled = true
          if (this.current_page == 'ManageTest') {
            flagDisabled = false
          } else {
            if (this.can_edit && !lockFieldChecked) {
              flagDisabled = false
            }
          }

          return flagDisabled
        }
      },
      data() {
          return {
            fields_display: [],
            can_edit: true,
            current_page: '',
            ui_have_lock_field: false
          }
      },
      created() {
        this.can_edit = CAN_EDIT
        this.current_page = CURRENTPAGE
        if (this.current_page == 'ManageTest') {
          this.ui_have_lock_field = true
        }
      },
      template: `
          <div class="sections__wrapper--inner sections_content">
            <div class="accordion accordion__sections" v-if="fields_display.length > 0">
              <div class="accordionDiv" v-if="fields_display.includes('mustAnswerAllQuestionsSectionItems')">
                <div class="accordion-title vue-accordion-title first active">
                  <h3>Navigation/Display</h3>
                  <i class="fa-solid fa-chevron-down ml-10"></i>
                </div>
                <div class="accordion-content active" >
                  <table class="table table-accordion u-w-p-100">
                    <tbody>
                      <tr>
                        <td class="w-35 table-accordion-title">Must Answer All Questions</td>
                        <td class="w-35">
                          <label>{{active_section_option_tags.mustAnswerAllQuestionsSectionItems == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <div class="accordionDiv mt-16" v-if="fields_display.includes('timeLimitSectionItems')">
                <div class="accordion-title vue-accordion-title active">
                  <h3>Accommodations</h3>
                  <i class="fa-solid fa-chevron-down ml-10"></i>
                </div>
                <div class="accordion-content active">
                  <table class="table table-accordion u-w-p-100">
                    <tbody>
                      <tr>
                        <td class="w-35 table-accordion-title">Time Limit - Duration</td>
                        <td class="w-35">
                          <div>
                            <div>
                              <label>{{active_section_option_tags.timeLimitSectionItems == '1' ? 'On' : 'Off' }}</label>
                            </div>

                            <div class="clear"></div>
                            <div id="divDurationSectionItem" style="float: right;" v-show="active_section_option_tags.timeLimitSectionItems == 1">
                              <label>{{active_section_option_tags.duration_timeLimitSectionItems}} Minutes</label>
                              <div id="showTimeLimitWarningON" style="display: block; clear: both;">
                                <label>{{active_section_option_tags.timeLimitDurationType_timeLimitSectionItems == '1' ? 'Time Remaining' : 'Time Spent' }}</label>
                              </div>
                            </div>
                          </div>
                        </td>
                      </tr>
                      <tr>
                        <td class="w-35 table-accordion-title">Show Time Limit Warning</td>
                        <td class="w-35">
                          <div style="float: right;">
                            <label>{{show_time_limit_warning_readonly == '1' ? 'On' : 'Off' }}</label>
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
              <div class="accordionDiv mt-16" v-if="tools_handle()">
                <div class="accordion-title vue-accordion-title active">
                  <h3>Tools</h3>
                  <i class="fa-solid fa-chevron-down ml-10"></i>
                </div>
                <div class="accordion-content active">
                  <table class="table table-accordion u-w-p-100">
                    <tbody>
                      <tr v-if="fields_display.includes('simplePaletteSectionItems')">
                        <td class="w-35 table-accordion-title">Basic Science Palette</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.simplePaletteSectionItem}} {{active_section_tool_tags.simplePaletteSectionItems == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('mathPaletteSectionItems')">
                        <td class="w-35 table-accordion-title">Math/Science Palette</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.mathPaletteSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('spanishPaletteSectionItems')">
                        <td class="w-35 table-accordion-title">Spanish Palette</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.spanishPaletteSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>

                      </tr>
                      <tr v-if="fields_display.includes('frenchPaletteSectionItems')">
                        <td class="w-35 table-accordion-title">French Palette</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.frenchPaletteSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>

                      </tr>
                      <tr v-if="fields_display.includes('protractorSectionItems')">
                        <td class="w-35 table-accordion-title">Protractor</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.protractorSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('ruler6inchSectionItems')">
                        <td class="w-35 table-accordion-title">6" Ruler</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.ruler6inchSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('ruler12inchSectionItems')">
                        <td class="w-35 table-accordion-title">12" Ruler</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.ruler12inchSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('ruler15cmSectionItems')">
                        <td class="w-35 table-accordion-title">15cm Ruler</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.ruler15cmSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('ruler30cmSectionItems')">
                        <td class="w-35 table-accordion-title">30cm Ruler</td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.ruler30cmSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('supportCalculatorSectionItems')">
                        <td class="w-35 table-accordion-title">
                          Standard Calculator
                        </td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.supportCalculatorSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>

                      </tr>
                      <tr v-if="fields_display.includes('scientificCalculatorSectionItems')">
                        <td class="w-35 table-accordion-title">
                          Scientific Calculator
                        </td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.scientificCalculatorSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>
                      </tr>
                      <tr v-if="fields_display.includes('supportHighlightTextSectionItems')">
                        <td class="w-35 table-accordion-title">
                          Support Highlight Text
                        </td>
                        <td class="w-35">
                          <label>
                            {{active_section_tool_tags.supportHighlightTextSectionItems  == '0' ? 'Off' : '' }}
                            {{active_section_tool_tags.supportHighlightTextSectionItems  == '1' ? 'On' : '' }}
                            {{active_section_tool_tags.supportHighlightTextSectionItems  == '2' ? 'On ( Only Student )' : '' }}
                          </label>
                        </td>

                      </tr>
                      <tr v-if="fields_display.includes('eliminateChoiceToolSectionItems')">
                        <td class="w-35 table-accordion-title">
                          Eliminate Choice Tool
                        </td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.eliminateChoiceToolSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>

                      </tr>
                      <tr v-if="fields_display.includes('flagItemToolSectionItems')">
                        <td class="w-35 table-accordion-title">
                          Flag Item Tool
                        </td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.flagItemToolSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>

                      </tr>
                      <tr v-if="fields_display.includes('equationeditorSectionItems')">
                        <td class="w-35 table-accordion-title">
                          Equation Editor
                        </td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.equationeditorSectionItems  == '1' ? 'On' : 'Off' }}</label>
                        </td>

                      </tr>
                      <tr v-if="fields_display.includes('drawingToolSectionItems')">
                        <td class="w-35 table-accordion-title">
                          Drawing Tool
                        </td>
                        <td class="w-35">
                          <label>{{active_section_tool_tags.drawingToolSectionItems  == '1' ? 'Graphing and Labeling' : 'Free Formatted' }}</label>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <h2 class='note_txt' style="position: absolute;top: 50%;width: 100%;margin-top: -20px;left: 0;text-align: center;font-weight: bold;" v-else>
              There is no Section Level Setting.
            </h2>
          </div>
          `
  };

  defaultSectionLvlSettings = new Vue({
    el: `#settings_sections_default`,
    components: {
      section__template
    },
    data: {
      active_section_id: null,
      list_setting_checked: [],
      list_sections: [],

      active_section_option_tags: null,
      active_section_tool_tags: null,
      show_time_limit_warning_readonly: 0,
    },
    watch: {
      list_sections(val) {
        if (val) {
          if (val && this.active_section_id) {
            this.section_detail_actived()
          }
        }
      },
      active_section_id(val) {
        if (val) {
          this.section_detail_actived()
        }
      }
    },
    methods: {
      section_detail_actived() {
        this.active_section_option_tags = this.list_sections.filter(v => v.VirtualSectionId == this.active_section_id)[0].option_tags
        this.active_section_tool_tags = this.list_sections.filter(v => v.VirtualSectionId == this.active_section_id)[0].tool_tags
      }
    },
    template: `
    <div class="sections__wrapper--inner">
      <section__template
        :active_section_option_tags="active_section_option_tags"
        :active_section_tool_tags="active_section_tool_tags"
        :list_setting_checked="list_setting_checked"
        :show_time_limit_warning_readonly="show_time_limit_warning_readonly"
        >
      </section__template>
    </div>
  `
  });

}

function closeConfirmEdit() {
  $('.sectionsAvailabilityList1_Edit').hide();
  $(`input[name='sectionAvailability_Edit']`).prop('checked', false)
  $('#confirmEdit').dialog('close')
}

function confirmYes() {
  $('#lblsectionBasedTesting').text('On');
  $(`input[name='sectionAvailability_Edit']`).prop('checked', true);
  $('.section-open-close').prop('checked', true);
  $('.sectionsAvailabilityList1_Edit').show();
  $('#confirmEdit').dialog('close')
}

function confirmEdit(message) {
  confirmMessageV2(
    {
      message,
      cbYesBtnFuncName: 'confirmYes()',
      cbCancelBtnFuncName: 'closeConfirmEdit()'
    },
    {
      dialogAttr: {
        attr: {
          id: 'confirmEdit'
        }
      }
    }
  )
}

function initViewSetting() {
  var getSectionSetting = (section_id, setting_name, key) => {
    let currentSettings = sectionsOptionsToolsTagsDefaultSettings.map(s => ([s.Key, s.SectionItems]))
    let currentSettingVals = currentSettings.filter(s => s[0] == setting_name)

    let haveSetting = false
    let haveObjectSetting = []
    try {
      if (currentSettingVals.length) {
        let currentAttrs = currentSettingVals[0][1].map(a => a.Attributes)
        currentAttrs.forEach(attrArr => {
          attrArr.forEach(attr => {
            if (attr.Value == section_id) {
              haveObjectSetting = attrArr
              return false
            }
          })
        })

        if (haveObjectSetting.length > 0) {
          let getKeyObj = haveObjectSetting.filter(s => s.Key == key)[0]
          if (getKeyObj) {
            let haveSettingVal = getKeyObj.Value || ''

            haveSetting = haveSettingVal
            if (haveSettingVal.toString().toLowerCase() == 'true') {
              haveSetting = true
            }

            if (haveSettingVal.toString().toLowerCase() == 'false') {
              haveSetting = false
            }
          }
        }
      }
    } catch (error) {
      console.error(error)
    }

    return haveSetting
  }

  customSectionsSetttingDefaultSettings = testSectionsDefaultSettings.map(section => {
    return {
      ...section,
      ...{
        option_tags: {
          timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', 'on'),
          chklock_timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', 'lock'),
          duration_timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', 'duration') || '',
          timeLimitDurationType_timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', "timeLimitDurationType") || '1',

          mustAnswerAllQuestionsSectionItems: getSectionSetting(section.VirtualSectionId, 'mustAnswerAllQuestionsSectionItems', 'on'),
          chklock_mustAnswerAllQuestionsSectionItems: getSectionSetting(section.VirtualSectionId, 'mustAnswerAllQuestionsSectionItems', 'lock'),
        },
        tool_tags: {
          mathPaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'mathPaletteSectionItems', 'on'),
          spanishPaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'spanishPaletteSectionItems', 'on'),
          frenchPaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'frenchPaletteSectionItems', 'on'),
          protractorSectionItems: getSectionSetting(section.VirtualSectionId, 'protractorSectionItems', 'on'),
          ruler6inchSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler6inchSectionItems', 'on'),
          ruler12inchSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler12inchSectionItems', 'on'),
          ruler15cmSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler15cmSectionItems', 'on'),
          ruler30cmSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler30cmSectionItems', 'on'),
          supportCalculatorSectionItems: getSectionSetting(section.VirtualSectionId, 'supportCalculatorSectionItems', 'on'),
          scientificCalculatorSectionItems: getSectionSetting(section.VirtualSectionId, 'scientificCalculatorSectionItems', 'on'),
          simplePaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'simplePaletteSectionItems', 'on'),
          supportHighlightTextSectionItems: getSectionSetting(section.VirtualSectionId, 'supportHighlightTextSectionItems', 'on'),
          eliminateChoiceToolSectionItems: getSectionSetting(section.VirtualSectionId, 'eliminateChoiceToolSectionItems', 'on'),
          flagItemToolSectionItems: getSectionSetting(section.VirtualSectionId, 'flagItemToolSectionItems', 'on'),
          equationeditorSectionItems: getSectionSetting(section.VirtualSectionId, 'equationeditorSectionItems', 'on'),
          drawingToolSectionItems: getSectionSetting(section.VirtualSectionId, 'drawingToolSectionItems', 'on'),

          chklock_mathPaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'mathPaletteSectionItems', 'lock'),
          chklock_spanishPaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'spanishPaletteSectionItems', 'lock'),
          chklock_frenchPaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'frenchPaletteSectionItems', 'lock'),
          chklock_protractorSectionItems: getSectionSetting(section.VirtualSectionId, 'protractorSectionItems', 'lock'),
          chklock_ruler6inchSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler6inchSectionItems', 'lock'),
          chklock_ruler12inchSectionItems: getSectionSetting(section.VirtualSectionId, '23-inchSectionItems', 'lock'),
          chklock_ruler15cmSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler15cmSectionItems', 'lock'),
          chklock_ruler30cmSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler30cmSectionItems', 'lock'),
          chklock_supportCalculatorSectionItems: getSectionSetting(section.VirtualSectionId, 'supportCalculatorSectionItems', 'lock'),
          chklock_scientificCalculatorSectionItems: getSectionSetting(section.VirtualSectionId, 'scientificCalculatorSectionItems', 'lock'),
          chklock_simplePaletteSectionItems: getSectionSetting(section.VirtualSectionId, 'simplePaletteSectionItems', 'lock'),
          chklock_supportHighlightTextSectionItems: getSectionSetting(section.VirtualSectionId, 'supportHighlightTextSectionItems', 'lock'),
          chklock_eliminateChoiceToolSectionItems: getSectionSetting(section.VirtualSectionId, 'eliminateChoiceToolSectionItems', 'lock'),
          chklock_flagItemToolSectionItems: getSectionSetting(section.VirtualSectionId, 'flagItemToolSectionItems', 'lock'),
          chklock_equationeditorSectionItems: getSectionSetting(section.VirtualSectionId, 'equationeditorSectionItems', 'lock'),
          chklock_drawingToolSectionItems: getSectionSetting(section.VirtualSectionId, 'drawingToolSectionItems', 'lock')
        }
      }
    }
  })

  defaultSectionLvlSettings.list_sections = customSectionsSetttingDefaultSettings

  DISPLAY_SetAtSectionLevels()

  // testNavigationMethod = linear Test (value 1) && have sections
  // console.log(testNavigationMethod, testSectionsDefaultSettings)
  if (testNavigationMethod == 1 && testSectionsDefaultSettings.length > 1) {
    $('.sectionAvailability__wrapper').show();
    testSectionsDefaultSettings.forEach((section, index) => {
      $(`.testPreferences__panel--${CURRENTPAGE}`)
        .append(
          $(`<div class="btn__setting btn__section__created1 with-tip" data-section-active="${section.VirtualSectionId}" title="${section.SectionName}">
                <span>${section.SectionName}</span>
                </div>` )
        )

      $('.sectionsAvailabilityList1 table')
        .append($(`
        <tr>
            <td style="width: 340px;">Section ${index + 1}: ${section.SectionName}</td>
            <td style="padding-right: 0px !important">
                <label id="lbl_section_${section.VirtualSectionId}_open1">Open</label>
                <label id="lbl_section_${section.VirtualSectionId}_close1">Closed</label>
            </td>
        </tr>
        `))
      if ($('.sectionsAvailabilityList1_Edit table'))
        $('.sectionsAvailabilityList1_Edit table')
          .append($(`
        <tr>
            <td style="width: 232px;">Section ${index + 1}: ${section.SectionName}</td>
            <td>
                  <label class="switch" style="margin-right: 20px">
                      <input type="checkbox" value="1" class='section-open-close' name="section_${section.VirtualSectionId}_open_ckb"
                        id="section_${section.VirtualSectionId}_open" data-id="${section.VirtualSectionId}">
                      <span class="slider round"></span>
                  </label>
                  <div class="hide-input">
                      <input type="radio" value="1" id="section_${section.VirtualSectionId}_open"
                          name="section_${section.VirtualSectionId}_open" checked="checked"/>
                      <label for="section_${section.VirtualSectionId}_open">Open</label>
                      <input type="radio" value="0" id="section_${section.VirtualSectionId}_close"
                          name="section_${section.VirtualSectionId}_open"/>
                      <label for="section_${section.VirtualSectionId}_close">Closed</label>
                  </div>
            </td>
        </tr>
        `))
    })
  } else {
    $('.cb__sectionLevel').hide()
    $('.sectionAvailability__wrapper').hide();
  }

  $('.with-tip').tip()
  DISPLAY_sectionAvailability()

  $(document).off('click', '.btn__overall1')
  $(document).on('click', '.btn__overall1', function (e) {
    e.stopImmediatePropagation()
    $('.btn__section__created1').removeClass('active__state')
    $(this).addClass('active__state')
    $('.sections__main1').addClass('hide').slideUp()
    $('.overall__settings1').removeClass('hide').slideDown()
    return false
  });

  $(document).off('click', '.btn__section__created1')
  $(document).on('click', '.btn__section__created1', function (e) {
    e.stopImmediatePropagation()
    $('.btn__section__created1').removeClass('active__state')
    $('.btn__overall1').removeClass('active__state')
    $(this).addClass('active__state')
    defaultSectionLvlSettings.active_section_id = $(this).data('section-active')
    $('.overall__settings1').addClass('hide').slideUp()
    $('.sections__main1').removeClass('hide').slideDown()
    return false
  });

  function updateValueForStaticView() {
    let sectionAvailabilityValue = $(`input[name='sectionAvailability_Edit']:checked`).val();
    if (sectionAvailabilityValue === '1') {
      $('#lblsectionAvailability').text("On");
      $('.sectionsAvailabilityList1').removeClass('hide');
    }
    else {
      $('#lblsectionAvailability').text("Off")
      $('.sectionsAvailabilityList1').addClass('hide');
    }
    sectionOpenCloseValues().each(function (index, value) {


      if (value.value === "1") {
        $(`#lbl_section_${value.sectionId}_close1`).addClass('hide');
        $(`#lbl_section_${value.sectionId}_open1`).removeClass('hide');
      }
      else {
        $(`#lbl_section_${value.sectionId}_close1`).removeClass('hide');
        $(`#lbl_section_${value.sectionId}_open1`).addClass('hide');
      }
    });

  }
  function sectionOpenCloseValues() {
    return $('.section-open-close')
      .filter(function (index, item) {
        return item.value === '1'
      }).map(function (index, item) {
        return {
          sectionId: $(item).attr('data-id'),
          name: $(item).attr("name"),
          value: $(item).is(':checked') ? "1" : "0"
        }
      });
  }

  $('#btnEnableEditSectionAvailability').click(function () {
    $('.sectionAvailability__wrapper').hide();
    $('#btnEnableEditSectionAvailability').hide();
    $('.sectionAvailability_Edit_wrapper').show();
  });

  $('#btnCancelSectionAvailabilityList').click(function () {
    $('.sectionAvailability__wrapper').show();
    $('#btnEnableEditSectionAvailability').show();
    $('.sectionAvailability_Edit_wrapper').hide();
  });

  $('#btnSaveSectionAvailabilityList').click(function () {

    let sectionAvailabilityValue = $(`input[name='sectionAvailability_Edit']:checked`).val();
    var sectionBasedTestingValue = "";
    testPreferenceModel.OptionTags.filter(function (item) {
      return item.Key == 'sectionBasedTesting';
    }).forEach(function (item) {
      sectionBasedTestingValue = item.Value;
    })

    if (sectionAvailabilityValue === "1") {
      sectionBasedTestingValue = "1";
    }

    let sectionAvailabilityTags = {
      Key: "sectionAvailability",
      Value: "",
      Attributes: [
        { Key: "on", Value: sectionAvailabilityValue === '1' ? '1' : '0' },
        { Key: "lock", Value: "false" },
      ],
      SectionItems: []
    };
    let sectionBasedSetting = {
      Key: "sectionBasedTesting",
      Value: sectionBasedTestingValue
    };

    sectionOpenCloseValues().each(function (index, value) {

      sectionAvailabilityTags.SectionItems.push({
        Key: "sectionItem",
        Value: "",
        Attributes: [
          {
            Key: "sectionId",
            Value: `${value.sectionId}`
          },
          {
            Key: "open",
            Value: `${value.value}`
          },
          {
            Key: "lock",
            Value: "True"
          }
        ],
        SectionItems: []
      })
    });


    var jsonData = JSON.stringify({ testAssignmentId: testSchedule.testClassAssignmentId, sectionAvailabilityTags: [sectionBasedSetting, sectionAvailabilityTags] });

    ShowBlock($('#accordion-preferences-setting'), 'Loading');
    $.ajax({
      url: "TestAssignment/UpdateSectionAvailability",
      type: 'POST',
      dataType: 'json',
      contentType: 'application/json',
      data: jsonData,
      cache: false
    }).done(function (data) {
      $('#accordion-preferences-setting').unblock();
      if (data.Status) {
        updateValueForStaticView();
        $("#btnCancelSectionAvailabilityList").click();
        $('#lastUpdateInfor label:first-child').empty();
        $('#lastUpdateInfor label:first-child').append('<label>Last Update: ' + data.UpdatedDate + '</label>');
      }
      else {
        alertMessageSetting("Update failed. Please try again.");
      }
    });
    
  });

  $('.section-open-close').change(function (item) {
    var openingCount = $('.section-open-close').filter(function (index, item) {
      return ((item.value === "1" && item.checked) || $(item).is(':checked')) && !$($(item).parent()).hasClass('hide-input');
    }).length;
    if (openingCount == 0) {
      customAlert('At least 1 section must be open.', { width: 350, contentStyle: { minWidth: 'auto' } })
      $(`input[name='${this.name}'][value='1']`).prop("checked", true)
    };
  });

  $(`input[name='sectionAvailability_Edit']`).change(function (arg) {
    if ($(this).is(':checked')) {
      if ($('#lblsectionBasedTesting').text().toLowerCase() === "off") {
        $(this).prop('checked', false)
        confirmEdit('Enabling "Section Availability" will turn on "Section-Based Testing". Do you want to proceed?')
      }
      else {
        $('.sectionsAvailabilityList1_Edit').show();
      }
    }
    else {
      $('.sectionsAvailabilityList1_Edit').hide();
    }
  });
}

function defineButtonRequireTestTakerAuthentication() {

  function setToggleRequireTestTakerAuthentication() {
    let requireTestTakerAuthentication = testPreferenceModel.OptionTags.find(o => o.Key === 'requireTestTakerAuthentication');

    if (requireTestTakerAuthentication?.Value === '1') {
      $("#requireTestTakerAuthentication1").click();
      $("input[name='requireTestTakerAuthentication']").attr('checked', true);
    } else {
      $("#requireTestTakerAuthentication0").click();
      $("input[name='requireTestTakerAuthentication']").attr('checked', false);
    }
  }

  function showEditRequireTestTakerAuthentication() {
    $('.require-testTaker-authentication-group').show();
    $('.require-testTaker-authentication-action-group').show();
    $('#lblrequireTestTakerAuthentication').hide();
    $("#btnEnableEditRequireTestTakerAuthentication").hide();
  }

  function hideEditRequireTestTakerAuthentication() {
    $('.require-testTaker-authentication-action-group').hide();
    $('.require-testTaker-authentication-group').hide();
    $("#btnEnableEditRequireTestTakerAuthentication").show();
    $('#lblrequireTestTakerAuthentication').show();
  }

  // enable edit mode
  $("#btnEnableEditRequireTestTakerAuthentication").on("click", function () {
    setToggleRequireTestTakerAuthentication();
    showEditRequireTestTakerAuthentication();
  });

  // cancel()
  $("#btnCancelRequireTestTakerAuthentication").on("click", function () {
    hideEditRequireTestTakerAuthentication();
  });

  // save()
  $("#btnSaveRequireTestTakerAuthentication").on("click", function () {
    var requireTestTakerAuthentication = $("input[name='requireTestTakerAuthentication']:checked").val();
    requireTestTakerAuthentication = requireTestTakerAuthentication === 'ON' ? '1' : '0';

    var updateScheduleData = [{ Key: "requireTestTakerAuthentication", Value: requireTestTakerAuthentication }]
    var jsonData = JSON.stringify({ testAssignmentId: testSchedule.testClassAssignmentId, tags: updateScheduleData });

    ShowBlock($('#requireTestTakerAuthenticationDetail'), 'Loading');
    $.ajax({
      url: "TestAssignment/UpdateRequireTestTakerAuthentication",
      type: 'POST',
      dataType: 'json',
      contentType: 'application/json',
      data: jsonData,
      cache: false
    }).done(function (data) {
      $('#requireTestTakerAuthenticationDetail').unblock();

      if (data.Status) {
        let item = testPreferenceModel.OptionTags.find(o => o.Key === 'requireTestTakerAuthentication');
        if (item) item.Value = requireTestTakerAuthentication;

        let text = requireTestTakerAuthentication === '1' ? 'On' : 'Off';
        $('#lblrequireTestTakerAuthentication').text(text);

        $("#btnCancelRequireTestTakerAuthentication").click();
        $('#lastUpdateInfor label:first-child').empty();
        $('#lastUpdateInfor label:first-child').append('<label>Last Update: ' + data.UpdatedDate + '</label>');
      }
      else {
        alertMessageSetting("Update failed. Please try again.");
      }
    });
  });

  hideEditRequireTestTakerAuthentication();
}


$(document).ready(function () {
  initVueInstant()
  reLoadBlockDefaultSettings();
  // Working
  // var testSchedule = new testSchedule('@ViewBag.DateFormat');
  // var testNavigationMethod = @Model.NavigationMethodID
  // have testSchedule, testSectionsDefaultSettings, testNavigationMethod, sectionsOptionsToolsTagsDefaultSettings from outside
  initViewSetting()
  defineButtonRequireTestTakerAuthentication();
  $('.section-open-close').change(function (item) {
    var openingCount = $('.section-open-close').filter(function (index, item) {
      return ((item.value === "1" && item.checked) || $(item).is(':checked')) && !$($(item).parent()).hasClass('hide-input');
    }).length;
    if (openingCount == 0) {
      customAlert('At least 1 section must be open.', { width: 350, contentStyle: { minWidth: 'auto' } })
      $(`input[name='${this.name}'][value='1']`).prop("checked", true)
    };
  });
});

$(document).off('click', '.vue-accordion-title')
$(document).on('click', '.vue-accordion-title', function () {
  $(this).next().slideToggle();
  var isActive = $(this).hasClass("active");
  if (isActive) {
    $(this).removeClass("active");
  } else {
    $(this).addClass("active");
  }
})
