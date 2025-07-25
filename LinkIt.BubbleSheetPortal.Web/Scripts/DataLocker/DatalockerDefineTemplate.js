var DefineTemplateModel = new Vue({
    el: '.divDefineTemplate',
    data: {
      listLabelValue: [
          { Option: "", Label: "" }
      ],
      hasAssociated: true,
      hasAssociatedAutoSave: false
    },
    methods: {
      addLabelValue: function () {
          var item = {
              Option: "",
            Label: "",
            isNew: true
          }
          this.listLabelValue.push(item);
      },
      deleteLabelValue: function (index) {
        var itemLabel = this.listLabelValue.find((item, i) => {
          return index === i
        });
        if (this.hasAssociatedAutoSave) {
          if (itemLabel != null && (itemLabel.isNew === undefined || itemLabel.isNew === false)) {
            customAlertMessage({ message: 'Unable to delete Label when the template has associated auto save.' });
          } else {
            this.listLabelValue.splice(index, 1);
          }
        } else {
          this.listLabelValue.splice(index, 1);
        } 
      },
      inputValueChange: function() {
          if ($('#NumberOfDecimalPoint').val().length == 0) {
              return;
          }
          for (var i = 0; i < this.listLabelValue.length; i++) {
              if (this.listLabelValue[i].Option != "") {
                  this.listLabelValue[i].Option = formatDecimal(this.listLabelValue[i].Option);
              }
          }
      },
      formatDecimal: function(item) {
          var result = parseFloat(item);
          var numberOfDecimalPoint = parseInt($('#NumberOfDecimalPoint').val());
          result = roundN(result, numberOfDecimalPoint)
          return result;
      },
      disableInput: function() {
          this.hasAssociated = false;
      }
    }
});
