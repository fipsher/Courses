using Quartz;

namespace LNU.Courses.PeriodicalJobMaker
{
    public interface IPeriodicalyJobMaker
    {
        void Start<TJob>(string dateString) where TJob : class, IJob;
        void ClearSchedule();
    }
}