using Assets.TabletopUi.Scripts.Infrastructure.Modding;

namespace GreatWork.Utils
{
    public static class MiscUtils
    {

        public static string GetLongOrNormalDescription(this Mod m)
        {
            return m.DescriptionLong ?? m.Description;
        }
        
    }
}