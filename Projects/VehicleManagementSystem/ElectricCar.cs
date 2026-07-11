using System;

public class ElectricCar : Car
{
    public int BatteryCapacity { get; private set; }
    public int CurrentBatteryPercentage { get; private set; }
    public bool ChargingPortOpen { get; private set; } = false;
    public int EstimatedRange { get; private set; }

    public ElectricCar(int vehicleId, string brand, string model, int year, string fuelType, int numberofDoors,
    string transmissionType, decimal trunkCapacity, bool hasSunRoof, int batteryCapacity, int currentBatteryPercentage, int estimatedRage) :
    base(vehicleId, brand, model, year, fuelType, numberofDoors, transmissionType, trunkCapacity, hasSunRoof)
    {
        BatteryCapacity = !(batteryCapacity <= 0) ? batteryCapacity : throw new ArgumentException(nameof(batteryCapacity));
        CurrentBatteryPercentage = !(currentBatteryPercentage < 0 || currentBatteryPercentage >100) ? currentBatteryPercentage : throw new ArgumentException(nameof(currentBatteryPercentage));
        EstimatedRange = (estimatedRage > 0) ? estimatedRage : throw new ArgumentException(nameof(estimatedRage));
    }

    public override void Start()
    {
        Console.WriteLine("Checking Battery...");
        Console.WriteLine("Starting Electric Car....");
        base.Start();
    }
    public override void DisplayVehicleDetails()
    {
        base.DisplayVehicleDetails();
        Console.WriteLine("----------------------------------------------Displaying Electric Car Details---------------------------------------------");
        Console.WriteLine($"Battery Capacity :{BatteryCapacity}");
        Console.WriteLine($"Current Battery Percentage :{CurrentBatteryPercentage}");
        Console.WriteLine($"Charging PortOpen : {ChargingPortOpen}");
        Console.WriteLine($"Estimated Range :{EstimatedRange}");
    }
    public void OpenChargingPort()
    {
        if (!IsStarted && !ChargingPortOpen)
            ChargingPortOpen = true;
    }
    public void CloseChargingPort()
    {
        if (!IsStarted && ChargingPortOpen)
            ChargingPortOpen = false;
    }
    public void ChargeBattery(int percentage)
    {
        if (!IsStarted && ChargingPortOpen)
        {
            if(percentage <=0)
            {
                throw new ArgumentException(nameof(percentage), "Percentage should be greater then zero");
            }
            CurrentBatteryPercentage = (CurrentBatteryPercentage + percentage <=100) ? CurrentBatteryPercentage + percentage : 100 ;
        }
    }
    public void DriveEcoMode()
    {
        if(IsStarted)
        {
            Console.WriteLine("Driving car in Eco mode");
        }
        else
        {
            throw new ArgumentException("Car should be started first before enabling the eco mode");
        }
    }

}