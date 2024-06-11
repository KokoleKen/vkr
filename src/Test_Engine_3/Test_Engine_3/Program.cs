using System.Configuration;

namespace Test_Engine_3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            //Task.Run(() =>
            //{
            //    ApplicationConfiguration.Initialize();
            //    Application.Run(new Form1());
            //});

            using (Engine game = new Engine(100, 100, "3D Engine"))
            {
                game.self = game;
                game.Run();
            }
        }
    }
}