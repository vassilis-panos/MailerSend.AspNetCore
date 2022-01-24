# MailerSend.AspNetCore [![NuGet](https://img.shields.io/nuget/v/MailerSend.AspNetCore.svg)](https://www.nuget.org/packages/MailerSend.AspNetCore)

ASP.NET Core library for [MailerSend](https://www.mailersend.com/)

## Installation

.NET CLI
```
dotnet add package MailerSend.AspNetCore
```

Package Manager
```
Install-Package MailerSend.AspNetCore
```

## Configuration

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMailerSend(options =>
        {
            options.ApiToken = "API-TOKEN";
            options.SenderEmail = "mail@domain.com";
            options.SenderName = "MailerSend";
        });
    }
}
```

### appsettings.json

```json
{
  "MailerSend": {
    "ApiToken": "API-TOKEN",
    "SenderEmail": "mail@domain.com",
    "SenderName": "MailerSend"
  }
}
```

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<MailerSendOptions>(
            Configuration.GetSection("MailerSend"));
        services.AddMailerSend();
    }
}
```