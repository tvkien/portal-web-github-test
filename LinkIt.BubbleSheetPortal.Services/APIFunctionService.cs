using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class APIFunctionService
    {
        private readonly IReadOnlyRepository<APIFunction> repository;

        public APIFunctionService(IReadOnlyRepository<APIFunction> repository)
        {
            this.repository = repository;
        }

        public bool CheckValidURL(List<int> lst, string strURL)
        {
            var listFunction =
                repository.Select().Where(f => lst.Contains(f.APIFunctionId)).ToList();
            
            foreach (APIFunction apiFunction in listFunction)
            {
                Regex regex = new Regex(apiFunction.URI);
                if (regex.IsMatch(strURL))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
