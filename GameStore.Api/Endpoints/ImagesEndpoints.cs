using GameStore.Api.Data;
using GameStore.Api.Dtos.Images;
using GameStore.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Endpoints;

public static class ImagesEndpoints
{
    const string GetImageEndpoint = "GetImageById";

    public static void MapImagesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/images");

        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var image = await dbContext.Images.FindAsync(id);

            if (image is null)
                return Results.NotFound();

            return Results.File(image.Data, contentType: GetContentType(image.FileExtention), fileDownloadName: $"image_{id}{image.FileExtention}");
        }).WithName(GetImageEndpoint);

        group.MapPost("/upload", async ([FromForm] ImageDto newImage, GameStoreContext dbContext) =>
        {
            if (newImage.File == null || newImage.File.Length == 0)
                return Results.BadRequest("No file was uploaded.");

            var file = newImage.File;
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            Image image = new()
            {
                Description = newImage.Description,
                Data = memoryStream.ToArray(),
                FileExtention = Path.GetExtension(file.FileName),
                UploadDate = DateTime.UtcNow
            };

            dbContext.Images.Add(image);
            await dbContext.SaveChangesAsync();
            return Results.CreatedAtRoute(GetImageEndpoint, new { id = image.Id }, new { image.Id, image.Description });
        })
        .DisableAntiforgery()
        .RequireAuthorization("AdminOnly")
        .Accepts<ImageDto>("multipart/form-data");
    }

    public static string GetContentType(string fileExtension) => fileExtension.ToLower() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        _ => "application/octet-stream"
    };
}
