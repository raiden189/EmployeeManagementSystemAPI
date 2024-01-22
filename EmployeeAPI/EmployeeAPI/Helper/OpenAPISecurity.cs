using Microsoft.OpenApi.Models;
using System.Runtime.CompilerServices;

namespace EmployeeAPI.Helper
{
    public static class OpenAPISecurity
    {
        public static OpenApiSecurityScheme GetSecurityScheme() => new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "Jwt",
            In = ParameterLocation.Header,
            Description = "JWT Authentication"
        };

        public static OpenApiSecurityRequirement GetSecurityRequirements() => new OpenApiSecurityRequirement() 
        {
            { 
                new OpenApiSecurityScheme()
                { 
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[]{}
            }
        };

        public static OpenApiInfo GetInfo() => new OpenApiInfo()
        {
            Title = "EmployeeAPI",
            License = GetLicense()
        };

        private static OpenApiLicense GetLicense() => new OpenApiLicense() { Name = "Free License" };
    }
}
