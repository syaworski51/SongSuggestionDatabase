using System.Drawing;

namespace SongSuggestionDatabase.Models.Output
{
    public class Message
    {
        public string Type { get; set; }
        public string Body { get; set; }


        public Message(string type, string body)
        {
            Type = type;
            Body = body;
        }
    }
}
