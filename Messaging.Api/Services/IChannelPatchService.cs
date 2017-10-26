using Messaging.Contract.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Messaging.Api.Services
{
    /// <summary>
    /// Functionality allowing race-condition-free application of JSON patches.
    /// </summary>
    public interface IChannelPatchService
    {
        /// <summary>
        /// Converts channel IDs in the operation paths to indexes of channels in given <paramref name="application"/>.
        /// </summary>
        /// <param name="patch">Patch to be converted</param>
        /// <param name="application">Application the patch should be applied to.</param>
        /// <returns>Processed patch document that can be applied to the <paramref name="application"/>.</returns>
        JsonPatchDocument<Application> ConvertOperationPaths(JsonPatchDocument<Application> patch, Application application);
    }
}