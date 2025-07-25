using System;

namespace LinkIt.BubbleSheetPortal.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class UriAttribute : Attribute
    {
        /// <summary>
        /// Initialize a new instance of the UriAttribute class.
        /// </summary>
        /// <param name="uri">The URI associated with the enumerated field value.</param>
        public UriAttribute(string uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// Get or Set the URI associated with the enumberated field value.
        /// </summary>
        public string Uri { get; set; }
    }
}
