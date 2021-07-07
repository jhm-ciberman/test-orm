using System;

namespace Test
{
    public class Dog : Model
    {
        [PrimaryKey] public int DogId { get; set; }
        [Column] public int OwnerId { get; set; }
        [Column] public string Name { get; set; }
        [CreatedAt] public DateTime CreatedAt { get; set; }
        [UpdatedAt] public DateTime UpdatedAt { get; set; }

        public Relation<Person> OwnerRelation => this.HasOne<Person>("OwnerId", "DogId");
        public Dog()
        {
            //
        }
    }
}