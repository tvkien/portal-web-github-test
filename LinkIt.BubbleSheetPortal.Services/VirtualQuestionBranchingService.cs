using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionBranchingService
    {
        private readonly IRepository<VirtualQuestionBranching> _virtualQuestionBranchingRepository;

        public VirtualQuestionBranchingService(IRepository<VirtualQuestionBranching> virtualQuestionBranchingRepository)
        {
            _virtualQuestionBranchingRepository = virtualQuestionBranchingRepository;
        }

        public void UpdateVirtualQuestionBranching(List<VirtualQuestionBranching> virtualQuestionBranchings)
        {
            foreach (var questionBranching in virtualQuestionBranchings)
            {
                var virtualQuestionBranching =
                    _virtualQuestionBranchingRepository.Select()
                        .FirstOrDefault(
                            v =>
                                v.VirtualQuestionID == questionBranching.VirtualQuestionID &&
                                v.VirtualTestID == questionBranching.VirtualTestID &&
                                v.AnswerChoice == questionBranching.AnswerChoice);

                if (virtualQuestionBranching != null)
                {
                    if (questionBranching.TargetVirtualQuestionID == -1)
                    {
                        _virtualQuestionBranchingRepository.Delete(virtualQuestionBranching);
                    }
                    else
                    {
                        virtualQuestionBranching.TargetVirtualQuestionID = questionBranching.TargetVirtualQuestionID;
                        _virtualQuestionBranchingRepository.Save(virtualQuestionBranching);
                    }
                }
                else
                {
                    if (questionBranching.TargetVirtualQuestionID != -1)
                    {
                        virtualQuestionBranching = new VirtualQuestionBranching
                        {
                            VirtualQuestionID = questionBranching.VirtualQuestionID,
                            TargetVirtualQuestionID = questionBranching.TargetVirtualQuestionID,
                            AnswerChoice = questionBranching.AnswerChoice,
                            VirtualTestID = questionBranching.VirtualTestID,
                            Comment = questionBranching.Comment
                        };

                        _virtualQuestionBranchingRepository.Save(virtualQuestionBranching);
                    }
                }
            }
        }

        public List<VirtualQuestionBranching> GetVirtualQuestionBranching(int virtualQuestionId, int virtualTestId)
        {
            return
                _virtualQuestionBranchingRepository.Select()
                    .Where(v => v.VirtualQuestionID == virtualQuestionId && v.VirtualTestID == virtualTestId)
                    .OrderBy(v => v.AnswerChoice)
                    .ToList();
        }
    }
}
