using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.BusinessLogics.Repository;
using OnlineLearning.BusinessLogics.Services;
using OnlineLearning.Repositories;
using OnlineLearning.Views.Services;

namespace OnlineLearning.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<ICourseLevelRepository, CourseLevelRepository>();
            services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseSectionRepository, CourseSectionRepository>(); 
            services.AddScoped<ICourseSubSectionRepository,CourseSubSectionRepository>();
            services.AddScoped<ICourseVideoRepository, CourseVideoRepository>();
            services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IInstructorRepository, InstructorRepository>();
            services.AddScoped<IPQJQuestionRepository, PQJQuestionRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISubscriberRepository, SubscriberRepository>();
            services.AddTransient<IReadingQuestionRepository, ReadingQuestionRepository>();
            services.AddDataProtection();
            services.AddScoped<ProtectorService>();
        }
    }
}
