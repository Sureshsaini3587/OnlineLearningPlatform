using OnlineLearning.DTO;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IStudentRepository : ICommonRepository<StudentDTO>
    {
       Task<List<StudentDTO>> GetAllWithDetails();
       Task<List<GenderDTO>> GetGender();
    }
}
