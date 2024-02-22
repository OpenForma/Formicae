namespace Formicae
{
    static class Config
    {
        public enum FormicaeSubTab
        {
            Geometry,
            API,
            Show,
        }

        //Name for the GH tab
        public static string FormicaeTab { get => " Formicae"; }
        public static string Suffix { get => " Formicae"; }

        public static class Tabs
        {
            public static string Geometry { get => $"{(FormicaeSubTab)0}"; }
            public static string API { get => $"{(FormicaeSubTab)1}"; }
            public static string Show { get => $"{(FormicaeSubTab)2}"; }



        }
    }
}
