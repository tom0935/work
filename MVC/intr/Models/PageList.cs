using System;

namespace IntranetSystem.Models
{
    public class PageList
    {
        public static string PageString(int total, int pageSize, int currentPage)
        {
            string page = string.Empty;
            int pageCount = (int)Math.Ceiling((double)total / (double)pageSize);

            page = "<ul class='pagination'>"
                + "<li><a href='?page=1'>&laquo;</a></li>";

            for (int i = 1; i <= pageCount; i++)
            {
                if (i == currentPage)
                    page += "<li class='active'><a href='?page=" + i + "'>" + i + "</a></li>";
                else
                    page += "<li><a href='?page=" + i + "'>" + i + "</a></li>";
            }
            page += "<li class='disabled'><a href='#'>(Total:" + total + ")</a></li>"
                +"<li><a href='?page=" + pageCount + "'>&raquo;</a></li>"
                + "</ul>";

            return page;
        }
    }
}