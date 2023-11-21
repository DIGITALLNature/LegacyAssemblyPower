// ReSharper disable once CheckNamespace
namespace D365.Extension.Core
{
    public interface IRandomService
    {
        /// <summary>
        /// get next andom value
        /// </summary>
        /// <param name="minValue">value</param>
        /// <param name="maxValue">value</param>
        /// <returns></returns>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// stop thread for a short random time period (123, 357) 
        /// </summary>
        void Sleep();

        /// <summary>
        /// stop thread for a short random time period
        /// </summary>
        /// <param name="minValue">milliseconds to sleep</param>
        /// <param name="maxValue">milliseconds to sleep</param>
        void Sleep(int minValue, int maxValue);
    }
}
