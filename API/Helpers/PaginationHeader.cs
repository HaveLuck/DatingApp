using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int _curentPage, int _itemsPerPage, int _totalItems, int _totalPages)
        {
            currentPage = _curentPage;
            itemsPerPage = _itemsPerPage;
            totalItems = _totalItems;
            totalPages = _totalPages;
        }

        public int currentPage { get; set; }
        public int itemsPerPage { get; set; }
        public int totalItems { get; set; }
        public int totalPages { get; set; }
    }
}