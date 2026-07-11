public class Program
{
    static void Main(string[] args)
    {
        Vehicle vehicle1 = new Car(1, "Toyota", "Camry", 2025, "Petrol", 4, "Automatic", 520, true);
        Vehicle vehicle2 = new Bike(2, "Royal Enfield", "Classic 350", 2025, "Petrol", 350, 12);
        Vehicle vehicle3 = new ElectricCar(3, "Tesla", "Model 3", 2025, "Electric", 4, "Automatic", 425, true, 75, 8, 520);

        List<Vehicle> vehicles = new List<Vehicle>();
        vehicles.Add(vehicle1);
        vehicles.Add(vehicle2);
        vehicles.Add(vehicle3);

        StartVehicle(vehicle1);
        StartVehicle(vehicle2);
        StartVehicle(vehicle3);

        // foreach (Vehicle vehicle in vehicles)
        // {
        //     if (vehicle is ElectricCar electricCar)
        //     {
        //         Console.WriteLine("This vehicle is an Electric Car");
        //         electricCar.Start();
        //         electricCar.DriveEcoMode();
        //     }
        //     else if (vehicle is Car car)
        //     {
        //         Console.WriteLine("This vehicle is a Car");

        //         car.Start();
        //         car.Honk();
        //     }
        //     else if (vehicle is Bike bike)
        //     {
        //         Console.WriteLine("This vehicle is a Bike");
        //         bike.Start();
        //         bike.Accelerate(30);
        //         bike.Wheelie();
        //     }

        // }
    }
    public static void StartVehicle(Vehicle vehicle)
    {
        // TODO
        vehicle.Start();
        vehicle.DisplayVehicleDetails();
    }
}