using System.Text.Json.Serialization;

namespace TestJob.DTOs.Elements;

public sealed class ParseRequest
{
    public string? Selector { get; set; }

    public string? Attribute { get; set; }

    [JsonPropertyName("url_b64")]
    public string? UrlB64 { get; set; }

    [JsonPropertyName("encrypted_text_bytes_b64")]
    public string? EncryptedTextBytesB64 { get; set; }

    [JsonPropertyName("key_bytes_b64")]
    public string? KeyBytesB64 { get; set; }

    [JsonPropertyName("page_b64")]
    public string? PageB64 { get; set; }
}