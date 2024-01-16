// See https://aka.ms/new-console-template for more information
using com.Instant.Mishor.Data;
using com.Instant.Mishor.Program;
using com.Instant.Mishor.Program.Enums;

Console.WriteLine("Start");

using (var dataContext = new DataContext())
{
    dataContext.Database.EnsureDeleted();
    dataContext.Database.EnsureCreated();
}

try
{
    var importer = new Importer();

    importer.Clear();

    importer.Run(ObjectType.Countries);
    importer.Run(ObjectType.Destinations);
    importer.Run(ObjectType.Hotels);
    importer.Run(ObjectType.HotelDestinations);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("Complete");

    //Console.ReadLine();
}