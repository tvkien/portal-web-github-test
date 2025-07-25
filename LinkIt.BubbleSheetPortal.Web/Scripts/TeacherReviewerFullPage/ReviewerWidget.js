(function ($) {
  $.widget('jquery.ReviewerWidget', {
    options: {
      WidgetUtil: null,
      GetStudentsForAssignmentUrl: '',
      GetQuestionsForAssignmentUrl: '',
      GetTestOnlineSessionAnswersURL: '',
      UpdateAnswerTextURL: '',
      UpdateAnswerPointsEarnedURL: '',
      GetViewReferenceContentURL: '',
      GetViewReferenceImgFullPath: 'TestAssignmentRegrader/GetViewReferenceImgFullPath?imgPath=',
      PrintTestOfStudentURL: '',
      SaveFeedbackOverallURL: '',
      SaveFeedbackQuestionURL: '',
      SubmitTestURL: '',
      DownloadRubricFileURL: '',
      GetRubricByVirtualTestURL: '',
      GetOverrideAutoGradedOfAssignmentURL: '',
      GetNextApplicableStudentURL: '',
      ViewBatchPrintingURL: '',
      UpdateAutoSaveToAnswerTextURL: '',
      ViewAttachmentUrl: '',
      DownloadAudioFileUrl: ''
    },
    GetStudentsForAssignment: function (qtiTestClassAssignmentID, studentId, successCallBack, errorCallBack) {
      var that = this;
      var options = that.options;

      $.ajax({
        type: 'GET',
        url: options.GetStudentsForAssignmentUrl,
        cache: false,
        data: {
          qtiTestClassAssignmentID: qtiTestClassAssignmentID,
          studentId: studentId
        },
        datatype: 'json',
        success: function (data) {
          if (!options.WidgetUtil.IsNullOrEmpty(successCallBack)) {
            successCallBack(data);
          }
        },
        error: function (data) {
          if (!options.WidgetUtil.IsNullOrEmpty(errorCallBack)) {
            errorCallBack(data);
          }
        }
      });
    },
    GetQuestionsForAssignment: function (virtualTestID, qtiTestClassAssignmentID, districtId, successCallBack, errorCallBack) {
      var that = this;
      var options = that.options;
      var $assignmentFullPage = $('body');

      ShowBlock($assignmentFullPage, 'Loading');

      $.ajax({
        type: 'GET',
        url: options.GetQuestionsForAssignmentUrl,
        cache: false,
        data: {
          virtualTestID: virtualTestID,
          qtiTestClassAssignmentID: qtiTestClassAssignmentID,
          moduleCode: RestrictionModule.MANUAL_GRADE,
          districtId: districtId
        },
        datatype: 'json',
        success: function (data) {
          $assignmentFullPage.unblock();
          $assignmentFullPage.removeAttr('style');
          if (!options.WidgetUtil.IsNullOrEmpty(successCallBack)) {
            successCallBack(data);
          }
        },
        error: function (data) {
          $assignmentFullPage.unblock();
          $assignmentFullPage.removeAttr('style');
          if (!options.WidgetUtil.IsNullOrEmpty(errorCallBack)) {
            errorCallBack(data);
          }
        }
      });
    },
    GetTestOnlineSessionAnswers: function (qtiOnlineTestSessionID, qtiTestClassAssignmentID, startDate, loadingMsg, successCallBack, errorCallBack) {
      var that = this;
      var options = that.options;
      var $assignmentFullPage = $('body');

      ShowBlock($assignmentFullPage, loadingMsg);

      $.ajax({
        type: 'GET',
        url: options.GetTestOnlineSessionAnswersURL,
        cache: false,
        data: {
          qtiOnlineTestSessionID: qtiOnlineTestSessionID,
          qtiTestClassAssignmentID: qtiTestClassAssignmentID,
          startDate: startDate
        },
        datatype: 'json',
        success: function (data) {
          $assignmentFullPage.unblock();
          $assignmentFullPage.removeAttr('style');
          if (!options.WidgetUtil.IsNullOrEmpty(successCallBack)) {
            successCallBack(data);
          }
        },
        error: function (data) {
          $assignmentFullPage.unblock();
          $assignmentFullPage.removeAttr('style');
          if (!options.WidgetUtil.IsNullOrEmpty(errorCallBack)) {
            errorCallBack(data);
          }
        }
      });
    },
    GetViewReferenceContent: function (answerImage, refObjectID, refObjectData) {
      var that = this;
      var options = that.options;

      var url = options.GetViewReferenceContentURL + '?refObjectID=' + refObjectID + '&data=' + refObjectData;

      $.ajax({
        url: url,
        cache: false,
        data: {
          refObjectID: refObjectID,
          data: refObjectData
        }
      }).done(function (content) {
        that.ShowReferenceContent(content, answerImage, refObjectID, refObjectData);
      });
    },
    ShowReferenceContent: function (content, answerImage, refObjectID, refObjectData) {
      var that = this;
      var passageHtml = '';
      var passageContent = '';
      var $passage = $('<div/>');

      $passage.html(content);

      // Remove audio in passage
      $passage.find('.testtaker-audio').remove();

      if ($passage.text().trim() === '' && !$passage.find('img').length) {
        passageContent = '<div class="reference-passage--notfound u-text-center">The Passage #' + refObjectID + ' Not Found.</div>';
      } else {
        passageContent = $passage.html();
      }

      passageHtml = '<div id="referencePassage">' + passageContent + '</div>';

      // Call popup show reference(passage)
      Reviewer.popupAlertMessage(passageHtml, 'ui-popup-fullpage', 700, 500);

      var $referencePassage = $(document).find('#referencePassage');
      var $referencePassageImages = $referencePassage.find('img');
      var count = 0;

      // Show URL Images
      $referencePassageImages.each(function (ind, img) {
        var $img = $(img);
        var imgFloat = $img.attr('float');
        var imageUrl = $img.attr('src');

        if (!Reviewer.IsNullOrEmpty(refObjectData)) {
          if (imageUrl.indexOf('http') == -1) {
            var imagePath = refObjectData.substring(0, refObjectData.lastIndexOf('/'));
            imagePath = imagePath.substring(0, imagePath.lastIndexOf('/'));
            imageUrl = imageUrl.substring(imageUrl.indexOf('/') + 1);
            imageUrl = imagePath + '/' + imageUrl;
          }
        } else {
          if (imageUrl == null) {
            imageUrl = '';
          }

          if (imageUrl.indexOf('http') !== 0 && imageUrl.indexOf('data:image') !== 0) {
            var S3Url = window.S3Domain;

            if (S3Url.slice(-1) !== '/') {
              S3Url += '/';
            }

            if (imageUrl.charAt(0) === '/') {
              imageUrl = imageUrl.slice(1);
            }

            imageUrl = S3Url + imageUrl;
          }
        }

        // Get images highlighted passage or passage not highlighted from html file
        if (refObjectData !== '' && refObjectData !== null) {
          if (refObjectData.toLowerCase().indexOf('htm') >= 0 ||
            refObjectData.toLowerCase().indexOf('html') >= 0) {
            var regex = /(.*?)\/passages\//g;
            var regexMatch = refObjectData.match(regex);
            var pathUrlImage = regexMatch[0].slice(0, -10);

            if (imageUrl.toLowerCase().indexOf('http') !== -1) {
              pathUrlImage = '';
            } else if (imageUrl.toLowerCase().indexOf('..') >= 0) {
              imageUrl = imageUrl.substring(2);
              imageUrl = pathUrlImage + imageUrl;
            } else {
              imageUrl = pathUrlImage + imageUrl;
            }

            $img.attr('src', imageUrl);
          }
        }

        $img.attr('src', imageUrl).css('float', imgFloat);
      });

      // Load Images
      $referencePassageImages.imagesLoaded(function () {
        $referencePassageImages.each(function (ind, img) {
          var $img = $(img);
          if ($img.attr('drawable') === 'true') {
            $img.unbind('load');
            $img.attr('index', count);
            that.ShowDrawImage('passage', $img, answerImage, refObjectID);
            count++;
          }
        });
      });

      // Load MathJax
      MathJax.Hub.Queue(['Typeset', MathJax.Hub]);
    },
    ViewAttachment: function (documentGuid, qtiOnlineTestSessionId, canPlayback) {

      var $assignmentFullPage = $('body');
      ShowBlock($assignmentFullPage, 'Downloading');

      var that = this;
      var options = that.options;

      var url = options.ViewAttachmentUrl + '?documentGuid=' + documentGuid + '&qtiOnlineTestSessionId=' + qtiOnlineTestSessionId;

      $.ajax({
        url: url,
        cache: false,
        data: {
          documentGuid,
          qtiOnlineTestSessionId
        }
      }).done(function (attachment) {
        if (attachment.FileContent && attachment.FileContent.length) {
          if (canPlayback) {
            var now = new Date().getTime();
            var contentHtml = '';
            var $div = $('<div />');
            var $doc = $(document);

            contentHtml += '<div class="popup-fullpage">';
            contentHtml += '<div class="popup-fullpage-content">';
            contentHtml += '<p style="margin-bottom: 4px;">' + attachment.FileName + '</p>';
            contentHtml += '</div>';
            contentHtml += '<div class="popup-fullpage-controls">';
            contentHtml += '<button class="btn-close">Close</button>';
            contentHtml += '</div>';
            contentHtml += '</div>';

            var url = that.getObjectURL(attachment.FileContent, attachment.FileName);
            var audioDom = new AudioPlayer(url, { removeable: false });
            $div.html(contentHtml).find('.popup-fullpage-content').append(audioDom);
            $div.attr('id', 'popup-fullpage-' + now)
              .appendTo('body')
              .dialog({
                modal: true,
                width: 500,
                maxHeight: 400,
                resizable: false,
                dialogClass: 'ui-popup-fullpage ui-popup-fullpage-nostudent',
                close: function () {
                  $doc.find('#popup-fullpage-' + now).dialog('destroy').remove();
                }
              });
            $div.on('click', '.btn-close', function () {
              $doc.find('#popup-fullpage-' + now).dialog('destroy').remove();
            });
          } else {
            that.SaveAttachmentToFile(attachment);
          }
        } else {
          window.open(
            attachment.ViewDsServerLink,
            '_blank'
          );
        }

        $assignmentFullPage.unblock();
        $assignmentFullPage.removeAttr('style');
      });
    },
    SaveAttachmentToFile: function (file) {
      var url = this.getObjectURL(file.FileContent, file.FileName);
      var link = document.createElement('a');
      link.href = url;
      link.download = file.FileName;
      link.click();
    },
    getObjectURL: function (fileContent, ext) {
      ext = ext.split('.').pop()
      var bytes = new Uint8Array(fileContent);
      var blob = new Blob([bytes], { type: 'audio/' + ext });
      return window.URL.createObjectURL(blob);
    },
    ShowDrawImage: function (type, image, drawAnswer, refObjectID) {
      // if (image.parents('extendedtextinteraction[data-type="basic"]').length) {
      //     return;
      // }

      var index = image.attr('index');
      var typeSelector = '[Type="' + type + '"]';
      var indexSelector = '[Index="' + index + '"]';
      var drawImageSelector = 'DrawImg' + typeSelector + indexSelector;

      if (refObjectID && refObjectID !== '') {
        drawImageSelector = drawImageSelector + '[RefObjectID="' + refObjectID + '"]';
      }

      // Correct image incase image has width or height is NaN
      var imageNewHeight = image.height();
      var imageNewWidth = image.width();
      var imageHeight = image.attr('height');
      var imageWidth = image.attr('width');
      var imagePercent = image.attr('percent');

      if (imageHeight === undefined || imageHeight.toString() === 'NaN') {
        if (imagePercent !== undefined) {
          imageNewHeight = (imageNewHeight * parseInt(imagePercent.toString() + '0')) / 100;
          image.attr({
            'height': imageNewHeight
          });
        } else {
          image.attr({
            'height': imageNewHeight
          });
        }
      }

      if (imageWidth === undefined || imageWidth.toString() === 'NaN') {
        if (imagePercent !== undefined) {
          imageNewWidth = (imageNewWidth * parseInt(imagePercent.toString() + '0')) / 100;
          image.attr({
            'width': imageNewWidth
          });
        } else {
          image.attr({
            'width': imageNewWidth
          });
        }
      }

      var canvasID = type + 'Canvas' + index;
      var canvas;

      if (!($('#' + canvasID).length > 0)) {
        var margin = 'margin: 0px;';
        var canvasWidth = image.width();
        var canvasHeight = image.height();

        if (image.parent() !== undefined &&
          image.parent().hasClass('center')) {
          margin = ' margin: 0px auto;';
        }

        image.wrap('<div class="divdraw" style="width: ' + canvasWidth + 'px; height: ' + canvasHeight + 'px; ' + margin + 'position:relative;"></div>');
        canvas = $('<canvas width="' + canvasWidth + '" height="' + canvasHeight + '"></canvas>');

        canvas.attr('id', canvasID);
        image.after(canvas);
      }

      canvas = document.getElementById(canvasID);
      var context = canvas.getContext('2d');
      var emptyImg = context.createImageData(canvas.width, canvas.height);
      context.putImageData(emptyImg, 0, 0);

      if (drawAnswer === '') {
        return;
      }
      var drawImageData = '';

      $(drawAnswer).find(drawImageSelector).each(function () {
        drawImageData = $(this).attr('Data');
      });

      if (drawImageData === '') {
        return;
      }

      var drawImageElement = new Image();
      drawImageElement.onload = function () {
        canvas.width = drawImageElement.width;
        canvas.height = drawImageElement.height;
        context.drawImage(drawImageElement, 0, 0);
        image.width = canvas.width;
        image.height = canvas.height;
        image.parent().width = canvas.width;
        image.parent().height = canvas.height;
        image.parents('extendedtextinteraction').width = canvas.width;
        image.parents('extendedtextinteraction').height = canvas.width;
      };

      drawImageElement.src = drawImageData;

      $('extendedtextinteraction[notreviewed="true"]').find('canvas').addClass('red-border');
      $('extendedtextinteraction[notreviewed="false"]').find('canvas').removeClass('red-border');
      //for complex-item
      $('.box-answer').each(function (ind, boxanswer) {
        var $boxanswer = $(boxanswer);
        var countRevied = $boxanswer.find('img[isreviewed="true"]').length;

        if (countRevied > 0) {
          $boxanswer.find('canvas').removeClass('red-border');
        }

        var countUnrevied = $boxanswer.find('img[isreviewed="false"]').length;

        if (countUnrevied > 0) {
          $boxanswer.find('canvas').addClass('red-border');
        }
      });
    },
    WarningUnsavedAnswer: function (unsavedStudentAnswer) {
      if (!unsavedStudentAnswer.UnSaved) return;
      var options = {
        Message: 'There are unsaved changes to question ' + unsavedStudentAnswer.Question.QuestionOrder() + '. Would you like to keep them?',
        HideCloseButton: 1
      };
      ConfirmSubmitTest(options, function () {
        self.UpdateAnswerText(unsavedStudentAnswer, true);
      }, function () {
        self.UpdateAnswerText(unsavedStudentAnswer, false);
      });
    },
    UpdateAnswerText: function (unsavedStudentAnswer, saved) {
      var answerID = unsavedStudentAnswer.Answer == null ? null : unsavedStudentAnswer.Answer.QTIOnlineTestSessionAnswerID();
      var answerSubID = unsavedStudentAnswer.AnswerSub == null ? null : unsavedStudentAnswer.AnswerSub.QTIOnlineTestSessionAnswerSubID();
      $.ajax({
        type: 'POST',
        data: {
          answerID: answerID,
          answerSubID: answerSubID,
          saved: saved
        },
        async: false,
        dataType: 'json',
        url: UpdateAnswerTextURL
      })
        .success(function (result) {
          if (!saved) return;
          if (unsavedStudentAnswer.AnswerSub != null) {
            unsavedStudentAnswer.AnswerSub.AnswerText(unsavedStudentAnswer.AnswerSub.AnswerTemp());
            unsavedStudentAnswer.AnswerSub.Answered(true);
          } else if (unsavedStudentAnswer.Answer != null) {
            unsavedStudentAnswer.Answer.AnswerText(unsavedStudentAnswer.Answer.AnswerTemp());
            unsavedStudentAnswer.Answer.Answered(true);
          }

          if (unsavedStudentAnswer.Question != null) {
            unsavedStudentAnswer.Question.Answered(true);
            unsavedStudentAnswer.Question.Unanswered(false);
          }
        })
        .error(function (error) {
        });
    },
    FormatStudentInSelect2: function (studentOption) {
      var studentVisible = $(studentOption.element).attr('data-visible');
      if (studentVisible != 'true') {
        return null;
      }

      var $student;
      var studentStatus = $(studentOption.element).attr('data-status');
      var studentHtml = '';

      if (!studentOption.id || studentOption.id == -1) {
        return studentOption.text;
      }

      studentHtml += '<div class="assignment-list-student">';
      studentHtml += '<span class="assignment-list-student-icon"><span class="icon icon-' + studentStatus + '"></span></span>';
      studentHtml += '<span class="assignment-list-student-name">' + studentOption.text + '</span></div>';
      studentHtml += '</div>';

      $student = $(studentHtml);

      return $student;
    },
    FormatQuestionInSelect2: function (questionOption) {
      var questionVisible = $(questionOption.element).attr('data-visible');
      if (questionVisible != 'true') {
        return null;
      }

      var $question;
      var questionHtml = '';

      if (!questionOption.id || questionOption.id == -1) {
        return questionOption.text;
      }

      questionHtml += '<div class="assignment-list-student">';
      questionHtml += '<span class="assignment-list-student-name">Item ' + questionOption.text + '</span></div>';
      questionHtml += '</div>';

      $question = $(questionHtml);

      return $question;
    },
    CreateQuestionMenuItem: function (html) {
      var tree = $(html);

      var replaces = [{
        from: 'inlinechoiceinteraction',
        to: '<select class="teacher-review-inline-choice"></select>'
      },
      {
        from: 'textentryinteraction',
        to: '<input type="text" class="teacher-review-fill-in-blank"></input>'
      },
      {
        from: 'choiceinteraction',
        to: ''
      },
      {
        from: 'sourceobject',
        to: ''
      },
      {
        from: 'destinationobject',
        to: ''
      },
      {
        from: 'imagehotspot',
        to: ''
      },
      {
        from: 'numberline',
        to: ''
      },
      {
        from: 'videolinkit',
        to: ''
      },
      {
        from: 'table.tableHotspotInteraction',
        to: ''
      }
      ];

      ko.utils.arrayForEach(replaces, function (replace) {
        tree.find(replace.from).replaceWith(function () {
          return replace.to;
        });
      });

      return tree.outerHTML();
    },
    Plus: function (self) {
      if (!self.CanGrading()) {
        return;
      }

      self.PointsEarned(Reviewer.ParseInt(self.PointsEarned()));

      if (self.PointsEarned() >= 0 &&
        self.PointsPossible() >= 0 &&
        self.PointsEarned() < self.PointsPossible()) {
        self.PointsEarned(parseInt(self.PointsEarned()) + 1);
      }
    },
    Minus: function (self) {
      if (!self.CanGrading()) {
        return;
      }

      self.PointsEarned(Reviewer.ParseInt(self.PointsEarned()));

      if (self.PointsEarned() > 0 && self.PointsPossible() >= 0) {
        self.PointsEarned(parseInt(self.PointsEarned()) - 1);
      }
    },

    UpdateAsnswerPointsEarned: function (self) {
      var that = this;
      var options = that.options;
      var isPendingReview = self.IsPendingReview() && !self.SelectedStudent.CanBulkGrading();

      if (!(self.QTIOnlineTestSessionID() > 0 && self.AnswerID() > 0 &&
        self.PointsEarned() >= 0)) {
        return;
      }

      self.PointsEarned(parseInt(self.PointsEarned()));

      var pointsEarned = $('.PointsEarned').val();
      if (pointsEarned.length > 0) {
        if (parseInt(pointsEarned) > parseInt(self.PointsPossible())) {
          $('.PointsEarned').val(self.PointsPossible());
          self.PointsEarned(parseInt(self.PointsPossible()));
          return;
        }
      }

      self.BlockUI();

      var virtualQuestionID = self.SelectedQuestion().VirtualQuestionID();
      var postModel = {
        qtiOnlineTestSessionID: self.QTIOnlineTestSessionID(),
        answerID: self.AnswerID(),
        answerSubID: self.AnswerSubID(),
        pointsEarned: self.PointsEarned(),
        virtualQuestionID: virtualQuestionID,
        rubricTestResultScores: self.RubricTestResultScores(),
      };

      $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        url: options.UpdateAnswerPointsEarnedURL,
        data: JSON.stringify(postModel),
        traditional: true,

        success: function (response, status, xhr) {
          var reApplyGrade = self.SelectedQuestion().IsReviewed();
          if (!response.success) {
            var reloadStudentID = self.SelectedStudentID();
            self.SelectedStudentID(-1);
            self.SelectedStudentID(reloadStudentID);
            self.CanGradingForceUIRefresh();
            return;
          }

          // Assign PointsEarned to Selected Tier Manually
          self.SelectedQuestion().AssignSelectedTier();

          if (
            !self.IsComplete() ||
            self.QTIItemSchemaID() != 9 ||
            !self.OverrideAutoGraded()
          ) {
            self.IsGraded(true);
          }

          var updatedAnswer = ko.utils.arrayFirst(
            self.TestOnlineSessionAnswers(),
            function (answer) {
              return answer.QTIOnlineTestSessionAnswerID() == self.AnswerID();
            }
          );
          var isReviewed = true;
          var answerSubID;
          if (!options.WidgetUtil.IsNullOrEmpty(updatedAnswer)) {
            updatedAnswer.UpdatedBy(self.DisplayName());
            updatedAnswer.UpdatedDate(response.updatedDate);
            updatedAnswer.Overridden(true);
            self.UpdatedBy(self.DisplayName());
            self.UpdatedDate(response.updatedDate);
            self.Overridden(true);

            if (options.WidgetUtil.IsNullOrEmpty(self.AnswerSubID())) {
              updatedAnswer.PointsEarned(self.PointsEarned());
              updatedAnswer.IsReviewed(true);
            } else {
              answerSubID = self.AnswerSubID();
              var updatedAnswerSub = ko.utils.arrayFirst(
                updatedAnswer.TestOnlineSessionAnswerSubs(),
                function (answerSub) {
                  return (
                    answerSub.QTIOnlineTestSessionAnswerSubID() == self.AnswerSubID()
                  );
                }
              );
              if (!options.WidgetUtil.IsNullOrEmpty(updatedAnswerSub)) {
                var answerPointsEarned =
                  updatedAnswer.PointsEarned() -
                  updatedAnswerSub.PointsEarned() +
                  self.PointsEarned();
                if (answerPointsEarned < 0) answerPointsEarned = 0;
                updatedAnswer.PointsEarned(answerPointsEarned);
                updatedAnswerSub.PointsEarned(self.PointsEarned());
                updatedAnswerSub.IsReviewed(true);
                updatedAnswerSub.UpdatedBy(self.DisplayName());
                updatedAnswerSub.UpdatedDate(response.updatedDate);
                updatedAnswerSub.Overridden(true);
              }
              var notYetReviewedAnswerSub = ko.utils.arrayFirst(
                updatedAnswer.TestOnlineSessionAnswerSubs(),
                function (answerSub) {
                  return !answerSub.IsReviewed();
                }
              );
              if (options.WidgetUtil.IsNullOrEmpty(notYetReviewedAnswerSub))
                updatedAnswer.IsReviewed(true);
              else {
                isReviewed = false;
                reApplyGrade = false;
                //self.QuestionClick(self.SelectedQuestion());
              }
            }

            if (
              isReviewed &&
              (self.IsComplete() || self.SelectedStudent.CanBulkGrading())
            ) {
              self.IsScrollToFirstManuallyGradedItem(false);
            } else {
              self.IsScrollToFirstManuallyGradedItem(true);
            }
            self.OldPointsEarned(self.PointsEarned());
            self.QuestionClick(self.SelectedQuestion());
            self.FilterQuestionByCredit(self.QuestionFilters());
          }

          if (
            self.SubmitTestButtonCss() &&
            (self.SelectedStudentFilter() ==
              self.PendingReviewStudentFilter().FilterValue() ||
              self.SelectedStudentFilter() == self.AllStudentFilter().FilterValue())
          ) {
            if (!options.WidgetUtil.IsNullOrEmpty(self.SelectedStudent)) {
              // Update Filter Student and Select Student From Pending to Ready Submit
              self.SelectedStudent.CanBulkGrading(true);
              self.IsGraded(false);

              if (
                self.SelectedStudentFilter() ==
                self.PendingReviewStudentFilter().FilterValue()
              ) {
                if (self.PendingReviewStudentFilter().FilterCount() == 0) { } else {
                  self.SelectedStudentFilterFunction();
                  self.SelectFilterStudentChangeTrigger();
                  self.SelectStudentChangeTrigger();
                }
              } else if (
                self.SelectedStudentFilter() == self.AllStudentFilter().FilterValue()
              ) {
                self.SelectedStudentFilterFunction();
                self.SelectFilterStudentChangeTrigger();
                self.SelectStudentChangeTrigger();
              }
            }
          }

          if (self.GradingType() == 'student') {
            if (isReviewed) {
              self.ReviewerWidget.ReviewerWidget(
                'SelectNextApplicableQuestion',
                self,
                isPendingReview,
                answerSubID
              );
            }
          }

          if (
            self.GradingType() == 'item'
            //&& self.IsPendingReview()
          ) {
            if (isReviewed) {
              //loading to next student
              self.ReviewerWidget.ReviewerWidget(
                'SelectNextApplicableStudent',
                self,
                isPendingReview,
                answerSubID
              );
            }
          }

          self.FilterQuestionByCredit(self.QuestionFilters());

          if (
            self.SubmitTestButtonCss() &&
            (self.SelectedStudentFilter() ==
              self.PendingReviewStudentFilter().FilterValue() ||
              self.SelectedStudentFilter() == self.AllStudentFilter().FilterValue())
          ) {
            if (!options.WidgetUtil.IsNullOrEmpty(self.SelectedStudent)) {
              // Update Filter Student and Select Student From Pending to Ready Submit
              self.SelectedStudent.CanBulkGrading(true);
              self.IsGraded(false);

              if (
                self.SelectedStudentFilter() ==
                self.PendingReviewStudentFilter().FilterValue()
              ) {
                if (self.PendingReviewStudentFilter().FilterCount() == 0) {
                  var selectedStudentID = self.SelectedStudentID();
                  self.SelectedStudentFilterNew(
                    self.ReadyToSubmitStudentFilter().FilterValue()
                  );
                  self.SelectedStudentID(null);
                  self.SelectedStudentID(selectedStudentID);
                } else {
                  self.SelectedStudentFilterFunction();
                  self.SelectFilterStudentChangeTrigger();
                  self.SelectStudentChangeTrigger();
                }
              } else if (
                self.SelectedStudentFilter() == self.AllStudentFilter().FilterValue()
              ) {
                self.SelectedStudentFilterFunction();
                self.SelectFilterStudentChangeTrigger();
                self.SelectStudentChangeTrigger();
              }
            }
          }

          self.IsScrollToFirstManuallyGradedItem(true);
          self.UnBlockUI();
        },
        error: function (xhr, status, error) {
          console.error('error', status, error)
          self.UnBlockUI();
        },
      });
    },

    GetNextApplicableStudent: function (self, reApplyGrade) {
      var that = this;
      var options = that.options;
      var selectedQuestion = self.SelectedQuestion();

      $.post(options.GetNextApplicableStudentURL, {
        qtiTestClassAssignmentId: self.QTITestClassAssignmentID(),
        virtualQuestionId: self.SelectedQuestion().VirtualQuestionID()
      }, function (response) {
        if (response == null) // no have next student
        {
          self.IsLastStudentGradingItem(false);
          if (!reApplyGrade && self.SelectedQuestion().Answered()) {
            var msgStudent = 'You have reached the last student on the list.';
            Reviewer.popupAlertMessage(msgStudent, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
            return;
          }
          return;
        }

        //load to next student
        self.IsLastStudentGradingItem(response.IsLastStudent);
        self.IsNextApplicableStudent(true);
        self.SelectedStudentID(response.StudentID);
        self.QuestionClick(selectedQuestion);
        return;
      });
    },

    SelectNextApplicableStudent: function (self, isPendingReview, answerSubID) {
      var that = this;
      var options = that.options;
      var selectedQuestion = self.SelectedQuestion();
      var selectedStudent = self.SelectedStudent;
      var nextQuestion = null;
      var nextStudent = null;
      if (isPendingReview && self.PendingReviewStudentFilter().FilterCount() == 0 &&
        self.ReadyToSubmitStudentFilter().FilterCount() > 0) {
        self.ReviewerWidget.ReviewerWidget('ConfirmAutoBulkSubmitTest', self);
        return;
      }
      if (self.SelectedStudentFilter() == self.AllStudentFilter().FilterValue() ||
        self.SelectedStudentFilter() == self.PendingReviewStudentFilter().FilterValue()) {
        if (self.PendingReviewStudentFilter().FilterCount() > 0) {
          $.post(options.GetNextApplicableStudentURL, {
            qtiTestClassAssignmentId: self.QTITestClassAssignmentID(),
            virtualQuestionId: self.SelectedQuestion().VirtualQuestionID()
          }, function (response) {
            //if (response == null && self.ReadyToSubmitStudentFilter().FilterCount() > 0) {
            //    self.ReviewerWidget.ReviewerWidget('ConfirmAutoBulkSubmitTest', self);
            //    return;
            //}

            self.IsNextApplicableStudent(true);
            if (selectedStudent.StudentID() != response.StudentID)
              self.SelectedStudentID(response.StudentID);

            nextQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
              return question.VirtualQuestionID() == response.VirtualQuestionID;
            });

            self.QuestionClick(nextQuestion);
            self.SelectedQuestionVirtualQuestionID(nextQuestion.VirtualQuestionID());
          });
          return;
        }
      }

      if (self.IsComplete() || selectedStudent.CanBulkGrading()) {
        if (selectedQuestion.QTIItemSchemaID() == 21) {
          if (self.ReviewerWidget.ReviewerWidget('SelectNextApplicableSubPart', self, answerSubID))
            return;
        }

        var lastStudent = [];
        if (self.GetReadySubmitAndCompleteStudents().length > 0) {
          lastStudent = self.GetReadySubmitAndCompleteStudents().reduce(function (prev, current) {
            return prev.StudentOrder() > current.StudentOrder() ? prev : current;
          });
        }

        var lastQuestion = [];
        if (self.GetReviewableQuestions().length > 0) {
          lastQuestion = self.GetReviewableQuestions().reduce(function (prev, current) {
            return prev.QuestionOrder() > current.QuestionOrder() ? prev : current;
          });
        }
        if (!selectedQuestion.Reviewable()) {
          nextQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
            return question.Reviewable();
          });
          if (nextQuestion) {
            self.SelectedQuestionVirtualQuestionID(nextQuestion.VirtualQuestionID());
          }
          return;
        }

        if (selectedQuestion.QuestionOrder() != lastQuestion.QuestionOrder()) {
          if (selectedStudent.StudentOrder() != lastStudent.StudentOrder()) {
            nextStudent = ko.utils.arrayFirst(self.GetReadySubmitAndCompleteStudents(), function (student) {
              return student.StudentOrder() > selectedStudent.StudentOrder();
            });
          }

          if (selectedStudent.StudentOrder() == lastStudent.StudentOrder()) {
            nextQuestion = ko.utils.arrayFirst(self.GetReviewableQuestions(), function (question) {
              return question.QuestionOrder() > selectedQuestion.QuestionOrder();
            });
            nextStudent = self.GetReadySubmitAndCompleteStudents()[0];
          }
        } else {
          if (selectedStudent.StudentOrder() != lastStudent.StudentOrder()) {
            nextStudent = ko.utils.arrayFirst(self.GetReadySubmitAndCompleteStudents(), function (student) {
              return student.StudentOrder() > selectedStudent.StudentOrder();
            });
          }
          if (selectedStudent.StudentOrder() == lastStudent.StudentOrder()) {
            nextQuestion = self.GetReviewableQuestions()[0];
            nextStudent = self.GetReadySubmitAndCompleteStudents()[0];
          }
        }

        if (nextStudent)
          self.SelectedStudentID(nextStudent.StudentID());
        if (nextQuestion)
          self.SelectedQuestionVirtualQuestionID(nextQuestion.VirtualQuestionID());
      }
    },

    GetNextApplicableQuestion: function (self, reApplyGrade) {
      var that = this;
      var options = that.options;
      var $assignmentFullPage = $('body');
      ShowBlock($assignmentFullPage, "Loading Next Question");

      var nextQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
        return question.VisibleQuestion() && question.Reviewable() && !question.IsReviewed();
      });

      if (!options.WidgetUtil.IsNullOrEmpty(nextQuestion)) {
        self.QuestionClick(nextQuestion);
        $assignmentFullPage.unblock();
        $assignmentFullPage.removeAttr('style');
        return;
      }

      $.post(options.GetNextApplicableQuestionURL, {
        qtiTestClassAssignmentId: self.QTITestClassAssignmentID(),
        studentId: self.SelectedStudentID()
      }, function (response) {
        if (response.error == 'false')
          return;

        if (response.data == null) {
          self.ReviewerWidget.ReviewerWidget('ConfirmAutoBulkSubmitTest', self);
          return;
        }

        nextQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
          return question.VirtualQuestionID() == response.data.VirtualQuestionID;
        });

        if (self.SelectedStudentID() == response.data.StudentID) {
          self.QuestionClick(nextQuestion);
        } else {
          self.IsNextApplicableQuestionGradeByStudent(true);
          self.SelectedStudentID(response.data.StudentID);
          self.QuestionClick(nextQuestion);
        }
        return;
      }).always(function () {
        $assignmentFullPage.unblock();
        $assignmentFullPage.removeAttr('style');
      });
    },

    SelectNextApplicableQuestion: function (self, isPendingReview, answerSubID) {
      var that = this;
      var options = that.options;
      var selectedQuestion = self.SelectedQuestion();
      var selectedStudent = self.SelectedStudent;

      // All and Pending review filter
      if (self.SelectedStudentFilter() == self.AllStudentFilter().FilterValue() ||
        self.SelectedStudentFilter() == self.PendingReviewStudentFilter().FilterValue()) {
        var nextQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
          return question.VisibleQuestion() && question.Reviewable() && !question.IsReviewed();
        });

        if (nextQuestion) {
          self.QuestionClick(nextQuestion);
          return;
        }

        if (isPendingReview && self.PendingReviewStudentFilter().FilterCount() == 0 &&
          self.ReadyToSubmitStudentFilter().FilterCount() > 0) {
          self.ReviewerWidget.ReviewerWidget('ConfirmAutoBulkSubmitTest', self);
          return;
        }

        if (self.PendingReviewStudentFilter().FilterCount() > 0) {
          var $assignmentFullPage = $('body');
          ShowBlock($assignmentFullPage, "All questions have been graded for this student. Loading next student");

          $.post(options.GetNextApplicableQuestionURL, {
            qtiTestClassAssignmentId: self.QTITestClassAssignmentID(),
            studentId: self.SelectedStudentID()
          }, function (response) {
            if (response.error == 'false')
              return;

            //if (response.data == null && self.ReadyToSubmitStudentFilter().FilterCount() > 0) {
            //    self.ReviewerWidget.ReviewerWidget('ConfirmAutoBulkSubmitTest', self);
            //    return;
            //}
            if (response.data != null) {
              nextQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
                return question.VirtualQuestionID() == response.data.VirtualQuestionID;
              });

              if (self.SelectedStudentID() == response.data.StudentID) {
                self.QuestionClick(nextQuestion);
              } else {
                self.IsNextApplicableQuestionGradeByStudent(true);
                self.SelectedStudentID(response.data.StudentID);
                self.QuestionClick(nextQuestion);
              }
            }

            return;
          }).always(function () {
            $assignmentFullPage.unblock();
            $assignmentFullPage.removeAttr('style');
          });
        }

        //return;
      }

      if (self.IsComplete() || selectedStudent.CanBulkGrading()) {
        // auto forward multipart question
        if (selectedQuestion.QTIItemSchemaID() == 21) {
          if (self.ReviewerWidget.ReviewerWidget('SelectNextApplicableSubPart', self, answerSubID))
            return;
        }

        var nextQ = ko.utils.arrayFirst(self.Questions(), function (question) {
          return question.VisibleQuestion() && question.Reviewable() && question.QuestionOrder() > selectedQuestion.QuestionOrder();
        });
        if (nextQ == null) {
          var nextS = ko.utils.arrayFirst(self.GetVisibleStudents(), function (student) {
            return (student.IsComplete() || student.CanBulkGrading()) && student.StudentOrder() > selectedStudent.StudentOrder();
          });
          if (nextS == null) {
            nextS = ko.utils.arrayFirst(self.GetVisibleStudents(), function (student) {
              return (student.IsComplete() || student.CanBulkGrading()) && student.StudentOrder() < selectedStudent.StudentOrder();
            });
          }
          if (nextS) {
            nextQ = ko.utils.arrayFirst(self.Questions(), function (question) {
              return question.VisibleQuestion() && question.Reviewable();
            });
          }
        }

        if (nextS && nextQ) {
          self.SelectedStudentID(nextS.StudentID());
        }
        if (nextQ) {
          self.QuestionClick(nextQ);
        }
      }
    },

    SelectNextApplicableSubPart: function (self, subID) {
      var nextSubIndex = 0;
      var $firstManuallyGraded;
      var $assignmentQuestion = $('.assignment-desc-question');
      var $manuallyGraded = $assignmentQuestion.find('.is-manually-graded');

      if ($manuallyGraded) {
        for (var i = 0; i < $manuallyGraded.length; i++) {
          if ($manuallyGraded[i].outerHTML.indexOf(subID) > 0) {
            //$firstManuallyGraded = $manuallyGraded[i];
            nextSubIndex = i + 1;
            break;
          }
        }
      }

      if (nextSubIndex <= $manuallyGraded.length - 1)
        $firstManuallyGraded = $manuallyGraded[nextSubIndex];
      else
        return false;

      var answerSubelement = $($firstManuallyGraded);

      if ($firstManuallyGraded) {
        if (answerSubelement.attr('onclick')) {
          answerSubelement.trigger('click')
        } else {
          if (answerSubelement.find('div[onclick]').length) {
            answerSubelement.find('div[onclick]').trigger('click');
          }
        }
      }

      //if ($firstManuallyGraded && t.find('div[onclick]').length) {
      //    t.find('div[onclick]').trigger('click');
      //}

      var offsetFirstManually = answerSubelement.offset().top;
      var offsetParent = $assignmentQuestion.offset().top;
      if (offsetFirstManually - offsetParent > 0) {
        $assignmentQuestion.animate({
          scrollTop: offsetFirstManually - offsetParent
        }, 500);
      }

      return true;
    },

    ResetReviewer: function (self) {
      self.TestOnlineSessionAnswers(null);
      self.Respones('');
      self.PointsEarned(0);
      self.PointsPossible(0);
      self.RefObjects([]);
      self.AnswerAttachments([]);
      self.SelectedStudent = null;
      self.QTIOnlineTestSessionID(null);
      self.SectionInstruction('');
      self.TotalSpentTimeOnTest('0s');

      ko.utils.arrayForEach(self.Questions(), function (question) {
        question.Answer(null);
        question.QuestionSelectedCss(false);
        question.PointsEarnedCredit(null);
      });

      self.ChooseSelectedQuestion();
      self.FilterQuestionByCredit(self.QuestionFilters());
    },
    CalculateTotalPointsPossible: function (questions, virtualTest, subtractFrom100PointPossible) {
      var totalPointsPossible = 0;

      ko.utils.arrayForEach(questions, function (question) {
        if (question.IsAssignForStudent()) {
          totalPointsPossible += question.PointsPossible();
        }
      });

      if (!Reviewer.IsNullOrEmpty(virtualTest) && virtualTest.TestScoreMethodID() == 2) // Subtract From 100
      {
        if (subtractFrom100PointPossible >= 0) {
          totalPointsPossible = subtractFrom100PointPossible;
        } else {
          if (totalPointsPossible < 100) {
            totalPointsPossible = 100;
          }
        }
      }
      return totalPointsPossible;
    },
    CalculateTotalPointsEarned: function (answers, questions, isComplete, scoreRaw, virtualTest) {
      var total = 0;
      var totalPointsPossible = 0;
      if (Reviewer.IsNullOrEmpty(answers) ||
        Reviewer.IsNullOrEmpty(questions)) {
        return 0;
      }

      ko.utils.arrayForEach(answers, function (testOnlineSessionAnswer) {
        var currentQuestion = ko.utils.arrayFirst(questions, function (question) {
          return question.IsAssignForStudent() && question.VirtualQuestionID() == testOnlineSessionAnswer.VirtualQuestionID();
        });

        if (!Reviewer.IsNullOrEmpty(currentQuestion)) {
          total += testOnlineSessionAnswer.PointsEarned();
        }
      });

      ko.utils.arrayForEach(questions, function (question) {
        if (question.IsAssignForStudent()) {
          totalPointsPossible += question.PointsPossible();
        }
      });

      // Subtract From 100
      if (!Reviewer.IsNullOrEmpty(virtualTest) && virtualTest.TestScoreMethodID() == 2) {
        total = total + 100 - totalPointsPossible;
        total = total < 0 ? 0 : total;
      }

      return total;
    },
    CanGrading: function (qtiOnlineTestSessionID, mode, selectedQuestion, answerID, pointsEarned,
      isPendingReview, paused, inProgress, expired, qtiSchemaID, overrideAutoGraded, responseProcessingTypeID,
      isComplete, overrideItems, answerSubID, gradingProcessStatus) {

      if (responseProcessingTypeID == 7) // AlgorithmicScoring
        return overrideAutoGraded;

      if (gradingProcessStatus != 1 && gradingProcessStatus != 2) // SuccessAndInPendingReview , SuccessAndInCompleted
        return false;

      if (Reviewer.IsNullOrEmpty(qtiOnlineTestSessionID) || mode == '2' ||
        (!Reviewer.IsNullOrEmpty(selectedQuestion) && selectedQuestion.IsBaseVirtualQuestion())) {
        return false;
      }

      if ((qtiSchemaID == 9 || qtiSchemaID == 10) && responseProcessingTypeID == 3) { //TextEntry , ExtendedText , Ungraded
        return false;
      }

      if (selectedQuestion.IsRestrictedManualGrade) {
        var manualQuestion = qtiSchemaID == 10
          && (Reviewer.IsNullOrEmpty(responseProcessingTypeID) || responseProcessingTypeID == '1');
        manualQuestion = manualQuestion || (qtiSchemaID == 9 && responseProcessingTypeID == '2');

        if (!manualQuestion && qtiSchemaID != 21)
          return false;
      }

      var result = qtiOnlineTestSessionID > 0 && answerID > 0 && pointsEarned >= 0;

      if ((paused || inProgress) && !expired) {
        return false;
      }
      else if (isPendingReview || ((paused || inProgress) && expired)) {
        if (!overrideAutoGraded) {
          result = result && qtiSchemaID == 9 && responseProcessingTypeID == '2'; // TextEntry -  manual
          result = result || (qtiSchemaID == 10 && (Reviewer.IsNullOrEmpty(responseProcessingTypeID) || responseProcessingTypeID == '1'));
        } else {
          var existItem = false;
          ko.utils.arrayForEach(overrideItems, function (item) {
            if (qtiSchemaID == item) {
              existItem = true;
              return;
            }
          });

          result = result && existItem;

          if (qtiSchemaID == 21) {
            result = result && !Reviewer.IsNullOrEmpty(answerSubID);
          }
        }
      } else if (isComplete) {
        var isManualQuestion = qtiSchemaID == 9 && responseProcessingTypeID == '2'; // TextEntry -  manual

        isManualQuestion = isManualQuestion || (qtiSchemaID == 10 && (Reviewer.IsNullOrEmpty(responseProcessingTypeID) || responseProcessingTypeID == '1'));

        if (!overrideAutoGraded && !isManualQuestion) {
          return false;
        }
        result = result || isManualQuestion;

        var existItem = false;
        ko.utils.arrayForEach(overrideItems, function (item) {
          if (qtiSchemaID == item) {
            existItem = true;
            return;
          }
        });

        result = result && existItem;

        if (qtiSchemaID == 21) {
          result = result && !Reviewer.IsNullOrEmpty(answerSubID);
        }
      }

      if (!Reviewer.IsNullOrEmpty(selectedQuestion) && selectedQuestion.IsInformationalOnly()) {
        result = false;
      }

      return result;
    },
    CanEnableGradingShotcut: function (qtiOnlineTestSessionID, mode, selectedQuestion, answerID, pointsEarned,
      isPendingReview, paused, inProgress, expired, qtiSchemaID, overrideAutoGraded, responseProcessingTypeID,
      isComplete, overrideItems, gradingProcessStatus) {
      if (gradingProcessStatus != 1 && gradingProcessStatus != 2) return false;

      if (Reviewer.IsNullOrEmpty(qtiOnlineTestSessionID) ||
        mode == '2' ||
        (!Reviewer.IsNullOrEmpty(selectedQuestion) &&
          selectedQuestion.IsBaseVirtualQuestion())) {
        return false;
      }
      if ((qtiSchemaID == 9 || qtiSchemaID == 10) && responseProcessingTypeID == 3)
        return false;

      var result = qtiOnlineTestSessionID > 0 && answerID > 0 && pointsEarned >= 0;

      if ((paused || inProgress) && !expired) {
        return false;
      } else if (isPendingReview || ((paused || inProgress) && expired)) {
        if (!overrideAutoGraded) {
          result = result && qtiSchemaID == 9 && responseProcessingTypeID == '2';
          result = result || (qtiSchemaID == 10 && (Reviewer.IsNullOrEmpty(responseProcessingTypeID) || responseProcessingTypeID == '1'));
        } else {
          var existItem = false;
          ko.utils.arrayForEach(overrideItems, function (item) {
            if (qtiSchemaID == item) {
              existItem = true;
              return;
            }
          });

          result = result && existItem;
        }
      } else if (isComplete) {
        if (!overrideAutoGraded) {
          return false;
        }

        var existItem = false;
        ko.utils.arrayForEach(overrideItems, function (item) {
          if (qtiSchemaID == item) {
            existItem = true;
            return;
          }
        });

        result = result && existItem;
      }

      if (qtiSchemaID == 21 && !Reviewer.IsNullOrEmpty(selectedQuestion) && !Reviewer.IsNullOrEmpty(selectedQuestion.Answer())) {
        if (Reviewer.IsNullOrEmpty(selectedQuestion.Answer().TestOnlineSessionAnswerSubs())) {
          return false;
        }

        var reviewableAnswerSub = ko.utils.arrayFirst(
          selectedQuestion.Answer().TestOnlineSessionAnswerSubs(),
          function (testOnlineSessionAnswerSub) {
            if (testOnlineSessionAnswerSub.QTISchemaID() == 9 &&
              testOnlineSessionAnswerSub.ResponseProcessingTypeID() == 2) {
              return true;
            }

            if (testOnlineSessionAnswerSub.QTISchemaID() == 10 && testOnlineSessionAnswerSub.ResponseProcessingTypeID() == 1) {
              return true;
            }

            return false;
          });

        if ((isPendingReview || ((paused || inProgress) && expired)) && !overrideAutoGraded) {
          return result || !Reviewer.IsNullOrEmpty(reviewableAnswerSub);
        }

        result = result && !Reviewer.IsNullOrEmpty(reviewableAnswerSub);
      }
      return result;
    },
    SaveFeedbackOverall: function (teacherreviewer, student, studentTestFeedbackId, studentOnlineTestSessionId, studentFeebackOverall) {
      var that = this;
      var options = that.options;
      var $assignmentFullPage = $('body');

      ShowBlock($assignmentFullPage, 'Loading');
      $.post(options.SaveFeedbackOverallURL, {
        testFeedbackId: studentTestFeedbackId,
        qtiOnlineTestSessionId: studentOnlineTestSessionId,
        feedbackContent: encodeURIComponent(studentFeebackOverall)
      }, function (response) {
        teacherreviewer.LockSaveFeedbackOverallBtn(false);

        $assignmentFullPage.unblock();
        $assignmentFullPage.removeAttr('style');

        if (response.success) {
          if (response.hasChanged) {
            var historyUpdated = 'Updated by ';
            var lastDateUpdatedFeedback = '';

            if (!Reviewer.IsNullOrEmpty(response.lastUserUpdatedFeedback)) {
              historyUpdated += getFullNameOnly(response.lastUserUpdatedFeedback);
            }

            if (!Reviewer.IsNullOrEmpty(response.lastDateUpdatedFeedback) &&
              response.lastDateUpdatedFeedback.trim().length > 0) {
              lastDateUpdatedFeedback = response.lastDateUpdatedFeedback;
              lastDateUpdatedFeedback = displayDateWithFormat(moment.utc(lastDateUpdatedFeedback).toDate().valueOf(), true);
              historyUpdated += ' on ' + lastDateUpdatedFeedback;
            }

            teacherreviewer.OldFeedbackOverall(teacherreviewer.FeedbackOverall());
            teacherreviewer.FeedbackOverallHistory(historyUpdated);
            student.TestFeedbackId(response.testFeedbackID);
            student.FeedbackContent(studentFeebackOverall);
            student.LastUserUpdatedFeedback(response.lastUserUpdatedFeedback);
            student.LastDateUpdatedFeedback(response.lastDateUpdatedFeedback);
          }
        } else {
          Reviewer.popupAlertMessage(response.message, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
        }
      });
    },
    SaveFeedbackQuestion: function (teacherreviewer, testOnlineAnswer, testOnlineAnswerItemFeedbackID, testOnlineAnswerOnlineTestSessionAnswerID, testOnlineAnswerID, testOnlineAnswerFeedbackQuestion, virtualQuestionID, qTIOnlineTestSessionID, file, fileDelete) {
      var that = this;
      var options = that.options;
      var $assignmentFullPage = $('body');

      ShowBlock($assignmentFullPage, 'Loading');
      var request = {
        ItemFeedbackId: testOnlineAnswerItemFeedbackID,
        qtiOnlineTestSessionAnswerId: testOnlineAnswerOnlineTestSessionAnswerID,
        answerId: testOnlineAnswerID,
        virtualQuestionID: virtualQuestionID,
        qTIOnlineTestSessionID: qTIOnlineTestSessionID,
        feedbackContent: encodeURIComponent(testOnlineAnswerFeedbackQuestion),
        fileDelete: fileDelete
      };
      var formData = new FormData();
      formData.append("file", file);
      formData.append('request', JSON.stringify(request));


      $.ajax({
        url: options.SaveFeedbackQuestionURL,
        type: "POST",
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
          teacherreviewer.LockSaveFeedbackQuestionBtn(false);
          $assignmentFullPage.unblock();
          $assignmentFullPage.removeAttr('style');

          if (response && response.success) {
            if(response.data.HasChanged)
            {
              teacherreviewer.OldFeedbackQuestion(teacherreviewer.FeedbackQuestion());
              testOnlineAnswer.ItemFeedbackID(response.data.ItemFeedbackId);
              testOnlineAnswer.Feedback(testOnlineAnswerFeedbackQuestion);
              testOnlineAnswer.UserUpdatedFeedback(response.data.LastUserUpdatedFeedback);
              testOnlineAnswer.DateUpdatedFeedback(response.data.LastDateUpdatedFeedback);
            }
            
            teacherreviewer.TeacherReviewFeedbackFile(null);
            teacherreviewer.HasChangedTeacherAttachment(false);

            if (fileDelete){
              testOnlineAnswer.TeacherFeebackAttachment(null);
              teacherreviewer.TeacherFeebackAttachment(null)
            }

            if(response.data.DocumentGuid){
              var newAttachment = {
                DocumentGuid : response.data.DocumentGuid,
                FileContent : file,
                FileName: response.data.FileName,
                FilePath: response.data.FilePath
              };
              testOnlineAnswer.TeacherFeebackAttachment(newAttachment);
              teacherreviewer.TeacherFeebackAttachment(newAttachment);
            }
            var attachment = testOnlineAnswer.TeacherFeebackAttachment();
            if (attachment && attachment.DocumentGuid){
              teacherreviewer.LockRecordFeedbackBtn(true);
            }
            else {
              teacherreviewer.LockRecordFeedbackBtn(false);
            }
          } else {
            Reviewer.popupAlertMessage(response.message, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
          }
        },
        error: function (err) {
          Reviewer.popupAlertMessage(err.statusText, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
        }
      });
    },
    PrintTestOfStudent: function (self) {
      var that = this;
      var options = that.options;
      var $formPrintTestOfStudent = $('#frmPrintTestOfStudent');

      ShowBlock($formPrintTestOfStudent, 'Loading');

      $.ajax({
        type: 'POST',
        url: options.PrintTestOfStudentURL,
        data: $formPrintTestOfStudent.serialize(),
        success: function (response) {
          // Wait for 3 seconds to make sure pdf has been generated
          var obj = {};
          obj.message = '<div class="u-text-center">Printing in progress. View Batch Printing to download PDF.</div>';
          $formPrintTestOfStudent.unblock();
          $formPrintTestOfStudent.removeAttr('style');
          that.PrintTestOfStudentSuccess(obj, function () {
            self.ClosePrintTestOfStudentPopup();
          });
        },
        error: function () {
          $formPrintTestOfStudent.unblock();
          $formPrintTestOfStudent.removeAttr('style');
        }
      });
    },
    PrintTestOfStudentSuccess: function (obj, callback) {
      var now = new Date().getTime();
      var $div = $('<div />');
      var dialogHtml = '';

      dialogHtml += '<div class="popup-fullpage">';
      dialogHtml += '<div class="popup-fullpage-content">';
      dialogHtml += obj.message;
      dialogHtml += '</div>';
      dialogHtml += '<div class="popup-fullpage-controls">';
      dialogHtml += '<button id="btn-ok-' + now + '" class="u-w-60">OK</button>';
      dialogHtml += '</div>';
      dialogHtml += '</div>';

      $div.html(dialogHtml)
        .attr('id', 'popup-fullpage-' + now)
        .appendTo('body')
        .dialog({
          modal: true,
          width: 450,
          maxHeight: 200,
          resizable: false,
          dialogClass: 'ui-popup-fullpage ui-popup-fullpage-nostudent',
          close: function () {
            $(document).find('#popup-fullpage-' + now).dialog('destroy').remove();
          }
        });

      // Close Popup and Open PDF In Show Full Page
      $(document).on('click', '#btn-ok-' + now, function () {
        $(document).find('#popup-fullpage-' + now).dialog('destroy').remove();
        var toString = Object.prototype.toString;
        if (toString.call(callback) === '[object Function]') {
          callback(obj);
        }
      });
    },
    LoadImages: function (el) {
      var that = this;
      var $el = $(el);
      var $imgs = $el.find('img');

      $imgs.each(function (ind, image) {
        var $image = $(image);
        var imageFloat = $image.attr('float');

        if (Reviewer.IsNullOrEmpty(imageFloat)) {
          imageFloat = '';
        }

        var imageUrl = $image.attr('src');

        if (Reviewer.IsNullOrEmpty(imageUrl)) {
          imageUrl = $image.attr('source');
        }

        if (Reviewer.IsNullOrEmpty(imageUrl)) {
          imageUrl = '/Content/images/emptybg.png';
        } else if (imageUrl.charAt(0) === '/') {
          imageUrl = imageUrl.substring(1);
        }

        $image.css('float', imageFloat)
          .attr({
            'source': '',
            'src': imageUrl
          });

        if ($image.attr('alt') == null) {
          $image.attr('alt', '');
        }

        if (imageUrl && (imageUrl.toLowerCase().indexOf('ro/ro') !== -1 ||
          imageUrl.toLowerCase().indexOf('itemset') >= 0) &&
          imageUrl.toLowerCase().indexOf('http') < 0 &&
          imageUrl.toLowerCase().indexOf('getviewreferenceimg') < 0) {
          imageUrl = 'TestAssignmentRegrader/GetViewReferenceImg?imgPath=' + imageUrl;
          $image.attr('src', imageUrl);
        }
      });

      if ($el.attr('id') === 'divQuestionDetails') {
        var drawableIndex = 0;

        $imgs.each(function (ind, image) {
          var $image = $(image);
          var imageDrawable = $image.attr('drawable');
          if (imageDrawable === 'true') {
            $image.unbind('load');
            $image.attr('index', drawableIndex);
            that.ShowDrawImage('question', $image, viewModel.AnswerImage(), null);
            drawableIndex++;
          }
        });
      }
    },
    PostSubmitTestData: function (self) {
      var that = this;
      var options = that.options;

      self.BlockUI();

      var sessionIDs = '';
      if (self.IsBulkGrading()) {
        ko.utils.arrayForEach(self.QTIOnlineTestSessionIDBulks(), function (student) {
          sessionIDs = sessionIDs + ',' + student.QTIOnlineTestSessionID();
        });
      } else {
        if (!Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
          sessionIDs = self.SelectedStudent.QTIOnlineTestSessionID();
        }
      }

      $.ajax({
        type: 'POST',
        data: {
          qtiOnlineTestSessionIDs: sessionIDs
        },
        async: false,
        dataType: 'json',
        url: options.SubmitTestURL
      })
        .success(function (result) {
          var selectedStudent = null;

          self.ScoreRaw(self.TotalPointsEarned());

          if (!Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
            selectedStudent = self.SelectedStudent;
            self.SelectedStudent.IsPendingReview(false);
            self.SelectedStudent.InProgress(false);
            self.SelectedStudent.IsNotStart(false);
            self.SelectedStudent.Paused(false);
            self.SelectedStudent.IsComplete(true);
            self.SelectedStudent.CanBulkGrading(false);
            self.ChooseSelectedQuestion();
          }

          self.IsConfirmAutoBulkSubmitTestPopup(false);
          self.IsPendingReview(false);
          self.InProgress(false);
          self.IsNotStart(false);
          self.Paused(false);
          self.IsComplete(true);
          self.IsGraded(false);

          if (self.IsBulkGrading()) {
            ko.utils.arrayForEach(self.QTIOnlineTestSessionIDBulks(), function (student) {
              student.IsPendingReview(false);
              student.InProgress(false);
              student.IsNotStart(false);
              student.Paused(false);
              student.IsComplete(true);
              student.CanBulkGrading(false);
            });
          }
          self.IsBulkGrading(false);
          if (self.IsAnonymizedScoring()
            && self.ReadyToSubmitStudentFilter().FilterCount() == 0
            && self.PendingReviewStudentFilter().FilterCount() == 0
            && self.InprogressStudentFilter().FilterCount() == 0
            && self.PausedStudentFilter().FilterCount() == 0
            && self.NotStartedStudentFilter().FilterCount() == 0
          ) {
            self.RefreshStudentNames();
          }

          self.UnBlockUI();
        })
        .error(function (error) {
          self.IsConfirmAutoBulkSubmitTestPopup(false);
          self.IsBulkGrading(false);
          self.UnBlockUI();
        });
    },
    ConfirmBulkSubmitTest: function (self, msg) {
      var now = new Date().getTime();
      var contentHtml = '';
      var $div = $('<div />');
      var $doc = $(document);

      contentHtml += '<div class="popup-fullpage">';
      contentHtml += '<div class="popup-fullpage-content">';
      contentHtml += '<p class="u-text-center">' + msg + '</p>';
      contentHtml += '</div>';
      contentHtml += '<div class="popup-fullpage-controls">';
      contentHtml += '<button id="btn-bulk-yes-' + now + '" class="btn-bulk">Yes</button>';
      contentHtml += '<button id="btn-bulk-no-' + now + '" class="btn-bulk">No</button>';
      contentHtml += '<button id="btn-bulk-cancel-' + now + '" class="btn-bulk">Cancel</button>';
      contentHtml += '</div>';
      contentHtml += '</div>';

      $div.html(contentHtml)
        .attr('id', 'popup-fullpage-' + now)
        .appendTo('body')
        .dialog({
          modal: true,
          width: 500,
          maxHeight: 400,
          resizable: false,
          dialogClass: 'ui-popup-fullpage ui-popup-fullpage-nostudent',
          close: function () {
            $doc.find('#popup-fullpage-' + now).dialog('destroy').remove();
          }
        });

      // Register yes event when submiting with item
      $doc.on('click', '#btn-bulk-yes-' + now, function () {
        $doc.find('#popup-fullpage-' + now).dialog('destroy').remove();
        self.IsBulkGrading(true);
        self.PostSubmitTestData();

        self.SelectFilterStudentChangeTrigger();
        self.SelectedStudentFilterFunction();
        self.SelectedStudentID(null);
        var selectedStudentID = self.DefaultDisplayStudentFilter();
        self.SelectedStudentID(selectedStudentID);
        self.SelectStudentChangeTrigger();
      });

      // Register no event when submiting with item
      $doc.on('click', '#btn-bulk-no-' + now, function () {
        $doc.find('#popup-fullpage-' + now).dialog('destroy').remove();
        self.IsBulkGrading(false);
        self.PostSubmitTestData();

        self.SelectFilterStudentChangeTrigger();
        self.SelectedStudentFilterFunction();
        self.SelectedStudentID(null);
        var selectedStudentID = self.DefaultDisplayStudentFilter();
        self.SelectedStudentID(selectedStudentID);
        self.SelectStudentChangeTrigger();
      });

      // Register cancel event when submiting with item
      $doc.on('click', '#btn-bulk-cancel-' + now, function () {
        $doc.find('#popup-fullpage-' + now).dialog('destroy').remove();
      });
    },
    DownloadRubricFile: function (key) {
      var that = this;
      var options = that.options;
      var url = options.DownloadRubricFileURL + '?key=' + key;
      return url;
    },
    GetRubricByVirtualTest: function (virtualTestID, successCb, errorCb) {
      var that = this;
      var options = that.options;

      $.ajax({
        type: 'GET',
        cache: false,
        url: options.GetRubricByVirtualTestURL,
        data: {
          virtualTestID: virtualTestID
        },
        success: function (data) {
          if (!options.WidgetUtil.IsNullOrEmpty(successCb)) {
            successCb(data);
          }
        },
        error: function () {
          if (!options.WidgetUtil.IsNullOrEmpty(errorCb)) {
            errorCb();
          }
        }
      });
    },
    GetOverrideAutoGradedOfAssignment: function (qtiTestClassAssignmentID, successCallBack, errorCallBack) {
      var that = this;
      var options = that.options;

      $.ajax({
        type: 'GET',
        url: options.GetOverrideAutoGradedOfAssignmentURL,
        cache: false,
        data: {
          qtiTestClassAssignmentID: qtiTestClassAssignmentID
        },
        datatype: 'json',
        success: function (data) {
          if (!options.WidgetUtil.IsNullOrEmpty(successCallBack)) {
            successCallBack(data);
          }
        },
        error: function (data) {
          if (!options.WidgetUtil.IsNullOrEmpty(errorCallBack)) {
            errorCallBack(data);
          }
        }
      });
    },
    ViewBatchPrinting: function (qtiTestClassAssignmentID) {
      var that = this;
      var options = that.options;

      var worker = $('<div />');
      worker
        .addClass('dialog')
        .attr('id', 'popup-batchPrinting')
        .appendTo('body')
        .load(options.ViewBatchPrintingURL + '?classTestAssignmentId=' + qtiTestClassAssignmentID, function () {
          worker.dialog({
            title: $(this).attr('View Batch Printing'),
            dialogClass: 'ui-popup-fullpage',
            close: function () {
              $('.ui-widget-overlay').remove();
              $(this).remove();
              clearInterval(window.intervalBatchPrinting);
            },
            modal: false,
            width: 750,
            resizable: false
          });

          // Check when ckeditor dialog exist
          var $body = $('body');
          var $popupOverlay = $('<div/>');

          $popupOverlay
            .addClass('ui-widget-overlay')
            .css({
              'background': '#000',
              'width': $body.width() + 'px',
              'height': $body.height() + 'px',
              'opacity': 0.3,
              'z-index': 1001
            });

          $body.prepend($popupOverlay);

          var zIndexArr = [];
          var maxIndex = 0;

          if ($('.ui-dialog').length) {
            $('.ui-dialog').each(function (ind, dialog) {
              var $dialog = $(dialog);
              var zIndex = $dialog.css('z-index');

              zIndexArr.push(zIndex);
            });

            maxIndex = Math.max.apply(Math, zIndexArr);
          }

          // Ui dialog appearance
          var $uiDialog = $('.ui-dialog');
          $uiDialog.css('height', 'auto');

          if (maxIndex) {
            $uiDialog.css('z-index', maxIndex + 2);
            $popupOverlay.css('z-index', maxIndex + 1);
          }
        });
    },
    UpdateAutoSaveToAnswerText: function (self) {
      var that = this;
      var data = {
        QtiOnlineTestSessionID: self.QTIOnlineTestSessionID(),
        AnswerText: self.SelectedAnswerTemp(),
        AnswerId: self.AnswerID(),
        AnswerSubId: self.AnswerSubID()
      };

      $.ajax({
        type: 'POST',
        data: data,
        async: false,
        dataType: 'json',
        url: that.options.UpdateAutoSaveToAnswerTextURL
      })
        .success(function (result) {
          self.SelectedAnswerTemp(self.defautPostAnswerLog());
        })
        .error(function (error) {
          console.log(error);
        });
    },

    ConfirmAutoBulkSubmitTest: function (self) {
      self.IsConfirmAutoBulkSubmitTestPopup(true);
    }
  });
}(jQuery));
