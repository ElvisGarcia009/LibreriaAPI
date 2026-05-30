using Microsoft.OpenApi.Models;

internal class OpenApiReference
{
    public ReferenceType Type { get; set; }
    public required string Id { get; set; }
}