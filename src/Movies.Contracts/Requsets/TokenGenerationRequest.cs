namespace Movies.Contracts.Requsets;

public class TokenGenerationRequest
{
    public required Guid userId { get; init; }
    public required string email { get; init; }
    public required Dictionary<string,object> CustomClaims { get; init; }
}
