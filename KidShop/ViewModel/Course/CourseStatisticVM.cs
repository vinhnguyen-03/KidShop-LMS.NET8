namespace KidShop.ViewModel.Course
{
    public class CourseStatisticVM
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int StudentCount { get; set; }       // Số học viên đăng ký
        public decimal TotalRevenue { get; set; }   // Doanh thu
        public Dictionary<int, decimal> MonthlyRevenue { get; set; } = new(); // key = tháng (1-12)
    }
}
