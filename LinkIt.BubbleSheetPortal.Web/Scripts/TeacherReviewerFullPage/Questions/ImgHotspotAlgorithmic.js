(function ($) {
    $.widget('jquery.ImgHotspotAlgorithmic', {
        options: {
            ImgHotspotAlgorithmicUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            // answerText = 'DEST_2,DEST_1,DEST_3,DEST_5';
            var answerText = '';
            var mappingsAnswer = [];
            var mappingsCorrect = [];
            var answerHtml = '';
            var correctHtml = '';
            var questionDetails = '';

            // Update answer text
            if (!options.ImgHotspotAlgorithmicUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.ImgHotspotAlgorithmicUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer.push(answerText);
            }

            // Update correct answer
            if (!options.ImgHotspotAlgorithmicUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect.push(question.CorrectAnswer());
            }

            // Display answer of student
            answerHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();

            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }

            if (options.PostProcessQuestionDetails !== null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
        },

        ShowCorrectAnswer: function (self, question) {
            var $contentCorrect = $(question.CorrectAnswerHTML);
            $contentCorrect.find('.highlighted').removeAttr('style').removeClass();

            question.CorrectAnswerHTML = $contentCorrect.outerHTML();
            Reviewer.popupAlertMessage(question.CorrectAnswerHTML, 'ui-popup-fullpage', 700, 500);
            self.ReviewerWidget.ReviewerWidget('LoadImages', $('.ui-popup-fullpage'));
        },

        ShowAllCorrectAnswers: function (self, question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            var algorithmicCorrectAnswers = $('<div/>');
            var algorithmicPoints = [];

            ko.utils.arrayForEach(question.AlgorithmicCorrectAnswers(), function (item) {
                var correctAnswer;
                var tree;
                var correctHtml;

                if (typeof item.Amount === 'function' && item.Amount() > 0) {
                    correctAnswer = item.ConditionValue();
                    correctHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), correctAnswer);
                    tree = $('<div/>');
                    tree.addClass('box-answer');
                    var elAtleast = Reviewer.getAtleast(item.Amount(), item.PointsEarned(), question.QTIItemSchemaID());
                    tree.append(elAtleast.outerHTML);
                    correctHtml.appendTo(tree);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                } else {
                    correctAnswer = item.ConditionValue();
                    correctHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), correctAnswer);
                    tree = $('<div/>');
                    tree.addClass('box-answer');
                    correctHtml.appendTo(tree);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                }
            });

            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500);
            self.ReviewerWidget.ReviewerWidget('LoadImages', $('.ui-popup-fullpage'));
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },


        GetContentImgHotspot: function (self, selector, mappings) {
            var $selector = $(selector);

            // Remove if content contains question number when higlighted
            $selector.find('#questionNumber').remove();
            $selector.find('imageHotSpot').replaceWith(function () {
                var $imgHotSpot = $(this);
                var $image = $('<img />');
                var $result = $('<div class="imageHotspotInteraction"/>');

                $image
                    .attr('src', $imgHotSpot.attr('src'))
                    .css({
                        'width': $imgHotSpot.attr('width') + 'px',
                        'height': $imgHotSpot.attr('height') + 'px'
                    });

                $result
                    .attr('responseIdentifier', $imgHotSpot.attr('responseIdentifier'))
                    .css({
                        'width': $imgHotSpot.attr('width') + 'px',
                        'height': $imgHotSpot.attr('height') + 'px'
                    });

                $result.html($imgHotSpot.html());
                $result.prepend($image);

                $result.find('sourceItem').replaceWith(function () {
                    var $sourceItem = $(this);
                    var sourceItemTop = $sourceItem.attr('top');
                    var sourceItemLeft = $sourceItem.attr('left');
                    var sourceItemWidth = $sourceItem.attr('width');
                    var sourceItemHeight = $sourceItem.attr('height');
                    var sourceItemType = $sourceItem.attr('typeHotSpot');
                    var sourceItemIdentifier = $sourceItem.attr('identifier');
                    var sourceItemShowBorder = $sourceItem.attr('showBorderHotSpot');
                    var sourceItemFill = $sourceItem.attr('fillHotSpot');
                    var sourceItemRollover = $sourceItem.attr('rolloverPreviewHotSpot');

                    var $newSourceItem = $('<span class="hotspot-item-type"/>');
                    var mapper = [];
                    $newSourceItem.html('<span class="hotspot-item-value">' + $sourceItem.html() + '</span>');
                    $newSourceItem
                        .attr({
                            'typehotspot': sourceItemType,
                            'identifier': sourceItemIdentifier,
                            'showborderhotspot': sourceItemShowBorder,
                            'fillhotspot': sourceItemFill,
                            'rolloverhotspot': sourceItemRollover
                        })
                        .css({
                            'position': 'absolute',
                            'top': sourceItemTop + 'px',
                            'left': sourceItemLeft + 'px',
                            'width': sourceItemWidth + 'px',
                            'height': sourceItemHeight + 'px',
                            'line-height': sourceItemHeight + 'px'
                        });

                    // Check answer from student or correct answer
                    if (mappings.length > 0) {
                        for (var i = 0, len = mappings.length; i < len; i++) {
                            mapper = mappings[i].split(',');
                            for (var j = 0, lenMapper = mapper.length; j < lenMapper; j++) {
                                var mapping = mapper[j];

                                if (mapping === sourceItemIdentifier) {
                                    $newSourceItem.addClass('checked');
                                }
                            }
                        }
                    }

                    return $newSourceItem;
                });

                return $result;
            });

            return $selector;
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
