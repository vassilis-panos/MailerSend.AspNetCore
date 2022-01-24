using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MailerSend.AspNetCore;

public class MailerSendService
{
    private readonly HttpClient _httpClient;
    private readonly MailerSendOptions _options;

    public MailerSendService(HttpClient httpClient, IOptions<MailerSendOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.ApiToken))
            throw new ArgumentException("Api token is null or empty");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiToken);
    }

    public async Task SendMailAsync(
        IEnumerable<Recipient> to,
        IEnumerable<Recipient>? cc = default,
        IEnumerable<Recipient>? bcc = default,
        string? subject = default,
        string? text = default,
        string? html = default,
        string? templateId = default,
        IEnumerable<Attachment>? attachments = default,
        DateTime? sendAt = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.SenderEmail))
            throw new ArgumentException("Sender email address is null or empty");

        var from = new
        {
            Email = _options.SenderEmail,
            Name = _options.SenderName
        };

        var variables = to.Select(recipient => new
        {
            recipient.Email,
            Substitutions = recipient?.Substitutions?
                .Select(kvp => new { Var = kvp.Key, kvp.Value })
        });

        if (cc is not null)
        {
            variables = variables.Concat(cc.Select(recipient => new
            {
                recipient.Email,
                Substitutions = recipient?.Substitutions?
               .Select(kvp => new { Var = kvp.Key, kvp.Value })
            }));
        }

        if (bcc is not null)
        {
            variables = variables.Concat(bcc.Select(recipient => new
            {
                recipient.Email,
                Substitutions = recipient?.Substitutions?
               .Select(kvp => new { Var = kvp.Key, kvp.Value })
            }));
        }

        var sendAtUts = (int?)sendAt?
            .ToUniversalTime()
            .Subtract(DateTime.UnixEpoch)
            .TotalSeconds;

        var requestBody = new
        {
            From = from,
            To = to,
            Cc = cc,
            Bcc = bcc,
            Subject = subject,
            Text = text,
            Html = html,
            Template_id = templateId,
            Attachments = attachments,
            Variables = variables,
            Send_at = sendAtUts
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await _httpClient.PostAsJsonAsync(
                "email", requestBody, jsonOptions, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
