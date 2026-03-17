namespace back_end.Hubs.Dtos;

// Minimal encrypted message DTO. Clients are responsible for encrypting and
// decrypting payloads. The server treats this as opaque data.
public record EncryptedMessageDto(string CipherText, string Nonce, string? EphemeralPublicKey = null, string? Algorithm = "AES-GCM");
