using KidShop.Models;

namespace KidShop.ViewModel.Course
{
    public class CourseDetailVM
    {
        public tbl_Course Course { get; set; }
        public tbl_Lecturer? Lecturer { get; set; }
        public tbl_CourseVideo? FirstVideo { get; set; }
        public List<tbl_CourseVideo> Videos { get; set; }
        public List<tbl_Quiz> Quizzes { get; set; } = new();
        public int TotalVideos { get; set; }
        
    }
}
