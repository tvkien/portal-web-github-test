using System.Collections;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITopicRepository
    {
        IList<string> GetTopicsBySearchText(string textToSearch, string inputIdString, string type);
    }
}