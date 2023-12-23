using System.Text.Json.Serialization;

namespace FundaAggregator.Model;

public class ApiResultObject
{
    public int AccountStatus { get; set; }
    public bool EmailNotConfirmed { get; set; }
    public bool ValidationFailed { get; set; }
    public object ValidationReport { get; set; }
    public int Website { get; set; }
    public Metadata Metadata { get; set; }
    [JsonPropertyName("Objects")]
    public Listing[] Listings { get; set; }
    public Paging Paging { get; set; }
    public int TotaalAantalObjecten { get; set; }
}

public class Metadata
{
    public string ObjectType { get; set; }
    public string Omschrijving { get; set; }
    public string Titel { get; set; }
}

public class Paging
{
    public int AantalPaginas { get; set; }
    public int HuidigePagina { get; set; }
    public string VolgendeUrl { get; set; }
    public object VorigeUrl { get; set; }
}

public class Listing
{
    public string AangebodenSindsTekst { get; set; }
    public string AanmeldDatum { get; set; } // TODO: check later
    public object AantalBeschikbaar { get; set; }
    public int AantalKamers { get; set; }
    public object AantalKavels { get; set; }
    public string Aanvaarding { get; set; }
    public string Adres { get; set; }
    public int Afstand { get; set; }
    public string BronCode { get; set; }
    public object[] ChildrenObjects { get; set; }
    public object DatumAanvaarding { get; set; }
    public object DatumOndertekeningAkte { get; set; }
    public string Foto { get; set; }
    public string FotoLarge { get; set; }
    public string FotoLargest { get; set; }
    public string FotoMedium { get; set; }
    public string FotoSecure { get; set; }
    public object GewijzigdDatum { get; set; }
    public int GlobalId { get; set; }
    public string GroupByObjectType { get; set; }
    public bool Heeft360GradenFoto { get; set; }
    public bool HeeftBrochure { get; set; }
    public bool HeeftOpenhuizenTopper { get; set; }
    public bool HeeftOverbruggingsgrarantie { get; set; }
    public bool HeeftPlattegrond { get; set; }
    public bool HeeftTophuis { get; set; }
    public bool HeeftVeiling { get; set; }
    public bool HeeftVideo { get; set; }
    public object HuurPrijsTot { get; set; }
    public object Huurprijs { get; set; }
    public object HuurprijsFormaat { get; set; }
    public string Id { get; set; }
    public object InUnitsVanaf { get; set; }
    public bool IndProjectObjectType { get; set; }
    public object IndTransactieMakelaarTonen { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsVerhuurd { get; set; }
    public bool IsVerkocht { get; set; }
    public bool IsVerkochtOfVerhuurd { get; set; }
    public int? Koopprijs { get; set; }
    public string KoopprijsFormaat { get; set; }
    public int? KoopprijsTot { get; set; }
    public object Land { get; set; }
    public int MakelaarId { get; set; }
    public string MakelaarNaam { get; set; }
    public string MobileURL { get; set; }
    public object Note { get; set; }
    public int Oppervlakte { get; set; }
    public int? Perceeloppervlakte { get; set; }
    public string Postcode { get; set; }
    public string PrijsGeformatteerdHtml { get; set; }
    public string PrijsGeformatteerdTextHuur { get; set; }
    public string PrijsGeformatteerdTextKoop { get; set; }
    public string[] Producten { get; set; }

    public object ProjectNaam { get; set; }
    public string PublicatieDatum { get; set; } // TODO: check later
    public int PublicatieStatus { get; set; }
    public object SavedDate { get; set; }
    [JsonPropertyName("Soort-aanbod")]
    public string Soortaanbod { get; set; }
    public int SoortAanbod { get; set; }
    public object StartOplevering { get; set; }
    public object TimeAgoText { get; set; }
    public object TransactieAfmeldDatum { get; set; }
    public object TransactieMakelaarId { get; set; }
    public object TransactieMakelaarNaam { get; set; }
    public int TypeProject { get; set; }
    public string URL { get; set; }
    public string VerkoopStatus { get; set; }
    public float WGS84_X { get; set; }
    public float WGS84_Y { get; set; }
    public int WoonOppervlakteTot { get; set; }
    public int Woonoppervlakte { get; set; }
    public string Woonplaats { get; set; }
    public int[] ZoekType { get; set; }
}