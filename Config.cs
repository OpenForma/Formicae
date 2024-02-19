namespace Formicae
{
    static class Config
    {
        public enum FormicaeSubTab
        {
            Geometry,
            API,
        }

        //Name for the GH tab
        public static string FormicaeTab { get => " Formicae"; }
        public static string Suffix { get => " Formicae"; }

        public static class Tabs
        {
            public static string Geometry { get => $"{(FormicaeSubTab)0}"; }
            public static string API { get => $"{(FormicaeSubTab)1}"; }


        }
    }
}
