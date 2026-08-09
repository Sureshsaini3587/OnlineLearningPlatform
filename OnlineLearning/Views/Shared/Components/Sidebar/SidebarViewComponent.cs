using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using System.Security.Claims;

namespace OnlineLearning.Views.Shared.Components.Sidebar
{
    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        { 
            var menuItems = GetSidebarMenu(UserClaimsPrincipal);
            return View(menuItems);
        }

        private List<NavigationItem> GetSidebarMenu(ClaimsPrincipal user)
        {
            var menu = new List<NavigationItem>(); 
            if (user.IsInRole("Student"))
            { 
                menu.Add(new NavigationItem { Title = "Dashboard", Controller = "Student", Action = "Dashboard", Icon = "bi bi-grid-1x2-fill" });
                menu.Add(new NavigationItem { Title = "My Courses", Controller = "Student", Action = "MyCourses", Icon = "bi bi-collection-play-fill" });
                menu.Add(new NavigationItem { Title = "Subscription", Controller = "Student", Action = "Subscription", Icon = "bi bi-gem" });
                menu.Add(new NavigationItem { Title = "PQJ Practice", Controller = "Student", Action = "PQJ", Icon = "bi bi-ui-checks-grid" });
                menu.Add(new NavigationItem { Title = "Profile", Controller = "Student", Action = "Profile", Icon = "bi bi-person-circle" });
                menu.Add(new NavigationItem { Title = "ReadingMode", Controller = "Student", Action = "ReadingMode", Icon = "bi bi-person-circle" });
                menu.Add(new NavigationItem { Title = "Logout", Controller = "Account", Action = "Logout", Icon = "bi bi-box-arrow-right" }); 
            } 
           
            if (user.IsInRole("Admin"))
            {
                menu = GetAdminMenu();
            } 

            return menu;
        }
        private List<NavigationItem> GetAdminMenu()
        {
            var menu = new List<NavigationItem>
             {
                 new NavigationItem { Title = "Dashboard", Controller = "Home", Action = "Index", Icon = "bi bi-house-door-fill" },
                 new NavigationItem { Title = "Instructors", Controller = "Instructor", Action = "Index", Icon = "bi bi-person-badge" },
                 new NavigationItem { Title = "PQJ Questions", Controller = "PQJQuestion", Action = "Index", Icon = "bi bi-question-diamond" },
                 new NavigationItem { Title = "Subscription Plans", Controller = "Subscription", Action = "Index", Icon = "bi bi-gem" },
                 new NavigationItem { Title = "Students", Controller = "Student", Action = "Index", Icon = "bi bi-people-fill" },
               
                 new NavigationItem { Title = "Courses", Icon = "bi bi-book-half", SubItems = new List<NavigationItem> {
                     new NavigationItem { Title = "Course List", Controller = "Course", Action = "Index" }, 
                     new NavigationItem { Title = "Course Categories", Controller = "Course", Action = "Categories" },
                     new NavigationItem { Title = "Course Section", Controller = "CourseSection", Action = "Index" },
                     new NavigationItem { Title = "Course Videos", Controller = "CourseVideo", Action = "Index" },
                     new NavigationItem { Title = "Course Plan Mapping", Controller = "PlanCourse", Action = "Index" }
                 }},

                 new NavigationItem { Title = "Study Material", Controller = "StudyMaterial", Action = "Index", Icon = "bi bi-people-fill" },
             };

            return menu;
        }
    }
}
