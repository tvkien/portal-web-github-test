Vue.component('select2', {
  props: ['options', 'value'],
  template: '<select><slot></slot></select>',
  ready: function () {
    var vm = this;
    $(vm.$el)
      //init select2
      .select2({ data: vm.options })
      .val(vm.value)
      .trigger('change')
      // emit event on change.
      .on('change', function (ev) {
        vm.$emit('input', +ev.target.value)
        vm.$emit('change', +ev.target.value)
      });
  },
  watch: {
    value: {
      deep: true,
      handler: function(value) {
        // update value
        $(this.$el)
          .val(value)
          .trigger('change')
      }
    },
    options: {
      deep: true,
      handler: function () {
        $(this.$el).empty().select2({ data: options });
      }
    }
  },
  destroyed: function () {
    $(this.$el).off().select2('destroy')
  }
})
