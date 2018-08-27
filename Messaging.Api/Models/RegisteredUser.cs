namespace Messaging.Api.Models
{
    /// <summary>
    /// Model for user registration.
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
