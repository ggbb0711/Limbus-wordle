using Limbus_wordle.Interfaces;

namespace Limbus_wordle.Models
{
    public class DailyIdentityFile
    {
        public String TodayID { get; set; }
        public Identity TodayIdentity { get; set; }
        public Identity YesterdayIdentity { get; set; }
    }
}