using System;

public class Vehicle
{

    private const int MaxSpeed = 180;
    private const int MaxYear = 2026;
    private const int MinYear = 2025;
    public int VehicleId { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public string FuelType { get; private set; }
    public int CurrentSpeed { get; private set; }
    public bool IsStarted { get; private set; }

    public Vehicle(int vehicleId, string brand, string model, int year, string fuelType)
    {
        VehicleId = vehicleId <= 0 ? throw new ArgumentOutOfRangeException(nameof(vehicleId),"Vehicle Id cannot be zero or less than zero") : vehicleId;
        Brand = string.IsNullOrWhiteSpace(brand) ? throw new ArgumentNullException(nameof(brand),"Brand cannot be null or empty") : brand;
        Model = string.IsNullOrWhiteSpace(model) ? throw new ArgumentNullException(nameof(model),"model cannot be null or empty") : model;
        Year = !(year >= MinYear && year <= MaxYear) ? throw new ArgumentOutOfRangeException(nameof(year),"year should be within defined range") : year;
        FuelType = string.IsNullOrWhiteSpace(fuelType)
            ? throw new ArgumentNullException(nameof(fuelType))
            : fuelType;
    }
    public void Start()
    {

        //ch
        if (IsStarted)
        {
            throw new InvalidOperationException("Vehicle is already started");
        }
        else
        {
            IsStarted = true;
            Console.WriteLine("Vehicle is started");
        }
    }
    public void Stop()
    {
        if (CurrentSpeed > 0)
            throw new InvalidOperationException("Break Vehicle First");
        IsStarted = false;
        Console.WriteLine("Vehicle is stopped");
    }
    public void Accelerate(int amount)
    {
        if (!IsStarted) throw new InvalidOperationException("Start the Vehicle first");
        if (amount > 0)
        {
            if (amount + CurrentSpeed <= MaxSpeed)
            {
                CurrentSpeed += amount;
            }
            Console.WriteLine($"Current Speed of Vehicle: {CurrentSpeed}");
        }
        else
        {
            throw new ArgumentOutOfRangeException("Speed cannot be less than zero");
        }
    }
    public void Brake(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException("Speed to be reduced cannot be less than zero");
        }
        else
        {
            CurrentSpeed = CurrentSpeed - amount < 0 ? 0 : CurrentSpeed - amount;
            Console.WriteLine("Break applied on vehicle");
        }
    }
    public void DisplayVehicleDetails()
    {
        Console.WriteLine("-------------Displaying Vehicle Details");
        Console.WriteLine($"Vehicle Id : {VehicleId}");
        Console.WriteLine($"Brand : {Brand}");
        Console.WriteLine($"Model : {Model}");
        Console.WriteLine($"Year: {Year}");
        Console.WriteLine($"FuelType : {FuelType}");
        Console.WriteLine($"Vehichle is started : {IsStarted}");
        Console.WriteLine($"Vehichle current Speed : {CurrentSpeed}");
    }

}