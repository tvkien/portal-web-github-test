function ACTReport() {
    this.diagnosticHistoryChartData = {};
    this.scoreImprovementChartData = {};

    this.initDateForCharts = function(model){
        this.diagnosticHistoryChartData = handlingDataForDiagnosticHistoryChart(model);
        this.scoreImprovementChartData = handlingDataForScoreImprovementChart(model);
    }
    
    this.renderDiagnosticHistoryChart = function (compId, englishId, mathId, readingId, scienceId) {
        var htmlComposite = renderHorizontalBarChart(this.diagnosticHistoryChartData.compositeModel);
        $("#" + compId).html(htmlComposite);

        var htmlEnglish = renderHorizontalBarChart(this.diagnosticHistoryChartData.englishModel, 4);
        $("#" + englishId).html(htmlEnglish);

        var htmlMath = renderHorizontalBarChart(this.diagnosticHistoryChartData.mathModel, 4);
        $("#" + mathId).html(htmlMath);

        var htmlReading = renderHorizontalBarChart(this.diagnosticHistoryChartData.readingModel, 4);
        $("#" + readingId).html(htmlReading);

        var htmlScience = renderHorizontalBarChart(this.diagnosticHistoryChartData.scienceModel, 4);
        $("#" + scienceId).html(htmlScience);
    }

    this.renderScoreImprovementBarChart = function (elementId){
        var html = renderVerticalBarChart(this.scoreImprovementChartData);
        $("#" + elementId).html(html);

    }

    this.renderSectionChart = function(model){
        return renderSectionHorizontalChart(model);
    }

    function renderHorizontalBarChart(objectModel, scaleRate) {
       // objectModel.data[0].value = 30;
        //objectModel.data[1].value = 10;

        if (!scaleRate) scaleRate = 7;

        var barHeightDefaultRate = 6;
        if(objectModel.name == 'Composite'){
            barHeightDefaultRate = 7;
        }

        var htmlElements = '';

        htmlElements += '<div style="font-family: \'Segoe UI\';position: relative;height: ' + (barHeightDefaultRate * scaleRate * 2 + 50) + 'px;left:' + (objectModel.data[0].label ? 0 : -100) + 'px">';
        htmlElements += '<table cellspacing="0" cellpadding="0" style="position:absolute;width:' + (scaleRate * objectModel.maxSize + 1) + 'px;border:1px solid #d1d2d4;border-right:none;left:104px;top:21px;height:' + (barHeightDefaultRate * scaleRate * objectModel.data.length) + 'px;z-index:1;">';
        htmlElements += '<tr>';

        var step = objectModel.maxSize / objectModel.stepSize;  // missing round

        for (var index = 0; index < step; index++) {
            htmlElements += '<td style="border-right: 1px solid #d1d2d4;"></td>';
        }

        htmlElements += '</tr>';
        htmlElements += '</div>';
        htmlElements += '<div>';
        htmlElements += '<div></div>';
        htmlElements += '<div style="margin-left: 105px;font-weight: 550;font-size:11pt;top:2px;position:absolute">' + objectModel.name + '</div>';
        htmlElements += '<div style="background-color:' + objectModel.data[0].color + ';width: 28px;display:' + (objectModel.data[0].value * scaleRate > ((objectModel.titleWidth + 3) * scaleRate) ? "block" : "none") + ';height: 18px;padding-top:2px;position: absolute;top: -2px;left: ' + (105 + scaleRate * objectModel.data[0].value - 15) + 'px;color: white;font-weight: 700;border-radius: 5px;text-align: center;font-size: 10pt;">';
        htmlElements += objectModel.data[0].value;
        htmlElements += '<div style="width: 0;height: 0;border-left: 6px solid transparent;border-right: 6px solid transparent;border-top: 7px solid ' + objectModel.data[0].color + ';margin-left: 8px;margin-top:3px"></span>';
        htmlElements += '</div>';
        htmlElements += '</div>';

        for (var index = 0; index < objectModel.data.length; index++) {
            var element = objectModel.data[index];

            htmlElements += '<div style="position:relative;z-index:100;top:' + (index * barHeightDefaultRate * scaleRate + 21) + 'px">';
            htmlElements += '<div style="position:absolute;left:0;width:100px;font-size:10pt;margin-top:' + (barHeightDefaultRate * scaleRate / 2 - 10) + 'px;text-align:right;">' + (element.label) + (element.isBaseLine && element.label ? ' Baseline' : '') + '</div>';
            htmlElements += '<div style="height:' + (barHeightDefaultRate * scaleRate) + 'px;background-color:' + element.color + ';width:' + element.value * scaleRate + 'px;position:absolute;left: 105px; text-align: right;font-weight: bold; font-size: 15pt;">';
            htmlElements += index > 0 || (element.value * scaleRate <= (objectModel.titleWidth + 3) * scaleRate) ? '<p style=" font-weight:700;color:' + ((index == 0 && (element.value * scaleRate <= (objectModel.titleWidth + 3) * scaleRate)) ? "white" : "black") + ';display:' + (element.value > 0 ? "block" : "none") + ' ;margin-top:' + (barHeightDefaultRate * scaleRate / 2 - 8) + 'px;margin-right:5px;font-size:10pt;">' + element.value + '</p>' : '';
            htmlElements += '</div></div>';
        }

        htmlElements += '</div>';

        return htmlElements;
    }

    function handlingDataForDiagnosticHistoryChart(model) {
        if (!model) {
            return null;
        } else {
            var result = null; // object return
            
            var maxScore = 36;
            var compositeTitleWidth = 11;
            var englishTitleWidth = 13;
            var mathTitleWidth = 10;
            var readingTitleWidth = 15;
            var scienceTitleWidth = 14;

            var selectedTestScore = _.find(model, function (item) { return item.IsSelected == true; });
            var baseLineTestScore = _.last(model);

            if (selectedTestScore) {
                result = {};

                result.compositeModel = {
                    name: 'Composite',
                    data: [
                        { label: selectedTestScore.TestDateText, value: selectedTestScore.CompositeScoreText, color: '#37592A' },
                    ],
                    stepSize: 1,
                    maxSize: maxScore,
                    titleWidth: compositeTitleWidth
                }

                result.englishModel = {
                    name: 'English',
                    data: [
                        { label: '', value: selectedTestScore.EnglishScoreText, color: '#0d6671' },
                    ],
                    stepSize: 1,
                    maxSize: maxScore,
                    titleWidth: englishTitleWidth
                }

                result.mathModel = {
                    name: 'Math',
                    data: [
                        { label: '', value: selectedTestScore.MathScoreText, color: '#eb6523' },
                    ],
                    stepSize: 1,
                    maxSize:maxScore,
                    titleWidth: mathTitleWidth
                }

                result.readingModel = {
                    name: 'Reading',
                    data: [
                        { label: '', value: selectedTestScore.ReadingScoreText, color: '#5f164d' },
                    ],
                    stepSize: 1,
                    maxSize: maxScore,
                    titleWidth: readingTitleWidth
                }

                result.scienceModel = {
                    name: 'Science',
                    data: [
                        { label: '', value: selectedTestScore.ScienceScoreText, color: '#a42630' },
                    ],
                    stepSize: 1,
                    maxSize: maxScore,
                    titleWidth: scienceTitleWidth
                }

                if (baseLineTestScore) {
                    result.compositeModel.data.push({ label: baseLineTestScore.TestDateText, value: baseLineTestScore.CompositeScoreText, color: '#d1d2d4', isBaseLine: true });
                    result.englishModel.data.push({ label: '', value: baseLineTestScore.EnglishScoreText, color: '#d1d2d4' });
                    result.mathModel.data.push({ label: '', value: baseLineTestScore.MathScoreText, color: '#d1d2d4' });
                    result.readingModel.data.push({ label: '', value: baseLineTestScore.ReadingScoreText, color: '#d1d2d4' });
                    result.scienceModel.data.push({ label: '', value: baseLineTestScore.ScienceScoreText, color: '#d1d2d4' });
                }
                // else {
                //     result.compositeModel.data.push({ label: '', value: 0, color: '#d1d2d4', isBaseLine: true });
                //     result.englishModel.data.push({ label: '', value: 0, color: '#d1d2d4', isBaseLine: true });
                //     result.mathModel.data.push({ label: '', value: 0, color: '#d1d2d4', isBaseLine: true });
                //     result.readingModel.data.push({ label: '', value: 0, color: '#d1d2d4', isBaseLine: true });
                //     result.scienceModel.data.push({ label: '', value: 0, color: '#d1d2d4', isBaseLine: true });
                // }
            }

            return result;
        }
    }

    function renderVerticalBarChart(objectModel) {
        var scaleRate = 5;

        var htmlElement = '';
        htmlElement += '<div class="bar-chart-content">';
        htmlElement += '<div class="bar-chart-box">';
        htmlElement += '<span class="max-score">' + objectModel.options.maxScore + '</span>';
        htmlElement += '<div class="grid-line">';
        for (var index = 0; index < 36; index++) {
            htmlElement += '<p class="line"></p>';
        }
        htmlElement += '</div>';
        htmlElement += '<div class="zero">0</div>';

        var left = 0;
        for (var index = 0; index < objectModel.data.length; index++) {
            var element = objectModel.data[index];
            var improveScore = element.newValue - element.oldValue;

            htmlElement += '<div class="one-col" style="left:' + left + 'px;">';
            htmlElement += '<div class="bar" style="z-index:0;height:' + (element.newValue * scaleRate) + 'px;background-color:' + element.color + ';">';
            htmlElement += '<span class="span1" style="display:'+ (!(improveScore >= 0 || improveScore <= -8) ? "block" : "none") +'">'+ element.newValue +'</span>';
            htmlElement += '<div class="bubble" style="display:'+(element.newValue > 0 && (improveScore >= 0 || improveScore <= -8) ? "block": "none")+';background: ' + element.color + ';">' + element.newValue + '</div>';
            htmlElement += '<div class="bubble1" style="display:'+(element.newValue > 0 && (improveScore >= 0 || improveScore <= -8) ? "block": "none")+';border-bottom: 6px solid ' + element.color + ';"></div>';
            htmlElement += '<span class="span2" style="display:'+ (improveScore >= 0 ? "block" : "none") +';color:' + element.color + ';">+' + improveScore + '</span>';
            htmlElement += '</div>';
            htmlElement += '<div class="two-col" style="height:' + (element.oldValue * scaleRate) + 'px;">';
            htmlElement += '<div class="bar1" style="display:'+ (element.oldValue > 0 ? "block": "none") +';">' + element.oldValue + '</div>';
            htmlElement += '<div class="bubble2" style="display:'+ (element.oldValue > 0 ? "block": "none") +';"></div>'
            htmlElement += '<span class="span3" style="display:'+ (improveScore < 0 ? "block" : "none") +';color:' + element.color + ';">' + improveScore + '</span>';

            htmlElement += '</div>';
            htmlElement += '<label style="font-size:10pt">' + element.title + '</label>';
            htmlElement += '</div>';

            left += 80;
        }

        htmlElement += '</div>';
        htmlElement += '<div style="width:35%;position:absolute;right:0">';
        htmlElement += '<table style="border: 1px solid;font-size:8pt;width:100%" cellspacing=0>';

        htmlElement += '<tr>';
        htmlElement += '<th style="border-bottom:1px solid;width: 55px;"></th>';
        htmlElement += '<th style="border-bottom:1px solid;width: 55px;padding:2px">Baseline</th>';
        htmlElement += '<th style="border-bottom:1px solid;width: 55px;">Best</th>';
        htmlElement += '<th style="border-bottom:1px solid;border-left:1px solid;width: 55px;">Improve</th>';
        htmlElement += '</tr>';

        for (var index = 0; index < objectModel.data.length; index++) {
            var element = objectModel.data[index];
            var improveScore = element.newValue - element.oldValue;

            htmlElement += '<tr>';
            htmlElement += '<td style="border-bottom: 1px solid '+(index == objectModel.data.length - 1 ? "black" : "#d1d2d4")+';border-left: 1px solid black;padding:1px">' + element.title + '</td>';
            htmlElement += '<td style="text-align:center;border-bottom: 1px solid '+(index == objectModel.data.length - 1 ? "black" : "#d1d2d4")+';border-left: 1px solid #d1d2d4;">' + element.oldValue + '</td>';
            htmlElement += '<td style="text-align:center;border-bottom: 1px solid '+(index == objectModel.data.length - 1 ? "black" : "#d1d2d4")+';border-left: 1px solid #d1d2d4;">' + element.newValue + '</td>';
            htmlElement += '<td style="text-align:center;border-bottom: 1px solid '+(index == objectModel.data.length - 1 ? "black" : "#d1d2d4")+';border-left: 1px solid black;">' + ((improveScore > 0 ? "+" : "") + improveScore) + '</td>';
            htmlElement += '</tr>';
        }

        htmlElement += '</table>';
        htmlElement += '<div style="margin-top:25px;">';
        for (var index = 0; index < objectModel.data.length; index++) {
            var element = objectModel.data[index];
            htmlElement += ' <div style="width:12px;height:12px;float:left;background-color:' + element.color + '"></div>';
        }
        htmlElement += '<div style="margin-left:20px;float:left;font-size:9pt">Best Score</div>';
        htmlElement += '</div>';
        htmlElement += '<div style="clear:both;padding-top:20px;">';
        htmlElement += '<div style="width:60px;height:12px;float:left;background-color:#d1d2d4;"></div>';
        htmlElement += '<div style="margin-left:20px;float:left;font-size:9pt">Baseline Score</span>';
        htmlElement += '</div>';
        htmlElement += '</div>';
        htmlElement += '</div>';
        htmlElement += '</div>';

        return htmlElement;
    }

    function handlingDataForScoreImprovementChart(model){
        if(!model){
            return null;
        } else{
            var result = null;
            var maxScore = 36;

            var selectedTestScore = _.find(model, function (item) { return item.IsSelected == true; });
            var baseLineTestScore = _.last(model);

            var baseLineComponentScore = 0, bestCompositeScore = 0;
            var baseLineEnglishScore = 0, bestEnglishScore = 0;
            var baseLineMathScore = 0, bestMathScore = 0;
            var baseLineReadingScore = 0, bestReadingScore = 0;
            var baseLineScienceScore = 0, bestScienceScore = 0;

            if(baseLineTestScore){
                baseLineComponentScore = baseLineTestScore.CompositeScoreText;
                baseLineEnglishScore = baseLineTestScore.EnglishScoreText;
                baseLineMathScore = baseLineTestScore.MathScoreText;
                baseLineReadingScore = baseLineTestScore.ReadingScoreText;
                baseLineScienceScore = baseLineTestScore.ScienceScoreText;
            }

            if(selectedTestScore){
                result = {};
                bestCompositeScore = Math.max.apply(Math,model.map(function(o){return o.CompositeScoreText ? parseInt(o.CompositeScoreText) : 0;}));
                bestEnglishScore = Math.max.apply(Math,model.map(function(o){return o.EnglishScoreText ? parseInt(o.EnglishScoreText) : 0;}))
                bestMathScore = Math.max.apply(Math,model.map(function(o){return o.MathScoreText ? parseInt(o.MathScoreText) : 0;}))
                bestReadingScore = Math.max.apply(Math,model.map(function(o){return o.ReadingScoreText ? parseInt(o.ReadingScoreText) : 0;}))
                bestScienceScore = Math.max.apply(Math,model.map(function(o){return o.ScienceScoreText ? parseInt(o.ScienceScoreText) : 0;}))

                result.options = {
                    maxScore: maxScore
                };

                result.data = [];
                result.data.push({ title: 'Composite', oldValue: baseLineComponentScore, newValue: bestCompositeScore, color: '#37592A' });
                result.data.push({ title: 'English', oldValue: baseLineEnglishScore, newValue: bestEnglishScore, color: '#0d6671' });
                result.data.push({ title: 'Math', oldValue: baseLineMathScore, newValue: bestMathScore, color: '#eb6523' });
                result.data.push({ title: 'Reading', oldValue: baseLineReadingScore, newValue: bestReadingScore, color: '#5f164d' });
                result.data.push({ title: 'Science', oldValue: baseLineScienceScore, newValue: bestScienceScore, color: '#a42630' });
            }

            return result;
        }
    }

    function renderSectionHorizontalChart(objectModel, scaleRate) {
        if (!scaleRate) scaleRate = 3;
        var htmlElements = '';

        htmlElements += '<div class="box-grid">';
        htmlElements += '<table cellspacing="0" cellpadding="0" class="box-grid-table">';
        htmlElements += '<tr>';

        var step = 50;  // missing round

        for (var index = 0; index < step; index++) {
            htmlElements += '<td class="box-grid-table-line"></td>';
        }

        htmlElements += '</tr>';
        htmlElements += '</div>';
        htmlElements += '<div>';
        htmlElements += '<div></div>';
        htmlElements += '<div class="tag-name">' + objectModel.itemTag + '</div>';
        htmlElements += '<div style="font-size:8pt">';
        htmlElements += '<label>Correct:&nbsp;</label><b>' + objectModel.correctAnswer + '&nbsp;</b>';
        htmlElements += '<label>Incorrect:&nbsp;</label><b>' + objectModel.incorrectAnswer + '&nbsp;</b>';
        htmlElements += '<label>Blank:&nbsp;</label><b>' + objectModel.blankAnswer + '&nbsp;</b>';
        htmlElements += '<label>Percent:&nbsp;</label><b>' + objectModel.percent + '%</b>';
        htmlElements += '</div>';

        htmlElements += '<div class="box-chart-one">';
        htmlElements += '<div style="background-color:' + objectModel.color + ';width:' + objectModel.percent * 2 + 'px;">';
        htmlElements += '</div></div>';

        htmlElements += '<div class="box-chart-two">';
        htmlElements += '<div style="width:' + objectModel.historicalAverage * 2 + 'px;">';
        htmlElements += '</div></div>';
        htmlElements += '</div>';
        htmlElements += '<div class="box-chart-label"><label>Historical Avg:&nbsp;</label><b>' + objectModel.historicalAverage + '%</b><div>';
        return htmlElements;
    }
}