using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Register EncryptionService as a singleton
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();

var app = builder.Build();

// Root endpoint
app.MapGet("/", () => "Hello World!");

// GET endpoints
app.MapGet("/encrypt/{text}", (string text, IEncryptionService service) =>
{
    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest("Text cannot be empty.");

    try
    {
        return Results.Ok(new { EncryptedText = service.Encrypt(text) });
    }
    catch (Exception ex)
    {
        return Results.Problem("Encryption failed: " + ex.Message);
    }
});

app.MapGet("/decrypt/{text}", (string text, IEncryptionService service) =>
{
    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest("Text cannot be empty.");

    try
    {
        return Results.Ok(new { DecryptedText = service.Decrypt(text) });
    }
    catch (Exception ex)
    {
        return Results.Problem("Decryption failed: " + ex.Message);
    }
});

// POST endpoints for JSON
app.MapPost("/encrypt", ([FromBody] InputModel model, IEncryptionService service) =>
{
    if (string.IsNullOrWhiteSpace(model.Text))
        return Results.BadRequest("Text cannot be empty.");

    try
    {
        return Results.Ok(new { EncryptedText = service.Encrypt(model.Text) });
    }
    catch (Exception ex)
    {
        return Results.Problem("Encryption failed: " + ex.Message);
    }
});

app.MapPost("/decrypt", ([FromBody] InputModel model, IEncryptionService service) =>
{
    if (string.IsNullOrWhiteSpace(model.Text))
        return Results.BadRequest("Text cannot be empty.");

    try
    {
        return Results.Ok(new { DecryptedText = service.Decrypt(model.Text) });
    }
    catch (Exception ex)
    {
        return Results.Problem("Decryption failed: " + ex.Message);
    }
});

app.Run("http://0.0.0.0:5000"); // Secure HTTPS

// Input model
public class InputModel
{
    public string Text { get; set; } = string.Empty;
}

// Encryption service interface
public interface IEncryptionService
{
    string Encrypt(string input);
    string Decrypt(string input);
}

// Encryption service implementation
public class EncryptionService : IEncryptionService
{
    private const int Shift = 3; // Shift for Caesar cipher

    public string Encrypt(string input)
    {
        return new string(input.Select(c => (char)(c + Shift)).ToArray());
    }

    public string Decrypt(string input)
    {
        return new string(input.Select(c => (char)(c - Shift)).ToArray());
    }
}
