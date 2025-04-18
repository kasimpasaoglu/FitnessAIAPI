public class LogModel : ILogEntry
{
    public string Date { get; set; } = null!;
    public string Method { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string? QueryString { get; set; }
    public Dictionary<string, string>? RouteValues { get; set; }
    public string IP { get; set; } = null!;
    public int ResponseStatusCode { get; set; }
    public long? ProcessDuration { get; set; }
    public string? Action { get; set; }
    public object? Payload { get; set; }

    // public BsonDocument ToBsonDocument()
    // {
    //     var doc = new BsonDocument
    //     {
    //         { "Date", Date },
    //         { "Method", Method },
    //         { "Path", Path },
    //         { "IP", IP },
    //         { "ResponseStatusCode", ResponseStatusCode },
    //         { "ProcessDuration", ProcessDuration },
    //         { "Action", Action != null ? Action : BsonNull.Value },
    //         {
    //             "Payload",
    //             Payload switch
    //             {
    //                 null => BsonNull.Value,
    //                 string str => BsonValue.Create(str), // string ise doğrudan değer olarak ekle
    //                 _ => BsonDocument.Create(Payload)    // object ise nested document olarak ekle
    //             }
    //         },
    //         { "QueryString", QueryString != null ? QueryString : BsonNull.Value }
    //     };

    //     if (RouteValues != null && RouteValues.Count > 0)
    //     {
    //         var routeDoc = new BsonDocument();
    //         foreach (var kvp in RouteValues)
    //         {
    //             routeDoc.Add(kvp.Key, kvp.Value ?? string.Empty);
    //         }

    //         doc.Add("RouteValues", routeDoc);
    //     }

    //     return doc;
    // }
}
