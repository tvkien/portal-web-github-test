(function ($) {
    $.widget('jquery.TOSQuestionRender', {
        OpenEndedWidget: null,
        TextEntryWidget: null,
        SimpleChoiceWidget: null,
        MultipleChoiceWidget: null,
        MultipleChoiceVariableWidget: null,
        InlineChoiceWidget: null,
        ComplexWidget: null,
        DragDropStandard: null,
        TextHotSpotWidget: null,
        ImageHotSpotWidget: null,
        DragDropNumerical: null,
        TableHotspot: null,
        NumberLineHotSpot: null,
        DragDropSequence: null,

        options: {
            Util: null,
            Self: null,
            TheCorrectAnswer: null,
            GuidanceAndRationale: null,
            TheQuestionContent: null
        },
        _create: function () {
            var that = this;
            var options = that.options;

            that.OpenEndedWidget = $('body').TOSOpenEnded(options);
            that.TextEntryWidget = $('body').TOSTextEntry(options);
            that.SimpleChoiceWidget = $('body').TOSSimpleChoice(options);
            that.MultipleChoiceWidget = $('body').TOSMultipleChoice(options);
            that.MultipleChoiceVariableWidget = $('body').TOSMultipleChoiceVariable(options);
            that.InlineChoiceWidget = $('body').TOSInlineChoice(options);
            that.ComplexWidget = $('body').TOSComplexItem(options);
            that.DragDropStandard = $('body').TOSDragDropStandard(options);
            that.TextHotSpotWidget = $('body').TOSTextHotspot(options);
            that.ImageHotSpotWidget = $('body').TOSImgHotspot(options);
            that.DragDropSequence = $('body').TOSDragDropSequence(options);
            that.DragDropNumerical = $('body').TOSDragDropNumerical(options);
            that.TableHotspot = $('body').TOSTableHotspot(options);
            that.NumberLineHotSpot = $('body').TOSNumberLineHotspot(options);
            that.DragDropSequence = $('body').TOSDragDropSequence(options);
        },
        Display: function (question) {
            var that = this;
            var qtiSchemaId = question.QTISchemaID;

            that._PreProcessQuestion(question);

            if (qtiSchemaId == 10) {
                that.OpenEndedWidget.TOSOpenEnded('Display', question);
            } else if (qtiSchemaId == 9) {
                that.TextEntryWidget.TOSTextEntry('Display', question);
            } else if (qtiSchemaId == 1) {
                that.SimpleChoiceWidget.TOSSimpleChoice('Display', question);
            } else if (qtiSchemaId == 3) {
                that.MultipleChoiceWidget.TOSMultipleChoice('Display', question);
            } else if (qtiSchemaId == 8) {
                that.InlineChoiceWidget.TOSInlineChoice('Display', question);
            } else if (qtiSchemaId == 37) {
                that.MultipleChoiceVariableWidget.TOSMultipleChoiceVariable('Display', question);
            } else if (qtiSchemaId == 21) {
                that.ComplexWidget.TOSComplexItem('Display', question);
            } else if (qtiSchemaId == 30) {
                that.DragDropStandard.TOSDragDropStandard('Display', question);
            } else if (qtiSchemaId == 31) {
                that.TextHotSpotWidget.TOSTextHotspot('Display', question);
            } else if (qtiSchemaId == 32) {
                that.ImageHotSpotWidget.TOSImgHotspot('Display', question);
            } else if (qtiSchemaId == 35) {
                that.DragDropNumerical.TOSDragDropNumerical('Display', question);
            } else if (qtiSchemaId == 36) {
                that.DragDropSequence.TOSDragDropSequence('Display', question);
            } else if (qtiSchemaId == 33) {
                that.TableHotspot.TOSTableHotspot('Display', question);
            } else if (qtiSchemaId == 34) {
                that.NumberLineHotSpot.TOSNumberLineHotspot('Display', question);
            } else {
                that.OpenEndedWidget.TOSOpenEnded('Display', question);
            }

            that._PostProcessQuestion(question);

            that._RenderPassages(question);
        },
        _PreProcessQuestion: function (question) {
            var that = this;
            var options = that.options;

            if (options.Self.Answers != null) {
                ko.utils.arrayForEach(options.Self.Answers, function (answer) {
                    if (question.VirtualQuestionID == answer.VirtualQuestionID) {
                        question.Answer = answer;
                    }
                });
            }

            if (question.Answer != null &&
                question.Answer.HighlightQuestion != null
                && question.Answer.HighlightQuestion != '') {
                that._ShowHightLight(question.Answer.HighlightQuestion, question);
            } else {
                question.HtmlContent = question.XmlContent;
            }

            var tree = $('<div>' + question.HtmlContent + '</div>');

            // Replace br with line break with u-pre class
            tree.find('br[style]').remove();
            tree.find('br').replaceWith(function () {
                return $('<span class="u-linebreak"/>');
            });

            tree.find('videolinkit').replaceWith(function () {
                return $('');
            });

            tree.find('assessmentitem, assessmentItem').replaceWith(function () {
                var assessmentItem = $(this);
                var newAssessmentItem = $('<div></div>');
                newAssessmentItem.html(assessmentItem.html());

                newAssessmentItem.find('itembody, itemBody').replaceWith(function () {
                    var itembody = $(this);
                    var newItemBody = $('<div></div>');
                    newItemBody.addClass('itemBody');
                    newItemBody.html(itembody.html());

                    return newItemBody;
                });

                return newAssessmentItem.html();
            });

            question.HtmlContent = tree.html();

            var treeResponseRubric = $('<div>' + question.ResponseRubric + '</div>');
            treeResponseRubric.find('videolinkit').replaceWith(function () {
                return $('');
            });
            question.ResponseRubric = treeResponseRubric.html();
        },
        _PostProcessQuestion: function (question) {
            var that = this;

            var tree = $('<div>' + question.HtmlContent + '</div>');
            tree.find('stylesheet, responsedeclaration').replaceWith(function () {
                return $('');
            });

            // Support Mathjax Teacher Feedback Print
            tree.find('div[xmlns]').removeAttr('xmlns');
            tree.find('math').removeAttr('mode');

            question.HtmlContent = tree.html();

            question.HtmlContent = that._CorrectImages(question, question.HtmlContent, 'question', null);
            question.GuidanceHTML = that._CorrectImages(question, question.GuidanceHTML, 'question', null);
            question.ResponseRubric = that._CorrectImages(question, question.ResponseRubric, 'responseRubric', null);
            that._RenderQuestion(question);
        },
        _GetDrawImage: function (type, image, drawAnswer, refObjectID) {
            var that = this;
            var options = that.options;

            var index = image.attr('index');
            var typeSelector = '[type="' + type + '"]';
            var indexSelector = '[index="' + index + '"]';
            var drawImageSelector = 'img' + typeSelector + indexSelector;
            // if (refObjectID && refObjectID != '') drawImageSelector = drawImageSelector + '[RefObjectID="' + refObjectID + '"]';
            var drawImageData = '';
            // $(drawAnswer).find(drawImageSelector).replaceWith(function () {
            //     drawImageData = $(this).attr('Data');
            // });

            var el = $(drawAnswer).find(drawImageSelector);

            if (el.length > 0) {
                for (var i = 0; i < el.length; i++) {
                    var element = el[i];
                    if (!refObjectID || (refObjectID && $(element).attr('RefObjectID') == refObjectID)) {
                        drawImageData = $(element).attr('Data');
                    }

                };
            }

            var drawImage = null;
            if (drawImageData != '') {
                drawImage = $('<img/>');
                options.Util.CopyAttributes(image, drawImage);
                drawImage.addClass("draw-data");
                drawImage.attr('src', drawImageData);
            }

            return drawImage;
        },
        _CorrectImages: function (question, content, imageLocation, refObjectID) {
            var that = this;
            var options = that.options;

            var tree = $('<div>' + content + '</div>');
            var drawableIndex = 0;
            tree.find('img').replaceWith(function () {
                var image = $(this);
                var floatVal = image.attr("float");
                var questionContentWidth = options.Self.WidthPrintByColumn;

                if (floatVal == null) floatVal = '';
                image.css("float", floatVal);

                var imageUrl = image.attr("src");
                if (imageUrl === undefined) {
                    imageUrl = image.attr("source");
                }

                if (imageUrl === null || imageUrl === '' || imageUrl === undefined) {
                    imageUrl = options.Self.MapPath + '/Content/images/emptybg.png';
                }
                if (imageUrl.charAt(0) == '/') imageUrl = imageUrl.substring(1);

                if (imageLocation === 'passage') {
                    questionContentWidth = 620;
                }

                image.attr("source", '');
                image.attr("src", imageUrl);

                if (imageUrl && (imageUrl.toLowerCase().indexOf("ro/ro") >= 0 ||
                    imageUrl.toLowerCase().indexOf("itemset") >= 0) &&
                    imageUrl.toLowerCase().indexOf("http") < 0
                    && imageUrl.toLowerCase().indexOf("getviewreferenceimg") < 0) {
                    imageUrl = options.Self.MapPath + '/Asset/GetViewReferenceImg?imgPath=' + imageUrl;
                    image.attr("src", imageUrl);
                }

                // Get images highlighted passage or passage not highlighted from html file
                if (refObjectID !== '' && refObjectID !== null) {

                    if (refObjectID.toLowerCase().indexOf('htm') >= 0 ||
                        refObjectID.toLowerCase().indexOf('html') >= 0) {
                        var regex = /(.*?)\/passages\//g;
                        var regexMatch = refObjectID.match(regex);
                        var pathUrlImage = regexMatch[0].slice(0, -10);

                        if (imageUrl.toLowerCase().indexOf('http') !== -1) {
                            pathUrlImage = '';
                        } else if (imageUrl.toLowerCase().indexOf('..') >= 0) {
                            imageUrl = imageUrl.substring(2);
                            imageUrl = pathUrlImage + imageUrl;
                        } else {
                            imageUrl = pathUrlImage + imageUrl;
                        }

                        image.attr('src', imageUrl);
                    }
                }

                imageUrl = image.attr("src");
                imageUrl = imageUrl.replace(/ /g, '%20'); //PrintPDF tool (Prince) does not understand file path includes space, so that replace space to %20 to let it understand
                image.attr("src", imageUrl);

                var wrapImg = $('<div></div>');
                var newImg = $('<img/>');
                options.Util.CopyAttributes(image, newImg);

                var newImgWidth = newImg.attr('width');
                var newImgHeight = newImg.attr('height');
                var reducePercent = 0;

                if (newImgWidth > questionContentWidth) {
                    reducePercent = (newImgWidth - questionContentWidth) / newImgWidth;
                    newImgWidth = questionContentWidth;
                    newImgHeight = newImgHeight - newImgHeight * reducePercent;

                    newImg.attr({
                        'width': newImgWidth,
                        'height': newImgHeight
                    });
                }

                wrapImg.append(newImg);
                if (question.Answer != null && question.Answer.AnswerImage != null && image.attr('drawable') == 'true') {
                    image.attr('index', drawableIndex);
                    var drawImageElement = that._GetDrawImage(imageLocation, image, question.Answer.AnswerImage, refObjectID);
                    if (drawImageElement != null) wrapImg.append(drawImageElement);
                    drawableIndex++;
                }

                // Update width and height draw interactive
                if (image.parents('.extendedTextInteractionDrawable').length > 0 ||
                    image.parents('.extendedtextinteractiondrawable').length > 0) {
                    image.parents('.extendedTextInteractionDrawable').css({
                        'width': newImgWidth + 'px',
                        'height': 'auto'
                    });

                    image.parents('.extendedtextinteractiondrawable').css({
                        'width': newImgWidth + 'px',
                        'height': 'auto'
                    });
                }

                return $(wrapImg.html());
            });

            return tree.html();
        },
        _RenderPassages: function (question) {
            var that = this;
            var options = that.options;

            var highlightPassage = null;
            if (question.Answer != null) highlightPassage = question.Answer.HighlightPassage;

            var passageContent = $('div [passageofquestionid="' + question.VirtualQuestionID + '"]');
            passageContent.html('');
            if (question.PassageTexts != null && question.PassageTexts.length > 0) {
                passageContent.css('display', 'block');
                ko.utils.arrayForEach(question.PassageTexts, function (passageText) {
                    var orginalPassageText = passageText;
                    var refObjectID = $(passageText).attr('refobjectid');

                    var highlightPassageContent = null;
                    if (highlightPassage != null) {
                        $(highlightPassage).find('passage').each(function () {
                            var passage = $(this);
                            passage.find('refobjectid').each(function () {
                                var refObjectElement = $(this);
                                if (refObjectElement.text() == refObjectID || refObjectElement.text() == refObjectID) {
                                    passage.find('passagecontent').each(function () {
                                        var $passageContent = $(this);
                                        var tree = $('<div></div>');
                                        tree.html($passageContent.text());
                                        tree.find('.highlighted').replaceWith(function () {
                                            var highlighted = $(this);

                                            var newHighlighted = $(highlighted.outerHTML());
                                            newHighlighted.css('background-color', 'yellow');

                                            return newHighlighted;
                                        });

                                        // Replace br with line break with u-pre class passage highlighted
                                        tree.find('br[style]').remove();
                                        tree.find('br').replaceWith(function () {
                                            return $('<span class="u-linebreak"/>');
                                        });

                                        highlightPassageContent = tree.html();
                                    });
                                }
                            });
                        });
                    }

                    if (highlightPassageContent != null && highlightPassageContent != '') {
                        var tree = $("<div></div>");
                        tree.html($(highlightPassageContent).html());
                        tree.addClass('passage');
                        passageText = tree.outerHTML();
                    }

                    var tree = $('<div></div>');
                    tree.html(passageText);

                    // Replace br with line break with u-pre class
                    tree.find('br[style]').remove();
                    tree.find('br').replaceWith(function () {
                        return $('<span class="u-linebreak"/>');
                    });

                    passageText = tree.html();

                    passageText = that._CorrectImages(question, passageText, 'passage', refObjectID);
                    //print passage for the first associated question
                    //if ($.inArray(orginalPassageText, that.options.Self.passageArray) < 0) {
                    if (question.StartNewPassage) {
                        //that.options.Self.passageArray.push(orginalPassageText);
                        passageContent.append(passageText);
                    } else {
                        if (highlightPassageContent != null && highlightPassageContent != '') {
                            //for the second and the other associated questions, only print passage when there's high light
                            passageContent.append(passageText);
                        }
                    }
                });
            } else {
                passageContent.css('display', 'none');
            }
        },
        _ShowHightLight: function (highlightQuestion, question) {
            if (highlightQuestion == null || highlightQuestion == '') return;

            var responseDeclarationSelector = 'responsedeclaration, responseDeclaration';
            var tree = $("<div>" + highlightQuestion + "</div>");

            // Replace br with line break with u-pre class
            tree.find('br[style]').remove();
            tree.find('br').replaceWith(function () {
                return $('<span class="u-linebreak"/>');
            });

            tree.find(responseDeclarationSelector)
                .replaceWith(function () { return $(''); });

            var correctResponseContent = '';
            $(question.XmlContent).find(responseDeclarationSelector).each(function () {
                var correctResponseElement = $(this);
                correctResponseContent = correctResponseContent + correctResponseElement.outerHTML();
            });

            tree.find('itembody, itemBody')
                .replaceWith(function () {
                    return $(correctResponseContent + $(this).outerHTML());
                });

            highlightQuestion = tree.html();
            question.HtmlContent = highlightQuestion;
        },
        _RenderQuestion: function (question) {
            var questionContent = $('div[virtualquestionid="' + question.VirtualQuestionID + '"]');
            questionContent.find('.jsQuestionContent').html(question.HtmlContent);
            questionContent.find('.jsResponseRubric').html(question.ResponseRubric);
            questionContent.find('.jsPointsPossible').html(question.PointsPossible);
            questionContent.find('outcomedeclaration').css('display', 'none');
            questionContent.find('.jsFeedbackVisible').css('display', 'none');
            questionContent.find('.jsGuidanceVisible').css('display', 'none');
            questionContent.find('.jsGuidanceVisible .guidance-printTOS').html('');

            if (question.Answer != null) {
                questionContent.find('.jsPointsEarned').html(question.Answer.PointsEarned);
                if (question.Answer.Feedback != null && question.Answer.Feedback != '') {
                    questionContent.find('.jsFeedbackVisible').css('display', 'block');
                    questionContent.find('.jsFeedback').html(question.Answer.Feedback);
                }
                else {
                    questionContent.find('.jsFeedbackVisible').css('display', 'none');
                    questionContent.find('.jsFeedback').html(question.Answer.Feedback);
                }
            } else {
                //question.Answer is null when the question had been added to the test after student's completed the test
                questionContent.find('.jsPointsEarned').html(0);
                questionContent.find('.jsFeedbackVisible').css('display', 'none');
            }

            if (question.GuidanceHTML != null && question.GuidanceHTML != '') {
                questionContent.find('.jsGuidanceVisible').css('display', 'block');
                questionContent.find('.jsGuidanceVisible .guidance-printTOS').html(question.GuidanceHTML);
            }
        }
    });
}(jQuery));