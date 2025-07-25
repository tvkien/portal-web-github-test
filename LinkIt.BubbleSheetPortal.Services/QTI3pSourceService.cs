using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTI3pSourceService 
    {
        private readonly IReadOnlyRepository<QTI3pSource> _repository;

        public QTI3pSourceService(IReadOnlyRepository<QTI3pSource> repository)
        {
            _repository = repository;
        }

        public IQueryable<QTI3pSource> GetAll()
        {
            return _repository.Select();
        }
        public List<QTI3pSource> GetAuthorizeQti3pSource(XliFunctionAccess xliFunctionAccess)
        {
            var qti3pSources = _repository.Select().ToList();
            var result = new List<QTI3pSource>(qti3pSources.Count);
            foreach (var qti3pSource in qti3pSources)
            {
                if (qti3pSource.QTI3pSourceId == (int) QTI3pSourceEnum.Mastery)
                {
                    //check right to access 
                    if (xliFunctionAccess.CerticaLibraryAccessible)
                    {
                        result.Add(qti3pSource);
                    }
                }
                else if (qti3pSource.QTI3pSourceId == (int) QTI3pSourceEnum.Progress)
                {
                    if (xliFunctionAccess.ProgressLibraryAccessible)
                    {
                        result.Add(qti3pSource);
                    }
                }
                else
                {
                    result.Add(qti3pSource);
                }
            }
            return result;
        }
    }
}
