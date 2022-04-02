namespace TimeTracker.Apps.Modeles
{
    public class GlobalVariables
    {
        private static GlobalVariables _instance;
        public static GlobalVariables GetInstance()
        {
            if (_instance == null)
                _instance = new GlobalVariables();
            return _instance;
        }

        public Time GlobalTimer;
        public string AccessToken;
        public string RefreshToken;
    }
}