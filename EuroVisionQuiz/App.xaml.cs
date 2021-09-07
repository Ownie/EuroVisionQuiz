using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace EuroVisionQuiz
{
    public partial class App : Application
    {
        public App()
        {
            Device.SetFlags(new string[] { "Expander_Experimental" });

            InitializeComponent();

            LoadDatabase();

            MainPage = new NavigationPage(new Views.MainPageView());
        }

        private void LoadDatabase()
        {
            if (!DesignMode.IsDesignModeEnabled)
            {
                // LOAD DATABASE
                // FIND EMBEDDED DB
                string embeddedResourceString = "";
                var embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                for (int i = 0; i < embeddedResources.Length; i++)
                {
                    if (embeddedResources[i].Contains(Constants.DatabaseFilename))
                    {
                        embeddedResourceString = embeddedResources[i];
                        break;
                    }
                }
                // GET EMBEDDED DB FILESTREAM
                var embeddedResourceDbStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceString);

                // EXTRACT TO READABLE PATH
                var dbPath = Constants.DatabasePath;
                if (File.Exists(dbPath)) { File.Delete(dbPath); };
                {
                    using (var br = new BinaryReader(embeddedResourceDbStream))
                    {
                        using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                        {
                            var buffer = new byte[2048];
                            int len;
                            while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, len);
                            }
                        }
                    }
                }

                // READ DB DATA
                Globals.Database.LoadData();

                // DELETE DB FILE
                if (File.Exists(dbPath)) { File.Delete(dbPath); };
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}