namespace EMS.BaseLibrary.Entities;

public class Town:BaseEntity
{
    //Relationship : One to Many with Employee
    public List<Employee>? Employees { get; set; }
    
    //Many to one relationship with city
    public City? City { get; set; }
    public int CityId { get; set; }
}