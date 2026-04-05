using OnlineLearning.DTO;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IInstructorRepository : ICommonRepository<InstructorDTO>
    {
       Task<List<InstructorDTO>> GetAllWithDetails(); 
    }
}
