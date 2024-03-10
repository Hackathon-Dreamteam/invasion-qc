﻿using InvasionQc.Core.Alerting;
using InvasionQc.Core.Constants;
using InvasionQc.Core.DataLoader;
using InvasionQc.Core.Observations;
using InvasionQc.Core.Utils;
using MediatR;

namespace InvasionQc.Core.Species;

public record GetSpeciesReport(string SpeciesName, Locations Location) : IRequest<SpeciesReports>;

public class GetSpeciesReportHandler : IRequestHandler<GetSpeciesReport, SpeciesReports>
{
    private readonly InvasiveSpeciesLoader _invasiveSpeciesLoader;
    private readonly PrecariousSpeciesLoader _precariousSpeciesLoader;
    private readonly AlertRepositories _alertRepositories;
    private readonly IMediator _mediator;

    public GetSpeciesReportHandler(InvasiveSpeciesLoader invasiveSpeciesLoader, PrecariousSpeciesLoader precariousSpeciesLoader, AlertRepositories alertRepositories, IMediator mediator)
    {
        this._invasiveSpeciesLoader = invasiveSpeciesLoader;
        this._precariousSpeciesLoader = precariousSpeciesLoader;
        _alertRepositories = alertRepositories;
        this._mediator = mediator;
    }

    public async Task<SpeciesReports> Handle(GetSpeciesReport request, CancellationToken cancellationToken)
    {
        var invasiveSpeciesInfo = (await this._invasiveSpeciesLoader.Load(cancellationToken))
                .FirstOrDefault(x =>
                    string.Equals(request.SpeciesName, x.FrenchName, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(request.SpeciesName, x.LatinName, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(request.SpeciesName, x.EnglishName, StringComparison.InvariantCultureIgnoreCase)
                    );

        var precariousSpeciesInfo = (await this._precariousSpeciesLoader.Load(cancellationToken))
            .FirstOrDefault(x =>
                string.Equals(request.SpeciesName, x.SpeciesName, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(request.SpeciesName, x.ScientificSpeciesName, StringComparison.InvariantCultureIgnoreCase)
            );

        var alerts = this._alertRepositories.GetAlerts(request.SpeciesName);

        var observations = await this._mediator.CreateStream(new GetObservationsQuery(request.Location), cancellationToken).ToListAsync();

        var observationsForSpecies = observations.Where(o => string.Equals(request.SpeciesName, o.SpeciesName, StringComparison.InvariantCultureIgnoreCase)).ToList();

        return new SpeciesReports(
            invasiveSpeciesInfo != null,
            precariousSpeciesInfo != null,
            precariousSpeciesInfo?.ConvervationStatus ?? string.Empty,
            alerts,
            observationsForSpecies);
    }
}
