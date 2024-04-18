namespace WebApi.Services
{

    public interface ITrainingService
    {
        public Task RegisterClass();
        public Task DropClass();
        public Task GetClassGrade();
        public Task<int> GetClassCount();
       
    }

    public class TrainingService
    {
    }
}
