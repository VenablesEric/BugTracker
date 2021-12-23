namespace BugTracker.Models
{
    public class DashModel
    {
        public PieChartVM priority { get; set; }
        public PieChartVM status { get; set; }
        public PieChartVM type { get; set; }
        public int count { get; set; }
    }
}
