function Section(data) {
    var self = this;

    self.SectionTitle = data.SectionTitle;
    self.Questions = [];
    if (data.Questions != null) {
        var mappedObjects = $.map(data.Questions, function (item) {
            return new Question(item);
        });
        self.Questions = mappedObjects;
    }
}

function Question(data) {
    var self = this;
    self.QTIItemID = data.QTIItemID;
    self.QTISchemaID = data.QTISchemaID;
    self.VirtualQuestionID = data.VirtualQuestionID;
    self.QuestionOrder = data.QuestionOrder;
    self.PointsPossible = data.PointsPossible;
    self.SectionInstruction = data.SectionInstruction;
    self.XmlContent = data.XmlContent;
    self.DataXmlContent = data.XmlContent;
    self.Answered = data.Answered;
    self.Identifier = '';
    self.HtmlContent = '';
    self.QTIItemAnswerScores = [];
    self.VirtualQuestionAnswerScoresDTO = [];
    if (data.QTIItemAnswerScoresDTO != null) {
        var mappedObjects = $.map(data.QTIItemAnswerScoresDTO, function (item) {
            return new QTIItemAnswerScore(item);
        });
        self.QTIItemAnswerScores = mappedObjects;
    }
    self.Answer = null;
    self.PassageTexts = data.PassageTexts;
    self.StartNewPassage = data.StartNewPassage;
    self.ResponseRubric = data.ResponseRubric;
    self.GuidanceHTML = '';
}

function QTIItemAnswerScore(data) {
    var self = this;
    self.QTIItemAnswerScoreID = data.QTIItemAnswerScoreID;
    self.QTIItemID = data.QTIItemID;
    self.ResponseIdentifier = data.ResponseIdentifier;
    self.Answer = data.Answer;
    self.Score = data.Score;
    self.VirtualQuestionAnswerScore = data.VirtualQuestionAnswerScore;
}

function Answer(data) {
    var self = this;
    self.QTIOnlineTestSessionAnswerID = data.QTIOnlineTestSessionAnswerID;
    self.QTIOnlineTestSessionID = data.QTIOnlineTestSessionID;
    self.VirtualQuestionID = data.VirtualQuestionID;
    self.AnswerChoice = data.AnswerChoice;
    self.Answered = data.Answered;
    self.AnswerImage = data.AnswerImage;
    self.AnswerText = data.AnswerText;
    self.Feedback = data.Feedback;

    self.HighlightQuestion = data.HighlightQuestion;
    self.HighlightPassage = data.HighlightPassage;

    self.PointsEarned = data.PointsEarned == null ? 0 : data.PointsEarned;
    self.IsReviewed = data.IsReviewed;
    self.ResponseProcessingTypeID = data.ResponseProcessingTypeID;
    self.AnswerSubs = [];
    if (data.TestOnlineSessionAnswerSubs != null) {
        var mappedObjects = $.map(data.TestOnlineSessionAnswerSubs, function (item) {
            return new AnswerSub(item);
        });
        self.AnswerSubs = mappedObjects;
    }
}

function AnswerSub(data) {
    var self = this;
    self.QTIOnlineTestSessionAnswerSubID = data.QTIOnlineTestSessionAnswerSubID;
    self.QTIOnlineTestSessionAnswerID = data.QTIOnlineTestSessionAnswerID;
    self.VirtualQuestionSubID = data.VirtualQuestionSubID;
    self.QTISchemaID = data.QTISchemaID;
    self.AnswerChoice = data.AnswerChoice;
    self.Answered = data.Answered;
    self.AnswerText = data.AnswerText;
    self.AnswerImage = data.AnswerImage;
    self.IsReviewed = data.IsReviewed;
    self.PointsPossible = data.PointsPossible;
    self.ResponseIdentifier = data.ResponseIdentifier;
    self.ResponseProcessingTypeID = data.ResponseProcessingTypeID;
    var pointsEarned = data.PointsEarned == null ? 0 : data.PointsEarned;
    self.PointsEarned = pointsEarned;
}

function TestOfStudentViewModel(sections, answers) {
    var self = this;

    self.QuestionRender = null;
    self.MapPath = '';

    self.Sections = sections;
    self.Answers = answers;

    self.TestName = '';
    self.StudentName = '';
    self.passageArray = [];

    self.PortalImgPrint = '';

    self.TheCorrectAnswer = true;
    self.GuidanceAndRationale = true;
    self.TheQuestionContent = true;

    self.ProcessQuestions = function () {
        if (self.Sections == null) return;
        ko.utils.arrayForEach(self.Sections, function (section) {
            if (section.Questions != null) {
                ko.utils.arrayForEach(section.Questions, function (question) {
                    self.QuestionRender.TOSQuestionRender('Display', question);
                });
            }
        });
    };

    self.ParagraphContent = function(element) {
        var $element = $(element);

        $element.each(function() {
            var $el = $(this);
            var elHtml = $el.html();

            // Replace "\n" to "<br/>" tag in extended text
            if (elHtml !== '' && elHtml !== null) {
                elHtml = elHtml.replace(new RegExp('\r?\n','g'), '<span class="u-linebreak"></span>');
            }

            $el.html(elHtml);
        });
    };

    self.Innit = function () {
        var options = {
            Util: TOSUtils,
            Self: self,
            TheCorrectAnswer: self.TheCorrectAnswer,
            GuidanceAndRationale: self.GuidanceAndRationale,
            TheQuestionContent: self.TheQuestionContent
        };

        self.QuestionRender = $('body').TOSQuestionRender(options);
        self.ProcessQuestions();
        $('.jsTestName').html(self.TestName);
        $('.jsStudentName').html(self.StudentName);
        $('.sectionTitle').each(function() {
            var sectionTitle = $(this);
            if (sectionTitle.html() === '') {
                sectionTitle.css('display', 'none');
                sectionTitle.parents('.sectionData').css('display', 'none');
            }
        });

        // Remove the first empty
        var $questionsFirst = $('div .questions').first();
        if ($questionsFirst.length) {
            if ($questionsFirst.html().length <= 1) {
                // Remove the first
                $questionsFirst.remove();
            }
        }

        // Print paragraph teacher feedback
        self.ParagraphContent('.jsTeacherFeedback');
        self.ParagraphContent('.jsFeedback');
    };
}
