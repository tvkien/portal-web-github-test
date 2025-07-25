// LNKT-71772 Custom Numeric Dropdown Option Not Appearing in DL Enter Results
// LNKT-70009 The label <300>? is not loaded correctly from the dropdown list at Enter Result screen
(function (context) {
  if (!context.Handsontable || context.Handsontable._fixHtmlCell) return;
  var editorOpen = Handsontable.editors.AutocompleteEditor.prototype.open;
  Handsontable.editors.AutocompleteEditor.prototype.open = function () {
    this.cellProperties
    var proto = Object.getPrototypeOf(this.cellProperties);
    var clonedObj = Object.create(proto);
    this.cellProperties = Object.assign(clonedObj, this.cellProperties, { allowHtml: true });
    editorOpen.apply(this, arguments);
    var choicesListHot = this.htEditor.getInstance();
    choicesListHot.updateSettings({
      afterRenderer: function (TD, row, col, prop, value, cellProperties) {
        TD.innerText = value;
      }
    })
  }

  var dropdownRenderer = Handsontable.renderers.DropdownRenderer;
  Handsontable.renderers.DropdownRenderer = function () {
    var proto = Object.getPrototypeOf(arguments[6]);
    var clonedObj = Object.create(proto);
    arguments[6] = Object.assign(clonedObj, arguments[6], { allowHtml: false });
    dropdownRenderer.apply(this, arguments);
  }
  context.Handsontable._fixHtmlCell = true;
})(window);
