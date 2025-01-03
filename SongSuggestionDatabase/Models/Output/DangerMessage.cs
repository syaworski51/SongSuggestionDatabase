namespace SongSuggestionDatabase.Models.Output
{
    public class DangerMessage : Message
    {
        public DangerMessage(string body) : base("danger", body) { }
    }
}
