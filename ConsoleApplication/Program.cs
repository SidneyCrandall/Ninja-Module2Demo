using System;
using System.Data.Entity;
using System.Linq;
using NinjaDomain.Classes;
using NinjaDomain.DataModel;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Line of code that will stop the database from initailizing. Primarily meant for production. 
            // Initialization may be needed throughout development of the application. 
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());
            InsertNinja();
            //InsertMultipleNinjas();
            //SimpleNinjaQuery();
            //SimpleNinjaGraphQuery();
            QueryAndUpdateNinja();
            QueryAndUpdateNinjaDisconnected();
            //DeleteNinja();
            Console.ReadKey();
        }

        // Insert one singular ninja into the table
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
                // Can substitute AddRange which will take an IEnumberable (can pass in a list instead of 1 object)
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }
        }

        // Running a query against the database
        private static void SimpleNinjaQueries()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninjas = context.Ninjas.
                    // Lambdic expression... use variable
                    // variable = parameterize the varible in the LINQ query
                    Where(n => n.Name == "Mikosan");
                    // return the first instance... can be used for your final query (executing query/filter)
                    // .FirstOrDefault(); --executing method... (returns only a single ninja)
                
                // enumeration
                foreach (var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
                // LINQ Execution method to query against the database
                // var ninjas = context.Ninjas.ToList();
                // -- ToList() LINQ method that is similar to the above statemnt
                // var query = context.Ninjas;
                // -- Execute that query 
                // var someNinjas = query.ToList();
                //-- Triggering a query to execute. Enumerate the variable. Can hold the connection open
                // foreach (var ninja in query)
                //{
                //    Console.WriteLine(ninja.Name);
                //}
            }
        }

        private static void QueryAndUpdateNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // Pick the first ninja that comes out of the database.
                var ninja = context.Ninjas.FirstOrDefault();
                // Chnage the boolean statement 
                ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);

                context.SaveChanges();
            }
        }

        private static void QueryAndUpdateNinjaDisconnected()
        {
            // Represents the object of a web API going to get a ninja
            Ninja ninja;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
            }

            ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);

            // Incorrect. Restating and saving the changes
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.Add(ninja);
                context.Ninjas.Attach(ninja);
                context.Entry(ninja).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
