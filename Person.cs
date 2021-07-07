using System;

namespace Test
{
    public class Person : Model
    {
        [PrimaryKey] public int PersonId { get; set; }
        [Column] public string Name { get; set; }
        [CreatedAt] public DateTime CreatedAt { get; set; }
        [UpdatedAt] public DateTime UpdatedAt { get; set; }
        public Relation<Dog> DogRelation => this.HasOne<Dog>("OwnerId", "PersonId");
        public Person()
        {
            //
        }
    }
}