﻿using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace WebAppIdentity;

public class SendGridEmailSender : IEmailSender
{
    private readonly IConfiguration configuration;
    private readonly ILogger logger;

    public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        string sendGridApiKey = "V64QJM1G7GF31P31XMQLS7X1";
        if (string.IsNullOrEmpty(sendGridApiKey))
        {
            throw new Exception("The 'SendGridApiKey' is not configured");
        }

        var client = new SendGridClient(sendGridApiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("Ronan team", "https://github.com/ronanmuller/"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        var response = await client.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Email queued successfully");
        }
        else
        {
            logger.LogError("Failed to send email");
            // Adding more information related to the failed email could be helpful in debugging failure,
            // but be careful about logging PII, as it increases the chance of leaking PII
        }
    }
}