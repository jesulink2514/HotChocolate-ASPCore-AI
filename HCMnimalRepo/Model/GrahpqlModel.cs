using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace HCMnimalRepo.Model
{
    public class AuthorType : ObjectType<Author>
    {
        protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
        {
            descriptor.Field(a => a.Id).Type<IdType>();
            descriptor.Field(a => a.Name).Type<StringType>();
            descriptor.Field(a => a.Surname).Type<StringType>();
        }
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class BookType : ObjectType<Book>
    {
        protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
        {
            descriptor.Field(b => b.Id).Type<IdType>();
            descriptor.Field(b => b.Title).Type<StringType>();
            descriptor.Field(b => b.Price).Type<DecimalType>();
        }
    }


    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }

    public class Query
    {
        public Query()
        {
        }
        [UsePaging(SchemaType = typeof(AuthorType))]
        public IQueryable<Author> Authors => new[]
        {
            new Author { Id = 1, Name = "Jesus", Surname = "Angulo" },
            new Author { Id = 2, Name = "Jesus", Surname = "Angulo" }
        }.AsQueryable();
    }
}
