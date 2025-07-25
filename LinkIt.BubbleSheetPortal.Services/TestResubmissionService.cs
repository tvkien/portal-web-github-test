using System;
using System.Collections.Generic;
using System.Linq;
//using LinkIt.BubbleService.Models.Reading;
using  LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestResubmissionService
    {
        public IEnumerable<UnansweredQuestionAnswer> CombinePreviouslyAnsweredQuestionsWithNewlyAnsweredQuestions(IEnumerable<PreviouslyAnsweredQuestion> previouslyAnsweredQuestions, IEnumerable<UnansweredQuestionAnswer> unansweredQuestionAnswers)
        {
            var combinedQuestions = previouslyAnsweredQuestions.Select(previouslyAnsweredQuestion => new UnansweredQuestionAnswer
                {
                    QuestionOrder = previouslyAnsweredQuestion.QuestionOrder, SelectedAnswer = previouslyAnsweredQuestion.AnswerLetter
                }).ToList();

            var newAnsweredQuestionOrder = unansweredQuestionAnswers.Select(x => x.QuestionOrder).ToList();
            combinedQuestions =
                combinedQuestions.Where(x => newAnsweredQuestionOrder.Contains(x.QuestionOrder) == false).ToList();

            combinedQuestions.AddRange(unansweredQuestionAnswers);

            return combinedQuestions;
        }

        public void AssignNewQuestions(IEnumerable<UnansweredQuestionAnswer> answers, ReadResult bubbleSheetFileCloudEntityReadResult)
        {
            var formattedQuestions = AssignQuestions(answers);
            foreach (var unansweredQuestionAnswer in formattedQuestions)
            {
                if (unansweredQuestionAnswer.ProblemNumber > 0)
                {
                    var problemNumber = unansweredQuestionAnswer.ProblemNumber - 1;
                    var listQuestion = bubbleSheetFileCloudEntityReadResult.Questions.ToList();
                    
                    ReadResultItem currentQuestion;
                    currentQuestion =
                        bubbleSheetFileCloudEntityReadResult.Questions.FirstOrDefault(
                            x => x.ProblemNumber == unansweredQuestionAnswer.ProblemNumber);
                    //if (bubbleSheetFileCloudEntityReadResult.Questions.Count <= problemNumber)
                    
                    if (currentQuestion == null)
                    {
                        var tempQuestion = bubbleSheetFileCloudEntityReadResult.Questions.FirstOrDefault();
                        currentQuestion = new ReadResultItem
                        {
                            IsACTSheet = tempQuestion?.IsACTSheet ?? false,
                            IsSATSheet = tempQuestion?.IsSATSheet ?? false,
                            OptionCount = -1,
                            ProblemNumber = unansweredQuestionAnswer.ProblemNumber,
                            SectionIndex = tempQuestion?.SectionIndex ?? 0,
                        };
                        bubbleSheetFileCloudEntityReadResult.Questions.Add(currentQuestion);
                    }
                    else
                    {
                        var index =
                            listQuestion.FindIndex(x => x.ProblemNumber == unansweredQuestionAnswer.ProblemNumber);
                        currentQuestion = bubbleSheetFileCloudEntityReadResult.Questions.ElementAt(index);
                    }
                    currentQuestion.Answers.Clear();
                    currentQuestion.Unreadable = false;
                    if (unansweredQuestionAnswer.Answers.Count == 1)
                    {
                        currentQuestion.AddAnswer(unansweredQuestionAnswer.Answers[0].BubbleIndex, 1000);
                    }
                    else
                    {
                        currentQuestion.Answers = unansweredQuestionAnswer.Answers;
                    }
                }
            }
        }

        private IEnumerable<ReadResultItem> AssignQuestions(IEnumerable<UnansweredQuestionAnswer> answers)
        {
            return answers.Select(answer => new ReadResultItem
            {
                Answers = AssignAnswers(answer),
                ProblemNumber = answer.QuestionOrder
            }).ToList();
        }

        private List<FilledBubble> AssignAnswers(UnansweredQuestionAnswer answer)
        {
            var lstFilledBubbles = new List<FilledBubble>();
            if (!string.IsNullOrEmpty(answer.SelectedAnswer))
            {
                string[] arrAnswerLatter = answer.SelectedAnswer.Split(',');
                if (arrAnswerLatter.Length == 1)
                {
                    if (arrAnswerLatter[0] != "U")
                    {
                        lstFilledBubbles.Add(new FilledBubble
                                             {
                                                 BubbleIndex = DetermineSelectedAnswerIndex(answer.SelectedAnswer),
                                                 Confidence = 1000
                                             });
                    }
                }
                else
                {
                    for (int index = 0; index < arrAnswerLatter.Length; index++)
                    {
                        string s = arrAnswerLatter[index];
                        if (!string.IsNullOrEmpty(s))
                        {
                            lstFilledBubbles.Add(new FilledBubble
                            {
                                BubbleIndex = DetermineSelectedAnswerIndex(s),
                                Confidence = 1000
                            });
                        }
                    }
                }
            }
            return lstFilledBubbles;
        }

        private static int DetermineSelectedAnswerIndex(string selectedAnswer)
        {
            int answer;
            return Int32.TryParse(selectedAnswer, out answer) ? answer : (selectedAnswer[0] - 65);
        }

        //\ ACT Page
        public void ACTAssignNewQuestions(IEnumerable<UnansweredQuestionAnswer> answers, ReadResult bubbleSheetFileCloudEntityReadResult)
        {
            try
            {
                List<ReadResultItem> formattedQuestions = ACTAssignQuestions(answers);
                bubbleSheetFileCloudEntityReadResult.Questions.Clear();
                for (int i = 0; i < formattedQuestions.Count(); i++)
                {
                    var currentQuestion = new ReadResultItem();
                    currentQuestion.Unreadable = false;
                    currentQuestion.IsACTSheet = true;
                    currentQuestion.ProblemNumber = formattedQuestions[i].ProblemNumber;
                    currentQuestion.SectionIndex = formattedQuestions[i].SectionIndex;

                    if (formattedQuestions[i].Answers.Count == 1)
                    {
                        currentQuestion.AddAnswer(formattedQuestions[i].Answers[0].BubbleIndex, 1000);
                    }
                    else
                    {
                        currentQuestion.Answers = formattedQuestions[i].Answers;
                    }
                    bubbleSheetFileCloudEntityReadResult.Questions.Add(currentQuestion);
                }
            }
            catch (Exception exception)
            {
                //TODO: Throw exception
                throw exception;
            }
        }

        public void SATAssignNewQuestions(IEnumerable<UnansweredQuestionAnswer> answers, ReadResult bubbleSheetFileCloudEntityReadResult)
        {
            try
            {
                List<ReadResultItem> formattedQuestions = SATAssignQuestions(answers);
                bubbleSheetFileCloudEntityReadResult.Questions.Clear();
                for (int i = 0; i < formattedQuestions.Count(); i++)
                {
                    var currentQuestion = new ReadResultItem();
                    currentQuestion.Unreadable = false;
                    currentQuestion.IsACTSheet = false;
                    currentQuestion.IsSATSheet = true;
                    currentQuestion.ProblemNumber = formattedQuestions[i].ProblemNumber;
                    currentQuestion.SectionIndex = formattedQuestions[i].SectionIndex;

                    if (formattedQuestions[i].Answers.Count == 1)
                    {
                        currentQuestion.AddAnswer(formattedQuestions[i].Answers[0].BubbleIndex, 1000);
                    }
                    else
                    {
                        currentQuestion.Answers = formattedQuestions[i].Answers;
                    }
                    currentQuestion.TextEntryAnswerInText = formattedQuestions[i].TextEntryAnswerInText;
                    bubbleSheetFileCloudEntityReadResult.Questions.Add(currentQuestion);
                }
            }
            catch (Exception exception)
            {
                //TODO: Throw exception
                throw exception;
            }
        }

        private List<ReadResultItem> ACTAssignQuestions(IEnumerable<UnansweredQuestionAnswer> answers)
        {
            return answers.Select(answer => new ReadResultItem
            {
                Answers = ACTAssignAnswers(answer),
                ProblemNumber = answer.SectionQuestionIndex,
                SectionIndex = answer.SectionIndex,
                IsACTSheet = true,
                IsSATSheet = false
            }).ToList();
        }

        private List<ReadResultItem> SATAssignQuestions(IEnumerable<UnansweredQuestionAnswer> answers)
        {
            return answers.Select(answer => new ReadResultItem
            {
                Answers = ACTAssignAnswers(answer),
                ProblemNumber = answer.SectionQuestionIndex,
                SectionIndex = answer.SectionIndex,
                IsACTSheet = false,
                IsSATSheet = true,
                TextEntryAnswerInText = answer.IsTextEntryQuestion ? SATAssignAnswerForTextEntry(answer) : string.Empty
            }).ToList();
        }

        private string SATAssignAnswerForTextEntry(UnansweredQuestionAnswer answer)
        {
            if (!string.IsNullOrEmpty(answer.SelectedAnswer) && !answer.SelectedAnswer.ToUpper().Equals("U"))
            {
                string[] arrAnswerLetter = answer.SelectedAnswer.Split(',');
                if (arrAnswerLetter.Length == 1)
                {
                    return answer.SelectedAnswer;
                }
            }
            return string.Empty;
        }

        public IEnumerable<UnansweredQuestionAnswer> ACTCombinePreviouslyAnsweredQuestionsWithNewlyAnsweredQuestions(IEnumerable<ACTAlreadyAnsweredQuestion> previouslyAnsweredQuestions, IEnumerable<UnansweredQuestionAnswer> unansweredQuestionAnswers)
        {
            List<int> lstQuestionResubmit = unansweredQuestionAnswers.Select(o => o.QuestionId).ToList();
            //TODO: maybe remove question answered and reanswer then add new

            var combinedQuestions = previouslyAnsweredQuestions.Where(o=> !lstQuestionResubmit.Contains(o.QuestionId) )
                .Select(previouslyAnsweredQuestion => new UnansweredQuestionAnswer
                {
                    QuestionId = previouslyAnsweredQuestion.QuestionId,
                    QuestionOrder = previouslyAnsweredQuestion.QuestionOrder,
                    SelectedAnswer = string.Empty,
                    SectionIndex = previouslyAnsweredQuestion.OrderSectionIndex,
                    SectionQuestionIndex = previouslyAnsweredQuestion.OrderSectionQuestionIndex
                }).ToList();
            
            combinedQuestions.AddRange(unansweredQuestionAnswers);
            return combinedQuestions;
        }

        private List<FilledBubble> ACTAssignAnswers(UnansweredQuestionAnswer answer)
        {
            var lstFilledBubbles = new List<FilledBubble>();
            if (!string.IsNullOrEmpty(answer.SelectedAnswer) && !answer.SelectedAnswer.ToUpper().Equals("U"))
            {
                string[] arrAnswerLetter = answer.SelectedAnswer.Split(',');
                if (arrAnswerLetter.Length == 1)
                {
                    if (answer.SelectedAnswer.Length == 1)
                    {
                        lstFilledBubbles.Add(new FilledBubble
                        {
                            BubbleIndex = DetermineSelectedAnswerIndex(answer.SelectedAnswer),
                            Confidence = 1000
                        });
                    }
                }
                else
                {
                    for (int index = 0; index < arrAnswerLetter.Length; index++)
                    {
                        string s = arrAnswerLetter[index];
                        if (!string.IsNullOrEmpty(s))
                        {
                            lstFilledBubbles.Add(new FilledBubble
                            {
                                BubbleIndex = DetermineSelectedAnswerIndex(s),
                                Confidence = 1000
                            });
                        }
                    }
                }
            }
            return lstFilledBubbles;
        }
    }
}