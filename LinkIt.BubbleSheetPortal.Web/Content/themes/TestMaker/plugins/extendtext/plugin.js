CKEDITOR.plugins.add('extendtext', {
  lang: 'en', // %REMOVE_LINE_CORE%
  icons: 'extendtext',
  requires: 'dialog',
  hidpi: true, // %REMOVE_LINE_CORE%
  init: function (editor) {
    var pluginName = 'insertExtendText';

    editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

    editor.ui.addButton('ExtendText', {
      label: 'Extended Text',
      command: pluginName,
      icon: this.path + 'icons/extendtext.png',
      toolbar: 'insertExtendText,30'
    });

    editor.widgets.add('extendtext', {
      inline: true,
      mask: true,
      allowedContent: {
        label: {
          classes: 'extendText',
          attributes: '!id,name,contenteditable'
        }
      },
      template: '<label class="extendText"></label>'
    });

    var isEditExtendText = false,
      currentExtendTextResId = '',
      eleExtendText = ''; //This use for first time load popup

    editor.on('doubleclick', function (evt) {
      var element = evt.data.element;

      if (element.hasClass('extentTextInteractionMark')) {
        var parents = element.getParents();
        var parent;
        for (var i = 0; i < parents.length; i++) {
          parent = parents[i];
          if (parent.hasClass('extendText')) {
            break;
          }
        }

        evt.data.dialog = pluginName;

        eleExtendText = parent;
        //The status to editor know this is update
        isEditExtendText = true;
        currentExtendTextResId = loadDataforExtendText(eleExtendText);

        dblickHandlerToolbar(editor);
      }
    });

    CKEDITOR.dialog.add(pluginName, function (editor) {
      var extendtextDialog =
        '\
                <div class="divExtendText">\
                    <div class="dialog-list dialog-list-extendtext">\
                      <div class="d-flex justify-content-between align-items-center">\
                        <div>\
                          <div class="dialog-list-item">\
                            <input type="radio" value="default" name="extendedtextgrade" id="extendedtextgrade-default" checked/>\
                            <label for="extendedtextgrade-default">Grade Manually</label>\
                          </div>\
                          <div class="dialog-list-item rubric-option">\
                            <input type="radio" value="rubricBasedGrading" name="extendedtextgrade" id="extendedtextgrade-rubricBasedGrading" />\
                            <label for="extendedtextgrade-rubricBasedGrading">Rubric Based Grading</label>\
                          </div>\
                          <div class="dialog-list-item">\
                            <input type="radio" value="ungraded" name="extendedtextgrade" id="extendedtextgrade-ungraded" />\
                            <label for="extendedtextgrade-ungraded">Ungraded</label>\
                          </div>\
                          <div class="dialog-list-item">\
                            <div class="extendedtextgrade-rubric-warning">\
                          <a href="javascript:void(0);" title="Modification of scoring or points<br/> will remove rubric from this question" class="with-tip"><img src="/Content/themes/Constellation/images/icons/fugue/exclamation-diamond.png" width="16" height="16" style="margin-right: 4px;margin-left:6px;margin-bottom:-2px"></a>\
                          </div>\
                          </div>\
                        </div>\
                        <div>\
                          <div class="point-value">Point value: <input type="text" id="point" class="point" value="1" /> <input class="text-point" type="text" value="1" /></div>\
                        </div>\
                      </div>\
                    </div>\
                    <div class="u-m-t-10">\
                        <div id="drawHoder" class="txtHoder"><textarea id="divContentPlaceholder" class="divContent" placeholder="Placeholder to display text area."></textarea></div>\
                        <div class="container-rubric hidden">\
                          <div class="category-content">\
                            <div class="container">\
                              <div class="header">\
                              <span class="icon-close-category">&#10006;</span>\
                              <span class="editable">\
                              <input placeholder="Add New Text" type="text"class="in_text-name-category"/></span> <span class="required-field">*</span> Max Points :\
                              <span class="max-point">0</span>\
                              <span class="arrow-content"><span class="arrow-expand up"></span></span> \
                              </div>\
                            <div class="content">\
                              <div class="content-tier">\
                              <div class="content-left">\
                              <input class="text-label" type="text" id="label" placeholder="Label (optional)" value="" />\
                              <div class="text-point-category"> Points <span class="required-field">*</span> : <input type="text" class="point-rubric" value="0" /></div>\
                              </div >\
                              <div class="content-right">\
                                <textarea wrap="hard" placeholder="Rubric description here (optional)"></textarea>\
                                <span class="icon-close">&#10006;</span>\
                              </div >\
                              </div>\
                            <button class="btn-tier btn-color"> Add Points</button>\
                            </div>\
                          </div>\
                          </div >\
                          <button class="btn-category btn-color">Add Category </button>\
                        </div>\
                    </div>\
                    <div class="divExtendText-options u-clearfix u-m-t-10">\
                        <div class="fleft">Maximum Allowed Characters: <input type="text" id="expertLength" class="expertLength" value="50000" /></div>\
                        <div class="fleft">Height: <input type="text" id="extendedtext-height" class="txtFullcreate u-w-90" value="90" /></div>\
                        <div class="fleft"><input type="checkbox" id="extendedtext-format" class="checkbox" checked="true" /> <label  for="extendedtext-format">Enable Text Formatting </label></div>\
                    </div>\
                </div>';
      return {
        title: 'Constructed Response Properties',
        minWidth: IS_V2 ? 640 : 500,
        width: IS_V2 ? 640 : undefined,
        minHeight: 200,
        contents: [
          {
            id: 'extendText',
            label: 'Settings',
            elements: [
              {
                type: 'html',
                html: extendtextDialog,
                onLoad: function () {
                  onLoadExtendText();
                },
                onShow: function () {
                  // Hide tooltip
                  $('#tips .tool-tip-tips').css({
                    display: 'none'
                  });
                  $('.with-tip').tip();
                  if (global.virtualQuestionRubricCount === 0 || global.isMultipart) {
                    $('.extendedtextgrade-rubric-warning').remove();
                  }
                  if (isEditExtendText) {
                    loadDataforExtendText(eleExtendText);

                    var $content = $('.divExtendText');
                    markTabIndex($content);
                  } else {
                    resetExtendTextOnload();
                  }

                  refreshResponseId();
                  // set arrIdResp null
                  checkElementRemoveIntoIResult();
                  if (global.isSurvey == 1) {
                    $(".dialog-list-extendtext").hide();
                    $('.divExtendText input[type="radio"][id="extendedtextgrade-ungraded"]').prop('checked', true)
                    onChangeOption('ungraded');
                  }
                }
              }
            ]
          }
        ],
        onOk: function () {
          var $extendtext = $('.divExtendText');
          var $extendtextPoint = $extendtext.find('.point');
          var $extendtextExpertLength = $extendtext.find('.expertLength');
          var $formatText = $extendtext.find('#extendedtext-format');
          var $placeholderText = $extendtext.find('#divContentPlaceholder')
          var extendtextMethod = $extendtext
            .find('input[type="radio"][name="extendedtextgrade"]:checked')
            .val();
          if (global.isAllowRubricGradingMode === 0) {
            global.extendtextMethod = extendtextMethod;
          }
          if (global.isAllowRubricGradingMode === 1 && global.scoringMethod === 'rubricBasedGrading') {
            global.extendtextMethod = extendtextMethod;
            global.scoringMethod = extendtextMethod;
            var message = 'Rubric base Question requires at least one Category with at least one Tier. Score for Tier must be numeric.';
            var points = $('.category-content .point-rubric');
            var $categoryName = $('.category-content .in_text-name-category');
            var requiredCate = $.grep($categoryName, function (el, index) {
              return $(el).val() === '';
            });

            $.each($categoryName, function (_, category) {
              var $category = $(category);

              $category.css('borderColor', '#ab9f9f');
            });

            if (requiredCate.length !== 0) {
              message = 'Rubric category names cannot be blank.';

              $.each(requiredCate, function (_, category) {
                var $category = $(category);

                $category.css('borderColor', 'red');
              });

              customAlert(message);
              $('.ui-dialog').css('z-index', 1000000);
              return false;
            }
            var requiredPoint = $.grep(points, function (el, index) {
              return $(el).val() === '';
            });

            $.each(points, function (_, pointInput) {
              var $pointInput = $(pointInput);

              $pointInput.css('borderColor', '#cccccc');
            });

            if (requiredPoint.length !== 0) {
              message = 'Tier Points cannot be blank.';

              $.each(requiredPoint, function (_, pointInput) {
                var $pointInput = $(pointInput);

                $pointInput.css('borderColor', 'red');
              });

              customAlert(message);
              $('.ui-dialog').css('z-index', 1000000);
              return false;
            }

            var isBreak = false;
            if (!points || points.length === 0) {
              customAlert(message);
              $('.ui-dialog').css('z-index', 1000000);
              return false;
            }
            $.each(points, function (index, point) {
              var $point = $(point);
              if (!parseFloat($point.val()) && parseFloat($point.val()) !== 0) {
                customAlert(message);
                $('.ui-dialog').css('z-index', 1000000);
                isBreak = true;
                return false;
              }
            })
            if (isBreak) return false;
            var $category = $('.category-content .container');
            $.each($category, function (index, category) {
              var $tier = $(category).find('.content-tier');
              if ($tier && $tier.length > 1) {
                $.each($tier, function (indexTier, tier) {
                  var point = $(tier).find('.point-rubric').val();
                  var item = $.grep($tier, function (element, i) {
                    return $(element).find('.point-rubric').val() === point && indexTier !== i;
                  })
                  if (item && item.length) {
                    message = 'Each rubric category must have a unique point value. Please update one of the duplicate values.'
                    customAlert(message);
                    $('.ui-dialog').css('z-index', 1000000);

                    $(category).find('.content').slideDown(200, function () {
                      $(category).find('.arrow-expand').removeClass('right');
                    });

                    $.each(item, function (_, tierItemEl) {
                      $(tierItemEl).find('.point-rubric').css('borderColor', 'red');
                    });

                    isBreak = true;
                    return false;
                  }
                })
                if (isBreak) return false
              }
            })
            if (isBreak) return false;
          }
          if (isEditExtendText) {
            // Update for current extend text
            for (n = 0; n < iResult.length; n++) {
              if (iResult[n].responseIdentifier == currentExtendTextResId) {
                iResult[
                  n
                ].responseDeclaration.pointsValue = $extendtextPoint.val();
                iResult[n].expectedLength = $extendtextExpertLength.val();
                iResult[n].formatText = $formatText.is(':checked');
                iResult[n].responseDeclaration.method = extendtextMethod;
                iResult[n].placeholderText = $placeholderText.val();
                $(eleExtendText.$).css(
                  'height',
                  $('.divExtendText #extendedtext-height').val() + 'px'
                );
                break;
              }
            }
            var placeholderText = $placeholderText.val() || 'Placeholder to display text area.';

            $(eleExtendText.$).find('.text-place-holder').text(placeholderText);
          } else {
            // Create response identify and make sure it doesn't conflict with current.
            responseId = createResponseId();
            var extendtextHeight = $(
              '.divExtendText #extendedtext-height'
            ).val();
            var extendedtextHtml =
              '<label class="extendText" id="' +
              responseId +
              '" title="' +
              responseId +
              '" contenteditable="false" style="height: ' +
              extendtextHeight +
              'px;"><span class="text-place-holder">'+  escapeHtml($placeholderText.val()) +'</span> <img class="cke_reset cke_widget_mask extentTextInteractionMark" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">&nbsp;</label>&nbsp;<br />';
            var extendedtextElement = CKEDITOR.dom.element.createFromHtml(
              extendedtextHtml
            );

            editor.insertElement(extendedtextElement);

            // Add data
            iResult.push({
              type: 'extendedTextInteraction',
              responseIdentifier: responseId,
              expectedLength: $extendtextExpertLength.val(),
              formatText: $formatText.attr('checked'),
              placeholderText: $placeholderText.val(),
              responseDeclaration: {
                baseType: 'string',
                cardinality: 'single',
                method: extendtextMethod,
                caseSensitive: 'false',
                type: 'string',
                pointsValue: $extendtextPoint.val()
              }
            });

            //Hide button on toolbar after add extended text incase qtItem is 10
            if (iSchemeID == '10') {
              $('.cke_button__extendtext')
                .parents('span.cke_toolbar')
                .hide();
            }

            ResIdElemModul = responseId;
          }

          // Reset to default after update or create new textEntry
          isEditExtendText = false;
          currentExtendTextResId = '';
          // ResIdElemModul = responseId;
          if (global.isAllowRubricGradingMode === 1 && global.scoringMethod === 'rubricBasedGrading') {
            var rubrics = global.rubricQuestionCategories || [];
            var $category = $('.category-content .container');
            $.each($category, function (index, el) {
              var textCategory = $(el).find('.in_text-name-category').val()
              var $tiers = $(el).find('.content .content-tier');
              var RubricCategoryTiers = []
              if (!(rubrics && rubrics.length) || index >= rubrics.length) {
                var rubric = {
                  VirtualQuestionID: null,
                  RubricQuestionCategoryID: null,
                  CategoryName: textCategory,
                  CategoryCode: null,
                  OrderNumber: index,
                  PointEarn: null
                };
                $.each($tiers, function (indexTier, $tier) {
                  var label = $($tier).find('.text-label').val();
                  var point = $($tier).find('.point-rubric').val();
                  var desc = $($tier).find('textarea').val();
                  var tier = {
                    RubricCategoryTierID: null,
                    RubricQuestionCategoryID: null,
                    Point: parseFloat(point),
                    Label: label,
                    Description: desc,
                    OrderNumber: indexTier,
                    Selected: true,
                    PointEarn: null
                  }
                  RubricCategoryTiers.push(tier);
                })
                rubric.RubricCategoryTiers = RubricCategoryTiers;
                rubrics.push(rubric);
              } else {
                rubrics[index].CategoryName = textCategory;
                RubricCategoryTiers = rubrics[index].RubricCategoryTiers;
                $.each($tiers, function (indexTier, $tier) {
                  var label = $($tier).find('.text-label').val();
                  var point = $($tier).find('.point-rubric').val();
                  var desc = $($tier).find('textarea').val();
                  if (!(RubricCategoryTiers && RubricCategoryTiers.length) || indexTier >= RubricCategoryTiers.length) {
                    var tier = {
                      RubricCategoryTierID: null,
                      RubricQuestionCategoryID: null,
                      Point: parseFloat(point),
                      Label: label,
                      Description: desc,
                      OrderNumber: indexTier,
                      Selected: true,
                      PointEarn: null
                    }
                    RubricCategoryTiers.push(tier);
                  } else {
                    RubricCategoryTiers[indexTier].Description = desc;
                    RubricCategoryTiers[indexTier].Point = parseFloat(point);
                    RubricCategoryTiers[indexTier].Label = label;
                  }
                });
                rubrics[index].OrderNumber = index;
                RubricCategoryTiers = RubricCategoryTiers.slice(0, $tiers.length);
                rubrics[index].RubricCategoryTiers = RubricCategoryTiers;
              }
            })
            rubrics = rubrics.slice(0, $category.length);

            global.rubricQuestionCategories = rubrics;
          }

          newResult = iResult;
        },
        onCancel: function () {
          // Reset to default after update or create new textEntry
          isEditExtendText = false;
          currentExtendTextResId = '';
          global.scoringMethod = global.extendtextMethodPrevious;
          global.extendtextMethod = global.extendtextMethodPrevious;
          onChangeOption(global.extendtextMethod);
        }
      };
    });
  }
});

function markTabIndex($content) {
  var $elements = $content.find('.in_text-name-category, .text-label, .point-rubric, textarea, .expertLength, .txtFullcreate');
  var $okButton = $('.cke_dialog_ui_button_ok');
  var $cancelButton = $('.cke_dialog_ui_button_cancel');
  var initialIndex = 1000;
  var lastIndex = initialIndex + $elements.length;

  $.each($elements, function (index, elItem) {
    var $element = $(elItem);
    $(elItem).attr('tabIndex', initialIndex + index);

    $element.keydown(function (event) {
      var keycode = event.which || event.keyCode;
      var KEYCODE_TAB = 9;

      if (keycode === KEYCODE_TAB) {
        event.stopPropagation();
        event.preventDefault();

        var currentIndex = $(this).attr('tabindex');
        var nextIndex = currentIndex + 1;

        $('[tabindex="' + nextIndex + '"]').focus();
      }

      if (event.shiftKey && keycode === KEYCODE_TAB) {
        event.stopPropagation();
        event.preventDefault();

        var currentIndex = $(this).attr('tabindex');
        var prevIndex = currentIndex - 1;

        $('[tabindex="' + prevIndex + '"]').focus();
      }
    });
  });

  var okButtonIndex = lastIndex;
  var cancelButtonIndex = okButtonIndex + 1;

  $okButton.attr('tabindex', okButtonIndex);
  $cancelButton.attr('tabindex', cancelButtonIndex);
};

function unescapeHtml(html) {
  return html
      .replace(/&amp;/g, "&")
      .replace(/&lt;/g, "<")
      .replace(/&gt;/g, ">")
      .replace(/&quot;/g, '"')
      .replace(/&#039;/g, "'");
}

/***
 * Load data to Draw Tool popup form
 * Return: responseIdentifier
 ***/
function loadDataforExtendText(element) {
  var currentExtendTextResId = '';
  var $extendtext = $('.divExtendText');
  var $extendtextPoint = $extendtext.find('.point');
  var $extendtextLength = $extendtext.find('.expertLength');
  var $extendtextHeight = $extendtext.find('#extendedtext-height');
  $extendtextPoint.parent().removeClass('is-disabled');
  var $formatText = $extendtext.find('#extendedtext-format');
  var $placeholderText = $extendtext.find('#divContentPlaceholder');
  for (var i = 0, iResultLength = iResult.length; i < iResultLength; i++) {
    var iResultItem = iResult[i];
    if (
      iResultItem.responseIdentifier == element.getId() &&
      iResultItem.type == 'extendedTextInteraction'
    ) {
      currentExtendTextResId = iResultItem.responseIdentifier;
      // Load data
      $extendtextPoint.val(iResultItem.responseDeclaration.pointsValue);
      $extendtextLength.val(iResultItem.expectedLength);
      $formatText.attr('checked', iResultItem.formatText);
      $placeholderText.val(unescapeHtml(iResultItem.placeholderText || ''));
      // Prefer height css than client height
      if (element.$.style.height) {
        $extendtextHeight.val(element.$.style.height.replace('px', ''));
      } else {
        $extendtextHeight.val($(element.$).outerHeight());
      }
      if (iResultItem.responseDeclaration.method === 'ungraded' && (global.rubricQuestionCategories || 0).length < 1) {
        $('#extendedtextgrade-ungraded').prop('checked', true);
        $extendtextPoint.parent().addClass('is-disabled');
        $extendtextPoint.val('0');
        global.extendtextMethodPrevious = 'ungraded';
      } else if (global.isAllowRubricGradingMode === 1 && global.scoringMethod === 'rubricBasedGrading' && (global.rubricQuestionCategories || 0).length > 0) {
        var rubrics = global.rubricQuestionCategories || []
        global.extendtextMethodPrevious = 'rubricBasedGrading';
        var htmlContent = '';
        var totalPoint = 0;
        for (var rubric = 0; rubric < rubrics.length; rubric++) {
          var $content = $(contentCategory);
          var categories = rubrics[rubric];
          $content.find('.in_text-name-category').attr('value', categories.CategoryName);
          var categoryTies = categories.RubricCategoryTiers;
          var htmlContentTier = ''
          var maxPoint = 0;
          for (var tier = 0; tier < categoryTies.length; tier++) {
            var $contentTier = $(contentTier);
            $contentTier.find('.text-label').attr('value', categoryTies[tier].Label);
            $contentTier.find('.point-rubric').attr('value', categoryTies[tier].Point);
            $contentTier.find('textarea').text(categoryTies[tier].Description);
            htmlContentTier += $contentTier[0].outerHTML;
            if (maxPoint < categoryTies[tier].Point) {
              maxPoint = categoryTies[tier].Point
            }
          }
          totalPoint += parseFloat(maxPoint);
          $content.find('.max-point').text(parseFloat(maxPoint));
          $content.find('.content-tier')[0].outerHTML = htmlContentTier;
          htmlContent += $content.html();
        }
        $('.point-value .text-point').attr('value', parseFloat(totalPoint));
        $extendtextPoint.val(parseFloat(totalPoint));
        $('.container-rubric .category-content').html(htmlContent);
        getUpDownNumber($extendtext.find('.point-rubric'), 0, 1000);
        onChangeOption('rubricBasedGrading');
        if (rubrics.length > 0) {
          $('#extendedtextgrade-rubricBasedGrading').prop('checked', true);
        }
      }
      else {
        if (global.extendtextMethod === 'ungraded') {
          $('#extendedtextgrade-ungraded').prop('checked', true);
          $('.point-value .text-point').attr('value', 0);
          global.extendtextMethodPrevious = 'ungraded';
          onChangeOption(global.extendtextMethod);
        } else {
          $('#extendedtextgrade-default').prop('checked', true);
          global.extendtextMethodPrevious = 'default';
          onChangeOption(global.extendtextMethod);
        }
      }
      break;
    }
  }

  return currentExtendTextResId;
}

/**
 * Reset data to extend text popup form
 * @return {[type]} [description]
 */
function resetExtendTextOnload() {
  var $extendtext = $('.divExtendText');
  $extendtext.find('#expertLength').val('50000');
  $extendtext
    .find('#point')
    .parent()
    .removeClass('is-disabled');
  $extendtext.find('#point').val('1');
  $extendtext.find('#extendedtextgrade-default').prop('checked', true);
  $extendtext.find('#extendedtext-height').val('90');
  $extendtext.find('#extendedtext-format').attr('checked', true)
  var $placeholderText = $extendtext.find('#divContentPlaceholder');
  $placeholderText.val('');
}

/**
 * Onload extended text
 * @return {[type]} [description]
 */
function getUpDownNumber(selector, min, max) {
  var $selector = $(selector);

  $selector.ckUpDownNumber({
    minNumber: min,
    maxNumber: max,
    width: 18,
    height: 13
  });
}

var contentCategory =
  '<div class="category-content">\
      <div class="container">\
        <div class="header">\
        <span class="icon-close-category">&#10006;</span>\
        <span class="editable">\
        <input type="text" placeholder="Add New Text" class="in_text-name-category"/></span> <span class="required-field">*</span> Max Points :\
        <span class="max-point">0</span>\
        <span class="arrow-content"><span class="arrow-expand up"></span></span> \
        </div>\
      <div class="content">\
        <div class="content-tier">\
        <div class="content-left">\
        <input class="text-label" type="text"  placeholder="Label (optional)" value="" />\
        <div class="text-point-category"> Points <span class="required-field">*</span> : <input type="text"  class="point-rubric" value="0" /></div>\
        </div >\
        <div class="content-right">\
          <textarea wrap="hard" placeholder="Rubric description here (optional)"></textarea>\
          <span class="icon-close">&#10006;</span>\
        </div >\
        </div>\
      <button class="btn-tier btn-color"> Add Points</button>\
      </div>\
    </div>';
var contentTier =
  ' <div class="content-tier">\
      <div class="content-left">\
      <input class="text-label" type="text"  placeholder="Label (optional)" value="" />\
      <div class="text-point-category"> Points <span class="required-field">*</span> : <input type="text"  class="point-rubric" value="0" /></div>\
      </div >\
      <div class="content-right">\
        <textarea wrap="hard" placeholder="Rubric description here (optional)"></textarea>\
        <span class="icon-close">&#10006;</span>\
      </div >\
      </div>';

function onChangeOption(extendtextgradeVal) {
  var $extendtext = $('.divExtendText');
  var textPoint = $extendtext.find('.text-point');

  switch (extendtextgradeVal) {
    case 'ungraded':
      $extendtext
        .find('.point')
        .parent()
        .addClass('is-disabled');
      textPoint.val('0');
      $extendtext.find('.point').val('0');
      textPoint.removeClass('is-show');
      textPoint.parents('.point-value').removeClass('is-show');

      $extendtext.find('#drawHoder').removeClass('hidden');
      $extendtext.find('.container-rubric').addClass('hidden');
      textPoint.attr('readonly', false);
      textPoint.parent().removeClass('is-show');
      textPoint.parents('.ckUpDownNumber').hide();
      global.scoringMethod = 'ungraded';
      global.extendtextMethod = 'ungraded';
      break;
    case 'rubricBasedGrading':
      textPoint.addClass('is-show');
      textPoint.parent().addClass('is-show')
      textPoint.parents('.point-value').addClass('is-show');
      $extendtext.find('#drawHoder').addClass('hidden');
      $extendtext.find('.container-rubric').removeClass('hidden');
      textPoint.attr('readonly', true);
      textPoint.parents('.ckUpDownNumber').show();
      global.scoringMethod = 'rubricBasedGrading';
      global.extendtextMethod = 'rubricBasedGrading';
      break;
    default:
      $extendtext
        .find('.point')
        .parent()
        .removeClass('is-disabled');
      textPoint.removeClass('is-show');
      textPoint.parents('.point-value').removeClass('is-show');
      $extendtext.find('#drawHoder').removeClass('hidden');
      $extendtext.find('.container-rubric').addClass('hidden');
      textPoint.attr('readonly', false);
      textPoint.parent().removeClass('is-show');
      global.scoringMethod = 'default';
      global.extendtextMethod = 'default';
      textPoint.parents('.ckUpDownNumber').hide();
      break;
  }
}
function onLoadExtendText() {
  var $extendtext = $('.divExtendText');

  // Initialize up and down number
  getUpDownNumber($extendtext.find('.point'), 0, 1000);
  getUpDownNumber($extendtext.find('.expertLength'), 1, 100000);
  getUpDownNumber($extendtext.find('#extendedtext-height'), 30, 300);
  getUpDownNumber($extendtext.find('.point-rubric'), 0, 1000);
  getUpDownNumber($extendtext.find('.text-point'), 0, 1000);
  if (global.isMultipart || global.isAllowRubricGradingMode === 0) {
    $('.dialog-list-extendtext .dialog-list-item.rubric-option').hide();
  }

  if (global.scoringMethod === 'rubricBasedGrading') {
    onChangeOption('rubricBasedGrading')
  } else {
    var extendtextgradeVal = $(
      '.dialog-list-extendtext input[type="radio"][name="extendedtextgrade"]:checked'
    ).val();
    onChangeOption(extendtextgradeVal);
  }

  var updatePoints = function ($categories) {
    var totalPoint = 0;

    $.each($categories, function (_, category) {
      var maxPoint = 0;
      var $category = $(category);
      var $point = $category.find('.point-rubric');

      $.each($point, function (_, pointValue) {
        var point = parseFloat($(pointValue).val());

        if (maxPoint < point) {
          maxPoint = point
        }
      })

      $category.find('.max-point').text(maxPoint);
      totalPoint += parseFloat(maxPoint);
    })
    var $extendtext = $('.divExtendText');
    var $extendtextPoint = $extendtext.find('.point');
    $('.point-value .text-point').attr('value', parseFloat(totalPoint));
    $extendtext.find('#extendedtext-format').attr('checked', true)
    $extendtextPoint.val(parseFloat(totalPoint));
  };

  var informRubricBasedGrading = function () {
    var $content = $('.divExtendText');
    var $categories = $('.container-rubric .category-content .container');

    // Remove category if last tier of them is removed
    var $copiedCategories = $categories;
    $.each($copiedCategories, function (index, categogy) {
      var $categogy = $(categogy);
      var $tierList = $categogy.find('.content-tier');

      if ($tierList.length <= 0) {
        $categogy.remove();
        $categories.splice(index, 1);
      }
    });

    // Hide Closed Icon of category when we have only category on board
    if ($categories.length <= 1) {
      $categories.find('.icon-close-category').hide();

      var $tierList = $categories.find('.content-tier');
      if ($tierList.length <= 1) {
        $tierList.find('.icon-close').hide();
      } else {
        $tierList.find('.icon-close').show();
      }
    } else {
      $categories.find('.icon-close-category').show();

      var $tierList = $categories.find('.content-tier');
      $tierList.find('.icon-close').show();
    }

    updatePoints($categories);
    markTabIndex($content);
  };

  // Initialize event change extend text grade
  $('.dialog-list-extendtext input[type="radio"][name="extendedtextgrade"]').on(
    'change',
    function () {
      var extendtextgradeVal = $(
        '.dialog-list-extendtext input[type="radio"][name="extendedtextgrade"]:checked'
      ).val();
      onChangeOption(extendtextgradeVal);
      if (global.scoringMethod === 'rubricBasedGrading') {
        informRubricBasedGrading();
      }
    }
  );
  $(document).on('click', '.category-content .arrow-content', function () {
    var $header = $(this);
    var $content = $header.parent().next();
    $content.slideToggle(200, function () {
      $header.find('.arrow-expand').toggleClass('right');
    });
  });
  $(document).on('click', '.category-content .btn-tier', function () {
    var $el = $(this);
    $el.before(contentTier);
    var $extendtext = $('.divExtendText');
    getUpDownNumber($extendtext.find('.point-rubric'), 0, 1000);

    informRubricBasedGrading();
  });
  $(document).on('click', '.container-rubric .btn-category', function () {
    $('.container-rubric .category-content').append($(contentCategory).html());
    var $extendtext = $('.divExtendText');
    getUpDownNumber($extendtext.find('.point-rubric'), 0, 1000);

    informRubricBasedGrading();
  })
  $(document).on('click', '.category-content .icon-close', function () {
    var $el = $(this);
    $el.parents('.content-tier').remove();

    informRubricBasedGrading();
  });
  $(document).on('click', '.category-content .icon-close-category', function () {
    var $el = $(this);
    $el.parents('.container').remove();

    informRubricBasedGrading();
  });
  $(document).on('change', '.category-content .point-rubric', function () {
    informRubricBasedGrading();
  });
}
