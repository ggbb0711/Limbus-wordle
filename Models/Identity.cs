using Limbus_wordle.Interfaces;

public class Identity:IEntity
{
    public string Name { get; set;} = "";
    public string Sinner { get; set;} = "";
    public string Icon { get; set;} = "";
    public List<Skill> Skills { get; set;} = [];
}