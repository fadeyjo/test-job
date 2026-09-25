using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using TestJob.DTOs.Elements;
using TestJob.Repositories;

namespace TestJob.Services;

public class ParseService(IElementsRepository repository) : IParseService
{
    public async Task<ParseResponse> CreateElementAsync(ParseRequest body)
    {
        // Task 2
        var encodedUrl = System.Text.Encoding.UTF8.GetString(
            Convert.FromBase64String(body.UrlB64));
        
        var encodePage = System.Text.Encoding.UTF8.GetString(
            Convert.FromBase64String(body.PageB64));
        
        var parser = new HtmlParser();
        var document = await parser.ParseDocumentAsync(encodePage);
        
        var elements = document.QuerySelectorAll(body.Selector).ToList();
        
        var attrValuesList = new List<string?>();

        // Task 3-4
        foreach (var el in elements)
        {
            var attrValue = el.GetAttribute(body.Attribute);
            attrValuesList.Add(attrValue);
            
            var fullHtmlCode = el.OuterHtml;

            await repository.CreateElementAsync(attrValue, fullHtmlCode);
        }
        
        // Task 5
        var emails = GetEmails(encodePage);
        
        // Task 6
        var decryptedText = DecryptText(body.EncryptedTextBytesB64, body.KeyBytesB64);

        return new ParseResponse()
        {
            IsError = 0,
            ErrorCode = "",
            ErrorMessage = "",
            ElementsCount = elements.Count,
            EmailsCount = emails.Count,
            Url = encodedUrl,
            DecryptedPlainText = decryptedText,
            ElementsAttrList = attrValuesList,
            EmailsList = emails
        };
    }

    private List<string> GetEmails(string page)
    {
        const string pattern =
            @"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+";

        return Regex
            .Matches(page, pattern)
            .Select(match => match.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
    
    private string DecryptText(string encryptedTextB64, string keyB64)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedTextB64);
        var keyBytes = Convert.FromBase64String(keyB64);

        if (keyBytes.Length != 32)
            throw new ArgumentException(
                "AES-256 key must contain exactly 32 bytes");

        using var aes = Aes.Create();

        aes.Key = keyBytes;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        using var decryptor = aes.CreateDecryptor();

        var decryptedBytes = decryptor.TransformFinalBlock(
            encryptedBytes,
            0,
            encryptedBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}