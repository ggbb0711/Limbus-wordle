using Limbus_wordle.Interfaces;

namespace Limbus_wordle.Models
{
    public class DailyIdentityFile
    {
        public Identity TodayIdentity { get; set; }
        public Identity YesterdayIdentity { get; set; }
        public DateTime ResetTimer {get; set;} = DateTime.Today.AddDays(1);
    }
}