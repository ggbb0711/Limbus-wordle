using Limbus_wordle.Interfaces;

namespace Limbus_wordle.Models
{
    public class ResponseService<ResponseType> : IResponse<ResponseType>
    {
        public ResponseType? Response { get ; set ; }
        public string Msg { get ; set ; } = "";
    }
}