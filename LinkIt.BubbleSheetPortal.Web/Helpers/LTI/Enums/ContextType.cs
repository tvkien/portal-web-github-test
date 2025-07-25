using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils;
using Newtonsoft.Json;
using System;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums
{
    [JsonConverter(typeof(ContextTypeConverter))]
    public enum ContextType
    {
        Unknown = 0,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/course#CourseTemplate")]
        [Uri("urn:lti:context-type:ims/lis/CourseTemplate")]
        [Uri("CourseTemplate")]
        CourseTemplate,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/course#CourseOffering")]
        [Uri("urn:lti:context-type:ims/lis/CourseOffering")]
        [Uri("CourseOffering")]
        CourseOffering,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/course#CourseSection")]
        [Uri("urn:lti:context-type:ims/lis/CourseSection")]
        [Uri("CourseSection")]
        CourseSection,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/course#Group")]
        [Uri("urn:lti:context-type:ims/lis/Group")]
        [Uri("Group")]
        Group
    }
}
