namespace Messaging.Api.ViewModels
{
    /// <summary>
    /// View-model for user registration.
    /// </summary>
    public class RegisteredUser
    {
        /// <summary>
        /// Email that identifies the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Optional application specific custom data for the user.
        /// </summary>
        public string CustomData { get; set; }
    }
}
