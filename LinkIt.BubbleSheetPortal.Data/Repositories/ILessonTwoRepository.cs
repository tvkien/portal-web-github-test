using System.Collections;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ILessonTwoRepository
    {
        IList<string> GetLessonTwosBySearchText(string textToSearch, string inputIdString, string type);
    }
}