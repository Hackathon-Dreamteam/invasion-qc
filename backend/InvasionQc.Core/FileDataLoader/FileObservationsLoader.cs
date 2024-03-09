﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using InvasionQc.Core.Constants;
using Microsoft.Extensions.Logging;

namespace InvasionQc.Core.FileDataLoader;

public class FileObservationsLoader
{
    private readonly ILogger<FileObservationsLoader> _logger;

    public FileObservationsLoader(ILogger<FileObservationsLoader> logger)
    {
        _logger = logger;
    }

    public async IAsyncEnumerable<FileObservations> GetSpecies(Locations location, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using FileStream json = File.OpenRead(GetFilePath());
        var observationsEnumerable =  JsonSerializer.DeserializeAsyncEnumerable(json, FileDataLoader.ObservationsSourceGenerationContext.Default.FileObservations, cancellationToken);
        await foreach (var observation in observationsEnumerable)
        {
            if(observation == null)
            {
                continue;
            }

            if(location == Locations.All)
            {
                yield return observation;
            }
            else if (observation.Location == location.ToString())
            {
                yield return observation;
            }
        }
    }

    private static string GetFilePath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/species_with_location.json");
    }
}

public class FileObservations
{
    [JsonPropertyName("species_guess")]
    public string SpeciesName { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("isEnvahissant")]
    public bool IsInvasive { get; set; }
}

[JsonSerializable(typeof(List<FileObservations>))]
internal partial class ObservationsSourceGenerationContext : JsonSerializerContext
{
}
