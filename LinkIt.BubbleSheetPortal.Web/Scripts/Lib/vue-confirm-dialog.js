var ConfirmModalComponent = Vue.extend({
  template: '\
        <div class="modal-component" v-show="compValue" transition="modal-component">\
            <div class="modal-component-wrapper">\
                <div class="modal-component-container" v-bind:style="styleModal">\
                    <div class="modal-component-content">\
                        <div class="modal-component-body">\
                            <slot name="body">\
                                {{message}}\
                            </slot>\
                        </div>\
                        <div class="modal-component-footer" style="text-align: center;">\
                            <button class="grey" @click="compValue = false">No</button>\
                            <button @click="[$emit(\'onYesClick\'),compValue = false]">Yes</button>\
                        </div>\
                    </div>\
                </div>\
            </div>\
        </div>',
  props: {
    value: {
      type: Boolean,
      default: false
    },
    width: {
      type: Number,
      default: 300
    },
    message: {
      type: String,
      default: ''
    }
  },
  data: function () {
    return {
      styleModal: {}
    }
  },
  computed: {
    compValue: {
      get: function () {
        return this.value;
      },
      set: function (val) {
        this.$emit('input', val);
      }
    }
  },
  created: function () {
    if (!!this.width) {
      this.styleModal = {
        width: this.width + 'px'
      };
    }
  },
  mounted: function () {
    var self = this;
    document.addEventListener('keydown', function (e) {
      if (self.show && e.keyCode == 27) {
        self.compValue = false;
      }
    });
  },
});

Vue.component('confirm-modal', ConfirmModalComponent)
