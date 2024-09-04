namespace Limbus_wordle.Interfaces
{
    public interface IResponse<ResponseType>
    {
        ResponseType? Response { get; set;}
        string Msg { get; set; }
    }
}