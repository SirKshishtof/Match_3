
namespace Match_3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary> 
      
        //private static Gameplay _game;
        //private static MainForm _mainForm;
        [STAThread]
        static void Main()
        {
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());

        }

    }
}