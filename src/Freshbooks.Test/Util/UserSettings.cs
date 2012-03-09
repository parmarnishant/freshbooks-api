namespace Freshbooks.Test.Util
{
    class UserSettings
    {
        private static Properties.Settings _settings;
        private static Properties.Settings Settings
        {
            get
            {
                if (_settings != null)
                    return _settings;

                if (Properties.Settings.Default.ShowSettings)
                {
                    using(SettingsForm dlg = new SettingsForm())
                        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            dlg.Settings.Save();
                            _settings = dlg.Settings;
                        }
                }

                _settings = Properties.Settings.Default;
                return _settings;
            }
        }


        public static string FreshbooksAccountName { get { return Settings.FreshbooksAccountName; } }
        public static string UserToken { get { return Settings.UserToken; } }

        public static string ConsumerKey { get { return Settings.ConsumerKey; } }
        public static string OAuthSecret { get { return Settings.OAuthSecret; } }

        public static int HttpCallbackPort { get { return Settings.HttpCallbackPort; } }
    }
}
