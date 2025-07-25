var formController = {
  elementConfig: null,
  hasNoDependent: function (item) {
    var hasDependent =
      (item.requiredParent && item.requiredParent.length > 0) || false;
    return !hasDependent;
  },
  applyDependents: function () {
    var self = this;
    if (self.elementConfig) {
      var self = this;
      $.each(self.elementConfig, function (i, item) {
        if (self.hasNoDependent(item)) {
          self.reloadDataSourceWhenParentChange(item);
        } else {
          $(item.requiredParent).change(function () {
            {
              self.reloadDataSourceWhenParentChange(item);
            }
          });
        }
      });
    }
  },
  reloadDataSource: function (id) {
    var self = this;
    self.elementConfig
      .filter(function (x) {
        return x.id === id
      })
      .forEach(function (item, i) {
        self.reloadDataSourceWhenParentChange(item);
      });
  },
  reloadDataSourceWhenParentChangeByParentId: function (parentName) {
    var self = this;
    self.elementConfig
      .filter(function (x) {
        return x.requiredParent === parentName
      })
      .forEach(function (item, i) {
        self.reloadDataSourceWhenParentChange(item);
      });
  },
  reloadDataSourceWhenParentChange: function (item) {
    var self = this;
    var inputParams = {};
    var needPopulateData = false;
    if (self.hasNoDependent(item)) {
      needPopulateData = true;
    } else {
      var currentParentValue = $(item.requiredParent).val();
      needPopulateData =
        (currentParentValue &&
          currentParentValue != -1 &&
          currentParentValue != 0) ||
        false;
    }

    if (needPopulateData && needPopulateData == true && item.params) {
      item.params.forEach(function (value, index, array) {
        var parentValue = $(value.parent).val();
        if (parentValue && parentValue != -1 && parentValue != 0) {
          inputParams[value.paramName] = parentValue;
        } else {
          needPopulateData = false;
        }
      });
      if (!needPopulateData) {
        $(item.id).empty();
      }
    }
    self.populateDropdownFields(item, inputParams, needPopulateData);
  },
  setEnableStateToSelectBySelectId: function (id) {
    var self = this;
    self.elementConfig
      .filter(function (x) { return x.id === id })
      .forEach(function (item, i) {
        self.setEnableStateToSelect(item);
      });
  },
  setEnableStateToSelect: function (dependentConfig) {
    var self = this;
    var parentID = dependentConfig.requiredParent;
    var isEnabled = false;
    if (parentID == undefined || parentID == null || parentID.length <= 0) {
      isEnabled = true;
    } else {
      if (dependentConfig.getEnableCallback) {
        isEnabled = dependentConfig.getEnableCallback();
      } else {
        var parentCurrentValue = $(parentID).val();
        isEnabled =
          (parentCurrentValue &&
            parentCurrentValue != -1 &&
            parentCurrentValue != 0) ||
          false;
      }
    }

    if (!isEnabled) {
      self.disableChildSelect(dependentConfig.id);
    }
    if (dependentConfig.useDisabledAttribute === true) {
      if (isEnabled)
        $(dependentConfig.id).removeClass("is-disabled");
      else
        $(dependentConfig.id).addClass("is-disabled");
    }
  },
  disableChildSelect: function (id) {
    var self = this;
    $(id).html("");
  },
  updateDependentStatusAfterReloadDataSource: function (selectListID) {
    var self = this;
    self.setEnableStateToSelectBySelectId(selectListID);
  },
  addSelectListItems: function (item, results) {
    var self = this;
    var selectList = $(item.id);
    var defaultValue = item.defaultValue;
    if (results.length == 0) {
      selectList.html('<option value="-1">No Results Found</option>');
    } else {
      selectList.html("");
      // if we need to set default value ~> all
      if (defaultValue.length > 0)
        selectList.append(
          $("<option></option>").attr("value", "-1").text(defaultValue)
        );
      $.each(results, function (i, value) {
        selectList.append(
          $("<option></option>").attr("value", value.id).text(value.name)
        );
      });
    }
  },
  populateDropdownFields: function (item, inputParams, needPopulateData) {
    var self = this;
    if (needPopulateData) {
      $.get(item.endPoint, inputParams, function (response) {
        var controlType = item.controlType || 'select';
        switch (controlType) {
          case 'tagit':
            self.populateDataForTagit(item, response.data);
            break;
          default:
            self.addSelectListItems(item, response.data);
        }
        self.updateDependentStatusAfterReloadDataSource(item.id);
        self.reloadDataSourceWhenParentChangeByParentId(item.id);
      });
    } else {
      self.updateDependentStatusAfterReloadDataSource(item.id);
      self.reloadDataSourceWhenParentChangeByParentId(item.id);
    }
  },
  populateDataForTagit: function (item, data) {
    var self = this;
    var id = item.id;
    item.availableTags = data.map(function (c) {
      return c.name
    });
    $(id).tagit({
      availableTags: item.availableTags,
      autocomplete: { delay: 0, minLength: 0 },
      placeholderText: item.hint,
      afterTagAdded: function () {
        var tags = $(item.id).tagit("assignedTags");
        $(item.valueId).val(tags);
        $(item.id).change();
      },
      afterTagRemoved: function () {
        var tags = $(item.id).tagit("assignedTags");
        $(item.valueId).val(tags);
        $(item.id).change();
      },
      beforeTagAdded: function (event, ui) {
        var matchTags = _.filter(item.availableTags, function (value) {
          return value.toLowerCase() == ui.tagLabel.toLowerCase()
        });
        if (matchTags.length == 0) {
          matchTags = _.filter(item.availableTags, function (value) {
            return value.toLowerCase().startsWith(ui.tagLabel.toLowerCase());
          });
        }
        if (matchTags.length == 0) {
          return false;
        }
        else {
          if (ui.tagLabel === matchTags[0]) {
            return true;
          }
          else {
            $(item.id).tagit("createTag", matchTags[0]);
            return false;
          }
        }
      }
    });
    //$(id).find(".ui-autocomplete-input").attr("readonly", true);
    $('ul.tagit input[type="text"]').css("min-width", "10px");
  },
};
