using System.Collections;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ILessonOneRepository
    {
        IList<string> GetLessonOnesBySearchText(string textToSearch, string virtualQuestionIdString, string type);
    }
}