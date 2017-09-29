using System;

namespace Messaging.Contract.Models
{
    public class FileMetadata
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public string CreatedBy { get; set; }

        /// <summary>
        /// In bytes
        /// </summary>
        public long FileSize { get; set; }

        // TODO: Mimetype?
    }
}