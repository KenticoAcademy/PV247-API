namespace Messaging.Api.ViewModels
{
    /// <summary>
    /// View-model of the message update.
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
        public string CustomData { get; set; }
    }
}