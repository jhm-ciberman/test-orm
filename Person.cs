using System;

namespace Test
{
    public class Person : Model<Person>
    {
        [PrimaryKey] public int PersonId { get; set; }
        [Column] public string Name { get; set; }
        [CreatedAt] public DateTime CreatedAt { get; set; }
        [UpdatedAt] public DateTime UpdatedAt { get; set; }
        [Relation] public HasOne<Dog> DogRelation() => this.HasOne<Dog>("OwnerId", "PersonId");
        public Dog Dog => this.One<Dog>("Dog");
        public Person()
        {
            //
        }
    }
}