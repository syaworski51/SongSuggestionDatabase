namespace SongSuggestionDatabase.Models.Output
{
    public class SuccessMessage : Message
    {
        public SuccessMessage(string body) : base("success", body) { }
    }
}
