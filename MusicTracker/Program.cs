using MusicTracker.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using MusicTracker.Api;
using MusicTracker.Data;
using MusicTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<MusicTrackerContext>(o =>
    o.UseSqlite("Data Source=musictracker.db"));
builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var api = app.MapGroup("/api");

api.MapGet("/albums", async Task<Ok<List<AlbumDto>>> (IDbContextFactory<MusicTrackerContext> f) =>
{
    await using var db = await f.CreateDbContextAsync();
    var data = await db.Albums
        .OrderBy(x => x.Id)
        .Select(x => new AlbumDto(
            x.Id,
            x.Title,
            x.Artist!.Name,
            x.Genre!.Name,
            x.DurationMinutes,
            x.ReleaseDate))
        .ToListAsync();

    return TypedResults.Ok(data);
});

api.MapGet("/albums/{id:int}", async Task<Results<Ok<AlbumDto>, NotFound>> (
    int id, IDbContextFactory<MusicTrackerContext> f) =>
{
    await using var db = await f.CreateDbContextAsync();
    var album = await db.Albums
        .Where(x => x.Id == id)
        .Select(x => new AlbumDto(
            x.Id,
            x.Title,
            x.Artist!.Name,
            x.Genre!.Name,
            x.DurationMinutes,
            x.ReleaseDate))
        .FirstOrDefaultAsync();

    return album is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(album);
});

api.MapGet("/summary", async Task<Ok<List<SummaryRow>>> (IDbContextFactory<MusicTrackerContext> f) =>
{
    await using var db = await f.CreateDbContextAsync();
    var rows = await SummaryCalculator.ByArtistAsync(db.Albums);
    return TypedResults.Ok(rows);
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
