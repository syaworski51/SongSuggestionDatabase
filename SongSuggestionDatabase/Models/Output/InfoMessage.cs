namespace SongSuggestionDatabase.Models.Output
{
    public class InfoMessage : Message
    {
        public InfoMessage(string body) : base("info", body) { }
    }
}
