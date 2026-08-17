using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseSubSectionRepository : ICourseSubSectionRepository
    {

        private readonly IConfiguration _config;
         
        public CourseSubSectionRepository(IConfiguration config)
        {
            _config = config; 
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(
                    _config.GetConnectionString("DbConnection"));
            }
        }

        public async Task<IEnumerable<SubSection>> GetAllAsync(int? sectionId = null)
        {
            using var db = Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@Action", "GETALL");
            parameters.Add("@SectionId", sectionId);

            var result = await db.QueryAsync<SubSection, CourseSection, SubSection>(
                "sp_ManageSubSection",
                (subSection, courseSection) =>
                {
                    subSection.CourseSection = courseSection;
                    return subSection;
                },
                parameters,
                splitOn: "SectionId",
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        // GET BY ID
        public async Task<SubSection> GetByIdAsync(int subSectionId)
        {
            using var db = Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@Action", "GETBYID");
            parameters.Add("@SubSectionId", subSectionId);

            return await db.QueryFirstOrDefaultAsync<SubSection>(
                "sp_ManageSubSection",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // INSERT / UPDATE (Upsert)
        public async Task<int> UpsertAsync(SubSection model)
        {
            using var db = Connection;
            var parameters = new DynamicParameters();

            string action = (model.SubSectionId == 0) ? "INSERT" : "UPDATE";

            parameters.Add("@Action", action);
            parameters.Add("@SubSectionId", model.SubSectionId);
            parameters.Add("@SubSectionTitle", model.SubSectionTitle);
            parameters.Add("@SortOrder", model.SortOrder);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@SectionId", model.SectionId);

            // Executes SP and returns the generated/affected ID
            var newId = await db.ExecuteScalarAsync<int>(
                "sp_ManageSubSection",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return newId == 0 ? model.SubSectionId : newId;
        }

        // DELETE
        public async Task<bool> DeleteAsync(int subSectionId)
        {
            using var db = Connection;
            var parameters = new DynamicParameters();
            parameters.Add("@Action", "DELETE");
            parameters.Add("@SubSectionId", subSectionId);

            var affectedRows = await db.ExecuteAsync(
                "sp_ManageSubSection",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return affectedRows > 0;
        }
    }
}