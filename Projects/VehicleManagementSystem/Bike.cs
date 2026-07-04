using System;

public class Bike : Vehicle
{
    public int EngineCapacity{get;private set;}
    public bool HasSideStand{get;private set;} = true;
    public decimal HelmetStorageCapacity{get;private set;}
    public bool IsSideStandDown {get;private set;}
    public Bike(int vehicleId, string brand, string model, int year, string fuelType, int engineCapacity, decimal helmetStorageCapacity)
    : base(vehicleId,brand,model,year,fuelType)
    {
        EngineCapacity = engineCapacity>0 ? engineCapacity : throw new ArgumentOutOfRangeException(nameof(engineCapacity));
        HelmetStorageCapacity = helmetStorageCapacity>0 ? helmetStorageCapacity : throw new ArgumentOutOfRangeException(nameof(helmetStorageCapacity));
    }

    public void DisplayBikeDetails()
    {
        DisplayVehicleDetails();
        Console.WriteLine("------------------------Displaying Bike Details-------------------------");
        Console.WriteLine($"Engine Capacity :{EngineCapacity} cc");
        Console.WriteLine($"Has Side Stand : {HasSideStand}");
        Console.WriteLine($"Helmet Storage Capacity:{HelmetStorageCapacity}");
    }
    public void PutSideStandDown()
    {
        if (CurrentSpeed > 0)
        {
            throw new InvalidOperationException("Side stand cannot be put down while bike is running");
        }
        IsSideStandDown = !(IsSideStandDown) ? true : throw new InvalidOperationException("Side Stand is already down");
    }
    public void LiftSideStand()
    {
        IsSideStandDown = (IsSideStandDown) ? false : throw new InvalidOperationException("Side Stand is already up");
    }
    public void Wheelie()
    {
        if((IsStarted) && (CurrentSpeed >=25)){
            Console.WriteLine("We're performing Wheelie on our bike");
        }
        else
        {
            throw new InvalidOperationException("Wheelie cannot be performed if bike is not started or speed is less than 25km/h");
        }
    }
}