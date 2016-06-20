using Quartz;

namespace JobsMakers
{
    public interface IPeriodicallyJobMaker
    {
        void Start<TJob>(string period) where TJob : class, IJob;
    }
}