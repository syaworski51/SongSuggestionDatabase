namespace SongSuggestionDatabase.Models.Output
{
    public class WarningMessage : Message
    {
        public WarningMessage(string body) : base("warning", body) { }
    }
}
