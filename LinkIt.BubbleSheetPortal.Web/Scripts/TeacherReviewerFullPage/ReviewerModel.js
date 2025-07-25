function Rubric(data) {
  var self = this;
  self.Success = ko.observable(data.Success);
  self.VirtualTestFileKey = ko.observable(data.VirtualTestFileKey);
  self.VirtualTestFileName = ko.observable(data.VirtualTestFileName);
}

function Student(data) {
  var self = this;
  if (data.QTIOnlineTestSessionID === null) {
    data.QTIOnlineTestSessionID = '0';
  }

  self.QTIOnlineTestSessionID = ko.observable(data.QTIOnlineTestSessionID);
  self.QTISchemaID = ko.observable(data.QTISchemaID);
  self.StudentName = ko.observable(data.StudentName);
  self.StudentID = ko.observable(data.StudentID);
  self.TestName = ko.observable(data.TestName);
  self.IsComplete = ko.observable(data.IsComplete);
  self.IsPendingReview = ko.observable(data.IsPendingReview);
  self.InProgress = ko.observable(data.InProgress);
  self.IsNotStart = ko.observable(data.IsNotStart);
  self.Paused = ko.observable(data.Paused);
  self.QTOStatusID = ko.observable(data.QTOStatusID);
  self.TimeOver = ko.observable(data.TimeOver);
  self.StartDate = ko.observable(data.StartDate);
  self.CanBulkGrading = ko.observable(data.CanBulkGrading);
  self.StudentVisible = ko.observable(true);
  self.RealStudentName = ko.observable(data.RealStudentName);

  self.Status = ko.computed(function () {
    if (self.IsComplete()) {
      return 'Completed';
    } else if (self.IsPendingReview()) {
      return 'Pending Review';
    } else if (self.InProgress()) {
      return 'Created';
    } else if (self.Paused()) {
      return 'Paused';
    }

    return 'NotStarted';
  });

  self.StatusClass = ko.computed(function () {
    if (self.IsComplete()) {
      return 'completed';
    } else if (!self.CanBulkGrading() && self.IsPendingReview()) {
      return 'pending';
    } else if (self.InProgress()) {
      return 'in-progress';
    } else if (self.Paused()) {
      return 'paused';
    } else if (self.CanBulkGrading()) {
      return 'ready-submit';
    }

    return 'not-started';
  });

  self.TestFeedbackId = ko.observable('');
  self.FeedbackContent = ko.observable('');
  self.LastUserUpdatedFeedback = ko.observable('');
  self.LastDateUpdatedFeedback = ko.observable('');

  self.StudentOrder = ko.observable(data.StudentOrder)
}

function Tier(data) {
  var self = this;

  self.TierID = ko.observable(data.RubricCategoryTierID);
  self.TierLabel = ko.observable(data.Label);
  self.TierPoint = ko.observable(data.Point);
  self.PointEarn = ko.observable(data.PointEarn);
  self.Selected = ko.observable(data.Selected);
  self.TierDescription = ko.observable(Reviewer.IsNullOrEmpty(data.Description) ? "<i class='text-muted'>No description</i>" : data.Description);

  self.IsActive = ko.computed(function () {
    return self.Selected();
  });

  self.IsVisibledTierLabel = ko.computed(function () {
    return !Reviewer.IsNullOrEmpty(self.TierLabel());
  });
}

function Category(data) {
  var self = this;
  var mappedTierList = $.map(data.RubricCategoryTiers, function (item) { return new Tier(item); });

  self.CategoryID = ko.observable(data.RubricQuestionCategoryID);
  self.CategoryName = ko.observable(data.CategoryName);
  self.CategoryTierList = ko.observableArray(mappedTierList);
  self.PointsPossible = ko.observable(data.PointsPossible);

  self.PointEarn = ko.computed(function () {
    var selectedTiers = $.grep(self.CategoryTierList(), function (tier) {
      return tier.Selected();
    });

    return selectedTiers.length ? selectedTiers[0].TierPoint() : 0;
  });

  self.TierClicked = function (selectedTier) {
    selectedTier.Selected(!selectedTier.Selected());

    ko.utils.arrayForEach(self.CategoryTierList(), function (tier) {
      var isSelectedTier = tier.TierID() === selectedTier.TierID();

      !isSelectedTier && tier.Selected(false);
    });

    var selectedQuestion = viewModel.SelectedQuestion();

    if (!Reviewer.IsNullOrEmpty(selectedQuestion)) {
      if (!Reviewer.IsNullOrEmpty(selectedQuestion.SelectedNextCategory())) {
        var selectedTiers = ko.utils.arrayFilter(selectedQuestion.SelectedCategory().CategoryTierList(), function (tier) {
          return tier.Selected();
        });
        var isSelected = selectedTiers.length;

        if (isSelected) {
          selectedQuestion.SelectedCategory(selectedQuestion.SelectedNextCategory());
        }
      }
    }
  };

  self.IsFill = ko.computed(function () {
    if (Reviewer.IsNullOrEmpty(self.CategoryTierList())) return true;

    return self.CategoryTierList().length <= 5;
  });

  self.IsMarked = ko.computed(function () {
    var selectedTiers = $.grep(self.CategoryTierList(), function (tier) {
      return tier.Selected();
    });

    return selectedTiers.length > 0;
  });
}

function StudentFilter(data) {
  var self = this;
  self.FilterValue = ko.observable(data.FilterValue);
  self.FilterName = ko.observable(data.FilterName);
  self.FilterCount = ko.observable(data.FilterCount);
  self.FilterText = ko.computed(function () {
    return self.FilterName() + ' (' + self.FilterCount() + ')';
  });

  self.Plus = function () {
    if (Reviewer.IsNullOrEmpty(self.FilterCount())) self.FilterCount(0);
    self.FilterCount(self.FilterCount() + 1);
  };

  self.Minus = function () {
    if (Reviewer.IsNullOrEmpty(self.FilterCount())) self.FilterCount(0);
    self.FilterCount(self.FilterCount() - 1);
    if (self.FilterCount() < 0) self.FilterCount(0);
  };
}

function PostAutoSaveFilter(data) {
  var self = this;

  self.ResponseIdentifier = ko.observable(data.TempResponseIdentifier);
  self.AnswerTemp = ko.observable(data.TempAnswerTemp);
  self.AnswerImage = ko.observable(data.AnswerImage);
  self.DrawingContent = ko.observable(data.DrawingContent);
  self.DumpCol = ko.observable(data.DumpCol);
  self.QTIOnlineTestSessionID = ko.observable(data.QTIOnlineTestSessionID);
  self.Timestamp = ko.observable(data.Timestamp);
  self.TimestampString = ko.observable(data.TimestampString);
  self.TextValue = ko.computed(function () {
    if (data.TempAnswerTemp == viewModel.defautPostAnswerLog()) {
      return data.TempAnswerTemp;
    } else {
      var count = 0;
      $('<div/>').append(data.TempAnswerTemp).children().each(function(idx, ele) {
        count += $(ele).text().length;
      });
      return toLocalTime(data.TimestampString) + ' ' + count + ' Character(s)';
    }
  });
}

function toLocalTime(timestamp) {
  var localTimeUtc = moment.utc(timestamp).toDate();
  var localTime = displayDateWithFormat(localTimeUtc.valueOf(), true);

  return localTime;
}

function RefObject(refObject) {
  var self = this;

  self.RefObjectID = ko.observable('');
  self.RefObjectData = ko.observable('');

  if (refObject) {
    self.RefObjectID($(refObject).attr('refobjectid'));
    self.RefObjectData($(refObject).attr('data'));
  }
}

function Question(data) {
  var self = this;
  // Replace for old inline choice before show on teacher review
  // Todo:
  // data.XmlContent = correctInlineChoice(data.XmlContent);
  if (data.XmlContent.includes('<m:math')) {
    data.XmlContent = data.XmlContent.replace(new RegExp("\>[\n\t ]+\<", "g"), "><")
      .replaceAll('<m:m', '<m').replaceAll('</m:m', '</m');
  }

  self.QuestionGroupID = ko.observable(data.QuestionGroupID);
  self.Answer = ko.observable(null);

  self.QTIItemID = ko.observable(data.QTIItemID);
  self.QTIItemSchemaID = ko.observable(data.QTIItemSchemaID);
  self.VirtualQuestionID = ko.observable(data.VirtualQuestionID);
  self.QuestionOrder = ko.observable(data.QuestionOrder);
  self.PointsPossible = ko.observable(data.PointsPossible);
  self.ResponseProcessingTypeID = ko.observable(data.ResponseProcessingTypeID);
  self.SectionInstruction = ko.observable(data.SectionInstruction);
  self.XmlContent = ko.observable(data.XmlContent);
  self.DataXmlContent = ko.observable(data.XmlContent);
  self.QuestionSelectedCss = ko.observable(false);
  self.ManuallyGradedOnly = ko.observable(data.IsRestrictedManualGrade);;
  self.VirtualTestSubtypeID = ko.observable('');
  self.BranchingTest = ko.observable('');
  self.IsRestrictedManualGrade = data.IsRestrictedManualGrade;
  self.IsRubricBasedQuestion = !!data.IsRubricBasedQuestion;

  self.RubricQuestionCategories = ko.observableArray([]);
  self.SelectedCategory = ko.observable(null);
  self.SelectedCategoryID = ko.computed(function () {
    return Reviewer.IsNullOrEmpty(self.SelectedCategory())
      ? null
      : self.SelectedCategory().CategoryID();
  });
  self.SelectedNextCategory = ko.computed(function () {
    if (Reviewer.IsNullOrEmpty(self.SelectedCategory())) return null;

    var currentIndex = ko.utils.arrayIndexOf(self.RubricQuestionCategories(), self.SelectedCategory());
    var nextIndex = currentIndex + 1;

    return !!self.RubricQuestionCategories()[nextIndex]
      ? self.RubricQuestionCategories()[nextIndex]
      : null;
  });

  self.InitializeRubricQuestionCategories = function (categories) {
    if (self.IsRubricBasedQuestion && !Reviewer.IsNullOrEmpty(categories)) {
      var mappedCategories = $.map(categories, function (item) { return new Category(item); });

      self.RubricQuestionCategories(mappedCategories);
      self.SelectedCategory(mappedCategories[0]);
    }
  };

  self.AssignSelectedTier = function () {
    ko.utils.arrayForEach(self.RubricQuestionCategories(), function (category) {
      ko.utils.arrayForEach(category.CategoryTierList(), function (tier) {
        tier.PointEarn(tier.Selected() ? tier.TierPoint() : null);
      });
    });
  };

  self.IsMarkedCategories = ko.computed(function () {
    var categoryLen = self.RubricQuestionCategories().length;
    var selectedTierLen = 0;

    ko.utils.arrayForEach(self.RubricQuestionCategories(), function (category) {
      ko.utils.arrayForEach(category.CategoryTierList(), function (tier) {
        selectedTierLen = tier.Selected() ? selectedTierLen + 1 : selectedTierLen;
      });
    });

    return categoryLen === selectedTierLen;
  });

  self.ResetRubricQuestionCategories = function () {
    if (!Reviewer.IsNullOrEmpty(self.RubricQuestionCategories())) {
      var firstCategory = self.RubricQuestionCategories()[0];

      self.SelectedCategory(firstCategory);
    } else {
      self.SelectedCategory(null);
    }

    ko.utils.arrayForEach(self.RubricQuestionCategories(), function (category) {
      ko.utils.arrayForEach(category.CategoryTierList(), function (tier) {
        var isSelected = tier.TierPoint() === tier.PointEarn();

        tier.Selected(isSelected);
      });
    });
  };

  self.InitializeRubricQuestionCategories(data.RubricQuestionCategories);

  self.RubricPointsEarned = ko.computed(function () {
    var rubricPointsEarned = 0;
    ko.utils.arrayForEach(self.RubricQuestionCategories(), function (category) {
      rubricPointsEarned += category.PointEarn();
    });

    return rubricPointsEarned;
  });

  self.RubricPointsEarned.subscribe(function (newValue) {
    if (self.IsRubricBasedQuestion) {
      viewModel.PointsEarned(newValue);
    }
  });

  self.CategoryListClicked = function (cate) {
    self.SelectedCategory(cate);
  };

  self.Answered = ko.computed(function () {
    if (Reviewer.IsNullOrEmpty(self.Answer())) {
      return false;
    }

    return self.Answer().Answered();
  });

  self.Unanswered = ko.computed(function () {
    return !self.Answered();
  });

  self.Reviewable = ko.computed(function () {
    var qtiSchemaId = self.QTIItemSchemaID();

    if (Reviewer.IsNullOrEmpty(self.Answer())) {
      return false;
    }

    if (qtiSchemaId == 9) {
      return self.Answer().ResponseProcessingTypeID() == 2;
    }

    if (qtiSchemaId == 10) {
      return self.Answer().ResponseProcessingTypeID() == 1;
    }

    if (qtiSchemaId == 21) {
      if (Reviewer.IsNullOrEmpty(self.Answer().TestOnlineSessionAnswerSubs())) {
        return false;
      }

      var reviewableAnswerSub = ko.utils.arrayFirst(
        self.Answer().TestOnlineSessionAnswerSubs(),
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

      return !Reviewer.IsNullOrEmpty(reviewableAnswerSub);
    }

    return false;
  });

  self.IsReviewed = ko.computed(function () {
    if (Reviewer.IsNullOrEmpty(self.Answer())) {
      return false;
    }

    return self.Answer().IsReviewed();
  });

  self.IsShowByQuestion = ko.observable(true);
  self.BaseVirtualQuestionID = ko.observable(data.BaseVirtualQuestionID);
  self.IsBaseVirtualQuestion = ko.observable(data.IsBaseVirtualQuestion);
  self.IsGhostVirtualQuestion = ko.observable(data.IsGhostVirtualQuestion);
  self.VisibleQuestion = ko.computed(function () {
    var result;

    if (self.VirtualTestSubtypeID() !== '5' || !self.BranchingTest()) {
      result = (!self.ManuallyGradedOnly() || self.Reviewable()) && self.IsShowByQuestion();
    } else {
      result = self.Answered();

      if (self.ManuallyGradedOnly()) {
        result = result && self.Reviewable();
      }

      result = result && self.IsShowByQuestion();
    }

    return result;
  });

  self.QuestionOrderDisplay = ko.computed(function () {
    if (self.VirtualTestSubtypeID() === '5' && self.BranchingTest()) {
      return self.AnswerOrder();
    }

    return self.QuestionOrder();
  });

  self.IsAssignForStudent = ko.computed(function () {
    if (self.VirtualTestSubtypeID() !== '5' || !self.BranchingTest()) {
      return true;
    }

    return self.Answered();
  });

  self.CorrectAnswer = ko.observable(data.CorrectAnswer);
  self.CorrectAnswerHTML = ko.observable(data.CorrectAnswer);

  self.NotYetReviewCSS = ko.computed(function () {
    return self.Reviewable() && !self.IsReviewed();
  });

  self.ReviewedCSS = ko.computed(function () {
    return self.Reviewable() && self.IsReviewed();
  });

  self.AnsweredCSS = ko.computed(function () {
    if (self.Answered()) return 'assignment-list-number is-answered';
    return 'assignment-list-number';
  });

  self.RefObjects = ko.observable([]);

  self.MainBody = ko.computed(function () {
    var mainContent = $(self.XmlContent()).find('div').filter(function () {
      if (this.className != undefined) {
        return this.className.toLowerCase() == 'mainbody';
      }
    });

    if (mainContent != undefined && mainContent.length > 0) {
      return mainContent.get(0).outerHTML;
    }

    return "";
  });

  self.Identifier = ko.observable('');

  self.ItemBody = ko.computed(function () {
    var rawXmlContent = self.XmlContent();
    var tree;
    var result = '';

    rawXmlContent = Reviewer.replaceParagraph(rawXmlContent);
    tree = $(rawXmlContent);

    // Replace list with ol
    tree.find('list').replaceWith(function () {
      var list = $(this);
      var newList = $('<ol/>');

      newList.html(list.html());

      newList.find('li').replaceWith(function () {
        var li = $(this);
        var newLi = $('<li/>');
        newLi.html(li.html());
        newLi.css('style-list', 'none outside none');
        return newLi;
      });

      return newList;
    });

    // Replace videolinkit
    tree.find('videolinkit').not('simplechoice videolinkit').each(function (ind, videolinkit) {
      var $videolinkit = $(videolinkit);

      $videolinkit.replaceWith(function () {
        var $self = $(this);
        var $video = $('<video/>');
        CopyAttributes($self, $video);
        $video.html($self.html());
        $video.removeAttr('autoplay');

        return $video;
      });
    });

    tree.find('sourcelinkit').not('simplechoice sourcelinkit').each(function (ind, sourcelinkit) {
      var $sourcelinkit = $(sourcelinkit);

      $sourcelinkit.replaceWith(function () {
        var $self = $(this);
        var $source = $('<source/>');
        CopyAttributes($self, $source);
        $source.html($self.html());

        return $source;
      });
    });

    tree.find('itembody').each(function () {
      var itemBody = $(this);
      if ($(self.DataXmlContent()).find("responsedeclaration").attr("partialgrading") == "1") {
        itemBody.find("sourcetext").each(function () {
          if ($(this).attr("pointvalue") > 0) {
            $(this).addClass("marker-correct");
          }
        });
      } else {
        $(self.DataXmlContent()).find("correctResponse").each(function () {
          var id = $(this).attr("identifier");
          itemBody.find("sourcetext[identifier=\"" + id + "\"]").addClass("marker-correct");
        });
      }

      result = $(this).outerHTML();
    });

    return result;
  });

  self.QuestionMenuItems = ko.observable(CreateQuestionMenuItem(self.ItemBody()));

  var refObjectElements = $(self.ItemBody()).find('object').filter(function () {
    return this.className.toLowerCase() == 'referenceobject';
  });

  if (refObjectElements && refObjectElements.length > 0) {
    var mappedObjects = $.map(refObjectElements, function (item) {
      return new RefObject(item);
    });
    self.RefObjects(mappedObjects);
  }

  var countResponse = $(data.XmlContent).find('responsedeclaration').length;

  var countValue = $(data.XmlContent).find('correctResponse').find('value').length;
  if (countResponse <= 1 && countValue <= 1) {
    self.CorrectResponse = $.trim($(data.XmlContent).find('correctResponse').text());
  } else {
    var dict = [];
    $(data.XmlContent).find('responsedeclaration').each(function () {
      var responseIdentifier = $(this).attr('identifier');
      $(this).find('correctresponse').each(function () {
        var valueArray = [];
        var i = 0;
        $(this).find('value').each(function () {
          valueArray[i] = $(this).text();
          i++;
        });
        dict.push({
          key: responseIdentifier,
          value: valueArray
        });
      });
    });
    self.CorrectResponse = dict;
  }

  self.StyleReference = ko.observable('');

  self.AnswerOrder = ko.observable(0);

  self.PointsEarnedCredit = ko.observable(data.PointsEarnedCredit);
  self.PointsPossibleCredit = ko.observable(data.PointsPossibleCredit);
  self.PointsFilter = ko.observable('');

  self.PointsClassCreditCSS = ko.computed(function () {
    var pointsEarned = self.PointsEarnedCredit();
    var pointsPossible = self.PointsPossibleCredit();

    if (Reviewer.IsNullOrEmpty(pointsEarned) || self.IsInformationalOnly()) {
      self.PointsFilter('');
      return '';
    } else if (pointsEarned === 0) {
      self.PointsFilter('credit-no-credit');
      return 'icon-credit-no-credit';
    } else if (pointsEarned < pointsPossible) {
      self.PointsFilter('credit-partial');
      return 'icon-credit-partial';
    } else if (pointsEarned === pointsPossible) {
      self.PointsFilter('credit-full');
      return 'icon-credit-full';
    }
  });

  self.AlgorithmicCorrectAnswers = ko.observable([]);
  //self.AtleastAlgorithmicCorrectAnswers = ko.observable([]);
  if (data.AlgorithmicCorrectAnswers != null) {
    var mappedObjects = $.map(data.AlgorithmicCorrectAnswers, function (item) {
      return new AlgorithmicCorrectAnswers(item);
    });
    self.AlgorithmicCorrectAnswers(mappedObjects);
  }

  self.IsApplyAlgorithmicScoring = ko.observable(data.IsApplyAlgorithmicScoring);

  self.IsInformationalOnly = ko.computed(function () {
    var rawXmlContent = self.XmlContent();
    var result = false;
    var $tree;

    rawXmlContent = Reviewer.replaceParagraph(rawXmlContent);
    $tree = $(rawXmlContent);

    if ($tree.find('responseDeclaration').attr('method') === 'informational-only') {
      result = true;
    }

    return result;
  });
}

function TestOnlineSessionAnswer(data) {
  var self = this;
  self.Expired = ko.observable(data.Expired);
  self.QTIOnlineTestSessionAnswerID = ko.observable(data.QTIOnlineTestSessionAnswerID);
  self.AnswerID = ko.observable(data.AnswerID);
  self.QTIOnlineTestSessionID = ko.observable(data.QTIOnlineTestSessionID);
  self.VirtualQuestionID = ko.observable(data.VirtualQuestionID);
  self.AnswerChoice = ko.observable(data.AnswerChoice);
  self.Answered = ko.observable(data.Answered);
  self.AnswerImage = ko.observable(data.AnswerImage);
  self.DrawingContent = ko.observable(data.DrawingContent);
  self.AnswerText = ko.observable(data.AnswerText);

  self.HighlightQuestion = ko.observable(data.HighlightQuestion);
  self.HighlightPassage = ko.observable(data.HighlightPassage);
  self.AnswerTemp = ko.observable(data.AnswerTemp);

  self.PointsEarned = ko.observable(Reviewer.IsNullOrEmpty(data.PointsEarned) ? 0 : data.PointsEarned);
  self.IsReviewed = ko.observable(data.IsReviewed);
  self.ResponseProcessingTypeID = ko.observable(data.ResponseProcessingTypeID);
  self.TestOnlineSessionAnswerSubs = ko.observable([]);
  if (data.TestOnlineSessionAnswerSubs != null) {
    var mappedObjects = $.map(data.TestOnlineSessionAnswerSubs, function (item) {
      return new TestOnlineSessionAnswerSub(item);
    });
    self.TestOnlineSessionAnswerSubs(mappedObjects);
  }

  self.Overridden = ko.observable(data.Overridden);
  self.AnswerOfBaseQuestion = ko.observable(false);
  self.UpdatedBy = ko.observable(data.UpdatedBy);
  self.UpdatedDate = ko.observable(data.UpdatedDate);

  //Teacher feedback for item
  self.ItemFeedbackID = ko.observable(data.ItemFeedbackID);
  self.ItemAnswerID = ko.observable(data.ItemAnswerID);
  self.Feedback = ko.observable(data.Feedback);
  self.UserUpdatedFeedback = ko.observable(data.UserUpdatedFeedback);
  self.DateUpdatedFeedback = ko.observable(data.DateUpdatedFeedback);
  self.GradingProcessStatus = ko.observable(data.GradingProcessStatus);

  self.IsLastStudentGradingItem = ko.observable(false);
  //display time spent
  self.VisitedTimes = ko.observable(data.TimesVisited);

  self.TimeSpent = ko.observable(data.TimeSpent);
  self.TotalSpentTimeOnQuestion = ko.computed(function () {
    if (data.TimeSpent > 0) {
      return Reviewer.prettyTime(data.TimeSpent);
    }
    return '0s';
  });

  self.PostAnswerLogs = ko.observable([]);
  if (data.PostAnswerLogs != null) {
    var mapObjs = [];

    var mappedObjects = $.map(data.PostAnswerLogs, function (item, index) {
      var tempAns = item.Answer == null ? [] : JSON.parse(item.Answer);
      if (tempAns) {
        for (var i = 0; i < tempAns.length; i++) {
          if (tempAns[i].AnswerTemp != undefined || tempAns[i].AnswerText != undefined) {
            var textTemp = tempAns[i].AnswerTemp;

            textTemp = textTemp != undefined ? textTemp : tempAns[i].AnswerText;
            item.TempResponseIdentifier = tempAns[i].ResponseIdentifier;

            // Create default for dropdown
            if (index == 0) {
              item.TempAnswerTemp = viewModel.defautPostAnswerLog();
              mapObjs.push(new PostAutoSaveFilter(item));
            }

            item.TempAnswerTemp = textTemp;

            if (item.TempAnswerTemp.indexOf('&#60;') > -1 && item.TempAnswerTemp.indexOf('&#62;') > -1) {
              item.TempAnswerTemp = Reviewer.replaceStringLessOrLarge(item.TempAnswerTemp);
            } else {
              item.TempAnswerTemp = item.TempAnswerTemp.replace(/[\r\n]+/g, '<br/>');
            }

            mapObjs.push(new PostAutoSaveFilter(item));
          }
        }
      }
    });
    self.PostAnswerLogs(mapObjs);
    self.TeacherFeebackAttachment =  ko.observable(data.TeacherFeebackAttachment);
    self.AnswerAttachments = ko.observable($.map(data.AnswerAttachments, function(item) {
      item.DisplayName = item.FileName;
      return item;
    }));
  }
}

function TestOnlineSessionAnswerSub(data) {
  var self = this;
  self.QTIOnlineTestSessionAnswerSubID = ko.observable(data.QTIOnlineTestSessionAnswerSubID);
  self.QTIOnlineTestSessionAnswerID = ko.observable(data.QTIOnlineTestSessionAnswerID);
  self.VirtualQuestionSubID = ko.observable(data.VirtualQuestionSubID);
  self.QTISchemaID = ko.observable(data.QTISchemaID);
  self.AnswerChoice = ko.observable(data.AnswerChoice);
  self.Answered = ko.observable(data.Answered);
  self.AnswerText = ko.observable(data.AnswerText);
  self.AnswerImage = ko.observable(data.AnswerImage);
  self.DrawingContent = ko.observable(data.DrawingContent);
  self.IsReviewed = ko.observable(data.IsReviewed);
  self.PointsPossible = ko.observable(data.PointsPossible);
  self.ResponseIdentifier = ko.observable(data.ResponseIdentifier);
  self.ResponseProcessingTypeID = ko.observable(data.ResponseProcessingTypeID);
  self.PointsEarned = ko.observable(data.PointsEarned == null ? 0 : data.PointsEarned);
  self.AnswerTemp = ko.observable(data.AnswerTemp);
  self.Overridden = ko.observable(data.Overridden);
  self.UpdatedBy = ko.observable(data.UpdatedBy);
  self.UpdatedDate = ko.observable(data.UpdatedDate);

  self.Reviewable = ko.computed(function () {
    var qtiSchemaId = self.QTISchemaID();

    if (qtiSchemaId == 9) {
      return self.ResponseProcessingTypeID() == 2;
    }
    if (qtiSchemaId == 10) {
      return self.ResponseProcessingTypeID() == 1;
    }
    return false;
  });
}

function VirtualTest(data) {
  var self = this;
  self.VirtualTestID = ko.observable(data.VirtualTestID);
  self.VirtualTestName = ko.observable(data.VirtualTestName);
  self.TestScoreMethodID = ko.observable(data.TestScoreMethodID);
}

function TeacherReviewerViewModel(options) {
  var self = this;

  self.ReviewerWidget = options.ReviewerWidget;
  self.QuestionRenderWidget = options.QuestionRenderWidget;
  self.ReviewerValidation = options.ReviewerValidation;
  self.StudentFilterWidget = options.StudentFilterWidget;
  self.GradingShortcutsWidget = options.GradingShortcutsWidget;

  self.SelectFilterStudentChangeTrigger = function () {
    $('#selectFilterStudents').trigger('change');
  };
  self.SelectStudentChangeTrigger = function () {
    $('#selectStudents').trigger('change');
  };
  self.SelectFilterQuestionChangeTrigger = function () {
    $('#selectFilterQuestions').trigger('change');
  };

  self.SelectQuestionChangeTrigger = function () {
    $('#selectQuestion').trigger('change');
  };

  self.QTITestClassAssignmentID = ko.observable(null);
  self.StudentId = ko.observable(null);
  self.VirtualTest = ko.observable(null);
  self.TestName = ko.computed(function () {
    if (!self.VirtualTest()) {
      return null;
    }

    return encodeURIComponent(self.VirtualTest().VirtualTestName());
  });

  self.SubmitGradingTests = false;
  self.TeacherName = ko.observable(null);
  self.ClassName = ko.observable(null);

  self.FirstName = ko.observable('');
  self.LastName = ko.observable('');
  self.DisplayName = ko.computed(function () {
    return self.LastName() + ', ' + self.FirstName()
  });

  self.Students = ko.observableArray([]);

  self.TotalVisibleStudents = ko.computed(function () {
    var studentsVisibleCount = 0;

    ko.utils.arrayForEach(self.Students(), function (student) {
      if (student.StudentVisible()) {
        studentsVisibleCount++;
      }
    });

    return studentsVisibleCount - 1;
  });

  self.VirtualTestSubtypeID = ko.observable('');
  self.BranchingTest = ko.observable('');

  self.GetVisibleStudents = function () {
    var result = ko.utils.arrayFilter(self.Students(), function (student) {
      return student.StudentVisible() && student.StudentID() != -1;
    });

    return result;
  };

  self.GetReadySubmitAndCompleteStudents = function () {
    var result = ko.utils.arrayFilter(self.Students(), function (student) {
      return student.StudentVisible() && student.StudentID() != -1 && (student.IsComplete() || student.CanBulkGrading());
    });
    return result;
  };

  self.GetReviewableQuestions = function () {
    var result = ko.utils.arrayFilter(self.Questions(), function (question) {
      return question.VisibleQuestion() && question.Reviewable() && question.VirtualQuestionID() > 0
    });
    return result;
  };

  self.SelectedStudentID = ko.observable();

  self.SelectedQTIOnlineTestSessionID = ko.computed(function () {
    var selectedStudent = self.StudentFilterWidget.StudentFilterWidget('GetStudentByStudentID', self.SelectedStudentID());
    if (!Reviewer.IsNullOrEmpty(selectedStudent)) {
      return selectedStudent.QTIOnlineTestSessionID();
    }
    return null;
  });

  self.EmptyStudentFilter = ko.observable(new StudentFilter({ FilterName: 'All', FilterValue: 'Empty', FilterCount: 0 }));
  self.AllStudentFilter = ko.observable(new StudentFilter({ FilterName: 'All', FilterValue: 'All', FilterCount: -1 }));
  self.CompletedStudentFilter = ko.observable(new StudentFilter({ FilterName: 'Completed', FilterValue: 'Completed', FilterCount: 0 }));
  self.ReadyToSubmitStudentFilter = ko.observable(new StudentFilter({ FilterName: 'Ready to Submit', FilterValue: 'ReadyToSubmit', FilterCount: 0 }));
  self.PendingReviewStudentFilter = ko.observable(new StudentFilter({ FilterName: 'Pending Review', FilterValue: 'PendingReview', FilterCount: 0 }));
  self.InprogressStudentFilter = ko.observable(new StudentFilter({ FilterName: 'In Progress', FilterValue: 'Inprogress', FilterCount: 0 }));
  self.PausedStudentFilter = ko.observable(new StudentFilter({ FilterName: 'Paused', FilterValue: 'Paused', FilterCount: 0 }));
  self.NotStartedStudentFilter = ko.observable(new StudentFilter({ FilterName: 'Not Started', FilterValue: 'NotStarted', FilterCount: 0 }));

  self.StudentFilters = ko.observableArray([]);

  self.StudentFilters.push(self.EmptyStudentFilter());
  self.StudentFilters.push(self.AllStudentFilter());
  self.StudentFilters.push(self.CompletedStudentFilter());
  self.StudentFilters.push(self.ReadyToSubmitStudentFilter());
  self.StudentFilters.push(self.PendingReviewStudentFilter());
  self.StudentFilters.push(self.InprogressStudentFilter());
  self.StudentFilters.push(self.PausedStudentFilter());
  self.StudentFilters.push(self.NotStartedStudentFilter());

  self.StudentFilterComputed = ko.computed(function () {
    self.AllStudentFilter().FilterCount(-1);
    self.CompletedStudentFilter().FilterCount(0);
    self.ReadyToSubmitStudentFilter().FilterCount(0);
    self.PendingReviewStudentFilter().FilterCount(0);
    self.InprogressStudentFilter().FilterCount(0);
    self.PausedStudentFilter().FilterCount(0);
    self.NotStartedStudentFilter().FilterCount(0);

    ko.utils.arrayForEach(self.Students(), function (student) {
      self.AllStudentFilter().Plus();

      if (student.IsComplete()) {
        self.CompletedStudentFilter().Plus();
      }

      if (student.CanBulkGrading()) {
        self.ReadyToSubmitStudentFilter().Plus();
      }

      if (student.IsPendingReview() && !student.CanBulkGrading()) {
        self.PendingReviewStudentFilter().Plus();
      }

      if (student.InProgress()) {
        self.InprogressStudentFilter().Plus();
      }

      if (student.Paused()) {
        self.PausedStudentFilter().Plus();
      }

      if (student.IsNotStart()) {
        self.NotStartedStudentFilter().Plus();
      }
    });
  });

  self.SelectedStudentFilter = ko.observable('');
  self.SelectedStudentFilterNew = ko.observable('');

  self.SelectedStudentFilterFunction = function () {
    var selectedStudentFilter = self.SelectedStudentFilter();
    if (Reviewer.IsNullOrEmpty(selectedStudentFilter) || selectedStudentFilter == 'Empty') return;
    self.StudentFilterWidget.StudentFilterWidget('FilterStudent', self.Students(), selectedStudentFilter);
  };

  self.IsConfirmUser = function () {
    return self.ReviewerValidation.ReviewerValidationWidget('CheckRequireSubmitTestFilterStudent');
  };

  self.SelectedStudentFilterNew.subscribe(function () {
    if (self.SelectedStudentFilterNew() === self.SelectedStudentFilter()) {
      return;
    }

    if (!self.IsConfirmUser()) {
      self.SelectedStudentFilter(self.SelectedStudentFilterNew());
    }
  });

  self.SelectedStudentFilter.subscribe(function () {
    self.SelectedStudentFilterFunction();
    self.SelectedStudentID(-1);
    self.SelectStudentChangeTrigger();
    self.ReviewerWidget.ReviewerWidget('ResetReviewer', self);
  });

  self.SetStudentOptionAttrs = function (option, item) {
    ko.applyBindingsToNode(option, { attr: { 'data-status': item.StatusClass, 'data-visible': item.StudentVisible } }, item);
  };

  self.Questions = ko.observableArray([]);
  self.QuestionsBulk = ko.observableArray([]);

  self.QuestionFiltersList = ko.observableArray([]);
  self.QuestionFilters = ko.observableArray();

  self.QuestionFilterCorrect = ko.observable(
    new QuestionFilter({
      Id: 'credit-filter-full',
      Value: 'credit-full',
      IconClasses: 'icon icon-credit-filter icon-credit-filter-full',
      Text: 'Correct'
    })
  );

  self.QuestionFilterPartial = ko.observable(
    new QuestionFilter({
      Id: 'credit-filter-partial',
      Value: 'credit-partial',
      IconClasses: 'icon icon-credit-filter icon-credit-filter-partial',
      Text: 'Partial'
    })
  );

  self.QuestionFilterNoCredit = ko.observable(
    new QuestionFilter({
      Id: 'credit-filter-no-credit',
      Value: 'credit-no-credit',
      IconClasses: 'icon icon-credit-filter icon-credit-filter-no-credit',
      Text: 'No Credit'
    })
  );

  self.QuestionFiltersList.push(self.QuestionFilterCorrect());
  self.QuestionFiltersList.push(self.QuestionFilterPartial());
  self.QuestionFiltersList.push(self.QuestionFilterNoCredit());

  self.QuestionFilters.subscribe(function () {
    self.FilterQuestionByCredit(self.QuestionFilters());
    self.ChooseSelectedQuestion();
  });

  self.FilterQuestionByCredit = function (filters) {
    var questions = self.Questions();

    if (filters.length) {
      ko.utils.arrayForEach(questions, function (question) {
        if (filters.indexOf(question.PointsFilter()) > -1) {
          question.IsShowByQuestion(true);
        } else {
          question.IsShowByQuestion(false);
        }
      });
    } else {
      ko.utils.arrayForEach(questions, function (question) {
        question.IsShowByQuestion(true);
      });
    }
  };

  self.TotalVisibleQuestions = ko.computed(function () {
    var result = 0;

    ko.utils.arrayForEach(self.Questions(), function (question) {
      if (question.VisibleQuestion()) {
        result++;
      }
    });
    return result;
  });

  self.TotalQuestion = ko.computed(function () {
    return self.Questions().length;
  });
  self.TestOnlineSessionAnswers = ko.observableArray([]);
  self.Respones = ko.observable('');
  self.Respones.extend({ notify: 'always' });
  self.showTimeLog = ko.observable('');
  self.SectionInstruction = ko.observable('');
  self.SectionInstruction.extend({ notify: 'always' });
  self.ShowSectionInstruction = ko.computed(function () {
    var $sectionInstruction = $(self.SectionInstruction());
    var isShowSectionInstruction = self.SelectedStudentID() !== -1 &&
      !Reviewer.IsNullOrEmpty(self.SelectedStudentID()) &&
      !Reviewer.IsNullOrEmpty(self.SectionInstruction()) &&
      !Reviewer.IsNullOrEmpty($sectionInstruction.text());
    return isShowSectionInstruction;
  });
  self.Mode = ko.observable(null);

  self.PointsPossible = ko.observable('');
  self.ResponseProcessingTypeID = ko.observable('');
  self.PointsEarned = ko.observable('');
  self.OldPointsEarned = ko.observable('');
  self.QTIOnlineTestSessionID = ko.observable('');
  self.AnswerID = ko.observable('');
  self.AnswerSubID = ko.observable('');
  self.AnswerImage = ko.observable('');
  self.DrawingContent = ko.observable(null);
  self.QTIItemSchemaID = ko.observable('');
  self.SelectedQuestion = ko.observable('');
  self.IsComplete = ko.observable('');
  self.IsPendingReview = ko.observable('');
  self.IsNotStart = ko.observable('');
  self.Paused = ko.observable('');
  self.InProgress = ko.observable('');
  self.RefObjects = ko.observable([]);
  self.AnswerAttachments = ko.observable([]);
  self.IsGraded = ko.observable(false);
  self.SelectedStudent = null;
  self.NextStudent = null;
  self.Rubric = ko.observable({ Success: false });
  self.StudentNameSortDirection = ko.observable('');
  self.StudentStatusSortDirection = ko.observable('');
  self.OverrideAutoGraded = ko.observable(false);
  self.OverrideItems = ko.observableArray([]);
  self.OverrideAutoGradedOptionValue = ko.observable('');
  self.Expired = ko.observable('');
  self.MultipleChoiceClickMethod = ko.observable('');
  self.ScoreRaw = ko.observable('');
  self.SubtractFrom100PointPossible = ko.observable(0);
  self.GradingType = ko.observable('student');

  self.IsRubricBasedQuestion = ko.computed(function () {
    if (Reviewer.IsNullOrEmpty(self.SelectedQuestion())) return false;

    return !!self.SelectedQuestion().IsRubricBasedQuestion;
  });

  self.RubricTestResultScores = ko.computed(function () {
    var rubricTestResultScores = [];

    if (!Reviewer.IsNullOrEmpty(self.SelectedQuestion())) {
      ko.utils.arrayForEach(self.SelectedQuestion().RubricQuestionCategories(), function (category) {
        rubricTestResultScores.push({
          rubricQuestionCategoryID: category.CategoryID(),
          score: category.PointEarn()
        });
      });
    }

    return self.IsRubricBasedQuestion() ? rubricTestResultScores : null;
  });

  self.GradingType.subscribe(function (newGradingType) {
    var elQuestionStudent = document.querySelector('.assignment-question-student');
    var elListQuestion = document.querySelectorAll('.assignment-list-question');

    if (self.IsFullScreen()) {
      for (var i = 0; i < elListQuestion.length; i++) {
        elListQuestion[i].style.width = elQuestionStudent.clientWidth - 150 + 'px';;
      }
    } else {
      for (var i = 0; i < elListQuestion.length; i++) {
        elListQuestion[i].style.width = '200px';
      }
    }
  });

  self.IsLastStudentGradingItem = ko.observable(false);
  self.IsNextApplicableStudent = ko.observable(false);
  self.IsNextApplicableQuestionGradeByStudent = ko.observable(false);
  self.VisitedTimes = ko.observable(0);
  self.TotalSpentTimeOnQuestion = ko.observable('0s');
  self.TotalSpentTimeOnTest = ko.observable('0s');
  self.PostAnswerLogs = ko.observableArray([]);

  self.ManuallyGradedOnly = ko.observable(false);//Init unchecked
  self.AutoSelectFirstPendingTestSession = ko.observable();

  self.GradingProcessStatus = ko.observable(5);

  self.UpdateManuallyGradedOnlyOfAllQuestion = function (manuallyGradedOnly) {
    ko.utils.arrayForEach(self.Questions(), function (item) {
      if (item.IsRestrictedManualGrade) {
        item.ManuallyGradedOnly(item.IsRestrictedManualGrade);
      }
      else {
        item.ManuallyGradedOnly(manuallyGradedOnly);
      }
    });
  };

  self.ChooseSelectedQuestion = function () {
    if (self.Questions().length <= 0) {
      return;
    }

    var selectedQuestionVirtualQuestionID = 0;

    if (Reviewer.IsNullOrEmpty(self.SelectedQuestion()) || !self.SelectedQuestion().VisibleQuestion()) {
      var firstVisibleQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
        return question.VisibleQuestion();
      });

      if (!Reviewer.IsNullOrEmpty(firstVisibleQuestion)) {
        self.QuestionClick(firstVisibleQuestion);
        selectedQuestionVirtualQuestionID = firstVisibleQuestion.VirtualQuestionID();
      } else {
        self.QuestionClick(self.Questions()[0]);
        selectedQuestionVirtualQuestionID = self.Questions()[0].VirtualQuestionID();
      }
    } else if (self.SelectedQuestion().VisibleQuestion()) {
      self.QuestionClick(self.SelectedQuestion());
      selectedQuestionVirtualQuestionID = self.SelectedQuestion().VirtualQuestionID();
    }

    self.SelectedQuestionVirtualQuestionID(selectedQuestionVirtualQuestionID);
    self.SelectQuestionChangeTrigger();
  };

  self.ManuallyGradedOnlyClickHandler = function (item) {
    self.UpdateManuallyGradedOnlyOfAllQuestion(item.ManuallyGradedOnly());
    self.ChooseSelectedQuestion();
    return true;
  };

  self.IsBulkGrading = ko.observable(false);
  self.UpdatedBy = ko.observable("");
  self.ItemFeedbackBeforeApplyGrade = '';
  self.UpdatedDate = ko.observable("");
  self.Overridden = ko.observable(false);
  self.PrintGuidance = ko.observable(true);

  // Feedback
  self.FeedbackOverall = ko.observable('');
  self.OldFeedbackOverall = ko.observable('');
  self.FeedbackQuestion = ko.observable('');
  self.OldFeedbackQuestion = ko.observable('');
  self.FeedbackOverallHistory = ko.observable('');

  self.LockSaveFeedbackOverallBtn = ko.observable(false);
  self.LockSaveFeedbackQuestionBtn = ko.observable(false);


  self.FeedbackQuestionHistoryMessage = ko.computed(function () {
    var testOnlineAnswer,
      feedbackUpdatedBy,
      feedbackUpdatedDate,
      scoreUpdatedBy = self.UpdatedBy(),
      scoreUpdatedDate = self.UpdatedDate();

    if (self.TestOnlineSessionAnswers()) {
      ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
        if (self.SelectedQuestion().VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
          testOnlineAnswer = testOnlineSessionAnswer;
        }
      });
    }

    if (testOnlineAnswer) {
      feedbackUpdatedBy = testOnlineAnswer.UserUpdatedFeedback();
      feedbackUpdatedDate = testOnlineAnswer.DateUpdatedFeedback();
    }

    if (Reviewer.IsNullOrEmpty(feedbackUpdatedDate) && Reviewer.IsNullOrEmpty(scoreUpdatedDate)) {
      return '';
    }

    if (Reviewer.IsNullOrEmpty(feedbackUpdatedDate) || moment.utc(feedbackUpdatedDate).isSameOrBefore(scoreUpdatedDate)) {
      return 'Updated by ' + scoreUpdatedBy + ' on ' + scoreUpdatedDate;
    }

    return 'Updated by ' + getFullNameOnly(feedbackUpdatedBy) + ' on ' + displayDateWithFormat(moment.utc(feedbackUpdatedDate).toDate().valueOf(), true);
  });
  // Audio feedback
  self.TeacherFeebackAttachment = ko.observable(null);
  self.TeacherFeedbackAttachmentSetting = ko.observable(null);
  self.LockRecordFeedbackBtn = ko.observable(true);
  self.IsRecordingFeedback = ko.observable(false);
  self.HasChangedTeacherAttachment = ko.observable(false);
  self.RecordAudioInstance = new RecordAudio()
  self.TeacherReviewFeedbackFile = ko.observable(null);
  self.RecordFeedbackPlaybackURL = ko.observable(null);

  self.AudioPlayerOptions = ko.observable({ url: '', options: { removeable: false } });

  // Message Dirty
  self.CheckFeedbackDirtyMessage = ko.observable('');

  self.BlockUI = function () {
    ShowBlock($('#StudentPreferenceArticle'), 'Waiting');
  };
  self.BlockUIGetNextStudent = function () {
    ShowBlock($('#StudentPreferenceArticle'), 'Loading Next Student...');
  };
  self.UnBlockUI = function () {
    $('#StudentPreferenceArticle').unblock();
  };

  self.FeedBackFormVisible = ko.computed(function () {
    var result = false;

    if (self.Students().length) {
      if (!Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
        if (self.SelectedStudent.Status() === 'NotStarted') {
          result = false;
        }
      }

      // Display overall test feedback and feedback for question
      // When the question was answered when student was completed the test
      // or pending review status
      // or expired time
      // or can grading
      if (!Reviewer.IsNullOrEmpty(self.SelectedQuestion())) {
        if ((self.IsPendingReview() || self.IsComplete() ||
          (self.Expired() && (self.InProgress() || self.Paused())))) {
          result = true;
        }
      }

      if (self.CanGrading()) {
        result = true;
      }
    }

    if (result) {
      self.LockSaveFeedbackOverallBtn(false);
    } else {
      self.LockSaveFeedbackOverallBtn(true);
    }

    return result;
  });

  self.FeedBackFormQuestionVisible = ko.computed(function () {
    var result = false;
    if (Reviewer.IsNullOrEmpty(self.AnswerID()) || self.AnswerID() == '0') {
      self.LockSaveFeedbackQuestionBtn(true);
      self.LockRecordFeedbackBtn(true);

      return false;
    }

    if (self.Students().length) {
      if (!Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
        if (self.SelectedStudent.Status() === 'NotStarted') {
          result = false;
        }
      }

      // Display overall test feedback and feedback for question
      // When the question was answered when student was completed the test
      // or pending review status
      // or expired time
      // or can grading
      if (!Reviewer.IsNullOrEmpty(self.SelectedQuestion())) {
        if ((self.IsPendingReview() || self.IsComplete() ||
          (self.Expired() && (self.InProgress() || self.Paused())))) {
          result = true;
        }
      }

      if (self.CanGrading()) {
        result = true;
      }
    }

    if (result) {
      self.LockSaveFeedbackQuestionBtn(false);
      self.LockRecordFeedbackBtn(false);
    } else {
      self.LockSaveFeedbackQuestionBtn(true);
      self.LockRecordFeedbackBtn(true);
      var playerOpts = self.AudioPlayerOptions();
      playerOpts.options = Object.assign(playerOpts.options, { removeable: false });
      self.AudioPlayerOptions(playerOpts);
    }

    return result;
  });
  self.ShowFeedbackOverallHistory = ko.computed(function () {
    return !Reviewer.IsNullOrEmpty(self.FeedbackOverallHistory());
  });

  self.ShowFeedbackQuestionHistory = ko.computed(function () {
    return !Reviewer.IsNullOrEmpty(self.FeedbackQuestionHistoryMessage());
  });

  self.ShowHightLight = function (highlightQuestion, question) {
    if (Reviewer.IsNullOrEmpty(highlightQuestion)) {
      question.XmlContent(question.DataXmlContent());
      return;
    }

    var responseDeclarationSelector = 'responsedeclaration, responseDeclaration';
    var tree = $("<div>" + highlightQuestion + "</div>");
    tree.find(responseDeclarationSelector)
      .replaceWith(function () { return $(''); });
    highlightQuestion = tree.html();

    var correctResponseContent = '';
    $(question.DataXmlContent()).find(responseDeclarationSelector).each(function () {
      var correctResponseElement = $(this);
      correctResponseContent = correctResponseContent + correctResponseElement.outerHTML();
    });

    tree = $("<div>" + highlightQuestion + "</div>");
    tree.find('itembody, itemBody')
      .replaceWith(function () {
        return $(correctResponseContent + $(this).outerHTML());
      });
    highlightQuestion = tree.html();
    question.XmlContent(highlightQuestion);
  };

  self.DefaultDisplayStudentFilter = function () {
    var selectedStudentID = null;

    if (self.PendingReviewStudentFilter().FilterCount() > 0) {
      self.SelectedStudentFilterNew(self.PendingReviewStudentFilter().FilterValue());
      selectedStudentID = self.GetVisibleStudents()[0].StudentID();
    } else if (self.ReadyToSubmitStudentFilter().FilterCount() > 0) {
      self.SelectedStudentFilterNew(self.ReadyToSubmitStudentFilter().FilterValue());
      selectedStudentID = self.GetVisibleStudents()[0].StudentID();
    } else {
      self.SelectedStudentFilterNew(self.AllStudentFilter().FilterValue());

      if (self.AllStudentFilter().FilterCount() == 1) {
        selectedStudentID = self.GetVisibleStudents()[0].StudentID();
      }
    }

    return selectedStudentID;
  };

  /*Group Question*/

  var answerLabels = [
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q',
    'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z', 'AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'AG', 'AH',
    'AI', 'AJ', 'AK', 'AL', 'AM', 'AN', 'AO', 'AP', 'AQ', 'AR', 'AS', 'AT', 'AU', 'AV', 'AW',
    'AX', 'AY', 'AZ', 'BA', 'BB', 'BC', 'BD', 'BE', 'BF', 'BG', 'BH', 'BI', 'BJ', 'BK', 'BL',
    'BM', 'BN', 'BO', 'BP', 'BQ', 'BR', 'BS', 'BT', 'BU', 'BV', 'BW', 'BX', 'BY', 'BZ', 'CA',
    'CB', 'CC', 'CD', 'CE', 'CF', 'CG', 'CH', 'CI', 'CJ', 'CK', 'CL', 'CM', 'CN', 'CO', 'CP',
    'CQ', 'CR', 'CS', 'CT', 'CU', 'CV', 'CW', 'CX', 'CY', 'CZ'
  ];
  self.GroupQuestions = ko.observableArray([]);

  function createGroupQuestions(questions) {
    var newTempGroup = function (groupID, isHeader) {
      return {
        groupID: ko.observable(groupID),
        items: ko.observableArray([]),
        isHeader: ko.observable(isHeader)
      };
    };
    var tempGroup, oldGroupID = null, newGroup, order = 0, isHeader = false;
    $.each(questions, function (index, item) {
      var groupID = item.QuestionGroupID();
      if (index == 0) {
        newGroup = true;
      }
      else {
        if (oldGroupID == groupID) {
          newGroup = false;
        }
        else {
          newGroup = true;
        }
      }

      var isLastItem = index + 1 == questions.length ? true : false;

      if (newGroup) {
        if (tempGroup && oldGroupID != groupID) {
          self.GroupQuestions.push(tempGroup);
          tempGroup = null;
        }
        if (groupID == null) {
          tempGroup = newTempGroup(null, false);
          isHeader = false;
        }
        else {
          tempGroup = newTempGroup(groupID, true);
          isHeader = true;
        }
      }
      oldGroupID = groupID;
      tempGroup.items.push(item);
      if (isLastItem && tempGroup != null) {
        self.GroupQuestions.push(tempGroup);
      }
    });

    getOrder(self.GroupQuestions());
  }

  self.GroupQuestionsDisplay = ko.observableArray([]);

  function getOrder(groups) {
    var order = 0, orderDisplay;
    $.each(groups, function (indexGroup, itemGroup) {
      if (!(itemGroup.isHeader())) {
        $.each(itemGroup.items(), function (index, item) {
          order += 1;
          item.QuestionOrderDisplay = ko.observable(order);
        });
      }
      else {
        order += 1;
        $.each(itemGroup.items(), function (index, item) {
          item.QuestionOrderDisplay = ko.observable(order + answerLabels[index].toLowerCase());
        });
      }

      self.GroupQuestionsDisplay.push(itemGroup);
    });
  }
  self.RefreshStudentNames = function () {
    ko.utils.arrayForEach(self.Students(), function (student) {
      if (student.RealStudentName()) {
        student.StudentName(student.RealStudentName());
      }
    });

    self.IsAnonymizedScoring(false);
    $('.teacherReviewerFullPage').css('background-color', 'initial');
    $('#grade-manually').prop('disabled', false)
    $('.anonymized-label').css('display', 'none');
    $('.block-reviewer-cancel .btn-assignment').css('background-color', 'initial');
  };
  self.RefreshStudents = function () {
    $('body').ReviewerWidget('GetStudentsForAssignment', self.QTITestClassAssignmentID(), self.StudentId(),
      function (data) {
        var mappedStudents = $.map(data.Result, function (item) { return new Student(item); });

        mappedStudents.unshift(new Student({ StudentName: 'Select Student', StudentID: -1, QTIOnlineTestSessionID: null }));

        self.Students(mappedStudents);
        self.IsAnonymizedScoring(data.IsAnonymizedScoring);

        $('body').ReviewerWidget(
          'GetQuestionsForAssignment',
          self.VirtualTest().VirtualTestID(),
          self.QTITestClassAssignmentID(),
          self.DistrictId,
          function (data) {
            var mappedQuestions = $.map(data, function (item) {
              var question = new Question(item);
              question.VirtualTestSubtypeID(viewModel.VirtualTestSubtypeID());
              question.PointsPossibleCredit(item.PointsPossible);
              return question;
            });

            createGroupQuestions(mappedQuestions);
            self.Questions(mappedQuestions);
            self.SelectFilterQuestionChangeTrigger();
            if (self.IsAnonymizedScoring()) {
              $('.teacherReviewerFullPage').css('background-color', '#e6e678');
              $('.anonymized-label').css('display', 'inline');
              $('.block-reviewer-cancel .btn-assignment').css('background-color', '#e6e678');
              self.ManuallyGradedOnly(true);
              $('#grade-manually').prop('disabled', true)
              $('#grade-manually').trigger('click');
            } else {
              $('.teacherReviewerFullPage').css('background-color', 'initial');
              $('#grade-manually').prop('disabled', false)
              $('.block-reviewer-cancel .btn-assignment').css('background-color', 'initial');
              $('.anonymized-label').css('display', 'none');
            }
            // After load all questions, we will selected student and load data of student
            var selectedStudentID = null;
            selectedStudentID = self.DefaultDisplayStudentFilter();
            self.SelectedStudentID(selectedStudentID);
          }, null);
      }, null);
  };

  self.QuestionsRendered = function () {
    var $divQuestionMenu = $('#divQuestionMenu');
    calculatorSequenceWidth($divQuestionMenu.find('partialsequence'));
    self.ReviewerWidget.ReviewerWidget('LoadImages', $divQuestionMenu);
  };

  self.AfterRenderQuestionDetails = function () {
    var $divQuestionDetails = $('#divQuestionDetails')
    calculatorSequenceWidth($divQuestionDetails.find('partialsequence'));
    self.ReviewerWidget.ReviewerWidget('LoadImages', $divQuestionDetails);
  };

  self.QuestionClick = function (question) {
    // Check Feedback Dirty Before Leaving Item
    self.CheckFeedbackDirtyMessage('L');
    if (!self.CheckFeedbackDirty()) {
      return;
    }

    question.ResetRubricQuestionCategories();
    self.QuestionRenderWidget.QuestionRender('Display', question);
  };

  self.SetSelectedQuestionCss = ko.computed(function () {
    // Active selected choose question
    if (self.Questions().length) {
      ko.utils.arrayForEach(self.Questions(), function (question) {
        question.QuestionSelectedCss(self.SelectedQuestion() === question);
      });
    }
  });

  self.RequireSubmitTest = ko.computed(function () {
    return !self.IsComplete() && self.IsGraded();
  });

  self.StudentClickList = function (student) {
    var selectedStudentID = student.StudentID();
    //self.SelectedStudentID(null);
    self.SelectedStudentID(selectedStudentID);
  };

  self.StudentClick = function (student) {
    if (student == null) {
      return;
    }
    // Check Feedback Dirty Before Select Other Student
    self.CheckFeedbackDirtyMessage('S');
    if (!self.CheckFeedbackDirty()) {
      return;
    }

    self.NextStudent = student;
    if (self.SelectedStudent == self.NextStudent) {
      return;
    }

    var isRequireSubmitTest = self.ReviewerValidation.ReviewerValidationWidget('CheckRequireSubmitTest');
    if (isRequireSubmitTest) {
      return;
    }
    self.IsComplete(student.IsComplete());
    self.IsPendingReview(student.IsPendingReview());
    self.IsNotStart(student.IsNotStart());
    self.Paused(student.Paused());
    self.InProgress(student.InProgress());
    self.QTIOnlineTestSessionID(student.QTIOnlineTestSessionID());
    self.SelectedStudent = student;

    if (student.StudentID() == -1) {
      return;
    }

    if (Reviewer.IsNullOrEmpty(student.QTIOnlineTestSessionID()) ||
      student.QTIOnlineTestSessionID() === '0') {
      var msgStudent = '<p>' + student.StudentName() + ' does not have any test data</p>';
      Reviewer.popupAlertMessage(msgStudent, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
      self.ReviewerWidget.ReviewerWidget('ResetReviewer', self);
      return;
    }

    // Show or hide view rubric button
    self.ReviewerWidget.ReviewerWidget(
      'GetRubricByVirtualTest',
      self.VirtualTest().VirtualTestID(),
      function (data) {
        self.Rubric(new Rubric(data));
        self.IsVisibleCodingGuide(self.Rubric().Success());
      },
      function () {
        var errorData = { Success: false };
        self.Rubric(new Rubric(errorData));
      }
    );

    var loadingMsg = 'Loading';

    if (self.IsNextApplicableStudent()) {
      loadingMsg = 'Loading Next Student';
    }

    if (self.IsNextApplicableQuestionGradeByStudent()) {
      loadingMsg = 'All questions have been graded for this student. Loading next student';
    }

    self.ReviewerWidget.ReviewerWidget(
      'GetTestOnlineSessionAnswers',
      student.QTIOnlineTestSessionID(),
      self.QTITestClassAssignmentID(),
      student.StartDate(),
      loadingMsg,
      function (allData) {
        if (allData.data != null && !allData.data.length) {
          self.ReviewerWidget.ReviewerWidget('ResetReviewer', self);
        }

        var mappedObjects = $.map(allData.data, function (item) {
          return new TestOnlineSessionAnswer(item);
        });

        self.TestOnlineSessionAnswers(mappedObjects);

        self.GradingProcessStatus(allData.gradingProcessStatus);
        self.Expired(allData.expired);
        self.MultipleChoiceClickMethod(allData.multipleChoiceClickMethod);
        self.BranchingTest(allData.branchingTest);
        self.ScoreRaw(allData.scoreRaw);
        self.SubtractFrom100PointPossible(allData.subtractFrom100PointPossible);
        student.TestFeedbackId(allData.testFeedbackId);
        student.FeedbackContent(allData.feedbackContent);

        var historyUpdated = 'Updated by ';
        var lastDateUpdatedFeedback = '';

        if (!Reviewer.IsNullOrEmpty(allData.lastUserUpdatedFeedback)) {
          historyUpdated += getFullNameOnly(allData.lastUserUpdatedFeedback);
        }

        if (!Reviewer.IsNullOrEmpty(allData.lastDateUpdatedFeedback) &&
          allData.lastDateUpdatedFeedback.trim().length > 0) {
          lastDateUpdatedFeedback = allData.lastDateUpdatedFeedback;
          lastDateUpdatedFeedback = displayDateWithFormat(moment.utc(lastDateUpdatedFeedback).toDate().valueOf(), true);
          historyUpdated += ' on ' + lastDateUpdatedFeedback;
        }

        self.OldFeedbackOverall(self.FeedbackOverall());
        self.FeedbackOverallHistory(historyUpdated);
        student.LastUserUpdatedFeedback(allData.lastUserUpdatedFeedback);
        student.LastDateUpdatedFeedback(allData.lastDateUpdatedFeedback);
        // Record audio teacher review
        self.TeacherFeedbackAttachmentSetting(allData.teacherFeedbackAttachmentSetting);
        self.TeacherReviewFeedbackFile(null);
        self.RecordFeedbackPlaybackURL(null);

        var totalTimeOnTest = 0;
        ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
          totalTimeOnTest += testOnlineSessionAnswer.TimeSpent();
        });

        var time = '0s';
        if (!isNaN(totalTimeOnTest) && totalTimeOnTest > 0) {
          time = Reviewer.prettyTime(totalTimeOnTest);
        }

        self.TotalSpentTimeOnTest(time);

        ko.utils.arrayForEach(self.Questions(), function (item) {
          item.Answer(null);
          item.BranchingTest(self.BranchingTest());

          ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
            if (item.VirtualQuestionID() == testOnlineSessionAnswer.VirtualQuestionID()) {
              item.PointsEarnedCredit(testOnlineSessionAnswer.PointsEarned());
            }
          });

          var anwserData = ko.utils.arrayFirst(allData.data, function (testOnlineSessionAnswer) {
            return item.VirtualQuestionID() == testOnlineSessionAnswer.VirtualQuestionID;
          });
          if (anwserData && anwserData.RubricQuestionCategories && anwserData.RubricQuestionCategories.length) {
            item.InitializeRubricQuestionCategories(anwserData.RubricQuestionCategories);
          }
          var answer = ko.utils.arrayFirst(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
            return item.VirtualQuestionID() == testOnlineSessionAnswer.VirtualQuestionID();
          });

          var testOnlineSessionAnswers = self.TestOnlineSessionAnswers();
          var hasTestOnlineSessionAnswers = !Reviewer.IsNullOrEmpty(testOnlineSessionAnswers);

          if (!Reviewer.IsNullOrEmpty(answer) && hasTestOnlineSessionAnswers) {
            item.Answer(answer);
            item.AnswerOrder(testOnlineSessionAnswers.indexOf(answer) + 1);
            answer.AnswerOfBaseQuestion(item.IsBaseVirtualQuestion());
          }
        });

        self.UpdateManuallyGradedOnlyOfAllQuestion(self.ManuallyGradedOnly());
        self.ChooseSelectedQuestion();
        self.BranchingTestSort();
        self.FilterQuestionByCredit(self.QuestionFilters());

        if (self.GradingProcessStatus() == 3 || self.GradingProcessStatus() == 4 || self.GradingProcessStatus() == 6) {
          var message = 'The test has been placed in the grading queue. Please check for results later.';
          if (self.GradingProcessStatus() == 4) {
            message = 'The grading has failed.';
            Reviewer.popupAlertMessage(message, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 250, 100);
            return true;
          }

          Reviewer.popupAlertMessage(message, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 575, 100);
          return true;
        }
      }, null);
    self.IsNextApplicableStudent(false);
    self.IsNextApplicableQuestionGradeByStudent(false);
  };

  self.SelectedStudentID.subscribe(function (newStudentID) {
    if (Reviewer.IsNullOrEmpty(self.SelectedStudentID()) || self.SelectedStudentID() === -1) {
      self.ReviewerWidget.ReviewerWidget('ResetReviewer', self);
      return;
    }

    var selectedStudent = self.StudentFilterWidget.StudentFilterWidget('GetStudentByStudentID', self.SelectedStudentID());
    if (!Reviewer.IsNullOrEmpty(selectedStudent)) {
      self.StudentClick(selectedStudent);
    }
  });

  self.SelectedQuestionVirtualQuestionID = ko.observable();

  self.SelectedQuestionVirtualQuestionID.subscribe(function (virtualQuestionID) {
    if (Reviewer.IsNullOrEmpty(self.SelectedQuestionVirtualQuestionID()) || self.SelectedQuestionVirtualQuestionID() === -1) {
      return;
    }

    var selectedQuestion = ko.utils.arrayFirst(self.Questions(), function (question) {
      return question.VirtualQuestionID() === virtualQuestionID;
    })
    self.QuestionClick(selectedQuestion);
  });

  self.SetQuestionOptionAttrs = function (option, item) {
    ko.applyBindingsToNode(option, { attr: { 'data-visible': item.VisibleQuestion } }, item);
    self.SelectedQuestionVirtualQuestionID(item.selectedQuestionVirtualQuestionID);
  };

  //build html type message guidance, rationale
  self.CreateGraphicGuidance = function (itemMessage, typeMessage, typeQuesiton) {
    var typeMessageContent = '';
    var itemMessageAudioRef = itemMessage.audioRef;
    var itemMessageValueContent = itemMessage.valueContent;
    var wContent = 415;
    var hContent = 200;

    switch (typeQuesiton) {
      case 'inlinechoice':
        if (typeMessage === 'rationale' || typeMessage === 'guidance_rationale') {
          if (itemMessageAudioRef === '' || itemMessageAudioRef === undefined) {
            typeMessageContent += '<div class="' + typeMessage + '" typemessage="' + typeMessage + '">';
            typeMessageContent += '<div class="guidance-content">' + itemMessageValueContent + '</div>';
          } else {
            typeMessageContent += '<div class="' + typeMessage + '" typemessage="' + typeMessage + '">';
            typeMessageContent += '<div class="audioIcon">';
            typeMessageContent += '<img alt="Play audio" class="imageupload bntPlay" src="../../Content/themes/TestMaker/images/small_audio_play.png" title="Play audio">';
            typeMessageContent += '<img style="display: none;" alt="Stop audio" class="bntStop" src="../../Content/themes/TestMaker/plugins/multiplechoice/images/small_audio_stop.png" title="Stop audio">';
            typeMessageContent += '<span style="display: none;" class="audioRef">' + itemMessageAudioRef + '</span></div>';
            typeMessageContent += '<div class="guidance-content">' + itemMessageValueContent + '</div>';
          }
        }
        break;
      case 'simpleChoice':
      case 'textEntryInteraction':
        if (itemMessageAudioRef === '' || itemMessageAudioRef === undefined) {
          typeMessageContent += '<div identifier_responseidentifier="' + itemMessage.identifier + '_' + itemMessage.responseidentifier + '" class="hide ' + typeMessage + '" typemessage="' + typeMessage + '">';
          typeMessageContent += '<div class="guidance">';
          typeMessageContent += '<div class="guidance-content">' + itemMessageValueContent + '</div>';
          typeMessageContent += '</div>';
        } else {
          typeMessageContent += '<div identifier_responseidentifier="' + itemMessage.identifier + '_' + itemMessage.responseidentifier + '" class="hide ' + typeMessage + '" typemessage="' + typeMessage + '">';
          typeMessageContent += '<div class="guidance">';
          typeMessageContent += '<div class="audioIcon">';
          typeMessageContent += '<img alt="Play audio" class="imageupload bntPlay" src="../../Content/themes/TestMaker/images/small_audio_play.png" title="Play audio">';
          typeMessageContent += '<img style="display: none;" alt="Stop audio" class="bntStop" src="../../Content/themes/TestMaker/plugins/multiplechoice/images/small_audio_stop.png" title="Stop audio">';
          typeMessageContent += '<span style="display: none;" class="audioRef">' + itemMessageAudioRef + '</span></div>';
          typeMessageContent += '<div class="guidance-content">' + itemMessageValueContent + '</div>';
          typeMessageContent += '</div>';
        }
        break;
    }

    typeMessageContent += '</div>';
    return typeMessageContent;
  };

  self.buildGuidanceInlineChoice = function (listObjTypeMessages, responseidentifier) {
    var htmlContent = '';
    var isRationale = false;

    htmlContent += '<div class="guidance">';
    htmlContent += '<div class="guidance-heading">Rationale</div>';

    for (var i = 0, lenlistObjTypeMessages = listObjTypeMessages.length; i < lenlistObjTypeMessages; i++) {
      var item = listObjTypeMessages[i];
      var htmlItem = '';

      for (var j = 0, lenArrMessage = item.arrMessage.length; j < lenArrMessage; j++) {
        var objItemType = item.arrMessage[j];

        if (objItemType.typeMessage === 'rationale' ||
          objItemType.typeMessage === 'guidance_rationale') {
          htmlItem = self.CreateGraphicGuidance(objItemType, objItemType.typeMessage, 'inlinechoice');
          htmlContent += '<div class="guidance-label">' + item.value + '</div>';
          htmlContent += htmlItem;

          isRationale = true;
          htmlItem = '';
        }
      }
    }

    htmlContent += '</div>';

    if (htmlItem === '' && !isRationale) {
      htmlContent = '';
    }
    return htmlContent;
  };

  self.CanGrading = ko.computed(function () {
    return self.ReviewerWidget.ReviewerWidget(
      'CanGrading',
      self.QTIOnlineTestSessionID(),
      self.Mode(),
      self.SelectedQuestion(),
      self.AnswerID(),
      self.PointsEarned(),
      self.IsPendingReview(),
      self.Paused(),
      self.InProgress(),
      self.Expired(),
      self.QTIItemSchemaID(),
      self.OverrideAutoGraded(),
      self.ResponseProcessingTypeID(),
      self.IsComplete(),
      self.OverrideItems(),
      self.AnswerSubID(),
      self.GradingProcessStatus()
    );
  });

  self.CanGradingForceUIRefreshSignature = ko.observable("CanGradingForceUIRefresh");
  self.CanGradingForceUIRefresh = function () {
    var $signature = $('[signature="' + self.CanGradingForceUIRefreshSignature() + '"]');

    if (self.CanGrading()) {
      $signature.prop('disabled', false);
    } else {
      $signature.prop('disabled', true);
    }
  };

  self.GradeNumberInputCss = ko.computed(function () {
    if (self.SelectedQuestion() && self.SelectedQuestion().IsRubricBasedQuestion) {
      return false;
    }

    return self.CanGrading();
  });

  self.GradeButtonCss = ko.computed(function () {
    if (!Reviewer.IsNullOrEmpty(self.SelectedQuestion()) && self.SelectedQuestion().IsRubricBasedQuestion) {
      if (!self.SelectedQuestion().IsMarkedCategories()) {
        return false;
      }
    }

    return self.CanGrading();
  });

  self.Plus = function () {
    self.ReviewerWidget.ReviewerWidget('Plus', self);
  };

  self.Minus = function () {
    self.ReviewerWidget.ReviewerWidget('Minus', self);
  };

  self.UpdateAsnswerPointsEarned = function () {
    // Check Feedback Dirty Before Apply Grade
    self.CheckFeedbackDirtyMessage('G');
    if (!self.CheckFeedbackDirty()) {
      return;
    }

    self.ReviewerWidget.ReviewerWidget('UpdateAsnswerPointsEarned', self);
  };

  self.ViewReference = function (refObject) {
    var testOnlineSessionAnswer;

    if (self.TestOnlineSessionAnswers() !== null) {
      ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (item) {
        if (self.SelectedQuestion() &&
          self.SelectedQuestion().VirtualQuestionID() === item.VirtualQuestionID()) {
          testOnlineSessionAnswer = item;
        }
      });
    }

    var highlightPassages = '';
    if (!Reviewer.IsNullOrEmpty(testOnlineSessionAnswer) &&
      !Reviewer.IsNullOrEmpty(testOnlineSessionAnswer.HighlightPassage())) {
      highlightPassages = testOnlineSessionAnswer.HighlightPassage();
    }

    var answerImage = self.AnswerImage();
    var refObjectID = refObject.RefObjectID() === undefined ? '' : refObject.RefObjectID();
    var refObjectData = refObject.RefObjectData() === undefined ? '' : refObject.RefObjectData();

    var highlightPassage = '';

    $(highlightPassages).find('Passage, passage').each(function () {
      var passage = $(this);
      passage.find('RefObjectID, refobjectid').each(function () {
        var refObjectElement = $(this);
        if (refObjectElement.text() === refObjectID ||
          refObjectElement.text() === refObjectData) {
          passage.find('PassageContent, passagecontent').each(function () {
            var passageContent = $(this);
            highlightPassage = passageContent.text();
          });
        }
      });
    });

    if (Reviewer.IsNullOrEmpty(highlightPassages) ||
      Reviewer.IsNullOrEmpty(highlightPassage)) {
      self.ReviewerWidget.ReviewerWidget(
        'GetViewReferenceContent',
        answerImage,
        refObjectID,
        refObjectData
      );
    } else {
      var content = $(highlightPassage).html();
      self.ReviewerWidget.ReviewerWidget(
        'ShowReferenceContent',
        content,
        answerImage,
        refObjectID,
        refObjectData
      );
    }
  };

  self.ViewAnswerAttachment = function (answerAttachment) {
    var fileExt = answerAttachment.FileName.split('.').pop();
    var canPlayback = AudioPlayer.isSupportedType(fileExt && fileExt.toLowerCase());
    self.ReviewerWidget.ReviewerWidget(
      'ViewAttachment',
      answerAttachment.DocumentGuid,
      self.QTIOnlineTestSessionID(),
      canPlayback
    );
  };

  self.TotalPointsPossible = ko.computed(function () {
    var total = self.ReviewerWidget.ReviewerWidget(
      'CalculateTotalPointsPossible',
      self.Questions(),
      self.VirtualTest(),
      self.SubtractFrom100PointPossible()
    );
    return total;
  });

  self.TotalPointsEarned = ko.computed(function () {
    var total = self.ReviewerWidget.ReviewerWidget(
      'CalculateTotalPointsEarned',
      self.TestOnlineSessionAnswers(),
      self.Questions(),
      self.IsComplete(),
      self.ScoreRaw(),
      self.VirtualTest()
    );
    return total;
  });

  self.IsStart = ko.computed(function () {
    if (!Reviewer.IsNullOrEmpty(self.IsNotStart())) return !self.IsNotStart();
    return false;
  });

  self.HasViewReference = ko.computed(function () {
    return self.RefObjects() && self.RefObjects().length > 0;
  });

  self.HasAnswerAttachments = ko.computed(function () {
    return self.AnswerAttachments() && self.AnswerAttachments().length > 0;
  });

  self.SaveFeedbackOverallClick = function () {
    if (Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
      var msgStudent = 'Please select a student';
      Reviewer.popupAlertMessage(msgStudent, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
      return false;
    }

    self.LockSaveFeedbackOverallBtn(true);
    self.SaveFeedbackOverall();
  };

  self.SaveFeedbackQuestionClick = function () {
    if (Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
      var msgStudent = 'Please select a student';
      Reviewer.popupAlertMessage(msgStudent, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
      return false;
    }

    self.LockSaveFeedbackQuestionBtn(true);
    self.LockRecordFeedbackBtn(true);
    self.SaveFeedbackQuestion();
  };

  self.RecordFeedbackClick = function () {
    self.HasChangedTeacherAttachment(true);
    if (!self.IsRecordingFeedback()) {
      // start record
      $('body').toggleClass('recordingAction');
      var maxFileSizeAllowUpload = +self.TeacherFeedbackAttachmentSetting().MaxFileSizeInBytes;
      self.RecordAudioInstance.startRecord({
        fileSizeLimit: maxFileSizeAllowUpload
      },
      // callback
      function (stream) {
        self.TeacherReviewFeedbackFile(null);
        self.IsRecordingFeedback(true);
        self.LockSaveFeedbackQuestionBtn(true);
        self.AudioPlayerOptions({
          url: '',
          options: {
            removeable: false,
            autoplay: true,
            muted: true,
            srcObject: stream,
            isRecord: true
          }
        });
      },
      // erorr
      function (err) {
        $('body').toggleClass('recordingAction');
        var message = err.message;
        if (err.type == 'fileSizeLimit') {
          self.handleStopRecord();
          message = 'File size exceeds limit of ' + RecordRTC.bytesToSize(maxFileSizeAllowUpload) + '.'
        }
        Reviewer.popupAlertMessage(message, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 300, 100);
      });
    } else {
      $('body').toggleClass('recordingAction');
      self.handleStopRecord();
    }
  }

  self.handleStopRecord = function () {
    self.IsRecordingFeedback(false);
    var questionOrder = self.SelectedQuestion().QuestionOrder();
    var userFullName = self.FirstName() + ' ' + self.LastName();
    var fileName = userFullName + ' Q#' + questionOrder + ' Feedback.wav';
    self.RecordAudioInstance.stopRecord(function (file) {

      self.TeacherReviewFeedbackFile(file);
      self.LockSaveFeedbackQuestionBtn(false);

      var playerOpts = {
        url: URL.createObjectURL(file),
        options: {
          onRemoveClick: self.HandleRemoveReviewFeedback
        }
      }
      self.AudioPlayerOptions(playerOpts);
      self.LockRecordFeedbackBtn(true);
      playerOpts.options.removeable = true;
    }, fileName);
  };

  self.HandleRemoveReviewFeedback = function () {
    self.TeacherReviewFeedbackFile(null);
    self.LockRecordFeedbackBtn(false);
    self.HasChangedTeacherAttachment(true);
    self.AudioPlayerOptions({ url: '', options: { removeable: false } });
  }
 
  self.SaveFeedbackOverall = function () {
    // Get the selected student
    var student = self.SelectedStudent;

    if (!student) {
      return;
    }

    var studentTestFeedbackId = student.TestFeedbackId();
    var studentOnlineTestSessionId = student.QTIOnlineTestSessionID();
    var studentFeebackOverall;

    if (Reviewer.IsNullOrEmpty(self.FeedbackOverall())) {
      studentFeebackOverall = '';
    } else {
      studentFeebackOverall = self.FeedbackOverall().trim();
    }

    self.ReviewerWidget.ReviewerWidget(
      'SaveFeedbackOverall',
      self,
      student,
      studentTestFeedbackId,
      studentOnlineTestSessionId,
      studentFeebackOverall
    );
  };

  self.SaveFeedbackQuestion = function () {
    //get then answer of the current selected question
    var testOnlineAnswer;

    if (!self.TestOnlineSessionAnswers()) {
      return;
    }

    ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
      if (self.SelectedQuestion().VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
        testOnlineAnswer = testOnlineSessionAnswer;
      }
    });

    if (!Reviewer.IsNullOrEmpty(testOnlineAnswer)) {
      var testOnlineAnswerItemFeedbackID = testOnlineAnswer.ItemFeedbackID();
      var testOnlineAnswerOnlineTestSessionAnswerID = testOnlineAnswer.QTIOnlineTestSessionAnswerID();
      var testOnlineAnswerID = testOnlineAnswer.AnswerID();
      var teacherReviewFeedbackFile = self.TeacherReviewFeedbackFile();
      var testOnlineAnswerFeedbackQuestion;

      if (Reviewer.IsNullOrEmpty(self.FeedbackQuestion())) {
        testOnlineAnswerFeedbackQuestion = '';
      } else {
        testOnlineAnswerFeedbackQuestion = self.FeedbackQuestion().trim();
      }

      var fileDelete;
      var teacherFeedbackAttachment = self.TeacherFeebackAttachment()

      if(teacherFeedbackAttachment && teacherFeedbackAttachment.IsDeleted)
      {
        fileDelete = {
          DocumentGuid: teacherFeedbackAttachment.DocumentGuid
        }
      }

      self.ReviewerWidget.ReviewerWidget(
        'SaveFeedbackQuestion',
        self,
        testOnlineAnswer,
        testOnlineAnswerItemFeedbackID,
        testOnlineAnswerOnlineTestSessionAnswerID,
        testOnlineAnswerID,
        testOnlineAnswerFeedbackQuestion,
        testOnlineAnswer.VirtualQuestionID(),
        self.QTIOnlineTestSessionID(),
        teacherReviewFeedbackFile,
        fileDelete
      );
    }
  };

  self.PrintTestOfStudentVisible = ko.computed(function () {
    if (self.SelectedStudentID() == -1) {
      return false;
    }

    if (self.SelectedStudentFilter() == 'NotStarted' ||
      self.SelectedStudentFilter() == 'Paused' ||
      self.SelectedStudentFilter() == 'Inprogress') {
      return false;
    }

    if (self.SelectedStudentFilter() == 'Completed' && self.CompletedStudentFilter().FilterCount() == 0) {
      return false;
    }

    if (self.SelectedStudentFilter() == 'ReadyToSubmit' && self.ReadyToSubmitStudentFilter().FilterCount() == 0) {
      return false;
    }

    if (self.SelectedStudentFilter() == 'PendingReview' && self.PendingReviewStudentFilter().FilterCount() == 0) {
      return false;
    }

    if (self.CompletedStudentFilter().FilterCount() == 0 &&
      self.ReadyToSubmitStudentFilter().FilterCount() == 0 &&
      self.PendingReviewStudentFilter().FilterCount() == 0) {
      return false;
    }

    if (Reviewer.IsNullOrEmpty(self.SelectedStudent) && Reviewer.IsNullOrEmpty(self.QTIOnlineTestSessionID())) {
      return false;
    }

    var result = self.Mode() != 2;

    if (!Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
      if (Reviewer.IsNullOrEmpty(self.QTIOnlineTestSessionID()))
        return false;

      var expiredStatus = self.Expired();
      var selectedStudentStatus = self.SelectedStudent.Status();
      var isCreatedOrPausedStatus = selectedStudentStatus === 'Created' || selectedStudentStatus === 'Paused';

      if (expiredStatus && (selectedStudentStatus === 'Created' || selectedStudentStatus === 'Paused')) {
        result = true;
      } else if (selectedStudentStatus === 'NotStarted' || selectedStudentStatus === 'Created' || selectedStudentStatus === 'Paused') {
        result = false;
      }
    }

    return result;
  });

  self.PrintTestOfStudent = function () {
    var manuallyGradedOnlyQuestionIds = '';
    var studentName = '';
    var qtiOnlineTestSessionIds = '';
    var printQuestionIds = '';
    var incorrectQuestionIds = '';

    if (!self.VirtualTest().VirtualTestID()) {
      return;
    }
    if ($('#questionEnterNumber').is(':checked')) {
      if ($('#questionEnterNumberText').val() == '') {
        Reviewer.popupAlertMessage('Please enter question number.', 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
        return;
      } else {
        var questionEnterNumberText = $('#questionEnterNumberText').val();
        var arrayQuestion = questionEnterNumberText.split(',');
        if (arrayQuestion.length > 0) {
          for (var i = 0; i < arrayQuestion.length; i++) {
            if (arrayQuestion[i].indexOf('-') > 0) {
              var getVirtualQuestionIds = function (minNumber, maxNumber, printQuestionIdList) {
                ko.utils.arrayForEach(self.Questions(), function (item) {
                  var questionOrder = parseInt(item.QuestionOrderDisplay());
                  if (questionOrder >= minNumber && questionOrder <= maxNumber) {
                    var qIdFormat = item.VirtualQuestionID() + ',';
                    if (printQuestionIdList.indexOf(qIdFormat) < 0)
                      printQuestionIdList += qIdFormat;
                  }
                  return printQuestionIdList;
                });
                return printQuestionIdList;
              }

              var arraySubQuestion = arrayQuestion[i].split('-');
              var minQuestionNumber = arraySubQuestion[0];
              var maxQuestionNumber = arraySubQuestion[arraySubQuestion.length - 1];
              var totalQuestion = 0;
              if (self.VirtualTestSubtypeID() == '5' && self.BranchingTest())
                totalQuestion = self.TotalVisibleQuestions();
              else {
                totalQuestion = self.TotalQuestion();
              }
              if (minQuestionNumber > totalQuestion || maxQuestionNumber > totalQuestion) {
                Reviewer.popupAlertMessage('One or more of the question numbers you have entered are not found on the test.', 'ui-popup-fullpage ui-popup-fullpage-nostudent', 600, 100);
                return;
              }
              var rs = getVirtualQuestionIds(minQuestionNumber, maxQuestionNumber, printQuestionIds);
              if (!Reviewer.IsNullOrEmpty(rs))
                printQuestionIds = rs;
            } else {
              var virtualQuestionId = function (index) {
                var question = ko.utils.arrayFirst(self.Questions(), function (item) {
                  return item.QuestionOrderDisplay() == arrayQuestion[index];
                });
                if (!Reviewer.IsNullOrEmpty(question))
                  return question.VirtualQuestionID();
                return '';
              }
              var result = virtualQuestionId(i);
              if (!Reviewer.IsNullOrEmpty(result)) {
                var idFormat = result + ',';
                if (printQuestionIds.indexOf(idFormat) < 0)
                  printQuestionIds += idFormat;
              } else {
                Reviewer.popupAlertMessage('One or more of the question numbers you have entered are not found on the test.', 'ui-popup-fullpage ui-popup-fullpage-nostudent', 600, 100);
                return;
              }
            }
          }
        }

        if (printQuestionIds == '') {
          Reviewer.popupAlertMessage('One or more of the question numbers you have entered are not found on the test.', 'ui-popup-fullpage ui-popup-fullpage-nostudent', 600, 100);
          return;
        }
      }
    }

    if ($('#studentCurrent').is(':checked')) {
      if (!Reviewer.IsNullOrEmpty(self.SelectedStudent)) {
        studentName = self.SelectedStudent.StudentName();
      } else {
        Reviewer.popupAlertMessage('Please select student.', 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
        return;
      }
      qtiOnlineTestSessionIds = self.QTIOnlineTestSessionID();
    } else if ($('#studentAll').is(':checked')) {
      var studentCanPrint = 0;
      ko.utils.arrayForEach(self.Students(), function (student) {
        if (student.QTIOnlineTestSessionID() > 0) {
          if (student.IsPendingReview() || student.IsComplete() || student.CanBulkGrading()) {
            qtiOnlineTestSessionIds += student.QTIOnlineTestSessionID() + ',';
            studentCanPrint++;
          }
        }
      });

      if (studentCanPrint == 1) {
        studentName = self.SelectedStudent.StudentName();
      }
    }

    $('input[name="VirtualTestID"]').val(self.VirtualTest().VirtualTestID());
    $('input[name="QTIOnlineTestSessionIDs"]').val(qtiOnlineTestSessionIds);
    $('input[name="TestName"]').val(self.TestName());
    $('input[name="StudentName"]').val(studentName);
    $('input[name="PrintQuestionIDs"]').val(printQuestionIds);

    if ($('#ManuallyGradedOnly').is(':checked')) {
      $('input[name="ManuallyGradedOnly"]').prop('checked', true);
      ko.utils.arrayForEach(self.Questions(), function (item) {
        if (item.VisibleQuestion()) {
          // Get manually Graded Only questions
          manuallyGradedOnlyQuestionIds += item.VirtualQuestionID() + ',';
        }
      });
    } else {
      $('input[name="ManuallyGradedOnly"]').prop('checked', false);
    }

    if ($('#GuidanceAndRationale').is(':checked')) {
      self.PrintGuidance(true);
    } else {
      self.PrintGuidance(false);
    }

    $('input[name="TotalPointsEarned"]').val(self.TotalPointsEarned());
    $('input[name="TotalPointsPossible"]').val(self.TotalPointsPossible());
    $('input[name="ManuallyGradedOnlyQuestionIds"]').val(manuallyGradedOnlyQuestionIds);
    $('input[name="PrintGuidance"]').val(self.PrintGuidance());
    $('input[name="QTITestClassAssignmentID"]').val(self.QTITestClassAssignmentID());
    $('input[name="IncorrectQuestionIds"]').val(incorrectQuestionIds);

    self.ReviewerWidget.ReviewerWidget('PrintTestOfStudent', self);
  };

  self.CheckFeedbackDirty = function () {
    // Check to alert if user do not save feedback before printing
    var student = self.SelectedStudent;
    var msg = '';
    var msgDirty = '';
    var feedBackCharacter = self.CheckFeedbackDirtyMessage();

    if (Reviewer.IsNullOrEmpty(student)) {
      return true; // No need to check if no student is selected
    }

    if (feedBackCharacter === 'G') {
      msgDirty = 'apply grade.';
    } else if (feedBackCharacter === 'L') {
      msgDirty = 'leaving this item.';
    } else if (feedBackCharacter === 'C') {
      msgDirty = 'show popup print options.';
    } else if (feedBackCharacter === 'S') {
      msgDirty = 'select other student.';
    }

    if (Reviewer.IsNullOrEmpty(self.OldFeedbackOverall())) {
      self.OldFeedbackOverall('');
    }

    if (Reviewer.IsNullOrEmpty(self.FeedbackOverall())) {
      self.FeedbackOverall('');
    }

    if (Reviewer.IsNullOrEmpty(self.OldFeedbackQuestion())) {
      self.OldFeedbackQuestion('');
    }

    if (Reviewer.IsNullOrEmpty(self.FeedbackQuestion())) {
      self.FeedbackQuestion('');
    }

    // Check Status Feedback Overall
    if (self.OldFeedbackOverall().trim() !== self.FeedbackOverall().trim()) {
      msg = 'Please save Overall Test Feedback before ';
      msg += msgDirty;
      Reviewer.popupAlertMessage(msg, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 500, 200);
      return false;
    }

    // Check Status Feedback for Question
    if (self.OldFeedbackQuestion().trim() !== self.FeedbackQuestion().trim() || self.HasChangedTeacherAttachment()) {
      msg = 'Please save Feedback For Question before ';
      msg += msgDirty;
      Reviewer.popupAlertMessage(msg, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 500, 300);
      return false;
    }

    self.CheckFeedbackDirtyMessage('');

    return true;
  };

  self.SubmitTestButtonCss = ko.computed({
    read: function () {
      if (Reviewer.IsNullOrEmpty(self.QTIOnlineTestSessionID()) || self.QTIOnlineTestSessionID() <= 0) return false;
      if (!Reviewer.IsNullOrEmpty(self.SelectedQuestion()) && self.SelectedQuestion().IsBaseVirtualQuestion()) return false;
      if (!self.IsPendingReview() && (!self.Expired() || (!self.Paused() && !self.InProgress()))) return false;

      //All answers of student must be reviewed
      if (Reviewer.IsNullOrEmpty(self.TestOnlineSessionAnswers())) return false;

      var unreviewedAnswer = ko.utils.arrayFirst(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
        if (self.VirtualTestSubtypeID() == '5' && self.BranchingTest() && !testOnlineSessionAnswer.Answered()) {
          return false;
        }

        if (!testOnlineSessionAnswer.IsReviewed() && !testOnlineSessionAnswer.AnswerOfBaseQuestion()) {
          return true;
        }

        if (Reviewer.IsNullOrEmpty(testOnlineSessionAnswer.TestOnlineSessionAnswerSubs())) {
          return false;
        }

        var unreviewedAnswerSub = ko.utils.arrayFirst(testOnlineSessionAnswer.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
          return !testOnlineSessionAnswerSub.IsReviewed();
        });

        return !Reviewer.IsNullOrEmpty(unreviewedAnswerSub);
      });

      return Reviewer.IsNullOrEmpty(unreviewedAnswer);
    },
    write: function (value) {
    }, owner: self
  });

  self.QTIOnlineTestSessionIDBulks = ko.computed(function () {
    var result = [];
    ko.utils.arrayForEach(self.Students(), function (student) {
      if (student.CanBulkGrading()) {
        result.push(student);
      }
    });
    return result;
  });

  self.PostSubmitTestData = function () {
    self.ReviewerWidget.ReviewerWidget('PostSubmitTestData', self);
  };

  self.SubmitTestForceUIRefreshSignature = ko.observable("SubmitTestForceUIRefresh");
  self.SubmitTestForceUIRefresh = function () {
    var $signature = $('[signature="' + self.SubmitTestForceUIRefreshSignature() + '"]');

    if (self.SubmitTestButtonCss()) {
      $signature.prop('disabled', false);
    } else {
      $signature.prop('disabled', true);
    }
  };

  self.SubmitTest = function () {
    self.SubmitTestForceUIRefresh();
    if (!self.SubmitTestButtonCss()) return;

    self.IsBulkGrading(false);
    self.LockSaveFeedbackOverallBtn(true);
    self.LockSaveFeedbackQuestionBtn(true);
    self.LockRecordFeedbackBtn(true);

    if (self.QTIOnlineTestSessionIDBulks().length > 1) {
      var count = self.QTIOnlineTestSessionIDBulks().length - 1;
      var msg = '';
      if (count == 1) {
        msg = 'is ' + count + ' student';
      } else {
        msg = 'are ' + count + ' student(s)';
      }
      self.ConfirmBulkSubmitTest('There ' + msg + ' with changed grades that have not been submitted.<br/> Do you want to submit their tests as well?');
      return;
    }

    self.PostSubmitTestData();
    self.SelectFilterStudentChangeTrigger();
    self.SelectedStudentFilterFunction();
    self.SelectedStudentID(null);
    var selectedStudentID = self.DefaultDisplayStudentFilter();
    self.SelectedStudentID(selectedStudentID);
    self.SelectStudentChangeTrigger();
  };

  self.ConfirmBulkSubmitTest = function (msg) {
    self.ReviewerWidget.ReviewerWidget('ConfirmBulkSubmitTest', self, msg);
  };

  self.DownloadRubricFile = function () {
    var key = self.Rubric().VirtualTestFileKey();
    var url = self.ReviewerWidget.ReviewerWidget('DownloadRubricFile', key);
    window.open(url, '_blank');
  };

  self.PointsEarned_Keyup = function (viewModel, event) {
    var pointsEarned = $('.PointsEarned').val();
    if (pointsEarned.length > 0) {
      pointsEarned = parseInt(pointsEarned);
      if (pointsEarned > viewModel.PointsPossible()) {
        pointsEarned = viewModel.PointsPossible();
      }
      viewModel.PointsEarned(pointsEarned);
    }

    return true;
  };

  self.QuestionDetailsVisible = ko.computed(function () {
    var question = ko.utils.arrayFirst(self.Questions(), function (item) {
      return item.VisibleQuestion();
    });

    return question != null;
  });

  // Popup Print Test Of Student
  self.IsOpenPrintTestOfStudentPopup = ko.observable(false);

  self.OpenPrintTestOfStudentPopup = function () {
    // Check Feedback Dirty Before Call Popup Print Options
    self.CheckFeedbackDirtyMessage('C');
    if (!self.CheckFeedbackDirty()) {
      return;
    }

    self.IsOpenPrintTestOfStudentPopup(true);
  };

  self.ClosePrintTestOfStudentPopup = function () {
    self.IsOpenPrintTestOfStudentPopup(false);
  };

  // Popup View Batch Printing
  self.OpenViewBatchPrintingPopup = function () {
    self.ReviewerWidget.ReviewerWidget(
      'ViewBatchPrinting',
      self.QTITestClassAssignmentID()
    );
  };

  self.CanGradingShortcuts = function (student, expired) {
    var visible = student.StudentVisible() && student.StudentID() != -1;
    var canGrading = (student.IsPendingReview || student.CanBulkGrading || ((student.InProgress || student.Paused) && expired))
      || (self.IsComplete && self.OverrideAutoGraded());
    var result = visible && canGrading;

    return result;
  };
  self.CanEnableGradingShotcut = ko.computed(function () {
    return self.ReviewerWidget.ReviewerWidget(
      'CanEnableGradingShotcut',
      self.QTIOnlineTestSessionID(),
      self.Mode(),
      self.SelectedQuestion(),
      self.AnswerID(),
      self.PointsEarned(),
      self.IsPendingReview(),
      self.Paused(),
      self.InProgress(),
      self.Expired(),
      self.QTIItemSchemaID(),
      self.OverrideAutoGraded(),
      self.ResponseProcessingTypeID(),
      self.IsComplete(),
      self.OverrideItems(),
      self.GradingProcessStatus()
    );
  });

  self.GetStudentGradingShortcuts = function () {
    var result = [];
    ko.utils.arrayForEach(self.Students(), function (student) {
      var canGradingShortcuts = self.CanGradingShortcuts(student, self.Expired());
      if (canGradingShortcuts) {
        var studentGradingShortcuts = {};
        studentGradingShortcuts.QTIOnlineTestSessionID = student.QTIOnlineTestSessionID();
        studentGradingShortcuts.StatusID = student.QTOStatusID();

        result.push(studentGradingShortcuts);
      }
    });

    return result;
  };

  self.ShortcutAnswered = ko.observable(false);
  self.ShortcutUnAnswered = ko.observable(false);
  self.ShortcutAssignType = ko.observable(null);
  self.ShortcutGradeBy = ko.observable(null);
  self.IsLoadingGradingShortcuts = ko.observable(false);

  self.GradingShortcutsPopupCss = ko.computed(function () {
    if (self.IsComplete() && self.OverrideAutoGradedOptionValue() != '1') {
      return false;
    }

    if (self.SelectedQuestion() && self.SelectedQuestion().IsRubricBasedQuestion) {
      return false;
    }

    return self.CanEnableGradingShotcut();
  });

  // Popup Grading Shortcuts
  self.IsOpenGradingShortcutsPopup = ko.observable(false);

  self.ShortcutReset = function () {
    self.ShortcutAnswered(false);
    self.ShortcutUnAnswered(false);
    self.ShortcutAssignType(null);
    self.ShortcutGradeBy(null);
  }

  self.OpenGradingShortcutsPopup = function () {
    self.ShortcutReset();
    self.IsOpenGradingShortcutsPopup(true);
  };

  // Popup Coding Guide
  self.IsOpenCodingGuidePopup = ko.observable(false);

  self.IsVisibleCodingGuide = ko.observable(false);

  self.CreatePdfEmbed = function (rubricPublicUrl) {
    var embed = document.createElement('embed');
    embed.src = rubricPublicUrl;
    embed.setAttribute('id', 'rubricPdfBox');
    embed.setAttribute('frameborder', '0');
    embed.style.width = '100%';
    embed.style.height = '600px';
    return embed;
  };

  self.OpenCodingGuidePopup = function () {
    // detech IE
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");

    if (!self.IsPdfFile(self.Rubric().VirtualTestFileName()) || msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./) || /Edge\/\d./i.test(navigator.userAgent)) {
      var url = self.GetRubricLink();
      window.open(url, '_blank');
      return;
    }

    self.IsOpenCodingGuidePopup(true);

    var $body = $('body');

    $body.find('.ui-overlay').remove();

    var $embedParent = $body.find('embed#rubricPdfBox').parent();
    var newEmbed = self.CreatePdfEmbed(self.GetRubricLink());

    $body.find('embed#rubricPdfBox').remove();
    $embedParent.append(newEmbed);

    interact('#pdfGuide').resizable({
      edges: { left: true, right: true, bottom: true, top: true },
      restrictSize: {
        min: { width: 400, height: 400 }
      },
      inertia: true
    }).on('resizemove', function (event) {
      var target = event.target;
      var width = event.rect.width;
      var height = event.rect.height;
      var padding = 25;
      var rubricPdfBox = document.getElementById('rubricPdfBox');

      target.style.width = width + 'px';
      target.style.height = height - padding + 'px';

      target.parentNode.style.width = width + padding + 'px';
      target.parentNode.style.height = height + 'px';

      rubricPdfBox.style.height = height - padding + 'px';
    });
  };

  self.GetRubricLink = function () {
    var key = self.Rubric().VirtualTestFileKey();
    var url = self.ReviewerWidget.ReviewerWidget('DownloadRubricFile', key);
    return url;
  };

  self.IsPdfFile = function (fileName) {
    var fileType = fileName.substr(fileName.lastIndexOf('.')).toLowerCase();
    if (fileType === '.pdf') {
      return true;
    }
    return false;
  };

  self.CloseGradingShortcutsPopup = function () {
    self.IsOpenGradingShortcutsPopup(false);
  };

  self.IsFullScreen = ko.observable(false);

  self.ToggleFullScreen = function () {
    var elBody = document.querySelector('body');
    var elFullscreen = document.querySelector('.js-fullscreen');
    var elQuestionStudent = document.querySelector('.assignment-question-student');
    var elDetailAndAnswers = document.querySelector('.js-assignment-detail-and-answers');
    var elDescQuestion = document.querySelector('.js-assignment-desc-question');
    var elFullscreenIcon = elFullscreen.querySelector('.icon');
    var elListQuestion = document.querySelectorAll('.assignment-list-question');
    var $menuItemCurrent = $('.menu-item.current');

    if (elBody.classList.contains('teacherReviewerFullscreen')) {
      elBody.classList.remove('teacherReviewerFullscreen');
      elFullscreen.setAttribute('data-tipsy', 'Fullscreen');
      elFullscreenIcon.classList.remove('icon-exit-fullscreen');
      elFullscreenIcon.classList.add('icon-fullscreen');
      $menuItemCurrent.showTip();
      self.IsFullScreen(false);

      for (var i = 0; i < elListQuestion.length; i++) {
        elListQuestion[i].style.width = '200px';
      }

      elDescQuestion.style.width = '535px';
    } else {
      elBody.classList.add('teacherReviewerFullscreen');
      elFullscreen.setAttribute('data-tipsy', 'Exit Fullscreen');
      elFullscreenIcon.classList.remove('icon-fullscreen');
      elFullscreenIcon.classList.add('icon-exit-fullscreen');
      $menuItemCurrent.hideTip();
      self.IsFullScreen(true);

      for (var i = 0; i < elListQuestion.length; i++) {
        elListQuestion[i].style.width = elQuestionStudent.clientWidth - 150 + 'px';;
      }

      elDescQuestion.style.width = elDetailAndAnswers.clientWidth - 14 + 'px';
    }

    elFullscreen.blur();
  };

  self.AssignPointsGradingShortcuts = function () {
    self.GradingShortcutsWidget.GradingShortcutsWidget(
      'GradingShortcuts',
      self,
      function (data) {
        // Reload page
        document.location.reload(true);
      }
    );
  };

  self.AssignPointsGradingShortcutsVisible = ko.computed(function () {
    if (self.ShortcutAnswered() == false && self.ShortcutUnAnswered() == false) {
      return false;
    }

    if (self.ShortcutAssignType() == null) {
      return false;
    }

    if (self.ShortcutGradeBy() == null) {
      return false;
    }

    return true;
  });

  self.OverriddenVisible = ko.computed(function () {
    return self.Overridden() &&
      !Reviewer.IsNullOrEmpty(self.UpdatedBy()) &&
      !Reviewer.IsNullOrEmpty(self.UpdatedDate());
  });

  self.BranchingTestSort = function () {
    self.Questions(self.Questions().sort(function (left, right) {
      return left.QuestionOrder() == right.QuestionOrder() ? 0 : (left.QuestionOrder() > right.QuestionOrder() ? 1 : -1);
    }));
  };

  // Popup Confirm Auto Bulk Submit Test
  self.IsConfirmAutoBulkSubmitTestPopup = ko.observable(false);

  self.YesConfirmAutoBulkSubmitTest = function () {
    self.SubmitTestForceUIRefresh();
    if (!self.SubmitTestButtonCss()) return;

    self.LockSaveFeedbackOverallBtn(true);
    self.LockSaveFeedbackQuestionBtn(true);
    self.LockRecordFeedbackBtn(true);

    self.IsBulkGrading(true);
    self.PostSubmitTestData();

    self.SelectFilterStudentChangeTrigger();
    self.SelectedStudentFilterFunction();
    self.SelectedStudentID(null);
    var selectedStudentID = self.DefaultDisplayStudentFilter();
    self.SelectedStudentID(selectedStudentID);
    self.SelectStudentChangeTrigger();
  };

  self.NoConfirmAutoBulkSubmitTest = function () {
    if (self.SelectedStudentFilter() == self.PendingReviewStudentFilter().FilterValue()) {
      var selectedStudentID = self.SelectedStudentID();
      self.SelectedStudentFilterNew(self.ReadyToSubmitStudentFilter().FilterValue());
      self.SelectedStudentID(null);
      self.SelectedStudentID(selectedStudentID);
    }

    self.IsConfirmAutoBulkSubmitTestPopup(false);
  }

  self.PostAnswerLogs = ko.observableArray([]);
  self.SelectedAnswerTemp = ko.observable();
  self.CurrentResponseIdPostAnswerLogs = ko.observable('');
  self.activePostAnswerLogs = ko.observable(false);
  self.defautPostAnswerLog = ko.observable('Answer History');

  self.showPostAsswerLogs = ko.computed(function () {
    if (self.PostAnswerLogs() == null || self.PostAnswerLogs().length == 0) return false;
    if (self.QTIItemSchemaID() != 10) return false;

    if (viewModel.SelectedStudent != null && (viewModel.SelectedStudent.IsComplete() == true || viewModel.SelectedStudent.IsPendingReview() == true || viewModel.SelectedStudent.Paused() == true)) {
      return true;
    } else {
      return false;
    }

    return true;
  });

  self.SetPostAsswerLogsOptionAttrs = function (option, item) {
    ko.applyBindingsToNode(option, { attr: { 'data-response-id': item.ResponseIdentifier() } }, item);
  };

  self.ResetPostAsswerLogs = function () {
    //trigger select answer test
    self.SelectedAnswerTemp(self.defautPostAnswerLog());
  };

  self.SavePostAsswerLogs = function () {
    if (self.SelectedQuestion().QTIItemSchemaID() == 10) {
      self.SelectedQuestion().Answer().AnswerText(self.SelectedAnswerTemp());
    } else if (self.SelectedQuestion().QTIItemSchemaID() == 21) {
      var answerSubs = self.SelectedQuestion().Answer().TestOnlineSessionAnswerSubs()
      for (var i = 0; i < self.SelectedQuestion().Answer().TestOnlineSessionAnswerSubs().length; i++) {
        if (answerSubs[i].QTIOnlineTestSessionAnswerSubID() == self.AnswerSubID()) {
          answerSubs[i].AnswerText(self.SelectedAnswerTemp());
        }
      }
    }

    self.ReviewerWidget.ReviewerWidget('UpdateAutoSaveToAnswerText', self);
  };

  self.ApplyPostAsswerLogs = function (value) {
    var self = this;
    var ResponseId = self.CurrentResponseIdPostAnswerLogs();

    if (self.SelectedQuestion().QTIItemSchemaID() == 10) {
      ResponseId = $("div[data-response-id]").attr("data-response-id");
    }

    $("div[data-response-id=\"" + ResponseId + "\"]").html(Reviewer.replaceStringLessOrLarge(value));
  };

  self.ResizeCodingGuide = function () {
    console.log(this.clientHeight);
  };

  self.SelectedAnswerTemp.subscribe(function (newValue) {
    if (newValue === self.defautPostAnswerLog()) {
      // Reset data and dropdown
      if (self.SelectedQuestion().QTIItemSchemaID() == 10) {
        self.ApplyPostAsswerLogs(self.SelectedQuestion().Answer().AnswerText());
      } else if (self.SelectedQuestion().QTIItemSchemaID() == 21) {
        var answerSubs = self.SelectedQuestion().Answer().TestOnlineSessionAnswerSubs()
        for (var i = 0; i < self.SelectedQuestion().Answer().TestOnlineSessionAnswerSubs().length; i++) {
          if (answerSubs[i].QTIOnlineTestSessionAnswerSubID() == self.AnswerSubID()) {
            self.ApplyPostAsswerLogs(answerSubs[i].AnswerText());
          }
        }
      }

      self.activePostAnswerLogs(false);
    } else {
      self.ApplyPostAsswerLogs(newValue);
      self.activePostAnswerLogs(true);
    }
  });

  self.IsScrollToFirstManuallyGradedItem = ko.observable(true);

  self.IsAnonymizedScoring = ko.observable(false);
}

function AlgorithmicCorrectAnswers(data) {
  var self = this;
  self.ResponseIdentifier = ko.observable(data.ResponseIdentifier);
  self.ConditionType = ko.observable(data.ConditionType);
  self.ConditionValue = ko.observable(data.ConditionValue);
  self.OriginalExpression = ko.observable(data.OriginalExpression);
  self.PointsEarned = ko.observable(data.PointsEarned);
  self.Amount = ko.observable(data.Amount);
}

var insertAudioDom = function (element, value) {
  var player = new AudioPlayer(value.url, value.options);
  $(element).html(player);
  $(element).find('audio');
  if (!value.url) {
    $(element).find('button').css('display', 'none');
    $('#btnRecordFeedback').css('display', 'inline-block');
  } else {
    $(element).find('button').css('display', 'inline-block');
    $('#btnRecordFeedback').css('display', 'none');
  }
};

$(document).ready(function () {
  insertAudioDom = _.debounce(insertAudioDom);
});

ko.bindingHandlers.audioPlayer = {
  init: function (element, valueAccessor, allBindings) {
    ko.bindingHandlers.value.init(element, valueAccessor, allBindings);
  },
  update: function (element, valueAccessor) {
    var value = valueAccessor();
    insertAudioDom(element, value);
    
    ko.bindingHandlers.value.update(element, valueAccessor);
  }
};


function getFullNameOnly(text) {
  var regex = /\((.*)\)/;
  return regex.exec(text)[1];
}
