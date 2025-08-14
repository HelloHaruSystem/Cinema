namespace Cinema.Utils;

/// <summary>
/// Result object for booking operations containing success/failure details.
/// Tracks which seats were successfully booked and which failed.
/// </summary>
public class BookingResult
{
    /// <summary>
    /// Gets or sets whether the entire booking operation was successful.
    /// True only if ALL requested seats were booked successfully.
    /// </summary>
    public bool IsSuccessful { get; set; }
    
    /// <summary>
    /// Gets or sets the list of seats that were successfully booked.
    /// Each array contains [row, seat] position.
    /// </summary>
    public List<int[]> SuccessfulSeats { get; set; } = new List<int[]>();
    
    /// <summary>
    /// Gets or sets the list of seats that failed to book with error descriptions.
    /// Contains human-readable error messages for each failed seat.
    /// </summary>
    public List<string> FailedSeats { get; set; } = new List<string>();
}