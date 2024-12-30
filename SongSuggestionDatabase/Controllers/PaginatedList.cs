using Microsoft.EntityFrameworkCore;

namespace SongSuggestionDatabase.Controllers
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }
        public bool HasNextPage
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }
        public int ItemsPerPage { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int itemsPerPage)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)itemsPerPage);
            AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int itemsPerPage)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, itemsPerPage);
        }
    }
}
