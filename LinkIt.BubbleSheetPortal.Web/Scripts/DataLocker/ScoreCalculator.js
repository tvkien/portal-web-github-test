/**
 * Handles logic for the  Score Calculator component
 */
var ScoreCalculator = (function () {
  var CalculationType = {
    ARITHMETIC_TAG: 'arithmetic-tag',
    SCORE_COLUMN_TAG: 'score-column-tag',
    OVERALL_SCORE_TAG: 'overall-score-tag',
    MATH_SYMBOL_TAG: 'math-symbol-tag',
    SUB_SCORE_TAG: 'sub-score-tag',
    MULTIPLICATION_TAG: 'x',
    DIVISION_TAG: '÷'
  };

  // Global variables to store score data
  var overalScoreOriginal;
  var subScoreOriginal;
  var subscoreId;
  var isDisabled = false;
  var initialTags = "";
  var action = "create";
  var customScoreName = "";
  var subScoreId = 0;
  var currentScoreType = "";
  var allScoreColumns = [];

  // Initialize the calculator
  function init(options) {
    console.log("Init calculation =>", options);
    // Store parameters as global variables within the module
    overalScoreOriginal = options.overallScore;
    subscoreId = options.subScoreId;
    currentScoreType = options.currentScoreType;
    isDisabled = options.disabled;
    initialTags = options.initialTags;
    customScoreName = options.customScoreName;
    action = options.action;
    subScoreOriginal = sortObject(options.subScore);
    subScoreId = options.subScoreId;
    mappingAllScore();

    if (action === "edit") {
      // Transform Overall Score 
      overalScoreOriginal = overalScoreOriginal.filter(function (item) {
        return item.name !== customScoreName;
      });

      // Transform Subscore
      for (var key in subScoreOriginal) {
        if (subScoreOriginal.hasOwnProperty(key)) {
          subScoreOriginal[key] = subScoreOriginal[key].filter(function (item) {
            if (key.indexOf(subScoreId) !== -1) {
              return item.name !== customScoreName;
            }

            return true;
          });
        }
      }
    }

    overalScoreOriginal && renderOveralScore();
    subScoreOriginal && renderSubScore();

    initializeTagEditor();
    bindEvents();
    renderDefaultScore();

    if (isDisabled) {
      $('.score-column-btn').attr('disabled', 'disabled');
      $('.symbol').attr('disabled', 'disabled');
      $('.symbol').addClass('disabled');
      $('#divScoreCalculator .tag-editor').addClass('d-none');
      $('#scoreCalculationReadonly').removeClass('d-none');
    } else {
      $('#divScoreCalculator .tag-editor').removeClass('d-none');
      $('#scoreCalculationReadonly').addClass('d-none');
    }

    // Disable keypress for score calculation tag editor
    $("#scoreCalculation .tag-editor").keypress(function (e) {
      return false;
    });

    checkSymbolsPresence();
  }

  function renderReadOnlyScoreCalculation(tags) {
    $('#scoreCalculationReadonly').empty();
    for (var i = 0; i < tags.length; i++) {
      var tag = tags[i];
      $('#scoreCalculationReadonly').append('<div class="' + tag.className + '">' + tag.name + '</div>');
    }
  }

  // Initialize the score calculation tag editor
  function initializeTagEditor() {
    $('#scoreCalculation').tagEditor({
      animateDelete: 0,
      maxLength: 200,
      forceLowercase: false,
      removeDuplicates: false,
      visibleIconDelete: isDisabled ? false : true,
      allowUserTags: false,
      readOnly: true,
      allowNumber: true,
      placeholder: 'Enter calculation formula...',
      delimiter: '___',
      onChange: function (field, editor, tags, tag, val) {
        checkSymbolsPresence();
      }
    });
  }

  // Bind events to elements
  function bindEvents() {
    // Symbol button click handlers
    $('.arithmetic').on('click', function (e) {
      e.preventDefault();
      var symbolText = $(this).text();
      $('#scoreCalculation').tagEditor('addValue', symbolText, false, CalculationType.ARITHMETIC_TAG);
    });

    $('.math-symbol').on('click', function (e) {
      e.preventDefault();
      var symbolText = $(this).text();
      $('#scoreCalculation').tagEditor('addTag', symbolText, false, CalculationType.MATH_SYMBOL_TAG);
    });

    // Add click handler for math symbol buttons (Highest, Lowest, Average)
    $('.symbol-btn').on('click', function (e) {
      e.preventDefault();
      var tags = $('#scoreCalculation').tagEditor('getTags')[0].tags;
      // Remove all tags
      for (var i = 0; i < tags.length; i++) {
        $('#scoreCalculation').tagEditor('removeTag', tags[i]);
      }
      // Add new tags
      var symbolText = $(this).text();
      $('#scoreCalculation').tagEditor('addTag', symbolText, false, CalculationType.MATH_SYMBOL_TAG); 
      $('#scoreCalculation').tagEditor('addTag', '(', false, CalculationType.MATH_SYMBOL_TAG);
      $('#scoreCalculation').tagEditor('addTag', ')', false, CalculationType.MATH_SYMBOL_TAG);
    });

    

    // Add click handler for score column buttons
    $('#divOveralScore .score-column-btn').on('click', function (e) {
      e.preventDefault();
      addTagScoreColumn($(this).text(), CalculationType.SCORE_COLUMN_TAG + ' ' + CalculationType.OVERALL_SCORE_TAG);
    });

    $('#divSubScore .score-column-btn').on('click', function (e) {
      e.preventDefault();
      var groupName = $(this).attr('group-name');
      var value = $(this).text().trim();

      addTagScoreColumn(groupName + ' (' + value + ')', CalculationType.SCORE_COLUMN_TAG + ' ' + CalculationType.SUB_SCORE_TAG);

    });
  }

  // Handle click on score column button
  function addTagScoreColumn(columnName, className) {

    var tags = $('#scoreCalculation').tagEditor('getTags')[0].tags;

    if (isMathSymbolOption() && tags.length > 1) {
      // Remove the last tag is ")"
      $('#scoreCalculation').tagEditor('removeTag', tags[tags.length - 1]);
      // Add the new tag
      $('#scoreCalculation').tagEditor('addTag', columnName, false, className);
      $('#scoreCalculation').tagEditor('addTag', ')', false, 'arithmetic-tag');
    } else {
      $('#scoreCalculation').tagEditor('addTag', columnName, false, className);
    }
  }

  // Check if Math Symbol exists in the tags array
  function isMathSymbolOption() {
    var calculationEditorTag = $('#scoreCalculation').tagEditor('getTags')[0];
    const calculationHtml = calculationEditorTag.editor.html();
    const parser = new DOMParser();
    const doc = parser.parseFromString(`<ul>${calculationHtml}</ul>`, 'text/html');


    const mathSymbolEditorTag = doc.querySelector(`.${CalculationType.MATH_SYMBOL_TAG}`);
    if (!mathSymbolEditorTag) {
      return false;
    }

    const mathSymbolText = mathSymbolEditorTag.textContent.trim();
    if (mathSymbolText === 'Highest' || mathSymbolText === 'Lowest' || mathSymbolText === 'Average') {
      return true;
    }

    return false;
  }

  function isOverallScoreTag(tag) {
    return /^0&/.test(tag);
  }

  function isSubScoreTag(tag) {
    return /^[0-9]+&/.test(tag);
  }

  // Check for Math Symbol in tags and adjust button state
  function checkSymbolsPresence() {
    // Check if Math Symbol exists in the tags array
    var hasMathSymbolOption = isMathSymbolOption();

    // Add or remove disabled attribute to symbol buttons based on condition
    if (hasMathSymbolOption) {
      $('.wrap-symbols .symbol').attr('disabled', 'disabled');
    } else {
      $('.wrap-symbols .symbol').removeAttr('disabled');
    }
  }

  // Build selected column for Overall Score
  function renderOveralScore() {
    $('#divOveralScore').empty();
    $('#divOveralScore').append('<h5 class="w-100">Overall Score</h5>');
    for (var i = 0; i < overalScoreOriginal.length; i++) {
      var item = overalScoreOriginal[i];
      if (action === "edit") {
        var exitUsingScoreType = exitsRelatedCalculation(item.expressionKey, item.expressionValue);
        if (!exitUsingScoreType) {
          // Ignore the item if it has related calculation (using in the calculation)
          $('#divOveralScore').append('<button class="score-column-btn" data-column="' + item.column + '"><i class="fa-solid fa-plus ms-2"></i> ' + item.name + '</button>');
        }
      } else {
        $('#divOveralScore').append('<button class="score-column-btn" data-column="' + item.column + '"><i class="fa-solid fa-plus ms-2"></i> ' + item.name + '</button>');
      }
    }
  }

  function sortObject(obj) {
    return Object.keys(obj).sort().reduce(function (a, v) {
      a[v] = obj[v];
      return a;
    }, {});
  }

  // Render sub score buttons
  function renderSubScore() {
    if (typeof subScoreOriginal === 'undefined') {
      return;
    }

    for (var subScore in subScoreOriginal) {
      if (subScoreOriginal.hasOwnProperty(subScore)) {
        if (subScore.length > 0) {
          var subScoreId = subScore.split('_')[1];
          var wrap = $('<div class="divSubScore columns mb-2" id="divSubScoreRadio_' + subScoreId + '"></div>')
          var titleHeader = $('#' + subScore).val();
          var header = $('<div class="mb-2"><h5>' + titleHeader + '</h5></div>')
          var body = $('<div class="d-flex flex-wrap gap-2"></div>')
          for (var i = 0; i < subScoreOriginal[subScore].length; i++) {
            var item = subScoreOriginal[subScore][i];
            if (action === "edit") {
              // Ignore the item if it has related calculation (using in the calculation)
              var exitUsingScoreType = exitsRelatedCalculation(item.expressionKey, item.expressionValue);
              if (!exitUsingScoreType) {
                body.append('<button class="score-column-btn" data-column="' + item.column + '" group-name="' + titleHeader + '"><i class="fa-solid fa-plus ms-2"></i> ' + item.name + '</button>');
              }
            } else {
              body.append('<button class="score-column-btn" data-column="' + item.column + '" group-name="' + titleHeader + '"><i class="fa-solid fa-plus ms-2"></i> ' + item.name + '</button>');
            }

          }
          wrap.append(header);
          wrap.append(body);
          $('#divSubScore').append(wrap);
        }
      }
    }
  }

  // val is a string of calculation formula
  function transfromHighestOrLowestToTag(val) {
    var result = [];

    if (val.indexOf("Math.max") !== -1) {
      result.push({
        type: "arithmetic-tag",
        value: "Highest"
      });
    } else if (val.indexOf("Math.min") !== -1) {
      result.push({
        type: "arithmetic-tag",
        value: "Lowest"
      });
    } else if (val.indexOf("Math.mean") !== -1) {
      result.push({
        type: "arithmetic-tag",
        value: "Average"
      });
    }

    result.push({
      type: CalculationType.MATH_SYMBOL_TAG,
      value: "("
    });

    var match = val.match(/\(([^)]+)\)/);
    if (match) {
      var items = match[1].split(",");

      for (var i = 0; i < items.length; i++) {
        var item = items[i].trim();

        if (isOverallScoreTag(item)) {
          result.push({
            type: CalculationType.OVERALL_SCORE_TAG,
            value: item.split("&")[1]
          });
        } else if (isSubScoreTag(item)) {
          var parts = item.split("&");
          result.push({
            id: parts[0],
            type: CalculationType.SUB_SCORE_TAG,
            value: parts[1]
          });
        }
      }
    }
    result.push({
      type: CalculationType.MATH_SYMBOL_TAG,
      value: ")"
    });

    return result;
  }

  function transformStringToTag(val) {
    var result = [];

    var tokens = val.split(/\s+/);

    for (var i = 0; i < tokens.length; i++) {
      var token = tokens[i].trim();

      if (isOverallScoreTag(token)) {
        result.push({
          type: CalculationType.OVERALL_SCORE_TAG,
          value: token.split("&")[1]
        });
      } else if (isSubScoreTag(token)) {
        var parts = token.split("&");
        result.push({
          id: parts[0],
          type: CalculationType.SUB_SCORE_TAG,
          value: parts[1]
        });
      } else if (token === "*") {
        result.push({
          type: CalculationType.MATH_SYMBOL_TAG,
          value: 'x'
        });
      } else if (token === "/") {
        result.push({
          type: CalculationType.MATH_SYMBOL_TAG,
          value: '÷'
        });
      } else {
        result.push({
          type: CalculationType.MATH_SYMBOL_TAG,
          value: token
        });
      }
    }
    return result;
  }

  function renderDefaultScore() {

    if (initialTags) {
      var result = [];
      if (initialTags.indexOf("Math.max") !== -1 || initialTags.indexOf("Math.min") !== -1 || initialTags.indexOf("Math.mean") !== -1) {
        result = transfromHighestOrLowestToTag(initialTags);
      } else {
        result = transformStringToTag(initialTags);
      }

      var tagsToAdd = [];
      for (var i = 0; i < result.length; i++) {
        if (result[i].type === CalculationType.OVERALL_SCORE_TAG) {
          var name = '';
          for (var j = 0; j < overalScoreOriginal.length; j++) {
            if (overalScoreOriginal[j].column === result[i].value) {
              name = overalScoreOriginal[j].name;
              break;
            }
          }
          tagsToAdd.push({
            name: name,
            className: CalculationType.SCORE_COLUMN_TAG + ' ' + CalculationType.OVERALL_SCORE_TAG
          });
        } else if (result[i].type === CalculationType.SUB_SCORE_TAG) {
          var name = '';
          var subscoreKey = 'SubscoreName_' + result[i].id;

          if (subScoreOriginal && subScoreOriginal[subscoreKey]) {
            for (var j = 0; j < subScoreOriginal[subscoreKey].length; j++) {
              if (subScoreOriginal[subscoreKey][j].column === result[i].value) {
                var titleHeader = $('#SubscoreName_' + result[i].id).val();
                name = titleHeader + ' (' + subScoreOriginal[subscoreKey][j].name + ')';
                break;
              }
            }
          }

          tagsToAdd.push({
            name: name,
            className: CalculationType.SCORE_COLUMN_TAG + ' ' + CalculationType.SUB_SCORE_TAG
          });
        } else {
          tagsToAdd.push({
            name: result[i].value,
            className: CalculationType.MATH_SYMBOL_TAG
          });
        }
      }

      // Add all tags in sequence
      tagsToAdd.forEach(function (tag) {
        $('#scoreCalculation').tagEditor('addTag', tag.name, false, tag.className);
      });

      isDisabled && renderReadOnlyScoreCalculation(tagsToAdd);
    }
  }

  // Transform overall score for calculation data
  function transformOverallScore(name) {
    for (var i = 0; i < overalScoreOriginal.length; i++) {
      if (overalScoreOriginal[i].name === name) {
        return "0&" + overalScoreOriginal[i].column;
      }
    }
    return name;
  }

  // Transform subscore for calculation data
  function transformSubScore(name) {
    for (var key in subScoreOriginal) {
      if (subScoreOriginal.hasOwnProperty(key)) {
        var titleHeader = $('#' + key).val();
        var arr = subScoreOriginal[key];
        for (var i = 0; i < arr.length; i++) {
          var itemName = titleHeader + ' (' + arr[i].name + ')';
          if (itemName === name) {
            var parts = key.split("_");
            var num = parts[1];
            return num + "&" + arr[i].column;
          }
        }
      }
    }
    return name;
  }

  // Transform calculation data for submission
  function transformCalculationData() {
    var calculationEditorTag = $('#scoreCalculation').tagEditor('getTags')[0];
    const calculationHtml = calculationEditorTag.editor.html();


    const parser = new DOMParser();
    const doc = parser.parseFromString(`<ul>${calculationHtml}</ul>`, 'text/html');

    // Calculation selected Data
    var overalScoreSelectedData = [];
    var subScoreSelectedData = [];

    const overalScoreEditorTags = doc.querySelectorAll(`.${CalculationType.OVERALL_SCORE_TAG}`);
    const subScoreEditorTags = doc.querySelectorAll(`.${CalculationType.SUB_SCORE_TAG}`);

    if (overalScoreEditorTags.length > 0) {
      overalScoreEditorTags.forEach((tag, index) => {
        const value = tag.textContent.trim();
        overalScoreSelectedData.push(value);
      });
    }

    if (subScoreEditorTags.length > 0) {
      subScoreEditorTags.forEach((tag, index) => {
        const value = tag.textContent.trim();
        subScoreSelectedData.push(value);
      });
    }


    var container = document.createElement('div');
    container.innerHTML = calculationHtml;
    var liElements = container.querySelectorAll('li');
    var values = [];
    for (var i = 0; i < liElements.length; i++) {
      var tagEl = liElements[i].querySelector('.tag-editor-tag');
      if (tagEl) {
        var text = tagEl.textContent.trim();
        if (tagEl.classList.contains(CalculationType.OVERALL_SCORE_TAG)) {
          values.push(transformOverallScore(text));
        } else if (tagEl.classList.contains(CalculationType.SUB_SCORE_TAG)) {
          values.push(transformSubScore(text));
        }
        // Handle other tags
        else {
          if (text === CalculationType.MULTIPLICATION_TAG) {
            values.push("*");
          } else if (text === CalculationType.DIVISION_TAG) {
            values.push("/");
          } else {
            values.push(text);
          }
        }
      }
    }

    var finalData = values.join(" ");
    if (finalData.indexOf("Highest") !== -1 || finalData.indexOf("Lowest") !== -1 || finalData.indexOf("Average") !== -1) {
      finalData = finalData.replace("Highest", "Math.max").replace("Lowest", "Math.min").replace("Average", "Math.mean");
      finalData = finalData.replace(/\(\s*([^)]+)\s*\)/, function (match, group1) {
        var replaced = group1.trim().split(/\s+/).join(",");
        return "(" + replaced + ")";
      });
    }
    return finalData;
  }

  // Reset tag editor
  function resetTagEditor() {
    var tags = $('#scoreCalculation').tagEditor('getTags')[0].tags;
    for (var i = 0; i < tags.length; i++) {
      $('#scoreCalculation').tagEditor('removeTag', tags[i]);
    }
  }

  function hasOveralOrSubScoreData() {
    var allData = overalScoreOriginal;
    for (var key in subScoreOriginal) {
      if (subScoreOriginal.hasOwnProperty(key)) {
        allData = allData.concat(subScoreOriginal[key]);
      }
    }
    return allData && allData.length > 0;
  }

  function hasOveralOrSubScoreSelectedData() {
    return /\b0&|\b\d+&/.test(transformCalculationData());
  }

  function isCorrectCalculation() {
    var calculationData = transformCalculationData();
    // Replace each number with a different random number
    var data = calculationData.replace(/\b\d+&\w+/g, function (match) {
      return Math.floor(Math.random() * 100).toString();
    });

    var result = "";
    try {
      if (isMathSymbolOption()) {
        result = math.evaluate(data.replace("Math.", ""));
      } else {
        result = math.evaluate(data);
      }

      return !isNaN(result) && result !== Infinity;
    } catch (e) {
      return false;
    }
  }

  function clearAllCalculation() {
    $('#scoreCalculation').next('.tag-editor').find('.tag-editor-delete').click();
  }

  function mappingAllScore() {
    for (var i = 0; i < overalScoreOriginal.length; i++) {
      allScoreColumns.push(overalScoreOriginal[i]);
    }
    if (subScoreOriginal) {
      for (var key in subScoreOriginal) {
        if (Array.isArray(subScoreOriginal[key])) {
          for (var i = 0; i < subScoreOriginal[key].length; i++) {
            allScoreColumns.push(subScoreOriginal[key][i]);
          }
        }
      }
    }
  }

  function exitsRelatedCalculation(expressionKey, expressionValue) {

    var breakR = false;
    function exitsRelated(expressionKey, expressionValue) {

      if (expressionKey == `${subscoreId ?? 0}&${currentScoreType}`) {
        return true;
      }
      if (expressionValue == null || (expressionValue && expressionValue.length == 0)) {
        return true;
      }
      columnExpression = handlGetKeyColumnByExpression(expressionValue);
      if (columnExpression.indexOf(`${subscoreId ?? 0}&${currentScoreType}`) != -1) {
        return true;
      }      
      var columnCompares = allScoreColumns.filter(function (col) {
        return columnExpression.some(function (key) {
          return col.expressionKey == key && col.expressionKey != expressionKey;
        });
      });
      if (columnCompares && columnCompares.length > 0) {
        for (let i = 0; i < columnCompares.length; i++) {
          var colId = columnCompares[i].expressionKey;
          var existCal = _.find(allScoreColumns, function (item) {
            return item.expressionKey === colId;
          });

          if (existCal && existCal.expressionValue) {
            columnExpression = handlGetKeyColumnByExpression(existCal.expressionValue);
            if (columnExpression.indexOf(`${subscoreId ?? 0}&${currentScoreType}`) != -1) {
              breakR = true;
              break;
            }
            if (existCal.isAutoCalculation && !breakR) {
              exitsRelated(existCal.expressionKey, existCal.expressionValue);
            }
          }
        }
      }
      return breakR;
    }

    return exitsRelated(expressionKey, expressionValue);

  }
  function handlGetKeyColumnByExpression(expression) {
    var columns = [];
    if (expression != null && expression.length > 0) {
      var expressionSplit = expression.replace('calculation:', '').replace(/Math\.min|Math\.max/g, '').replace(/[()]/g, '').split(/[ ,\x\*\+\-\/]+/);
      var overallScores = expressionSplit.filter(function (item) { return item.startsWith('0&') })
        .map(function (m) { return m.replace(/[()\s]/g, '') });
      if (overallScores != null && overallScores.length > 0) {
        overallScores.forEach(function (item) {
          var scoreSplit = item.split('&');
          if (scoreSplit.length > 1) {
            columns.push(`${0}&${scoreSplit[1]}`);
          }
        });
      }
      var subScores = expressionSplit.filter(function (item) { return item.includes('&') && !item.startsWith('0&') })
        .map(function (m) { return m.replace(/[()\s]/g, '') });
      if (subScores != null && subScores.length > 0) {
        subScores.forEach(function (item) {
          var subScoreSplit = item.split('&');
          if (subScoreSplit.length > 1) {
            columns.push(`${subScoreSplit[0]}&${subScoreSplit[1]}`);
          }
        });
      }
    }
    return columns;
  }

  return {
    init: init,
    resetTagEditor: resetTagEditor,
    getCalculationData: transformCalculationData,
    isMathSymbolOption: isMathSymbolOption,
    renderDefaultScore: renderDefaultScore,
    hasOveralOrSubScoreData: hasOveralOrSubScoreData,
    hasOveralOrSubScoreSelectedData: hasOveralOrSubScoreSelectedData,
    isCorrectCalculation: isCorrectCalculation,
    clearAllCalculation: clearAllCalculation
  };
})();
