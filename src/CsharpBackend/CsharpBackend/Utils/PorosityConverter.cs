using CsharpBackend.Config;

namespace CsharpBackend.Utils
{
    public abstract class PorosityConverter
    {
        protected static PoreClasses _poreClassses;
        protected static PoreColors _poreColors;
        protected static Dictionary<int, byte[]> IndexColorMap;

        public static void Init(PoreClasses poreClasses, PoreColors poreColors)
        {
            _poreClassses = poreClasses;
            _poreColors = poreColors;

            IndexColorMap = new();

            foreach (var poreClass in poreClasses.Classes)
            {
                IndexColorMap[poreClass.Index] = poreColors.GetBGR(poreClass.Color);
            }
        }
    }
}
