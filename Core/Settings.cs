namespace Core
{
    public class Settings
    {
        public required string EtiquetasDir { get; set; }
        public required string SqlConnection { get; set; }
        public string? RecallTemplate { get; set; }
    }
}
