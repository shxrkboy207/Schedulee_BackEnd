using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(IFormFile))]
public partial class IFormFileJsonContext : JsonSerializerContext
{
}
