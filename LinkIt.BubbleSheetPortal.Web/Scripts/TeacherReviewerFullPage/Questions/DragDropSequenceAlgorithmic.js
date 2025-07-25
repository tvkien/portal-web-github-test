(function ($) {
    $.widget('jquery.DragDropSequenceAlgorithmic', {
        options: {
            DragDropSequenceAlgorithmicUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            var answerText = '';
            if (!options.DragDropSequenceAlgorithmicUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);

            if (!options.DragDropSequenceAlgorithmicUtil.IsNullOrEmpty(answerText)) {
                var mappings = answerText.split(',');
                tree.find('partialsequence').replaceWith(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    CopyAttributes(partialsequence, result);
                    $.each(mappings, function (idx, mappingItem) {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = partialsequence.find('sourceItem[identifier="' + mappingItem + '"]');
                        if (currentSourceItem != null) {
                            CopyAttributes(currentSourceItem, newSourceItem);
                            newSourceItem.css({ "width": currentSourceItem.attr("width"), "height": currentSourceItem.attr("height") }).html(currentSourceItem.html());
                            result.append(newSourceItem);
                        }
                    });
                    return result;
                });
            } else {
                tree.find('partialsequence').replaceWith(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    CopyAttributes(partialsequence, result);
                    partialsequence.find('sourceItem').each(function () {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = $(this);
                        CopyAttributes(currentSourceItem, newSourceItem);
                        newSourceItem.css({ "width": currentSourceItem.attr("width"), "height": currentSourceItem.attr("height") }).html(currentSourceItem.html());
                        result.append(newSourceItem);
                    });

                    return result;
                });
            }

            var questionDetails = tree.outerHTML();
            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }

            var correctAnswer = $(question.DataXmlContent()).find('correctResponse value').text();

            if (!options.DragDropSequenceAlgorithmicUtil.IsNullOrEmpty(correctAnswer)) {
                var correctMappings = correctAnswer.split(',');
                tree = $('<div></div>');
                tree.addClass('box-answer');
                tree.html(question.ItemBody());
                tree.find('partialsequence').each(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    CopyAttributes(partialsequence, result);
                    $.each(correctMappings, function (idx, mappingItem) {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = partialsequence.find('sourceItem[identifier="' + mappingItem + '"]');
                        if (currentSourceItem != null) {
                            CopyAttributes(currentSourceItem, newSourceItem);
                            newSourceItem.html(currentSourceItem.html());
                            result.append(newSourceItem);
                        }
                    });
                });
            }

            if (options.PostProcessQuestionDetails != null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
        },

        ShowAllCorrectAnswers: function (self, question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            var algorithmicCorrectAnswers = $('<div/>');
            var algorithmicPoints = [];

            ko.utils.arrayForEach(question.AlgorithmicCorrectAnswers(), function (item) {
                var mappingCorrects;
                var correctHtml;

                if (typeof item.Amount === 'function' && item.Amount() > 0) {
                    mappingCorrects = item.ConditionValue();
                    var elAtleast = Reviewer.getAtleast(item.Amount(), item.PointsEarned(), question.QTIItemSchemaID());
                    correctHtml = that.GetContentDragDropSequence(self, $(question.ItemBody()), mappingCorrects, elAtleast);
                    correctHtml.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                } else {
                    mappingCorrects = item.ConditionValue();
                    correctHtml = that.GetContentDragDropSequence(self, $(question.ItemBody()), mappingCorrects);
                    correctHtml.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                }
            });

            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },

        GetContentDragDropSequence: function (self, selector, mappingCorrects, elAtleastHtml) {
            var tree = $('<div></div>');
            tree.addClass('box-answer');
            if (elAtleastHtml != null && elAtleastHtml != '') {
                tree.append(elAtleastHtml.outerHTML);
            }
            tree.append(selector.outerHTML());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);

            tree.find('partialsequence').replaceWith(function () {
                var partialsequence = $(this);
                var result = $('<partialsequence></partialsequence>');
                CopyAttributes(partialsequence, result);
                for (var i = 0, len = mappingCorrects.length; i < len; i++) {
                    var item = $('<div class="box-answer-correct-dnd"></div>');

                    var mappings = mappingCorrects[i].split(',');
                    $.each(mappings, function (idx, mappingItem) {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = partialsequence.find('sourceItem[identifier="' + mappingItem + '"]');
                        if (currentSourceItem != null) {
                            CopyAttributes(currentSourceItem, newSourceItem);
                            newSourceItem.css({ "width": currentSourceItem.attr("width"), "height": currentSourceItem.attr("height") }).html(currentSourceItem.html());
                            item.append(newSourceItem);
                        }
                    });
                    result.append(item);
                }
                return result;

            });
            
            return tree;
        },


        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
