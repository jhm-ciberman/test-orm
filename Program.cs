using System;
using System.Reflection;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DB.CreateTable("Dog", "DogId");
            DB.CreateTable("Person", "PersonId");

            Person jorge = new Person();
            jorge.Name = "Jorge";
            jorge.Save();

            Dog dog1 = new Dog();
            dog1.Name = "Pepe el perro";
            dog1.OwnerId = jorge.PersonId;
            dog1.Save();

            Console.WriteLine("MY DOG IS: " + jorge.Dog.Name);

            Dog dog2 = new Dog();
            dog2.Name = "Juancho el perro";
            dog2.Save();

            dog2.Name = "Juancho el perro copado";
            dog2.Save();

            new Person();
        }
    }
}
