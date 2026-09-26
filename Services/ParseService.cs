using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FluentValidation.Results;
using TestJob.DTOs.Elements;
using TestJob.Repositories;

namespace TestJob.Services;

public class ParseService(IElementsRepository repository) : IParseService
{
    public async Task<ParseResponse> CreateElementAsync(ParseRequest body, ValidationResult validationResult)
    {
        var errorCodes =
            validationResult.IsValid ?
                new List<string>() :
                validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        
        var result = new ParseResponse()
        {
            IsError = validationResult.IsValid ? 0 : 1,
            ErrorCodes = errorCodes,
            ErrorMessages = [],
            ElementsCount = 0,
            EmailsCount = 0,
            Url = TryDecodeBase64(body.UrlB64),
            DecryptedPlainText = DecryptText(body.EncryptedTextBytesB64, body.KeyBytesB64),
            ElementsAttrList = [],
            EmailsList = []
        };

        var encodedPage = TryDecodeBase64(body.PageB64);

        if (encodedPage == null) return result;
        
        var parser = new HtmlParser();
        IHtmlDocument? document = null;
            
        try
        {
            document = await parser.ParseDocumentAsync(encodedPage);
        }
        catch (Exception e)
        {
            result.ErrorMessages.Add(e.Message);
            result.IsError = 1;
        }

        if (document == null) return result;
        
        // Task 5
        result.EmailsList = GetEmails(encodedPage);
        result.EmailsCount = result.EmailsList.Count;
                
        // Task 3-4
        List<IElement>? elements = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(body.Selector))
                elements = document.QuerySelectorAll(body.Selector).ToList();
        }
        catch (Exception e)
        {
            result.ErrorMessages.Add(e.Message);
            result.IsError = 1;
        }

        if (elements == null) return result;
        
        result.ElementsCount = elements.Count;
        
        if (string.IsNullOrWhiteSpace(body.Attribute)) return result;
        
        foreach (var el in elements)
        {
            var attrValue = el.GetAttribute(body.Attribute);
            result.ElementsAttrList.Add(attrValue);
            
            var fullHtmlCode = el.OuterHtml;

            try
            {
                await repository.CreateElementAsync(attrValue, fullHtmlCode);
            }
            catch (Exception e)
            {
                result.ErrorMessages.Add(e.Message);
                result.IsError = 1;
            }
        }
        
        return result;
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

    private string? TryDecodeBase64(string? base64)
    {
        try
        {
            return string.IsNullOrWhiteSpace(base64) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    private string? DecryptText(string? encryptedTextB64, string? keyB64)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(encryptedTextB64) ||
                string.IsNullOrWhiteSpace(keyB64)) return null;
            
            var encryptedBytes = Convert.FromBase64String(encryptedTextB64);
            var keyBytes = Convert.FromBase64String(keyB64);

            if (keyBytes.Length != 32)
                throw new ArgumentException(
                    "AES-256 key must contain exactly 32 bytes");
        
            if (encryptedBytes.Length == 0 || encryptedBytes.Length % 16 != 0)
                throw new ArgumentException(
                    "AES-256 value must contain exactly 32 bytes");

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
        catch (Exception e)
        {
            return null;
        }
    }
}