using GameStore.Api.Data;
using GameStore.Api.Dtos.Images;
using GameStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class ImagesEndpoints
{
    const string GetImageEndpoint = "GetImageById";

    public static void MapImagesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/images")
            .WithTags("Images");

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

            var imageId = await SaveImageToDatabase(newImage, dbContext); 

            return Results.CreatedAtRoute(GetImageEndpoint, new { id = imageId }, new { imageId, newImage.Description });
        })
        .DisableAntiforgery()
        .RequireAuthorization("AdminOnly")
        .Accepts<ImageDto>("multipart/form-data");

        group.MapDelete("/delete/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await  dbContext.Images
                        .Where(image => image.Id == id)
                        .ExecuteDeleteAsync();
            
            return Results.NoContent();
        }).RequireAuthorization("AdminOnly");
    }

    public static string GetContentType(string fileExtension) => fileExtension.ToLower() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        _ => "application/octet-stream"
    };

    public static async Task<int> SaveImageToDatabase(ImageDto imageDto, GameStoreContext dbContext)
    {
        var file = imageDto.File;
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);

        Image image = new()
        {
            Description = imageDto.Description,
            Data = memoryStream.ToArray(),
            FileExtention = Path.GetExtension(file.FileName),
            UploadDate = DateTime.UtcNow
        };

        await dbContext.Images.AddAsync(image);
        await dbContext.SaveChangesAsync();

        return image.Id;
    }
}
