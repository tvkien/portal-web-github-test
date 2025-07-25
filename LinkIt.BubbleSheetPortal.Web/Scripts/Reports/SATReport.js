function SATReport() {

    this.diagnosticHistoryChartData = {};
    this.scoreImprovementChartData = {};

    this.initData = function (model, sectionScoreNames) {
        this.diagnosticHistoryChartData = handlingDataForDiagnosticChart(model, sectionScoreNames);
        this.scoreImprovementChartData = convertDataForScoreImprovementChart(model, sectionScoreNames);
    }


    this.renderDiagnosticCharts = function (compId, ebrawId, mathId, readingId, writingId, subMathId) {
        var htmlComposite = SATVerticalChart(this.diagnosticHistoryChartData.compositeModel);
        $("#" + compId).html(htmlComposite);

        var htmlEbraw = SATVerticalChart(this.diagnosticHistoryChartData.EBRAWModel);
        $("#" + ebrawId).html(htmlEbraw);

        var htmlMath = SATVerticalChart(this.diagnosticHistoryChartData.mathModel);
        $("#" + mathId).html(htmlMath);

        var htmlReading = SATVerticalChart(this.diagnosticHistoryChartData.readingModel);
        $("#" + readingId).html(htmlReading);

        var htmlWriting = SATVerticalChart(this.diagnosticHistoryChartData.writingModel);
        $("#" + writingId).html(htmlWriting);

        var htmlSubMath = SATVerticalChart(this.diagnosticHistoryChartData.subMathModel);
        $("#" + subMathId).html(htmlSubMath);
    };

    this.renderScoreImprovementChart = function (id) {
        var html = renderScoreImprovementChart(this.scoreImprovementChartData);
        $("#" + id).html(html);
    }

    function handlingDataForDiagnosticChart(model, sectionScoreNames) {
        if (!model) return null;

        var selectedTestScore = _.find(model, function (item) { return item.IsSelected == true; });
        var baseLineTestScore = _.last(model);

        var maxSubScoreValue = 40;
        var maxScoreValue = 800;
        var maxCompositeValue = 1600;
        var baselineColor = 'bg-baseline';

        var reading = { title: 'Reading', newValue: 0, oldValue: 0 };
        var writing = { title: 'Writing', newValue: 0, oldValue: 0 };
        var math = { title: 'Math', newValue: 0, oldValue: 0 };
        var EBRAW = { title: 'EBRAW', newValue: 0, oldValue: 0 };
        var bigMath = { title: 'Math', newValue: 0, oldValue: 0 };
        var composite = { title: 'Composite', newValue: 0, oldValue: 0 };

        for (var index = 0; index < selectedTestScore.SubScores.length; index++) {
            var newObj = selectedTestScore.SubScores[index];
            var oldObj = baseLineTestScore.SubScores[index];

            var sectionScore = sectionScoreNames.indexOf(newObj.SectionName);

            if(!newObj) newObj = {};
            if(!oldObj) oldObj = {};

            if(sectionScore == -1){
                if (newObj.SectionName.toLowerCase().indexOf('reading') > -1) {
                    reading.title = newObj.SectionName;
                    reading.newValue = newObj.ScoreText ? parseInt(newObj.ScoreText) : 0;
                    reading.oldValue = oldObj.ScoreText ? parseInt(oldObj.ScoreText) : 0;
                } else if (newObj.SectionName.toLowerCase().indexOf('writing') > -1) {
                    writing.title = newObj.SectionName;
                    writing.newValue = newObj.ScoreText ? parseInt(newObj.ScoreText) : 0;
                    writing.oldValue = oldObj.ScoreText ? parseInt(oldObj.ScoreText) : 0;
                } else if (newObj.SectionName.toLowerCase().indexOf('math') > -1) {
                    math.title = newObj.SectionName;
                    math.newValue = newObj.ScoreText ? parseInt(newObj.ScoreText) : 0;
                    math.oldValue = oldObj.ScoreText ? parseInt(oldObj.ScoreText) : 0;
                } else if (newObj.SectionName.toLowerCase().indexOf('composite') > -1) {
                    composite.newValue = newObj.ScoreText ? parseInt(newObj.ScoreText) : 0;
                    composite.oldValue = oldObj.ScoreText ? parseInt(oldObj.ScoreText) : 0;
                }
            }
            else{
                if(sectionScoreNames[0] && newObj.SectionName.indexOf(sectionScoreNames[0]) > -1){
                    EBRAW.newValue = newObj.ScoreText ? parseInt(newObj.ScoreText) : 0;
                    EBRAW.oldValue = oldObj.ScoreText ? parseInt(oldObj.ScoreText) : 0;
                }

                if(sectionScoreNames[1] && newObj.SectionName.indexOf(sectionScoreNames[1]) > -1){
                    bigMath.newValue = newObj.ScoreText ? parseInt(newObj.ScoreText) : 0;
                    bigMath.oldValue = oldObj.ScoreText ? parseInt(oldObj.ScoreText) : 0;
                }
            }
        }

        var result = {
            readingModel: {
                name: reading.title,
                data: [
                    {
                        value: reading.newValue,
                        percent: (reading.newValue / maxSubScoreValue * 100),
                        percent1600: (reading.newValue / maxSubScoreValue * 100),
                        color: 'bg-purple', colorCode: '#60164d'
                    },
                    {
                        value: reading.oldValue,
                        percent: (reading.oldValue / maxSubScoreValue * 100),
                        percent1600: (reading.oldValue / maxSubScoreValue * 100),
                        color: baselineColor
                    }
                ],
                height: 36,
                lineNumber: 22,
                width: 140
            },
            writingModel: {
                name: writing.title,
                data: [
                    {
                        value: writing.newValue,
                        percent: (writing.newValue / maxSubScoreValue * 100),
                        percent1600: (writing.newValue / maxSubScoreValue * 100),
                        color: 'bg-blue',
                        colorCode: '#0d6671'
                    },
                    {
                        value: writing.oldValue,
                        percent: (writing.oldValue / maxSubScoreValue * 100),
                        percent1600: (writing.oldValue / maxSubScoreValue * 100),
                        color: baselineColor
                    }
                ],
                height: 36,
                lineNumber: 22,
                width: 140
            },
            subMathModel: {
                name: math.title,
                data: [
                    {
                        value: math.newValue,
                        percent: (math.newValue / maxSubScoreValue * 100),
                        percent1600: (math.newValue / maxSubScoreValue * 100),
                        color: 'bg-orange',
                        colorCode: '#eb6523'
                    },
                    {
                        value: math.oldValue,
                        percent: (math.oldValue / maxSubScoreValue * 100),
                        percent1600: (math.oldValue / maxSubScoreValue * 100),
                        color: baselineColor
                    }
                ],
                height: 36,
                lineNumber: 22,
                width: 140
            },
            EBRAWModel: {
                name: 'EBRAW',
                data: [
                    {
                        value: EBRAW.newValue,
                        percent: EBRAW.newValue / maxScoreValue * 100,
                        percent1600: EBRAW.newValue / maxCompositeValue * 100,
                        color: 'bg-red',
                        colorCode: '#a42630'
                    },
                    {
                        value: EBRAW.oldValue,
                        percent: EBRAW.oldValue / maxScoreValue * 100,
                        percent1600: EBRAW.oldValue / maxCompositeValue * 100,
                        color: baselineColor
                    }
                ],
                height: 50,
                lineNumber: 36,
                width: 180
            },
            mathModel: {
                name: 'Math',
                data: [
                    {
                        value: bigMath.newValue,
                        percent: bigMath.newValue / maxScoreValue * 100,
                        percent1600: bigMath.newValue / maxCompositeValue * 100,
                        color: 'bg-orange',
                        colorCode: '#eb6523'
                    },
                    {
                        value: bigMath.oldValue,
                        percent: bigMath.oldValue / maxScoreValue * 100,
                        percent1600: bigMath.oldValue / maxCompositeValue * 100,
                        color: baselineColor
                    }
                ],
                height: 50,
                lineNumber: 36,
                width: 180
            }
        };

        var composite = {
            name: 'Composite',
            data: [
                {
                    title: selectedTestScore.TestDateText,
                    value: composite.newValue,
                    percent: composite.newValue / maxCompositeValue * 100,
                    percent1600: composite.newValue / maxCompositeValue * 100,
                    color: 'bg-green',
                    colorCode: '#37592a'
                },
                {
                    title: baseLineTestScore.TestDateText,
                    value: composite.oldValue,
                    percent: composite.oldValue / maxCompositeValue * 100,
                    percent1600: composite.oldValue / maxCompositeValue * 100,
                    color: baselineColor,
                    isBaseLine: true
                }
            ],
            height: 90,
            lineNumber: 25,
            width: 338
        }

        result.compositeModel = composite;
        return result;
    }

    function handlingDataForImprovementChart(model, sectionScoreNames) {
        if (!model) return null;
        var selectedTestScore = _.find(model, function (item) { return item.IsSelected == true; });
        var baseLineTestScore = _.last(model);
        var composites = [];
        var ebraws = [];
        var maths = [];
        var readings = [];
        var writings = [];
        var subMaths = [];
        for (var index = 0; index < model.length; index++) {
            if(model[index].SubScores){
                for (var i = 0; i < model[index].SubScores.length; i++) {
                    var element = model[index].SubScores[i];
                    var oldObj = baseLineTestScore.SubScores[i];

                     var sectionScore = sectionScoreNames.indexOf(oldObj.SectionName);
                    if(sectionScore == -1){
                        if (oldObj.SectionName.toLowerCase().indexOf('reading') > -1) {
                            readings.push(element.Score > 40 ? 40 : element.Score);
                        } else if (oldObj.SectionName.toLowerCase().indexOf('writing') > -1) {
                            writings.push(element.Score > 40 ? 40 : element.Score);
                        } else if (oldObj.SectionName.toLowerCase().indexOf('math') > -1) {
                            subMaths.push(element.Score > 40 ? 40 : element.Score);
                        } else if (oldObj.SectionName.toLowerCase().indexOf('composite') > -1) {
                            composites.push(element.Score > 1600 ? 1600 : element.Score);
                        }
                    }
                    else{
                        if(sectionScoreNames[0] && oldObj.SectionName.indexOf(sectionScoreNames[0]) > -1){
                            ebraws.push(element.Score > 800 ? 800 : element.Score);
                        }
        
                        if(sectionScoreNames[1] && oldObj.SectionName.indexOf(sectionScoreNames[1]) > -1){
                            maths.push(element.Score > 800 ? 800 : element.Score);
                        }
                    }
                }
            }
        }

        var maxSubScoreValue = 40;
        var maxScoreValue = 800;
        var maxCompositeValue = 1600;
        var baselineColor = 'bg-baseline';

        var reading = { title: 'Reading', newValue: 0, oldValue: 0 };
        var writing = { title: 'Writing', newValue: 0, oldValue: 0 };
        var math = { title: 'Math', newValue: 0, oldValue: 0 };
        var EBRAW = { title: 'EBRAW', newValue: 0, oldValue: 0 };
        var bigMath = { title: 'Math', newValue: 0, oldValue: 0 };
        var composite = { title: 'Composite', newValue: 0, oldValue: 0 };

        for (var index = 0; index < selectedTestScore.SubScores.length; index++) {
            var newObj = selectedTestScore.SubScores[index];
            var oldObj = baseLineTestScore.SubScores[index];

            var sectionScore = sectionScoreNames.indexOf(newObj.SectionName);

            if(!newObj) newObj = {};
            if(!oldObj) oldObj = {};

            if(sectionScore == -1){
                if (newObj.SectionName.toLowerCase().indexOf('reading') > -1) {
                    reading.title = newObj.SectionName;
                    reading.newValue = Math.max.apply(Math,readings.map(function(o){return o}));
                    reading.oldValue = oldObj.Score;
                } else if (newObj.SectionName.toLowerCase().indexOf('writing') > -1) {
                    writing.title = newObj.SectionName;
                    writing.newValue = Math.max.apply(Math,writings.map(function(o){return o}));
                    writing.oldValue = oldObj.Score;
                } else if (newObj.SectionName.toLowerCase().indexOf('math') > -1) {
                    math.title = newObj.SectionName;
                    math.newValue = Math.max.apply(Math,subMaths.map(function(o){return o}));
                    math.oldValue = oldObj.Score;
                } else if (newObj.SectionName.toLowerCase().indexOf('composite') > -1) {
                    composite.newValue = Math.max.apply(Math,composites.map(function(o){return o}));
                    composite.oldValue = oldObj.Score;
                }
            }
            else{
                if(sectionScoreNames[0] && newObj.SectionName.indexOf(sectionScoreNames[0]) > -1){
                    EBRAW.newValue = Math.max.apply(Math,ebraws.map(function(o){return o}));
                    EBRAW.oldValue = oldObj.Score;
                }

                if(sectionScoreNames[1] && newObj.SectionName.indexOf(sectionScoreNames[1]) > -1){
                    bigMath.newValue = Math.max.apply(Math,maths.map(function(o){return o}));
                    bigMath.oldValue = oldObj.Score;
                }
            }
        }

        var result = {
            readingModel: {
                name: reading.title,
                data: [
                    {
                        value: reading.newValue,
                        percent: (reading.newValue / maxSubScoreValue * 100),
                        percent1600: (reading.newValue / maxSubScoreValue * 100),
                        color: 'bg-purple', colorCode: '#60164d'
                    },
                    {
                        value: reading.oldValue,
                        percent: (reading.oldValue / maxSubScoreValue * 100),
                        percent1600: (reading.oldValue / maxSubScoreValue * 100),
                        color: baselineColor
                    }
                ],
                height: 36,
                lineNumber: 22,
                width: 140
            },
            writingModel: {
                name: writing.title,
                data: [
                    {
                        value: writing.newValue,
                        percent: (writing.newValue / maxSubScoreValue * 100),
                        percent1600: (writing.newValue / maxSubScoreValue * 100),
                        color: 'bg-blue',
                        colorCode: '#0d6671'
                    },
                    {
                        value: writing.oldValue,
                        percent: (writing.oldValue / maxSubScoreValue * 100),
                        percent1600: (writing.oldValue / maxSubScoreValue * 100),
                        color: baselineColor
                    }
                ],
                height: 36,
                lineNumber: 22,
                width: 140
            },
            subMathModel: {
                name: math.title,
                data: [
                    {
                        value: math.newValue,
                        percent: (math.newValue / maxSubScoreValue * 100),
                        percent1600: (math.newValue / maxSubScoreValue * 100),
                        color: 'bg-orange',
                        colorCode: '#eb6523'
                    },
                    {
                        value: math.oldValue,
                        percent: (math.oldValue / maxSubScoreValue * 100),
                        percent1600: (math.oldValue / maxSubScoreValue * 100),
                        color: baselineColor
                    }
                ],
                height: 36,
                lineNumber: 22,
                width: 140
            },
            EBRAWModel: {
                name: 'EBRAW',
                data: [
                    {
                        value: EBRAW.newValue,
                        percent: EBRAW.newValue / maxScoreValue * 100,
                        percent1600: EBRAW.newValue / maxCompositeValue * 100,
                        color: 'bg-red',
                        colorCode: '#a42630'
                    },
                    {
                        value: EBRAW.oldValue,
                        percent: EBRAW.oldValue / maxScoreValue * 100,
                        percent1600: EBRAW.oldValue / maxCompositeValue * 100,
                        color: baselineColor
                    }
                ],
                height: 50,
                lineNumber: 36,
                width: 180
            },
            mathModel: {
                name: 'Math',
                data: [
                    {
                        value: bigMath.newValue,
                        percent: bigMath.newValue / maxScoreValue * 100,
                        percent1600: bigMath.newValue / maxCompositeValue * 100,
                        color: 'bg-orange',
                        colorCode: '#eb6523'
                    },
                    {
                        value: bigMath.oldValue,
                        percent: bigMath.oldValue / maxScoreValue * 100,
                        percent1600: bigMath.oldValue / maxCompositeValue * 100,
                        color: baselineColor
                    }
                ],
                height: 50,
                lineNumber: 36,
                width: 180
            }
        };

        var composite = {
            name: 'Composite',
            data: [
                {
                    title: selectedTestScore.TestDateText,
                    value: composite.newValue,
                    percent: composite.newValue / maxCompositeValue * 100,
                    percent1600: composite.newValue / maxCompositeValue * 100,
                    color: 'bg-green',
                    colorCode: '#37592a'
                },
                {
                    title: baseLineTestScore.TestDateText,
                    value: composite.oldValue,
                    percent: composite.oldValue / maxCompositeValue * 100,
                    percent1600: composite.oldValue / maxCompositeValue * 100,
                    color: baselineColor,
                    isBaseLine: true
                }
            ],
            height: 90,
            lineNumber: 25,
            width: 338
        }

        result.compositeModel = composite;
        return result;
    }

    function SATVerticalChart(model) {
        var htmlElement = '';
        htmlElement += '<div class="chart" style="width:' + model.width + 'px;">';
        htmlElement += '<p class="chart-title' + (model.height < 50 ? " small-chart-title" : "") + (model.data[0].title ? " mar-left-20" : "") + '">' + model.name + '</p>';

        var rightClass = '';
        if (model.data[0].title) {
            rightClass = 'right-chart-panel';

            htmlElement += '<div class="left-chart-panel">';
            for (var index = 0; index < model.data.length; index++) {
                var element = model.data[index];
                htmlElement += '<div class="chart-content-item"><p>' + element.title + (element.isBaseLine ? "<br/>Baseline" : "") + '</p></div>';
            }
            htmlElement += '</div>';
        }

        htmlElement += '<div class="chart-content ' + rightClass + '" style="height:' + model.height + 'px">';
        htmlElement += '<table class="bg-horizontal" cellspacing="0" cellpadding="0">';
        htmlElement += '<tr>';

        for (var index = 0; index < model.lineNumber; index++) {
            htmlElement += '<td></td>';
        }

        htmlElement += '</tr>';
        htmlElement += '</table>';

        for (var index = 0; index < model.data.length; index++) {
            var element = model.data[index];
            var white = index == 0 ? 'white' : '';
            htmlElement += '<div class="chart-content-item ' + element.color + '" style="width: ' + element.percent + '%">';

            var titleLenght = model.name.length * 5; // each character = 5 %
            var percent = (model.data[0].title ? model.data[0].percent + 10 : model.data[0].percent) - 5;

            if (titleLenght < percent && index == 0) {
                htmlElement += '<span class="top-label" style="background:' + model.data[0].colorCode + '">' + model.data[0].value + '</span>';
                htmlElement += '<span class="top-lable-arrow" style="border-top: 5px solid ' + model.data[0].colorCode + ';"></span>';
            } if ((index != 0 || titleLenght >= percent) && element.value > 0) {
                htmlElement += '<span class="value ' + white + '">' + element.value + '</span>';
            }

            htmlElement += '</div>';
        }

        htmlElement += '</div>';
        htmlElement += '</div>';

        return htmlElement;
    }

    function convertDataForScoreImprovementChart(model, sectionScoreNames) {
        var data = handlingDataForImprovementChart(model, sectionScoreNames);
        var result = {
            leftChart: [
                data.compositeModel, data.EBRAWModel, data.mathModel
            ],
            rightChart: [
                data.readingModel, data.writingModel, data.subMathModel
            ],
            improveScore: [
                { title: data.compositeModel.name, oldValue: data.compositeModel.data[1].value, newValue: data.compositeModel.data[0].value },
                { title: data.EBRAWModel.name, oldValue: data.EBRAWModel.data[1].value, newValue: data.EBRAWModel.data[0].value },
                { title: data.mathModel.name, oldValue: data.mathModel.data[1].value, newValue: data.mathModel.data[0].value },
                { title: data.readingModel.name, oldValue: data.readingModel.data[1].value, newValue: data.readingModel.data[0].value },
                { title: data.writingModel.name, oldValue: data.writingModel.data[1].value, newValue: data.writingModel.data[0].value },
                { title: data.subMathModel.name, oldValue: data.subMathModel.data[1].value, newValue: data.subMathModel.data[0].value },
            ]
        }

        return result;
    }

    function renderScoreImprovementChart(model) {
        var htmlElement = '';

        htmlElement += '<div class="si-chart">';
        htmlElement += '<div class="si-section si-chart-left">';
        htmlElement += '<div class="bg">';
        htmlElement += ' <label class="bg-label label-top">1600</label>';
        htmlElement += '<label class="bg-label label-center">800</label>';
        htmlElement += '<label class="bg-label label-bottom">0</label>';

        htmlElement += '<table cellpadding="0" cellspacing="0">';
        for (var index = 0; index < 20; index++) {
            if(index == 9){
                htmlElement += '<tr><td style="border-bottom: 1px dashed #000000;"></td></tr>';
            }
            else{
                htmlElement += '<tr><td></td></tr>';
            }
            
        }
        htmlElement += '</table>';
        htmlElement += ' </div>';

        htmlElement += '<div>';

        htmlElement += renderBarChartItem(model.leftChart);

        htmlElement += '</div>';
        htmlElement += ' </div>';
        htmlElement += '<div class="si-section si-table">';
        htmlElement += '<table cellpadding="0" cellspacing="0">';
        htmlElement += '<tr>';
        htmlElement += '<th colspan="2" style="text-align:right">Baseline</th>';
        htmlElement += '<th>Best</th>';
        htmlElement += '<th>Improve</th>';
        htmlElement += '</tr>';

        for (var index = 0; index < model.improveScore.length; index++) {
            var element = model.improveScore[index];
            var improve = element.newValue - element.oldValue;
            if (improve >= 0) {
                improve = "+" + improve.toString();
            }

            htmlElement += '<tr>';
            htmlElement += '<td>' + element.title + '</td>';
            htmlElement += '<td>' + element.oldValue + '</td>';
            htmlElement += '<td>' + element.newValue + '</td>';
            htmlElement += ' <td>' + improve + '</td>';
            htmlElement += '</tr>';
        }

        htmlElement += '</table>';
        htmlElement += '<div class="small-note-group">';
        htmlElement += '<div class="small-note" style="background-color:#37592b"></div>';
        htmlElement += '<div class="small-note" style="background-color:#a42630"></div>';
        htmlElement += '<div class="small-note" style="float:left;background-color:#eb6523"></div>';
        htmlElement += '<div class="small-note" style="float:left;background-color:#5f164e"></div>';
        htmlElement += '<div class="small-note" style="float:left;background-color:#056671"></div>';
        htmlElement += '<div class="small-note-title"> Best Score</div>';
        htmlElement += '</div>';
        htmlElement += '<div class="small-note-group">';
        htmlElement += '<div class="small-note-baseline"></div>';
        htmlElement += '<div class="small-note-title"> Baseline Score</span>';
        htmlElement += '</div>';
        htmlElement += '</div>';
        htmlElement += '</div>';

        htmlElement += '<div class="si-section si-chart-right">';
        htmlElement += '<div class="bg">';
        htmlElement += ' <label class="bg-label label-top">40</label>';
        htmlElement += '<label class="bg-label label-bottom">0</label>';

        htmlElement += '<table cellpadding="0" cellspacing="0">';
        for (var index = 0; index < 20; index++) {
            htmlElement += '<tr><td></td></tr>';
        }
        htmlElement += '</table>';
        htmlElement += ' </div>';

        htmlElement += '<div>';

        htmlElement += renderBarChartItem(model.rightChart);

        htmlElement += '</div>';
        htmlElement += '</div>';
        htmlElement += '</div>';

        return htmlElement;
    }

    function renderBarChartItem(model) {
        var htmlElement = '';
        for (var index = 0; index < model.length; index++) {
            var element = model[index];
            var improve = element.data[0].value - element.data[1].value;

            if (improve > 0) {
                improve = "+" + improve.toString();
            }

            var improvePercent = element.data[0].percent1600 - element.data[1].percent1600;
            var hideValue = improvePercent > -17 && improvePercent < 2; // calculate base on UI view

            htmlElement += '<div class="chart-group">';
            htmlElement += '<div class="chart-item ' + element.data[0].color + '" style="height:' + element.data[0].percent1600 + '%">';
            htmlElement += '<label class="improve-score ' + element.data[0].color.replace('bg-', 'color-') + '">' + improve + '</label>';

            if (hideValue) {
                htmlElement += '<label class="white">' + element.data[0].value + '</label>';
            }
            else if (element.data[0].value > 0) {
                htmlElement += '<div class="chart-item-value up ' + element.data[0].color + '">' + element.data[0].value;
                htmlElement += '<div class="arrow-up"></div>';
                htmlElement += '</div>';
            }

            htmlElement += '</div>';
            htmlElement += '<div class="chart-item baseline" style="height:' + element.data[1].percent1600 + '%">';

            if (element.data[1].value > 0) {
                htmlElement += '<div class="chart-item-value down bg-baseline">' + element.data[1].value;
                htmlElement += '<div class="arrow-down"></div>';
                htmlElement += '</div>';
            }

            htmlElement += '</div>';
            htmlElement += '<p class="chart-label">' + element.name + '</p>';
            htmlElement += ' </div>';
        }

        return htmlElement;
    }
};