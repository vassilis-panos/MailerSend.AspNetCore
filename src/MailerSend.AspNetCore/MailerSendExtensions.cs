using MailerSend.AspNetCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class MailerSendExtensions
{
    public static IServiceCollection AddMailerSend(
        this IServiceCollection services, Action<MailerSendOptions> configure)
    {
        services.AddMailerSend();
        services.Configure(configure);
        return services;
    }

    public static IServiceCollection AddMailerSend(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddHttpClient<MailerSendService>(options =>
        {
            options.BaseAddress = new Uri("https://api.mailersend.com/v1/");
        });

        services.TryAddScoped<MailerSendService>();

        return services;
    }
}
