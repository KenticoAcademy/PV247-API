using System;

namespace Messaging.Api.Models
{
    /// <summary>
    /// Response model for the download link.
    /// </summary>
    public class DownloadLinkResponse
    {
        /// <summary>
        /// File download link
        /// </summary>
        public Uri FileUri { get; set; }
    }
}
