using Envoc.Core.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Classes
{
    public class MetaDataDto
    {
        public string SubjectMappingOptionJson { get; set; }
        public List<ClassMetaDto> ClassMetas { get; set; } = new List<ClassMetaDto>();
        public string ClassMetaStr { get; set; }
        public bool HasConfigClassMeta { get; set; }
    }
    public class ClassMetaDto
    {
        public int ClassMetaID { get; set; }
        public int ClassID { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public string Label { get; set; }
    }
    public class CreateClassMetas
    {
        public int ClassId { get; set; }
        public List<ClassMetaDto> ClassMetas { get; set; } = new List<ClassMetaDto>();
    }
}
