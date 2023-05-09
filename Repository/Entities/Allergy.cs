namespace Repository.Entities;

public class Allergy
{
    private string _name;
    private string _severity;
    public Allergy(string name, string severity)
    {
        this._name = name;
        this._severity = severity;
    }
}