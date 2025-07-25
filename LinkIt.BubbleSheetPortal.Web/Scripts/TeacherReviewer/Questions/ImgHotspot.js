(function ($) {
    $.widget('jquery.ImgHotspot', {
        options: {
            ImgHotspotUtil: null,
            PostProcessQuestionDetails: null
        },

        Display: function (self, question) {
            //DisplayItemFeedback(0, 0, 0, 0, '', '', '', 'ImgHotspot.js-Display');
            var that = this;
            var options = that.options;
            if (self.RequireApplyGrade()) {
                AlertMessage('The points has been changed. You must apply grade.');
                return;
            }

            self.SelectedQuestion(question);
            self.RefObjects(question.RefObjects());
            self.HightLightSelectedQuestion(question);
            self.PointsPossible(question.PointsPossible());
            self.QTIItemSchemaID(question.QTIItemSchemaID());
            self.AnswerSubID('');

            var answerOfStudentForSelectedQuestion;
            ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function(testOnlineSessionAnswer) {
                if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                    answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                }
            });

            var answerText = '';
            if (!options.ImgHotspotUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerText = answerOfStudentForSelectedQuestion.AnswerText();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
                self.DisplayItemFeedback(true,answerOfStudentForSelectedQuestion);
               
            } else {
                question.XmlContent(question.DataXmlContent());
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                answerText = '';
                self.AnswerImage('');
                self.ShowHightLight('', question);
                self.DisplayItemFeedback(false, null);
            }

            // answerText = 'DEST_2,DEST_1,DEST_3,DEST_5';
            var mappingsAnswer = answerText;
            var mappingsCorrect = question.CorrectAnswer();
            var answerHtml = '';
            var correctHtml = '';
            var questionDetails = '';

            // Display answer of student
            answerHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();

            var mark = $('');
            if (!options.ImgHotspotUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview())) {
                mark = $('<i class="jsIsAnswerCorrect" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>');
                if (self.PointsEarned() == self.PointsPossible()) {
                    mark.addClass('correct');
                } else if (self.PointsEarned() > 0) {
                    mark.addClass('partial');
                } else {
                    mark.addClass('incorrect');
                }
            }

            questionDetails += mark.outerHTML();
            questionDetails += '<div class="btn-show-correct-answer big-button" onClick="ShowCorrectAnswer()">Show Correct Answer</div>';

            if (options.PostProcessQuestionDetails !== null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            self.SectionInstruction(question.SectionInstruction());

            self.LoadImages();
            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        },

        ShowCorrectAnswer: function (self, question) {
            // popupAlertMessage from ckeditor_utils
            var $contentCorrect = $(question.CorrectAnswerHTML);
            $contentCorrect.find('.highlighted').removeAttr('style').removeClass();

            question.CorrectAnswerHTML = $contentCorrect.outerHTML();
            popupAlertMessage('alert', question.CorrectAnswerHTML, 700, 500);
        },

        GetContentImgHotspot: function(self, selector, mappings) {
            var $selector = $(selector);

            // Remove if content contains question number when higlighted
            $selector.find('#questionNumber').remove();
            $selector.find('imageHotSpot').replaceWith(function() {
                var $imgHotSpot = $(this);
                var $image = $('<img/>');
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

                $result.find('sourceItem').replaceWith(function() {
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
                    if (mappings !== '') {
                        mapper = mappings.split(',');

                        for (var i = 0, lenMapper = mapper.length; i < lenMapper; i++) {
                            var mapping = mapper[i];

                            if (mapping === sourceItemIdentifier) {
                                $newSourceItem.addClass('checked');
                            }
                        }
                    }

                    return $newSourceItem;
                });

                return $result;
            });

            self.LoadImagesSelector($selector);

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
