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
                // enumeration
                foreach (var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
                // return the first instance... can be used for your final query (executing query/filter)
                // .FirstOrDefault(); --executing method... (returns only a single ninja)

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
                // Retrieve a single ninja from the database. Pick the first ninja that comes out of the database. 
                var ninja = context.Ninjas.FirstOrDefault();
                // Chnage the boolean statement 
                ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);
                // save that change from line above
                context.SaveChanges();
            }
        }

        // For Disconnected apis or data
        private static void QueryAndUpdateNinjaDisconnected()
        {
            // Represents the object of a web API going to get a ninja
            Ninja ninja;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
            }

            // Alter the state of the object 
            ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);

            // Incorrect. Restating and saving the changes
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // context.Ninjas.Add(ninja);
                // unaware of the fact that there is a state of the ninja
                context.Ninjas.Attach(ninja);
                // letting the application know that there is a state to be updated
                context.Entry(ninja).State = EntityState.Modified;
                // All values will be passed back and saved
                context.SaveChanges();
            }
        }

        private static void RetrieveDataWithFind()
        {
            // A single value to be used in a query
            var keyval = 4;

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // Find query uses a single value to run through the data
                // Checks to see if the object is already there before finding it
                // Similar to the singleordefault execution LINQ.
                var ninja = context.Ninjas.Find(keyval);
                // It wants only to find one
                Console.WriteLine("After Find#1:" + ninja.Name);
                // wont rerun the query if the memory has stored the first query and still holds on to the object
                var someNinja = context.Ninjas.Find(keyval);

                Console.WriteLine("After Find #2:" + someNinja.Name);

                ninja = null;
            }
        }

        private static void RetrieveDataWithStoredProc()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // Schemas must match schema
                // execute stored procedures
                // Enumerated the method to run the query
                var ninjas = context.Ninjas.SqlQuery("exec GetOldNinjas");

                foreach (var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
            }
        }

        // Method to remove the oject
        private static void DeleteNinja()
        {
            // state the object
            Ninja ninja;
            // Retrive the data to be edited
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // Find the first instance of a ninja
                ninja = context.Ninjas.FirstOrDefault();
                //context.Ninjas.Remove(ninja);
                //context.SaveChanges();
            }
            // At this point it will throw an error since the conext is new and its unaware that one exists.
            // Entry method would be better than adding the method attach
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // Calls for the deletion of it. 
                /*context.Entry(ninja).State = EntityState.Deleted;*/
                context.Ninjas.Remove(ninja);
                // Then saves the changes
                context.SaveChanges();
            }
        }

        private static void DeleteNinjaViaStoredProcedure()
        {
            var keyval = 3;

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                context.Database.ExecuteSqlCommand(
                    "exec DeleteNinjaViaId {0}", keyval);
            }
        }

        // Attaching the object to the values 
        private static void InsertNinjaWithEquipment()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninja = new Ninja
                {
                    Name = "Fensu",
                    ServedInOniwaban = false,
                    DateOfBirth = new DateTime(2015, 5, 23),
                };
                var muscles = new NinjaEquipment
                {
                    Name = "Muscles",
                    Type = EquipmentType.Tool,
                };
                var spunk = new NinjaEquipment
                {
                    Name = "Spunk",
                    Type = EquipmentType.Weapon
                };

                context.Ninjas.Add(ninja);
                // Add the equipment to the ninja and not to the context. Entity framweork will track the adds
                // Hooking up objects that entity framework cant see
                ninja.EqupimentOwned.Add(muscles);
                ninja.EqupimentOwned.Add(spunk);
            }
        }

        // Loading Related Data
        private static void SimpleNinjaGraphQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // we would like for the equipment to also be brought along. Use an include and lambda expression
                // Eager loading
                var ninja = context.Ninjas.Include(n => n.EqupimentOwned)
                    .FirstOrDefault(n => n.Name.StartsWith("Fensu"));
                /*Explicit loading
                 * var ninja = context.Ninjas
                 *      .FirstOrDefault(n => n.Name.StartsWith("Fensu"));
                 * Console.WriteLine("Ninja Retrieved:" + ninja.Name);
                 * context.Entry(ninja).Collection(n => n.EquipmentOwned).Load();
                 * --- Another way to also load the collection
                 * Console.WriteLine("Ninja Equipment Count: {0}", ninja.EquipmentOwned.Count());
                 */

                /*Lazy loading mere mention of property entity framework will retrive it as long as virtual is added*/
            }
        }

        // Projection Query => returns something other than complete entities
        private static void ProjectionQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas
                    .Select(n => new { n.Name, n.DateOfBirth, n.EqupimentOwned })
                    .ToList();
            }
        }

    }
}
