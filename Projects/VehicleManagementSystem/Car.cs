using System;

public class Car : Vehicle
{
    public int NumberOfDoors{get;private set;}
    public string TransmissionType{get;private set; }
    public bool HasSunRoof{get;private set; }
    public decimal TrunkCapacity{get;private set; }
    public  bool IsTrunkOpen{get;private set;}

    public Car(int vehicleId, string brand, string model, int year, string fuelType,int numberofDoors, string transmissionType, decimal trunkCapacity, bool hasSunRoof) : base(vehicleId,brand,model,year,fuelType)
    {
        NumberOfDoors = (numberofDoors >0) ? numberofDoors : throw new ArgumentOutOfRangeException(nameof(numberofDoors));  
        TransmissionType = !(string.IsNullOrWhiteSpace(transmissionType)) ? transmissionType : throw new ArgumentNullException(nameof(transmissionType));
        TrunkCapacity = (trunkCapacity>0) ? trunkCapacity : throw new ArgumentOutOfRangeException(nameof(trunkCapacity));   
        HasSunRoof = hasSunRoof;

    }
    public void DisplayCarDetails()
    {
        DisplayVehicleDetails(); 

        Console.WriteLine("------------------------------Car Details-----------------------------");
        Console.WriteLine($"Number of doors i car : {NumberOfDoors}");
        Console.WriteLine($" Transmission Type: {TransmissionType}");
        Console.WriteLine($"Car Has Sun Roof : {HasSunRoof}");
        Console.WriteLine($"Trunk Capacity of car : {TrunkCapacity}");

    }
    public void OpenTrunk()
    {
        if(CurrentSpeed > 0)
        throw new InvalidOperationException($"Trunk cannot be opened as car is runing at : {CurrentSpeed} km/h");
        IsTrunkOpen = IsTrunkOpen == false ? true :  throw new InvalidOperationException("Trunk is already opened");
    }
    public void CloseTrunk()
    {
        IsTrunkOpen = IsTrunkOpen==true ? false : throw new InvalidOperationException("Trunk is already closed");
    }
    public void Honk()
    {
        if (IsStarted)
        {
            Console.WriteLine("Beep Beep");
        }
        else
        {
            throw new InvalidOperationException("Honk Cannot work when engine is OFF");
        }
    }

}