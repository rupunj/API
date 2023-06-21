using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using API.Filters;
using API.Const;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using API.Extentions;

JObject Settings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("./appsettings.json"));

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(Settings["Settings"]["BaseURL"].ToString());
builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824;
    options.Limits.MaxConcurrentConnections = 5000;
});

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

#region Default Thread Settings for parallel processing
int minWorker, minIOC;
ThreadPool.GetMinThreads(out minWorker, out minIOC);
ThreadPool.SetMinThreads(3000, minIOC);
#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.DocInclusionPredicate((version, apiDescription) =>
    {
        apiDescription.TryGetMethodInfo(out MethodInfo methodinfo);

        var actionVersions = methodinfo.GetCustomAttributes<MapToApiVersionAttribute>().SelectMany(attr => attr.Versions);

        var controllerVersions = methodinfo.DeclaringType.GetCustomAttributes<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions).FirstOrDefault();

        bool actionPriority = false;

        var actionver = actionVersions.Select(j => j.MajorVersion).ToList();
        var contver = controllerVersions == null ? 0 : controllerVersions.MajorVersion;

        if (controllerVersions != null && actionver.Count > 0)
        {
            if ((int)actionver[0] > (int)contver)
            {
                actionPriority = true;
            }
        }
        else if (controllerVersions == null && actionver.Count > 0)
        {
            actionPriority = true;
        }

        if (actionPriority)
        {
            return actionVersions.Any(v => $"v{v.MajorVersion.ToString()}" == version);
        }
        else
        {
            return $"v{controllerVersions.MajorVersion.ToString()}" == version;
        }

    });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cart API",
        Version = "v1",
        Description = "Cart API use to Create, View and Edit of Stock",
        TermsOfService = new Uri("http://www.dummy.cart.lk/"),
        Contact = new OpenApiContact()
        {
            Name = "Rupun Chathuranga",
            Email = "rupun.cj@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Rupun Chathuranga"
        },
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }});
});
builder.Services.AddApiVersioning(o => {
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new ExceptionFilter(builder.Configuration));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("Settings:IDP:BaseURL");
        options.RequireHttpsMetadata = false;
        options.Audience = Config.APPLICATION_NAME;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.Create, policy => policy.RequireClaim("scope", Policy.Create));
    options.AddPolicy(Policy.View, policy => policy.RequireClaim("scope", Policy.View));
    options.AddPolicy(Policy.Edit, policy => policy.RequireClaim("scope", Policy.Edit));
});
builder.Services.Configure<API.Models.Base.Settings>(builder.Configuration.GetSection("Settings"));
builder.Services.AddAntiforgery();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824;
    options.Limits.MaxConcurrentConnections = 5000;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseEnableRequestRewindMiddleware();
app.UseExceptionMiddleware();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.Use((context, next) => { context.Request.Scheme = "https"; return next(); });
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllers();
});
app.Run();
