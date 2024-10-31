namespace TeamsEats.Domain.Services;

/// <summary>
/// Service interface for interacting with Microsoft Graph for user-related operations.
/// </summary>
public interface IGraphService
{
    /// <summary>
    /// Retrieves a user's photo by their ID.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>The user's photo as a string.</returns>
    Task<string> GetPhoto(string userId);

    /// <summary>
    /// Retrieves the current user's ID.
    /// </summary>
    /// <returns>The user ID as a string.</returns>
    Task<string> GetUserId();

    /// <summary>
    /// Retrieves a user's display name by their ID.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>The display name of the user.</returns>
    Task<string> GetUserDisplayName(string userId);

    /// <summary>
    /// Sends a message from one user to another.
    /// </summary>
    /// <param name="addresserId">The sender's ID.</param>
    /// <param name="addresseeId">The recipient's ID.</param>
    /// <param name="message">The message content.</param>
    Task SendMessage(string addresserId, string addresseeId, string message);

    /// <summary>
    /// Sends a notification indicating an order has been closed.
    /// </summary>
    /// <param name="addresserId">The sender's ID.</param>
    /// <param name="addresseeId">The recipient's ID.</param>
    /// <param name="restaurant">The restaurant name.</param>
    /// <param name="orderId">The group order ID.</param>
    Task SendActivityFeedTypeOrdered(string addresserId, string addresseeId, string restaurant, int orderId);

    /// <summary>
    /// Sends a notification indicating an order has been deleted.
    /// </summary>
    /// <param name="addresserId">The sender's ID.</param>
    /// <param name="addresseeId">The recipient's ID.</param>
    /// <param name="restaurant">The restaurant name.</param>
    Task SendActivityFeedTypeDeleted(string addresserId, string addresseeId, string restaurant);

    /// <summary>
    /// Sends a notification indicating an order has been delivered.
    /// </summary>
    /// <param name="addresserId">The sender's ID.</param>
    /// <param name="addresseeId">The recipient's ID.</param>
    /// <param name="orderId">The group order ID.</param>
    Task SendActivityFeedTypeDelivered(string addresserId, string addresseeId, int orderId);
}