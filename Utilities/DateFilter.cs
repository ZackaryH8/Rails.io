using ParkSquare.RealTimeTrains;

namespace RailsIO.Utilities
{
    public static class DateFilterUtility
    {
        public static DateFilter Now => new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    }
}
