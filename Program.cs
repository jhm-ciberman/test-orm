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

            Dog dog1 = new Dog();
            dog1.Name = "Pepe el perro";
            dog1.Save();

            Dog dog2 = new Dog();
            dog2.Name = "Juancho el perro";
            dog2.Save();

            dog2.Name = "Juancho el perro copado";
            dog2.Save();

            new Person();
        }
    }
}
