using System.Text.Json.Serialization;

namespace FundaAggregator.Model;

public record ApiResultObject
{
    public Metadata Metadata { get; init; }

    [JsonPropertyName("Objects")]
    public Listing[] Listings { get; init; }

    public Paging Paging { get; init; }

    public int TotaalAantalObjecten { get; init; }
}

public record Metadata
{
    public string ObjectType { get; init; }
    public string Omschrijving { get; init; }
    public string Titel { get; init; }
}

public record Paging
{
    public int AantalPaginas { get; init; }
    public int HuidigePagina { get; init; }
}

public record Listing
{
    public string Adres { get; init; }
    public string Foto { get; init; }
    public int GlobalId { get; init; }
    public string Id { get; init; }
    public bool IsVerkocht { get; init; }
    public int? Koopprijs { get; init; }
    public int MakelaarId { get; init; }
    public string MakelaarNaam { get; init; }
    public int Oppervlakte { get; init; }
    public string Postcode { get; init; }
    public int SoortAanbod { get; init; }
    public string URL { get; init; }
    public float WGS84_X { get; init; }
    public float WGS84_Y { get; init; }
    public int? Woonoppervlakte { get; init; }
    public string Woonplaats { get; init; }
}