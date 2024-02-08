using System.Text.Json.Serialization;

public class PessoaModel
{
    [JsonPropertyName("Id")]
    public int Id { get; set; }
    [JsonPropertyName("Name")]
    public string? Name { get; set; }
    [JsonPropertyName("Email")]
    public string? Email { get; set; }
}