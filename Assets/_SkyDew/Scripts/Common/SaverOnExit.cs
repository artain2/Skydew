using AppBootstrap.Runtime;

namespace _SkyDew.Scripts.Common
{
    [Injectable]
    public class SaverOnExit : IApplicationQuitSub
    {
        [Inject] private IDataSaver _saver;
        public void AtApplicationQuit()
        {
            _saver.Save();
        }
    }
}