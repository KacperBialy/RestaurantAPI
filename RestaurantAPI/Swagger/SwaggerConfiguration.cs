using Microsoft.OpenApi.Models;

namespace RestaurantAPI.Swagger
{
    public static class SwaggerConfiguration
    {
        public static OpenApiSecurityScheme GetOpenApiBearerSecurityScheme()
        {
            OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            };
            return openApiSecurityScheme;
        }
        public static OpenApiSecurityRequirement GetOpenApiBearerSecurityRequirement()
        {
            OpenApiSecurityRequirement openApiSecurityRequirement = new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                };
            return openApiSecurityRequirement;
        }
    }
}
