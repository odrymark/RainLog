namespace Api.Dtos;

public sealed record RefreshTokenResult(
    string Token,
    string TokenHash,
    long ExpiresAt
);