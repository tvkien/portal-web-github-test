function EntryResultModel() {
  var self = this
  /**
   * Get list by key and assign key
   * @param  {[type]} key       [description]
   * @param  {[type]} assignkey [description]
   * @param  {[type]} items     [description]
   * @return {[type]}           [description]
   */
  self.getListByAssignKey = function(key, assignkey, items) {
    return items.filter(function(item) {
      return item[key] === assignkey
    })[0]
  }

  self.round = function(value, exp) {
    if (typeof exp === 'undefined' || +exp === 0)
      return Math.round(value);

    value = +value;
    exp = +exp;

    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
      return NaN;

    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

    // Shift back
    value = value.toString().split('e');
    return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
  }

  /**
   * Assign key value of student
   * @param  {[type]} key          [description]
   * @param  {[type]} assignkey    [description]
   * @param  {[type]} studentScore [description]
   * @param  {[type]} student      [description]
   * @return {[type]}              [description]
   */
  self.assignScoreStudentByKey = function(key, assignkey, studentScore, student, scoreInfos, isSubScore) {
    var scoreInfo = '';
    for (var i = 0; i < scoreInfos.length; i++) {
      if (scoreInfos[i].ScoreName === key) {
        scoreInfo = scoreInfos[i];
        break;
      }
    }

    var decimalScale = 0;
    if (scoreInfo) {
      decimalScale = scoreInfo.MetaData.DecimalScale;
    }

    if (key === 'Percentile') {
      if (scoreInfo && scoreInfo.MetaData.DataType === 'Numeric' && studentScore['ScorePercentage']) {
        var ScorePercentage = studentScore['ScorePercentage'];
        var newValue = this.round(ScorePercentage, decimalScale).toFixed(decimalScale);
        student[assignkey] = newValue;
      } else {
        student[assignkey] = studentScore['ScorePercentage'];
      }
    } else if (key.indexOf('note') > -1) {
      student[assignkey] = null;
      if (studentScore.Notes && studentScore.Notes.length > 0) {
        for (var i = 0; i < studentScore.Notes.length; i++) {
          var item = studentScore.Notes[i];
          if (item.NoteKey == key) {
            var result = '';
            var objectNote = item.NoteContents.Notes;
            if (objectNote.length > 0 && objectNote[0].NoteType == 'date') {
              //Sort array
              objectNote.sort(function(a, b) {
                return new Date(b.NoteDate) - new Date(a.NoteDate);
              });
            }
            var arrObjectNoteLength = this.Layout === 'labels' ? 1 : objectNote.length;
            for (var i = 0; i < arrObjectNoteLength; i++) {
              var item = objectNote[i];
              if (item.NoteType == 'date') {
                var noteDate = moment(item.NoteDate).format(this.DateFormatPrint);
                if (this.Layout === 'labels') {
                  result += '<strong>Note ' + noteDate + ':</strong> ' + item.Content;
                } else {
                  result += '<strong>Note ' + noteDate + ':</strong> ' + item.Content + '<br/>';
                }
              } else {
                result += item.Content;
              }
            }
            if (scoreInfo && scoreInfo.MetaData.DataType === 'Numeric' && item.Note) {
              var newNote = this.round(item.Note, decimalScale).toFixed(decimalScale);
              student[assignkey] = newNote;
              break;
            } else {
              student[assignkey] = result;
              break;
            }
          }
        }
      } else {
        student[assignkey] = null;
      }
    } else if (key.indexOf('CustomN_') > -1) {
      var valueCustom = this.getDisplayFromValue(key, studentScore['Score' + key], scoreInfos);
      if (scoreInfo && scoreInfo.MetaData.DataType === 'Numeric' && valueCustom) {
        var newValueCustom = this.round(valueCustom, decimalScale).toFixed(decimalScale);
        if (newValueCustom === "NaN") {
          newValueCustom = valueCustom;
        }
        student[assignkey] = newValueCustom;
      } else {
        student[assignkey] = valueCustom;
      }
    } else if (key === 'Artifact' && studentScore['Artifacts']) {
      var length = studentScore['Artifacts'].length === 0 ? '' : studentScore['Artifacts'].length;
      if (isSubScore) {
        student[assignkey] = length;
      } else {
        student[key] = length;
      }
    } else {

      student[assignkey] = studentScore['Score' + key]
      if (scoreInfo && scoreInfo.MetaData.DataType === 'Numeric' && studentScore['Score' + key]) {
        var score = parseFloat(studentScore['Score' + key]);
        var newScore = this.round(score, decimalScale).toFixed(decimalScale);
        student[assignkey] = newScore;
      } else {
        student[assignkey] = studentScore['Score' + key]
      }

    }
    return student
  }
  self.getDisplayFromValue = function(scoreName, value, scoreInfos) {
    var label = value;
    var scoreInfo = scoreInfos.filter(function(item) {
      return item.ScoreName === scoreName;
    });
    if (!!scoreInfo && scoreInfo.length > 0 && !!scoreInfo[0].MetaData && !!scoreInfo[0].MetaData.SelectListOptions && !!scoreInfo[0].MetaData.DisplayOption &&
      !!scoreInfo[0].MetaData.FormatOption && scoreInfo[0].MetaData.FormatOption.toLowerCase() === 'labelvaluetext') {
      var result = scoreInfo[0].MetaData.SelectListOptions.filter(function(option) {
        return parseFloat(option.Option) == parseFloat(value);
      });
      if (!!result && result.length > 0) {
        if (scoreInfo[0].MetaData.DisplayOption === 'label') {
          label = result[0].Label;
        }
        if (scoreInfo[0].MetaData.DisplayOption === "both") {
          label = result[0].Label + ' (' + result[0].Option + ')';
        }
      }
    }
    return label;
  }
  /**
   * Get student by overall score
   * @param  {[type]} overallScore [description]
   * @param  {[type]} studentScore [description]
   * @param  {[type]} student      [description]
   * @return {[type]}              [description]
   */
  self.getStudentByOverrallScore = function(overallScore, studentScore, student, scoreInfos) {
    for (var i = 0, len = overallScore.length; i < len; i++) {
      var key = overallScore[i]
      var assignkey = overallScore[i]

      student = this.assignScoreStudentByKey(key, assignkey, studentScore, student, scoreInfos)
    }

    return student;
  }

  /**
   * Get student by sub scores
   * @param  {[type]} subScores        [description]
   * @param  {[type]} studentScore     [description]
   * @param  {[type]} studentSubScores [description]
   * @param  {[type]} student          [description]
   * @return {[type]}                  [description]
   */
  self.getStudentBySubScores = function(subScores, studentScore, studentSubScores, student) {
    var studentFilters = entryResultModel.StudentTestResultSubScores.filter(function(stu) {
      return stu.StudentID === studentScore.StudentID
    })

    for (var i = 0, subScoresLength = subScores.length; i < subScoresLength; i++) {
      var subSore = subScores[i]
      var subScoreInfos = this.CustomSubScores.filter(function(sub) {
        return sub.Name === subSore.Name;
      });
      var subInfo = null;
      if (!!subScoreInfos && subScoreInfos.length > 0) {
        subInfo = subScoreInfos[0].ScoreInfos.filter(function(item) {
          return !!item.MetaData && !!item.MetaData.DisplayOption;
        });
      }

      for (var j = 0, studentFiltersLength = studentFilters.length; j < studentFiltersLength; j++) {
        var studentFilter = studentFilters[j]

        if (subSore.Name === studentFilter.Name) {
          for (var h = 0, len = subSore.SubScoreNameList.length; h < len; h++) {
            var key = subSore.SubScoreNameList[h]
            var assignkey = subSore.Name + '::' + subSore.SubScoreNameList[h]

            student = this.assignScoreStudentByKey(key, assignkey, studentFilter, student, subInfo, true)
          }
        }
      }
    }

    return student
  }

  /**
   * Print students
   * @param  {[type]} overrallScore    [description]
   * @param  {[type]} subScores        [description]
   * @param  {[type]} studentScores    [description]
   * @param  {[type]} studentSubScores [description]
   * @return {[type]}                  [description]
   */
  self.getStudentsPrint = function(overrallScore, subScores, studentScores, studentSubScores) {
    var students = []

    var scoreInfoValueLabelOverall = this.CustomScore.ScoreInfos.filter(function(item) {
      return !!item.MetaData && !!item.MetaData.DisplayOption;
    });
    for (var i = 0, len = studentScores.length; i < len; i++) {
      var studentScore = studentScores[i];
      var student = {}

      student['Student'] = studentScore.LastName + ', ' + studentScore.FirstName
      student['Result Date'] = studentScore.ResultDate ? moment.utc(+studentScore.ResultDate.replace(/\/Date\((\d+)\)\//g, "$1")).format(this.DateFormatPrint) : '';

      if (!!overrallScore) {
        student = this.getStudentByOverrallScore(overrallScore, studentScore, student, scoreInfoValueLabelOverall)
      }

      if (!!subScores) {
        student = this.getStudentBySubScores(subScores, studentScore, studentSubScores, student)
      }

      students.push(student)
    }

    return students
  }

  /**
   * Generate table
   * @param  {[type]} el       [description]
   * @param  {[type]} row      [description]
   * @param  {[type]} students [description]
   * @return {[type]}          [description]
   */
  self.generateDataTable = function(el, row, students) {
    var table = document.createElement('table')
    var tableHead = this.generateDataTableHead(row, students)
    var tableBody = this.generateDataTableBody(row, students)

    table.appendChild(tableHead)
    table.appendChild(tableBody)
    el.appendChild(table)
  };

  /**
   * Generate header of table
   * @param  {[type]} row      [description]
   * @param  {[type]} students [description]
   * @return {[type]}          [description]
   */
  self.generateDataTableHead = function(row, students) {
    var tableHead = document.createElement('thead')
    var trHeadNested = document.createElement('tr')
    var trHeadingNested = []
    var trHead = document.createElement('tr')
    var trHeading = ['Student', 'Result Date']
    var headCustomScore = []
    var i = 0;
    var len = 0;

    // Build header
    for (i = 0, len = row.length; i < len; i++) {
      var rowItem = row[i]
      var rowItemSplit = rowItem.split('::')

      // Get custom sub scores header
      if (rowItemSplit.length === 2) {
        var subScoreList = this.getListByAssignKey('Name', rowItemSplit[0], this.CustomSubScores)
        var subScoreItemList = this.getListByAssignKey('ScoreName', rowItemSplit[1], subScoreList.ScoreInfos)
        trHeading.push(subScoreItemList.ScoreLable)
        headCustomScore.push({
          id: rowItemSplit[0],
          name: rowItemSplit[1]
        })
      } else {
        // Get overall score header
        var overrallScoreList = this.getListByAssignKey('ScoreName', rowItem, this.CustomScore.ScoreInfos)
        trHeading.push(overrallScoreList.ScoreLable)
      }
    }

    for (i = 0, len = trHeading.length; i < len; i++) {
      var tdHead = this.generateDataTableTd(trHeading[i])
      trHead.appendChild(tdHead)
    }

    // Build nested header
    trHeadingNested.push({
      name: '',
      colspan: 2
    })

    var headOverallScore = row.filter(function(r) {
      return r.split('::').length === 1
    })

    // Overrall score
    if (headOverallScore.length) {
      trHeadingNested.push({
        name: this.OverallScore,
        colspan: headOverallScore.length
      })
    }

    // Custom score
    if (headCustomScore.length) {
      var headCustomScoreUniq = R.uniq(headCustomScore)
      var headCustomScoreById = R.countBy(R.prop('id'))(headCustomScoreUniq)

      for (var key in headCustomScoreById) {
        trHeadingNested.push({
          name: key,
          colspan: headCustomScoreById[key]
        })
      }
    }

    for (i = 0, len = trHeadingNested.length; i < len; i++) {
      var tdHeadNested = this.generateDataTableTd(trHeadingNested[i].name)
      tdHeadNested.setAttribute('colspan', trHeadingNested[i].colspan);
      //tdHeadNested.className = 'table-colspan-' + trHeadingNested[i].colspan
      trHeadNested.appendChild(tdHeadNested)
    }

    tableHead.appendChild(trHeadNested)
    tableHead.appendChild(trHead)

    return tableHead
  }

  /**
   * Generate body of table
   * @param  {[type]} row      [description]
   * @param  {[type]} students [description]
   * @return {[type]}          [description]
   */
  self.generateDataTableBody = function(row, students) {
    var tableBody = document.createElement('tbody')

    for (var i = 0, studentsLength = students.length; i < studentsLength; i++) {
      var student = students[i]
      var trBody = document.createElement('tr')

      for (var key in student) {
        if (key === 'Student' || key === 'Result Date') {
          var tdBody = this.generateDataTableTd(student[key])
          trBody.appendChild(tdBody)
        }

        for (var j = 0, rowLength = row.length; j < rowLength; j++) {
          if (key === row[j]) {
            var tdBody = this.generateDataTableTd(student[key], key.indexOf('note_') !== 0 && key.indexOf('::note_') === -1)
            tdBody.className = "word-break"
            trBody.appendChild(tdBody)
          }
        }
      }

      tableBody.appendChild(trBody)
    };

    return tableBody
  }

  function escapeHTML(str) {
    if (str == null || typeof str !== 'string') return str;
    return str.replace(/[&<>"']/g, function(match) {
        switch (match) {
            case '&':
                return '&amp;';
            case '<':
                return '&lt;';
            case '>':
                return '&gt;';
            case '"':
                return '&quot;';
            case "'":
                return '&#39;';
            default:
                return match;
        }
    });
  }

  /**
   * Generate td of table
   * @param  {[type]} text [description]
   * @return {[type]}      [description]
   */
  self.generateDataTableTd = function(text, isText) {
    var td = document.createElement('td')

    if (typeof text !== 'undefined' && text != null) {
      if (isText !== true) {
        td.innerHTML = text;
      } else {
        td.innerHTML = escapeHTML(text);
      }
    }

    return td
  }

  /**
   * Generate overall score description
   * @param  {[type]} el                [description]
   * @param  {[type]} overrallScorePart [description]
   * @param  {[type]} customScore       [description]
   * @return {[type]}                   [description]
   */
  self.generateOverallScoreDescription = function(el, overrallScorePart, customScore) {

    if (!!overrallScorePart) {
      var isHeading = false
      var headingCustomScore = this.generateHeadingScoreDescription(this.OverallScore)
      var contentCustomScore = document.createElement('div')

      for (var i = 0, overrallScorePartLength = overrallScorePart.length; i < overrallScorePartLength; i++) {
        var score = overrallScorePart[i]

        for (var j = 0, scoreInfosLength = customScore.ScoreInfos.length; j < scoreInfosLength; j++) {
          var scoreInfo = customScore.ScoreInfos[j]

          if (score === scoreInfo.ScoreName) {
            isHeading = true
            var paragraph = this.generateItemScoreDescription(scoreInfo.ScoreLable, scoreInfo.MetaData.Description);
            contentCustomScore.appendChild(paragraph)
          }
        }
      }

      if (isHeading) {
        contentCustomScore.className = 'printEntryResult__list word-break'
        el.appendChild(headingCustomScore)
        el.appendChild(contentCustomScore)
      }
    }
  }

  /**
   * Generate custom score description
   * @param  {[type]} el              [description]
   * @param  {[type]} subScorePart    [description]
   * @param  {[type]} customSubScores [description]
   * @return {[type]}                 [description]
   */
  self.generateCustomScoreDescription = function(el, subScorePart, customSubScores) {
    if (!!subScorePart) {
      for (var i = 0, subScorePartLength = subScorePart.length; i < subScorePartLength; i++) {
        var isHeading = false
        var score = subScorePart[i]
        var headingCustomScore = this.generateHeadingScoreDescription(score.Name)
        var contentCustomScore = document.createElement('div')

        for (var j = 0, customSubScoresLength = customSubScores.length; j < customSubScoresLength; j++) {
          var customSubScore = customSubScores[j]

          if (score.Name === customSubScore.Name) {
            for (var h = 0, subScoreNameListLength = score.SubScoreNameList.length; h < subScoreNameListLength; h++) {
              var scoreItem = score.SubScoreNameList[h]

              for (var m = 0, scoreInfosLength = customSubScore.ScoreInfos.length; m < scoreInfosLength; m++) {
                var scoreInfo = customSubScore.ScoreInfos[m]

                if (scoreItem === scoreInfo.ScoreName) {
                  isHeading = true
                  var paragraph = this.generateItemScoreDescription(scoreInfo.ScoreLable, scoreInfo.MetaData.Description)
                  contentCustomScore.appendChild(paragraph)
                }
              }
            }
          }
        }

        if (isHeading) {
          contentCustomScore.className = 'printEntryResult__list word-break'
          el.appendChild(headingCustomScore)
          el.appendChild(contentCustomScore)
        }
      }
    }
  }

  /**
   * Generate heading of score description
   * @param  {[type]} html [description]
   * @return {[type]}      [description]
   */
  self.generateHeadingScoreDescription = function(html) {
    var title = document.createElement('h3')

    title.className = 'printEntryResult__leading'
    title.innerHTML = html

    return title
  }

  /**
   * Generate item of score description
   * @param  {[type]} html [description]
   * @return {[type]}      [description]
   */
  self.generateItemScoreDescription = function(scoreLable, description) {
    var item = document.createElement('div');
    var html = '';

    if (!description) {
      description = '';
      html = '<strong>' + scoreLable + '</strong> ';
    } else {
      html = '<strong>' + scoreLable + ':</strong> ';
    }

    html += '<div class="printEntryResult__list--description">';
    html += this.unescapeHtml(description);
    html += '</div>';

    item.className = 'printEntryResult__list--item'
    item.innerHTML = html

    return item
  }

  /**
   * Generate rubric description
   * @param  {[type]} el          [description]
   * @param  {[type]} description [description]
   * @return [type]               [description]
   */
  self.generateRubricDescription = function(el, description) {
    var rubricDescription = document.createElement('div');
    if (!description) {
      description = ''
    }

    description = this.unescapeHtml(description)

    rubricDescription.className = 'printEntryResult__body word-break'
    rubricDescription.innerHTML = description

    el.appendChild(rubricDescription)
  }

  /**
   * Un escape html
   * @param  {[type]} safe [description]
   * @return {[type]}      [description]
   */
  self.unescapeHtml = function(safe) {
    if (!!safe) {
      return safe.replace(/&amp;/g, '&')
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/&quot;/g, '"')
        .replace(/&#039;/g, "'")
        .replace(/&nbsp;/gi, '');
    }

    return ''
  }

  /**
   * Initialize
   * @return {[type]} [description]
   */
  self.init = function() {
    var selectedLayout = this.Layout;
    if (selectedLayout === 'labels') {
      this.setPageLabel();
      var printEntryResultLabels = document.getElementById('printEntryResult-labels');

      document.getElementById("printEntryResult-description").style.display = "none";
      document.getElementById("printEntryResult-rubric-description").style.display = "none";

      var dataStudents = [];
      var subScoreNameList = [];

      dataStudents = this.getStudentsPrint(
        this.OverrallScoreNameList,
        this.SubScorePartList,
        this.StudentTestResultScores,
        this.StudentTestResultSubScores,
        this.CustomScore
      );

      // Get list sub score
      if (!!this.SubScorePartList) {
        for (var i = 0; i < this.SubScorePartList.length; i++) {
          var scoreSub = this.SubScorePartList[i]

          for (var j = 0; j < scoreSub.SubScoreNameList.length; j++) {
            subScoreNameList.push(scoreSub.Name + '::' + scoreSub.SubScoreNameList[j])
          }
        }
      }
      var scoreNameList = this.OverrallScoreNameList == null ? [] : this.OverrallScoreNameList;
      var allScore = scoreNameList.concat(subScoreNameList)
      this.generateLabels(printEntryResultLabels, allScore, dataStudents);

    } else {
      var printEntryResultTable = document.getElementById('printEntryResult-table')
      var printEntryResultTitle = document.querySelector('.jsTitle')
      var printEntryResultDateTime = document.querySelector('.jsDateTime')
      var printEntryResultDescription = document.getElementById('printEntryResult-description');
      var printResultRubricDescription = document.getElementById('printEntryResult-rubric-description');
      var dataStudents = []
      var scoreNameList = this.OverrallScoreNameList == null ? [] : this.OverrallScoreNameList
      var subScoreNameList = []


      var allScore = []
      var rowBySplit = 0
      var isSplitPage = false
      var selectedColumns = 0
      var rows = []
      var SPACE = '&nbsp;&nbsp;&nbsp;'
      var overallscore = this.OverallScore

      printEntryResultTitle.innerHTML = this.TestTitle + SPACE + this.ClassName
      printEntryResultDateTime.innerHTML = 'Run Date: ' + moment().format('MM/DD/YY h:mm A');

      dataStudents = this.getStudentsPrint(
        this.OverrallScoreNameList,
        this.SubScorePartList,
        this.StudentTestResultScores,
        this.StudentTestResultSubScores,
        this.CustomScore
      )
      // Get list sub score
      if (!!this.SubScorePartList) {
        for (var i = 0; i < this.SubScorePartList.length; i++) {
          var scoreSub = this.SubScorePartList[i]

          for (var j = 0; j < scoreSub.SubScoreNameList.length; j++) {
            subScoreNameList.push(scoreSub.Name + '::' + scoreSub.SubScoreNameList[j])
          }
        }
      }

      // Join list overall score and sub score
      allScore = scoreNameList.concat(subScoreNameList)
      selectedColumns = allScore.length

      if (selectedLayout === 'portrait' && selectedColumns > 6) {
        rowBySplit = 6
        isSplitPage = true
      } else if (selectedLayout === 'landscape' && selectedColumns > 8) {
        rowBySplit = 8
        isSplitPage = true
      }

      if (isSplitPage) {
        rows = R.splitEvery(rowBySplit, allScore)
      } else {
        rows.push(allScore)
      }

      for (var i = 0; i < rows.length; i++) {
        this.generateDataTable(printEntryResultTable, rows[i], dataStudents)
      }

      // Print include score description
      if (this.ScoreDescription === 'yes') {
        this.generateOverallScoreDescription(printEntryResultDescription, this.OverrallScoreNameList, this.CustomScore)
        this.generateCustomScoreDescription(printEntryResultDescription, this.SubScorePartList, this.CustomSubScores)
      } else {
        printEntryResultDescription.innerHTML = ''
      }

      // Print rubric description
      if (this.IncludeRubricDescription === 'yes') {
        this.generateRubricDescription(printResultRubricDescription, this.RubricDescription)
      } else {
        printResultRubricDescription.innerHTML = ''
      }

      if (selectedColumns === 0) {
        printEntryResultDescription.className += ' dn';
        printEntryResultTable.className += ' dn';
      }
    }
  };

  self.generateLabels = function(el, row, students) {
    var table = document.createElement('table')
    table.className = "table-labels";

    var tableBody = this.generateLabelsRow(row, students)
    table.appendChild(tableBody)
    el.appendChild(table)
  };

  self.generateLabelsRow = function(row, students) {
    var tableBody = document.createElement('tbody')
    for (var i = 0; i < students.length; i += 2) {
      var student1 = students[i];
      var student2 = students[i + 1];
      var tr = document.createElement('tr');

      for (var key in student1) {
        for (var j = 0, rowLength = row.length; j < rowLength; j++) {
          if (key === row[j]) {
            var td = this.generateLabelsTd(student1[key], student1.Student)
            tr.appendChild(td);
          }
        }
      }
      for (var key in student2) {
        for (var j = 0, rowLength = row.length; j < rowLength; j++) {
          if (key === row[j]) {
            var td = this.generateLabelsTd(student2[key], student2.Student)
            tr.appendChild(td);
          }
        }
      }

      tableBody.appendChild(tr)
    };

    return tableBody
  }

  self.generateLabelsTd = function(text, studentName) {
    var td = document.createElement('td');
    td.className = "td-labels";
    var div = document.createElement('div');
    div.className = 'max-height-table-row';
    var html = '';
    html += '<div class="h2-bold">';
    html += studentName;
    html += '</div>';

    if (typeof text !== 'undefined' && text != null) {
      if (this.Layout === 'labels') {
        text = this.RemoveHTMLTags(text);
        text = text.replace(/&nbsp;/g, " ");
      }

      html += '<div class="h2">' + text + '</div>';
    }
    div.innerHTML = html;
    td.appendChild(div);

    return td
  }
  self.setPageLabel = function() {
    var style = document.createElement('style');
    style.innerHTML = "@page { margin-top:18px; margin-bottom:18px; margin-left: 5px; margin-right: 5px; }";
    document.head.appendChild(style);
  }

  self.RemoveHTMLTags = function(html) {
    var regX = /(<([^>]+)>)/ig;
    return html.replace(regX, "");
  }
}
