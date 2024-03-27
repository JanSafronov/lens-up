using LensUp.PhotoCollectorService.API;
using LensUp.PhotoCollectorService.API.Channels;
using LensUp.PhotoCollectorService.API.Extensions;
using LensUp.PhotoCollectorService.API.Requests;
using LensUp.PhotoCollectorService.API.Validators;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAntiforgery()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapPost("/upload-photo/{enterCode}", async (
    int enterCode,
    [FromForm] UploadPhotoToGalleryRequest request, 
    IUploadPhotoToGalleryRequestValidator validator, 
    IPhotoChannel channel,
    CancellationToken cancellationToken) =>
{
    string photoFileExtension = validator.EnsureThatPhotoFileIsValid(request.PhotoFile);
    string galleryId = await validator.EnsureThatGalleryIsActivated(enterCode, cancellationToken);    

    await channel.PublishAsync(new PhotoProcessorRequest(galleryId, await request.PhotoFile.GetBytes(), photoFileExtension));
    return Results.Accepted();
}) 
.DisableAntiforgery() // Need this when you want to upload without a token
.WithName("AddPhotoToGallery")
.WithOpenApi();

app.Run();
