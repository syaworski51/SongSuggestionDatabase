using Microsoft.AspNetCore.SignalR;
using SongSuggestionDatabase.Data;

namespace SongSuggestionDatabase.Models
{
    public sealed class LiveDataHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public LiveDataHub(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task UpdateSongList()
        {
            await Clients.All.SendAsync("ReceiveUpdate");
        }
    }
}
