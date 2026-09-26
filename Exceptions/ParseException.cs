namespace TestJob.Exceptions;

public static class ParseException
{
    public static readonly string EmptySelector = "selector не найден";
    public static readonly string EmptyAttribute = "attribute не найден";
    public static readonly string EmptyUrlB64 = "url_b64 не найден";
    public static readonly string EmptyEncryptedTextBytesB64 = "encrypted_text_bytes_b64 не найден";
    public static readonly string EmptyKeyBytesB64 = "key_bytes_b64 не найден";
    public static readonly string EmptyPageB64 = "page_b64 не найден";
    public static readonly string InvalidUrlB64 = "невалидный url_b64 - невозможно преобразовать из base64";
    public static readonly string InvalidEncryptedTextBytesB64 = "невалидный encrypted_text_bytes_b64 - невозможно преобразовать из base64";
    public static readonly string InvalidKeyBytesB64 = "невалидный key_bytes_b64 - невозможно преобразовать из base64";
    public static readonly string InvalidPageB64 = "невалидный page_b64 - невозможно преобразовать из base64";
    public static readonly string InvalidAesKeyLength = "длина aes ключа должна составлять 32";
    public static readonly string InvalidAesTextLength = "длина aes текса должна быть кратна 16";
}