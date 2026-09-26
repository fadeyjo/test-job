using FluentValidation;
using TestJob.DTOs.Elements;
using TestJob.Exceptions;

namespace TestJob.Validators;

public class ParseRequestValidator : AbstractValidator<ParseRequest>
{
    public ParseRequestValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty()
            .WithMessage(ParseException.EmptySelector);

        RuleFor(x => x.Attribute)
            .NotEmpty()
            .WithMessage(ParseException.EmptyAttribute);

        RuleFor(x => x.UrlB64)
            .NotEmpty()
            .WithMessage(ParseException.EmptyUrlB64)
            .Must(ValidBase64)
            .WithMessage(ParseException.InvalidUrlB64);

        RuleFor(x => x.EncryptedTextBytesB64)
            .NotEmpty()
            .WithMessage(ParseException.EmptyEncryptedTextBytesB64)
            .Must(ValidBase64)
            .WithMessage(ParseException.InvalidEncryptedTextBytesB64)
            .Must(ValidAesBlockSize)
            .WithMessage(ParseException.InvalidAesTextLength);

        RuleFor(x => x.KeyBytesB64)
            .NotEmpty()
            .WithMessage(ParseException.EmptyKeyBytesB64)
            .Must(ValidBase64)
            .WithMessage(ParseException.InvalidKeyBytesB64)
            .Must(ValidAes256Key)
            .WithMessage(ParseException.InvalidAesKeyLength);

        RuleFor(x => x.PageB64)
            .NotEmpty()
            .WithMessage(ParseException.EmptyPageB64)
            .Must(ValidBase64)
            .WithMessage(ParseException.InvalidPageB64);
    }
    
    private static bool ValidBase64(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool ValidAes256Key(string? value)
    {
        if (!ValidBase64(value))
            return false;

        var bytes = Convert.FromBase64String(value);

        // AES-256 = 256 бита = 32 байта
        return bytes.Length == 32;
    }

    private static bool ValidAesBlockSize(string? value)
    {
        if (!ValidBase64(value))
            return false;

        var bytes = Convert.FromBase64String(value);

        // AES block size = 128 бит = 16 байт.
        // При PaddingMode.None длина ciphertext должна быть кратна размеру блока.
        return bytes.Length > 0 && bytes.Length % 16 == 0;
    }
}