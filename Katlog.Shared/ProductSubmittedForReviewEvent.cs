namespace Katlog.Shared;

public class ProductSubmittedForReviewEvent : BaseEvent
{
    public ProductSubmittedForReviewEvent()
    {
        EventType = "ProductSubmittedForReview";
    }

    public ProductSubmittedForReviewPayload Payload { get; set; } = null!;
}

public class ProductSubmittedForReviewPayload
{
    public int ProductId { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
}