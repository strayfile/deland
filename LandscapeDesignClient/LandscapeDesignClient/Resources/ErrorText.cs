namespace LandscapeDesignClient.Resources
{
    internal static class Texts {
        public static string Text(int i)
        {
            string err = ErrorsAndMessages.ResourceManager.GetString($"e_{i}", App.Language);
            if (err != null)
                return err;
            return "Unknown error!";
        }
    }
}
