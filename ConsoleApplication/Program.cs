using System;
using System.Data.Entity;
using NinjaDomain.Classes;
using NinjaDomain.DataModel;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());
            InsertNinja();
            //InsertMultipleNinjas();
            //SimpleNinjaQuery();
            //SimpleNinjaGraphQuery();
            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();
            //DeleteNinja();
            Console.ReadKey();
        }

        private static void InsertNinja()
        {
            var ninja = new Ninja
            {

                Name = "Mikosa",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1980, 1, 1),
                ClanId = 1
            };
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }
        }
    }
}
