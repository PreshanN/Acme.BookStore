using System;

namespace Acme.BookStore.Authors;

public class AuthorExcelDto
{
    public string Name { get; set; }

    public DateTime BirthDate { get; set; }

    public string? ShortBio { get; set; }
}
