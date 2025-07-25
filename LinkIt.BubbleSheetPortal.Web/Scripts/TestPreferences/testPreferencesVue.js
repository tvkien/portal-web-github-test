const section__template = {
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
      const FIELD_NAME_TOOL = [
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
      return  this.fields_display.find(function(item) {
        return FIELD_NAME_TOOL.includes(item)
      })
    },
    isNumber: function(evt) {
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
    this.current_page  = CURRENTPAGE
    if (this.current_page == 'ManageTest') {
      this.ui_have_lock_field = true
    }
  },
  template: `
<div class="sections__wrapper--inner">
  <div class="accordion accordion__sections" v-if="fields_display.length > 0">
    <div v-if="fields_display.includes('mustAnswerAllQuestionsSectionItems')">
      <div class="accordion-title vue-accordion-title first active">
        <h3>Navigation/Display</h3>
      </div>
      <div class="accordion-content active" >
        <table class="table table-accordion u-w-p-100">
          <tbody>
            <tr>
              <td class="w-35 table-accordion-title">Must Answer All Questions</td>
              <td class="w-35">
                <input
                  type="radio"
                  value="0"
                  v-model="active_section_option_tags.mustAnswerAllQuestionsSectionItems"
                  id="section_01_off"
                  :disabled="disabledBaseInKeyLockField(active_section_option_tags.chklock_mustAnswerAllQuestionsSectionItems)"
                />

                <label for="section_01_off">OFF</label>
                <input
                  type="radio"
                  value="1"
                  v-model="active_section_option_tags.mustAnswerAllQuestionsSectionItems"
                  id="section_01_on"
                  :disabled="disabledBaseInKeyLockField(active_section_option_tags.chklock_mustAnswerAllQuestionsSectionItems)"
                />
                <label for="section_01_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_mustAnswerAllQuestionsSectionItems"
                  v-model="active_section_option_tags.chklock_mustAnswerAllQuestionsSectionItems" />
                <label for="chklock_mustAnswerAllQuestionsSectionItems">Lock</label>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div v-if="fields_display.includes('timeLimitSectionItems')">
      <div class="accordion-title vue-accordion-title active">
        <h3>Accommodations</h3>
      </div>
      <div class="accordion-content active">
        <table class="table table-accordion u-w-p-100">
          <tbody>
            <tr>
              <td class="w-35 table-accordion-title">Time Limit - Duration</td>
              <td class="w-35">
                <div>
                  <div style="float: left;">
                      <input id="rdTimeLimitOFFSectionItemsOff" type="radio" value="0" v-model="active_section_option_tags.timeLimitSectionItems"
                      :disabled="disabledBaseInKeyLockField(active_section_option_tags.chklock_timeLimitSectionItems)"
                      />
                      <label for="rdTimeLimitOFFSectionItemsOff">OFF</label>
                      <input id="rdTimeLimitOFFSectionItemsOn" type="radio" value="1" v-model="active_section_option_tags.timeLimitSectionItems"
                      :disabled="disabledBaseInKeyLockField(active_section_option_tags.chklock_timeLimitSectionItems)"
                      />
                      <label for="rdTimeLimitOFFSectionItemsOn">ON</label>
                  </div>

                  <div class="clear" style="height: 10px"></div>
                  <div id="divDurationSectionItem" style="float: left;" v-show="active_section_option_tags.timeLimitSectionItems == 1">
                    <input type="text" maxlength="6" style="width: 105px;" v-model="active_section_option_tags.duration_timeLimitSectionItems" @keypress="isNumber($event)"
                    :disabled="disabledBaseInKeyLockField(active_section_option_tags.chklock_timeLimitSectionItems)"
                    />
                    Minutes
                    <div id="showTimeLimitWarningON" style="display: block; clear: both; padding-top: 21px;">
                        <input type="radio" value="1" v-model="active_section_option_tags.timeLimitDurationType_timeLimitSectionItems" id="timeRemaining01"
                        :disabled="disabledBaseInKeyLockField(active_section_option_tags.chklock_timeLimitSectionItems)"
                        />
                        <label for="timeRemaining01">Time Remaining</label>
                        <input type="radio" value="2" v-model="active_section_option_tags.timeLimitDurationType_timeLimitSectionItems" id="timespent01"
                        :disabled="disabledBaseInKeyLockField(active_section_option_tags.chklock_timeLimitSectionItems)"
                         />
                        <label for="timespent01">Time Spent</label>
                    </div>
                  </div>
                </div>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_timeLimitSectionItems" v-model="active_section_option_tags.chklock_timeLimitSectionItems" />
                <label for="chklock_timeLimitSectionItems">Lock</label>
              </td>
            </tr>
            <tr>
              <td class="w-35 table-accordion-title">Show Time Limit Warning</td>
              <td class="w-35">
                <div style="float: left;">
                  <input id="" type="radio" value="0" :checked="show_time_limit_warning_readonly == 0" disabled/>
                  <label for="">OFF</label>
                  <input id="" type="radio" value="1" :checked="show_time_limit_warning_readonly == 1" disabled/>
                  <label for="">ON</label>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="tools_handle()">
      <div class="accordion-title vue-accordion-title active">
      <h3>Tools</h3>
      </div>
      <div class="accordion-content active">
        <table class="table table-accordion u-w-p-100">
          <tbody>
            <tr v-if="fields_display.includes('simplePaletteSectionItems')">
              <td class="w-35 table-accordion-title">Basic Science Palette</td>
              <td class="w-35">
              <input type="radio" value="0" v-model="active_section_tool_tags.simplePaletteSectionItems" id="section_03_off"
              :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_simplePaletteSectionItems)"
              />
              <label for="section_03_off">OFF</label>
              <input type="radio" value="1" v-model="active_section_tool_tags.simplePaletteSectionItems" id="section_03_on"
              :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_simplePaletteSectionItems)"
              />
              <label for="section_03_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
              <input type="checkbox" id="chklock_simplePaletteSectionItems" v-model="active_section_tool_tags.chklock_simplePaletteSectionItems" />
              <label for="chklock_simplePaletteSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('mathPaletteSectionItems')">
              <td class="w-35 table-accordion-title">Math/Science Palette</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.mathPaletteSectionItems" id="section_04_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_mathPaletteSectionItems)"
                />
              <label for="section_04_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.mathPaletteSectionItems" id="section_04_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_mathPaletteSectionItems)"
                />
              <label for="section_04_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_mathPaletteSectionItems" v-model="active_section_tool_tags.chklock_mathPaletteSectionItems" />
                <label for="chklock_mathPaletteSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('spanishPaletteSectionItems')">
              <td class="w-35 table-accordion-title">Spanish Palette</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.spanishPaletteSectionItems" id="section_05_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_spanishPaletteSectionItems)"
                />
                <label for="section_05_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.spanishPaletteSectionItems" id="section_05_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_spanishPaletteSectionItems)"
                />
              <label for="section_05_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_spanishPaletteSectionItems" v-model="active_section_tool_tags.chklock_spanishPaletteSectionItems" />
                <label for="chklock_spanishPaletteSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('frenchPaletteSectionItems')">
              <td class="w-35 table-accordion-title">French Palette</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.frenchPaletteSectionItems" id="section_06_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_frenchPaletteSectionItems)"
                />
                <label for="section_06_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.frenchPaletteSectionItems" id="section_06_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_frenchPaletteSectionItems)"
                />
                <label for="section_06_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_frenchPaletteSectionItems" v-model="active_section_tool_tags.chklock_frenchPaletteSectionItems" />
                <label for="chklock_frenchPaletteSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('protractorSectionItems')">
              <td class="w-35 table-accordion-title">Protractor</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.protractorSectionItems" id="section_07_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_protractorSectionItems)"
                />
                <label for="section_07_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.protractorSectionItems" id="section_07_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_protractorSectionItems)"
                />
                <label for="section_07_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_protractorSectionItems" v-model="active_section_tool_tags.chklock_protractorSectionItems" />
                <label for="chklock_protractorSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('ruler6inchSectionItems')">
              <td class="w-35 table-accordion-title">6" Ruler</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.ruler6inchSectionItems" id="section_08_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler6inchSectionItems)"
                />
                <label for="section_08_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.ruler6inchSectionItems" id="section_08_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler6inchSectionItems)"
                />
                <label for="section_08_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_ruler6inchSectionItems" v-model="active_section_tool_tags.chklock_ruler6inchSectionItems" />
                <label for="chklock_ruler6inchSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('ruler12inchSectionItems')">
              <td class="w-35 table-accordion-title">12" Ruler</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.ruler12inchSectionItems" id="section_09_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler12inchSectionItems)"
                />
                <label for="section_09_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.ruler12inchSectionItems" id="section_09_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler12inchSectionItems)"
                />
                <label for="section_09_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_ruler12inchSectionItems" v-model="active_section_tool_tags.chklock_ruler12inchSectionItems" />
                <label for="chklock_ruler12inchSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('ruler15cmSectionItems')">
              <td class="w-35 table-accordion-title">15cm Ruler</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.ruler15cmSectionItems" id="section_10_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler15cmSectionItems)"
                />
                <label for="section_10_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.ruler15cmSectionItems" id="section_10_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler15cmSectionItems)"
                />
                <label for="section_10_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_ruler15cmSectionItems" v-model="active_section_tool_tags.chklock_ruler15cmSectionItems" />
                <label for="chklock_ruler15cmSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('ruler30cmSectionItems')">
              <td class="w-35 table-accordion-title">30cm Ruler</td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.ruler30cmSectionItems" id="section_11_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler30cmSectionItems)"
                />
                <label for="section_11_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.ruler30cmSectionItems" id="section_11_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_ruler30cmSectionItems)"
                />
                <label for="section_11_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_ruler30cmSectionItems" v-model="active_section_tool_tags.chklock_ruler30cmSectionItems" />
                <label for="chklock_ruler30cmSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('supportCalculatorSectionItems')">
              <td class="w-35 table-accordion-title">
                Standard Calculator
              </td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.supportCalculatorSectionItems" id="section_12_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_supportCalculatorSectionItems)"
                />
                <label for="section_12_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.supportCalculatorSectionItems" id="section_12_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_supportCalculatorSectionItems)"
                />
                <label for="section_12_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_supportCalculatorSectionItems" v-model="active_section_tool_tags.chklock_supportCalculatorSectionItems" />
                <label for="chklock_supportCalculatorSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('scientificCalculatorSectionItems')">
              <td class="w-35 table-accordion-title">
                Scientific Calculator
              </td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.scientificCalculatorSectionItems" id="section_13_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_scientificCalculatorSectionItems)"
                />
                <label for="section_13_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.scientificCalculatorSectionItems" id="section_13_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_scientificCalculatorSectionItems)"
                />
                <label for="section_13_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_scientificCalculatorSectionItems" v-model="active_section_tool_tags.chklock_scientificCalculatorSectionItems" />
                <label for="chklock_scientificCalculatorSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('supportHighlightTextSectionItems')">
              <td class="w-35 table-accordion-title">
                Support Highlight Text
              </td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.supportHighlightTextSectionItems" id="section_14_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_supportHighlightTextSectionItems)"
                />
                <label for="section_14_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.supportHighlightTextSectionItems" id="section_14_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_supportHighlightTextSectionItems)"
                />
                <label for="section_14_on">ON</label>
                <input type="radio" value="2" v-model="active_section_tool_tags.supportHighlightTextSectionItems" id="section_14_on_only_student"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_supportHighlightTextSectionItems)"
                />
                <label for="section_14_on_only_student">ON ( Only Student )</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_supportHighlightTextSectionItems" v-model="active_section_tool_tags.chklock_supportHighlightTextSectionItems" />
                <label for="chklock_supportHighlightTextSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('eliminateChoiceToolSectionItems')">
              <td class="w-35 table-accordion-title">
                Eliminate Choice Tool
              </td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.eliminateChoiceToolSectionItems" id="section_15_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_eliminateChoiceToolSectionItems)"
                />
                <label for="section_15_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.eliminateChoiceToolSectionItems" id="section_15_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_eliminateChoiceToolSectionItems)"
                />
                <label for="section_15_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_eliminateChoiceToolSectionItems" v-model="active_section_tool_tags.chklock_eliminateChoiceToolSectionItems" />
                <label for="chklock_eliminateChoiceToolSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('flagItemToolSectionItems')">
              <td class="w-35 table-accordion-title">
                Flag Item Tool
              </td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.flagItemToolSectionItems" id="section_16_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_flagItemToolSectionItems)"
                />
                <label for="section_16_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.flagItemToolSectionItems" id="section_16_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_flagItemToolSectionItems)"
                />
                <label for="section_16_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_flagItemToolSectionItems" v-model="active_section_tool_tags.chklock_flagItemToolSectionItems" />
                <label for="chklock_flagItemToolSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('equationeditorSectionItems')">
              <td class="w-35 table-accordion-title">
                Equation Editor
              </td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.equationeditorSectionItems" id="section_17_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_equationeditorSectionItems)"
                />
                <label for="section_17_off">OFF</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.equationeditorSectionItems" id="section_17_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_equationeditorSectionItems)"
                />
                <label for="section_17_on">ON</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_equationeditorSectionItems" v-model="active_section_tool_tags.chklock_equationeditorSectionItems" />
                <label for="chklock_equationeditorSectionItems">Lock</label>
              </td>
            </tr>
            <tr v-if="fields_display.includes('drawingToolSectionItems')">
              <td class="w-35 table-accordion-title">
                Drawing Tool
              </td>
              <td class="w-35">
                <input type="radio" value="0" v-model="active_section_tool_tags.drawingToolSectionItems" id="section_18_off"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_drawingToolSectionItems)"
                />
                <label for="section_18_off">Free Formatted</label>
                <input type="radio" value="1" v-model="active_section_tool_tags.drawingToolSectionItems" id="section_18_on"
                :disabled="disabledBaseInKeyLockField(active_section_tool_tags.chklock_drawingToolSectionItems)"
                />
                <label for="section_18_on">Graphing and Labeling</label>
              </td>
              <td v-if="ui_have_lock_field">
                <input type="checkbox" id="chklock_drawingToolSectionItems" v-model="active_section_tool_tags.chklock_drawingToolSectionItems" />
                <label for="chklock_drawingToolSectionItems">Lock</label>
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

var sectionLvlSettingsVue = new Vue({
  el: `#settings_sections`,
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
    active_section_id(val) {
      if (val) {
        this.section_detail_actived();
      }
    }
  },
  methods: {
    section_detail_actived() {
      let list_section = this.list_sections.find(v => v.VirtualSectionId == this.active_section_id);

      this.active_section_option_tags = list_section.option_tags;
      this.active_section_tool_tags = list_section.tool_tags;
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


  // Working
  // var testSchedule = new testSchedule('@ViewBag.DateFormat');
  // var testNavigationMethod = @Model.NavigationMethodID
  // have testSchedule, testSections, testNavigationMethod, sectionsOptionsToolsTags from outside
  const getSectionSetting = (section_id, setting_name, key) => {


    let currentSettings = sectionsOptionsToolsTags.map(s => ([s.Key, s.SectionItems]))
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
            if (haveSettingVal.toLowerCase() == 'true') {
              haveSetting = true
            }

            if (haveSettingVal.toLowerCase() == 'false') {
              haveSetting = false
            }
          }
        }
      }
    } catch (error) {
      console.error(error);
      // expected output: ReferenceError: nonExistentFunction is not defined
      // Note - error messages will vary depending on browser
    }
    if (key == 'on' && haveSetting == false) {
      haveSetting = '0'
    }

    return haveSetting
  }

  customSectionsSettting = testSections.map(section => {
    return {
      ...section,
      ...{
        option_tags: {
          mustAnswerAllQuestionsSectionItems: getSectionSetting(section.VirtualSectionId, 'mustAnswerAllQuestionsSectionItems', 'on'),
          chklock_mustAnswerAllQuestionsSectionItems: getSectionSetting(section.VirtualSectionId, 'mustAnswerAllQuestionsSectionItems', 'lock'),

          timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', 'on'),
          chklock_timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', 'lock'),
          duration_timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', 'duration') || '',
          timeLimitDurationType_timeLimitSectionItems: getSectionSetting(section.VirtualSectionId, 'timeLimitSectionItems', "timeLimitDurationType") || '1',

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
          chklock_ruler12inchSectionItems: getSectionSetting(section.VirtualSectionId, 'ruler12inchSectionItems', 'lock'),
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

  sectionLvlSettingsVue.list_sections = customSectionsSettting

  const renderSectionLvlSettingsVue = (ArrSectionsChecked) => {
    sectionLvlSettingsVue.show_time_limit_warning_readonly = $("input[name='showTimeLimitWarning']:checked").val()
    sectionLvlSettingsVue.list_setting_checked = ArrSectionsChecked
  }

  const buildSetAtSectionLevels = (resetSectionsLvlSetting = false) => {
    sectionsOptionsToolsTags.map(v => v.Key).forEach(v => {
      let currentChecked = $(`#${v}`)
      let currentParents = currentChecked.parents('tr').find('td:nth-child(2)')

      currentChecked.prop("checked", !resetSectionsLvlSetting)
      $(currentParents).find("input[type='radio']").attr("disabled", !resetSectionsLvlSetting)

    })
    if (resetSectionsLvlSetting) {
      sectionsOptionsToolsTags = []
    }
    renderSectionLvlSettingsVue(sectionsOptionsToolsTags)
  }
  buildSetAtSectionLevels()

  const ADD_sectionsOptionsToolsTags = (currentNameInput) => {
    sectionsOptionsToolsTags.push({ Key: currentNameInput, SectionItems: [], "Value": "", "Attributes": [] })
  }

  const REMOVE_sectionsOptionsToolsTags = (currentNameInput) => {
    sectionsOptionsToolsTags = sectionsOptionsToolsTags.filter(v => v.Key != currentNameInput)

    let currentChecked = $(`#${currentNameInput}`)
    let currentParents = currentChecked.parents('tr').find('td:nth-child(2)')
    currentChecked.prop("checked", false)
    $(currentParents).find("input[type='radio']").attr("disabled", false)
  }

  const RESET_SectionLvlSettingsVue = () => {
    buildSetAtSectionLevels(true)
  }

  const handleSetSectionLevel = (el) => {
    let currentNameInput = $(el).attr("id")
    if (el.is(":checked")) {
      ADD_sectionsOptionsToolsTags(currentNameInput)
    } else {
      REMOVE_sectionsOptionsToolsTags(currentNameInput)
    }

    buildSetAtSectionLevels()
  }

  $("input[name='sectionBasedTesting']").on('change', function() {

    if ($("input[name='sectionBasedTesting']:checked").val() == 1) {
      // console.log('[🐞]_________')
      // console.log('on')
      // console.log('_________[🐞]')
    } else {
      if (sectionsOptionsToolsTags.length > 0 || $("input[name='sectionAvailability']:checked").val() == 1) {
        let r = confirm('Turning off "Section-Based Testing" will remove all Section Level settings and Section Availability settings. Do you want to proceed?');
        if (r == true) {
          $("input[name='sectionBasedTesting'][value='0']").prop("checked", true)


          $("input[name='sectionAvailability'][value='0']").prop("checked", true)
          $('.sectionsAvailabilityList').slideUp()


          RESET_SectionLvlSettingsVue()
        } else {
          $("input[name='sectionBasedTesting'][value='1']").prop("checked", true)
        }
      }
    }
  })

  $(".chkSetAtSectionLevel").on("change", function() {
    let currentEl = $(this)

    if ( $("input[name='sectionBasedTesting']:checked").val() == 1 ) {
      handleSetSectionLevel(currentEl)
    } else {
      if (!this.checked) {
        // console.log('[🐞]_________')
        // console.log('in uncheck and sectionBasedTesting is OFF')
        // console.log('_________[🐞]')
        handleSetSectionLevel(currentEl)
      } else {
        // currentpage is `Preview Online Setting`, just display alert dont have confirm.
        if (CURRENTPAGE == 'PreviewOnlineSetting') {
          let haveLock = $("input[name='sectionBasedTesting']").attr("disabled") == 'disabled'
          if (haveLock) {
            alert('Section-Based Testing is OFF and locked. Please contact Test Author for Preference modification.')
            this.checked = false
          } else {
            let r = confirm('Enabling "Set at Section Level" will turn on "Section-Based Testing". Do you want to proceed?');
            if (r == true) {
              $("input[name='sectionBasedTesting'][value='1']").prop("checked", true);
              handleSetSectionLevel(currentEl)
            } else {
              this.checked = false
              $("input[name='sectionBasedTesting'][value='0']").prop("checked", true);
            }
          }

        } else {
          let r = confirm('Enabling "Set at Section Level" will turn on "Section-Based Testing". Do you want to proceed?');
          if (r == true) {
            $("input[name='sectionBasedTesting'][value='1']").prop("checked", true);
            handleSetSectionLevel(currentEl)
          } else {
            this.checked = false
            $("input[name='sectionBasedTesting'][value='0']").prop("checked", true);
          }
        }
      }
    }

    if ( $(this).attr('id') == 'timeLimitSectionItems' ) {
      if ($(this).is(":checked")) {
        $("input[name='timeLimit'][value='0']").prop("checked", true).click()
      }
    }
  });

  $("input[name='showTimeLimitWarning']").on("change", function() {
    sectionLvlSettingsVue.show_time_limit_warning_readonly = $(this).val()
  })

  // testNavigationMethod = linear Test (value 1) && have sections
  // console.log('in testPreferencesVue.js', testNavigationMethod, testSections)
  if (testNavigationMethod == 1 && testSections.length > 1) {
    $('.sectionAvailability__wrapper').show();
    testSections.forEach((section, index) => {
        $(`.testPreferences__panel--${CURRENTPAGE}`)
        .append(
            $( `<div class="btn__setting btn__section__created with-tip" data-section-active="${section.VirtualSectionId}" title="${section.SectionName}">
                <span>${section.SectionName}</span>
                </div>` )
        )

        $('.sectionsAvailabilityList table')
        .append($(`
        <tr>
            <td style="width: 232px;">Section ${index + 1}: ${section.SectionName}</td>
            <td>
                <input type="radio" value="1" id="section_${section.VirtualSectionId}_open"
                    data-id="${section.VirtualSectionId}" name="section_${section.VirtualSectionId}_open" checked="checked" class='section--open-close'/>
                <label for="section_${section.VirtualSectionId}_open">Open</label>
                <input type="radio" value="0" id="section_${section.VirtualSectionId}_close"
                    name="section_${section.VirtualSectionId}_open" class='section--open-close'/>
                <label for="section_${section.VirtualSectionId}_close">Closed</label>
            </td>
        </tr>
        `))
    })
  } else {
    $('.cb__sectionLevel').hide()
    $('.sectionAvailability__wrapper').hide();
  }

  $('.with-tip').tip()

  let sectionAvailibilityVal = testPreferenceModel.OptionTags.filter(v => v.Key == "sectionAvailability") || []
  if (sectionAvailibilityVal.length) {
      let sectionAvailibilityOn = sectionAvailibilityVal[0].Attributes.filter(a => a.Key == 'on')[0].Value
      let sectionAvailibilityLock = sectionAvailibilityVal[0].Attributes.filter(a => a.Key == 'lock')[0].Value

      if (sectionAvailibilityOn == 1) {
          $("input[name='sectionAvailability'][value='1']").prop("checked", true);
      } else {
          $("input[name='sectionAvailability'][value='0']").prop("checked", true);
      }

      if (sectionAvailibilityLock == 'true') {
        $(`#chklockSectionAvailability`).prop("checked", true)
        if (CURRENTPAGE != 'ManageTest') {
          $("input[name='sectionAvailability']").attr('disabled','disabled')
          $('.sectionsAvailabilityList').find("input[type='radio']").attr('disabled','disabled')
        }
      } else {
        $(`#chklockSectionAvailability`).prop("checked", false)
      }
  }
  const showAvailabilitySectionList = () => {
    if ($("input[name='sectionAvailability']:checked").val() == 0) {
      $('.sectionsAvailabilityList').slideUp();
    } else {

      if ($("input[name='sectionBasedTesting']:checked").val() == 0) {
        let r = confirm('Enabling "Section Availability" will turn on "Section-Based Testing". Do you want to proceed?');
        if (r == true) {
          $("input[name='sectionBasedTesting'][value='1']").prop("checked", true);
          $('.sectionsAvailabilityList').slideDown();
          var inputOnSections = $('.sectionsAvailabilityList input.section--open-close[value="1"]');
          var sectionOpen = false;
          $.each(inputOnSections, function(index, el) {
            if ($(el).prop("checked")) {
              sectionOpen = true;
              return;
            }
          })
          if (!sectionOpen) {
            inputOnSections.prop('checked', true)
          }
        } else {
          this.checked = false
          $("input[name='sectionBasedTesting'][value='0']").prop("checked", true);
          $("input[name='sectionAvailability'][value='0']").prop("checked", true);
        }
      }
      else
        $('.sectionsAvailabilityList').slideDown();
        var inputOnSections = $('.sectionsAvailabilityList input.section--open-close[value="1"]');
        var sectionOpen = false;
        $.each(inputOnSections, function(index, el) {
          if ($(el).prop("checked")) {
            sectionOpen = true;
            return;
          }
        })
        if (!sectionOpen) {
          inputOnSections.prop('checked', true)
        }
    }
  }
  showAvailabilitySectionList()
  let sectionAvailability = testPreferenceModel.OptionTags.filter(o => o.Key == 'sectionAvailability') || [];
  if (sectionAvailability.length) {
    let sectionList = sectionAvailability[0].SectionItems;
    var list_sections = sectionLvlSettingsVue.list_sections;
    for (var i = 0; i < list_sections.length; i++){
      if (sectionList.length > i) {
        var s = sectionList[i];
        $(`input[name='section_${s.Attributes[0].Value}_open'][value='${s.Attributes[1].Value}']`).attr("checked", true);
      } else {
        var section = list_sections[i];
        $(`input[name='section_${section.VirtualSectionId}_open'][value='1']`).attr("checked", true);
      }
    }
  }

$("input[name='sectionAvailability']").change(function() {
    showAvailabilitySectionList()
  })
  $(document).on('click', `.testPreferences__panel--${CURRENTPAGE} .btn__overall`, function (e) {
    $( `.testPreferences__panel--${CURRENTPAGE} .btn__section__created`).removeClass('active__state')
    $(this).addClass('active__state')
    $('.sections__main').addClass('hide').slideUp()
    $('.overall__settings').removeClass('hide').slideDown()
  });

$(document).on('click',  `.testPreferences__panel--${CURRENTPAGE} .btn__section__created`, function (e) {
  $(`.testPreferences__panel--${CURRENTPAGE} .btn__section__created`).removeClass('active__state')
  $( `.testPreferences__panel--${CURRENTPAGE} .btn__overall`).removeClass('active__state')
  $(this).addClass('active__state')
  sectionLvlSettingsVue.active_section_id = $(this).data('section-active')
  $('.overall__settings').addClass('hide').slideUp()
  $('.sections__main').removeClass('hide').slideDown()
});

$('.section--open-close').on('change', function() {
  let sectionsOpen = testSections.filter(section =>
    $(`input=[name="section_${section.VirtualSectionId}_open"]:checked`).val() == 1
  )

  if (!sectionsOpen || sectionsOpen.length == 0) {
    alert('At least 1 section must be open.')
    $(`input[name='${this.name}'][value='1']`).prop("checked", true)
  }
})

if ($("#timeLimitSectionItems").is(":checked")) {
  let timeLimitOnChecked = $('input[name="timeLimit"]:checked').val()
  let timeLimitDurationChecked = $('input[name="TimeLimitON"]:checked')

  if (timeLimitOnChecked == 1 && timeLimitDurationChecked.attr('title') == 'Duration') {
    $('#divDuration').css({display: 'none'})
  }
}

$('input[name="timeLimit"]').on('change', function() {
  let timeLimitDeadLineChecked = $('input[name="TimeLimitON"]:checked')
  if ($(this).val() == 0) {
    $('#timeLimitSectionItems').attr("disabled", false)
  } else {
    if (timeLimitDeadLineChecked.attr('title') == 'Deadline') {
      $('#timeLimitSectionItems').attr("disabled", true)
    }
  }
})

$('input[name="TimeLimitON"]').on('change', function() {
  if ($(this).attr('title')  == 'Deadline') {
    $('#timeLimitSectionItems').attr("disabled", true)
  } else {
    $('#timeLimitSectionItems').attr("disabled", false)
  }
})

if (CURRENTPAGE == 'TestAssignment') {
  $('.chkSetAtSectionLevel').attr('disabled', 'disabled')
}

$('.validateAtSectionLvl').on('click', function() {
  let validates_passed = false
  let timeLimitSectionItems_ON = customSectionsSettting.map(s => s.option_tags).filter(s => s.timeLimitSectionItems == 1)

  if (timeLimitSectionItems_ON.length > 0) {
    let timeLimitSectionItems_ON_duration = timeLimitSectionItems_ON.filter( s => s.duration_timeLimitSectionItems > 0 )
    if (timeLimitSectionItems_ON.length == timeLimitSectionItems_ON_duration.length) {
      validates_passed = true
    } else {
      CustomAlert('Section level: Time limit must be provided.');
      $('#divContentTestSettingTestProperty').unblock();
    }
  } else {
    validates_passed = true
  }

  return validates_passed
})

$(document).off('click', '.vue-accordion-title')
$(document).on('click', '.vue-accordion-title', function() {
$(this).next().slideToggle();
});
