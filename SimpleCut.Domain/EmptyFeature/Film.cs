using Dapper.Contrib.Extensions;

namespace SimpleCut.Domain.EmptyFeature
{
    [Table("films")]
    public class Film
    {
        [ExplicitKey]
        public string Code { get; set; }
        public string Title { get; set; }
        public int Did { get; set; }
        public string Kind { get; set; }
        public DateTime DateProd { get; set; }

    }
}
