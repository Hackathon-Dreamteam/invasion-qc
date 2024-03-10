﻿using InvasionQc.Core.Alerting;
using InvasionQc.Core.Constants;
using MediatR;

namespace InvasionQc.Core.Advisory;

public record GetSpeciesDetailsQuery(string SpeciesName, Locations Location) : IRequest<SpeciesDetails>;

public class GetSpeciesDetailsQueryHandler : IRequestHandler<GetSpeciesDetailsQuery, SpeciesDetails>
{
    private readonly IAdvisor _advisor;

    public GetSpeciesDetailsQueryHandler(IAdvisor advisor)
    {
        this._advisor = advisor;
    }

    public async Task<SpeciesDetails> Handle(GetSpeciesDetailsQuery request, CancellationToken cancellationToken)
    {
        var descriptionTask = this._advisor.GetMessage(GetAssistantContext(), GetAssistantInstruction(request.SpeciesName));
        var imageTask = this._advisor.GetImage(GetImagePrompt(request.SpeciesName, request.Location));

        await Task.WhenAll(descriptionTask, imageTask);

        return new SpeciesDetails(descriptionTask.Result, new Uri(imageTask.Result));
    }

    private string GetAssistantContext()
    {
        var context = """
                      Vous êtes un expert en génération de contenu et en biodiversité et vous devez aider les élues municipaux.
                      Je vais vous donner un nom d'animal et vous allez devoir me donner une description de l'animal: comment le reconnaitre et surtout son impact dans notre ecosysteme
                      ainsi que les raisons pourquoi on devrait s'en soucier.
                      """;

        return context;
    }

    private string GetAssistantInstruction(string speciesName)
    {
        return $"Donnez moi une description de {speciesName}.";
    }

    private string GetImagePrompt(string speciesName, Locations location)
    {
        return $"Return a realistic image of {speciesName} in the city of {location.ToString()}, the one in Canada.";
    }
}

public record SpeciesDetails(string Description, Uri ImageUri);
