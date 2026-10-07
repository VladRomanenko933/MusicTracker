using Microsoft.EntityFrameworkCore;
using MusicTracker.Models;

namespace MusicTracker.Services;

public record SummaryRow(string Name, int Count, int TotalMinutes);

public static class SummaryCalculator
{
    public static async Task<List<SummaryRow>> ByArtistAsync(IQueryable<Album> query) =>
        await query
            .GroupBy(x => x.Artist!.Name)
            .OrderByDescending(g => g.Sum(x => x.DurationMinutes))
            .Select(g => new SummaryRow(
                g.Key, 
                g.Count(), 
                g.Sum(x => x.DurationMinutes)))
            .ToListAsync();

    public static async Task<List<SummaryRow>> ByMonthAsync(IQueryable<Album> query)
    {
        var raw = await query
            .GroupBy(x => new { x.ReleaseDate.Year, x.ReleaseDate.Month })
            .Select(g => new
            {
                g.Key.Year, 
                g.Key.Month,
                Count = g.Count(),
                TotalMinutes = g.Sum(x => x.DurationMinutes)
            })
            .ToListAsync();

        return raw
            .OrderBy(r => r.Year).ThenBy(r => r.Month)
            .Select(r => new SummaryRow(
                $"{r.Month:D2}.{r.Year}", 
                r.Count, 
                r.TotalMinutes))
            .ToList();
    }

    public static async Task<SummaryRow> TotalAsync(IQueryable<Album> query) =>
        new SummaryRow(
            "Усього",
            await query.CountAsync(),
            await query.SumAsync(x => x.DurationMinutes));
}