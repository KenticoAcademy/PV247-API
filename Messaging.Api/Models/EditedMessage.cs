﻿using Newtonsoft.Json.Linq;

namespace Messaging.Api.Models
{
    /// <summary>
    /// Model of the message update.
    /// </summary>
    public class EditedMessage
    {
        /// <summary>
        /// Message text
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Additional data in custom format, presumably JSON.
        /// </summary>
        public JObject CustomData { get; set; }
    }
}