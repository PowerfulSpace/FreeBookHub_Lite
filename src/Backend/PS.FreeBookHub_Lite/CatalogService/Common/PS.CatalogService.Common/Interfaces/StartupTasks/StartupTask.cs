namespace PS.CatalogService.Common.Interfaces.StartupTasks
{
    public abstract class StartupTask : IStartupTask
    {
        public virtual void Execute()
        {
            ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected virtual Task ExecuteAsync() => Task.CompletedTask;
    }
}
