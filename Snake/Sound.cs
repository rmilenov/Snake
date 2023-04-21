using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Media;


namespace Snake
{
    public class Sound
    {
        public static readonly SoundPlayer collision = new SoundPlayer(global::Snake.resources.Resource2.crash);
        public static readonly SoundPlayer ding = new SoundPlayer(global::Snake.resources.Resource3.ding);
        MediaPlayer background = new MediaPlayer(); //Initialize a new instance of MediaPlayer of name wowSound
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public bool isPlaying = false;
        public static string executableFilePath = Assembly.GetExecutingAssembly().Location;
        public static string executableDirectoryPath = Path.GetDirectoryName(executableFilePath);
        public static string audioFilePath = Path.Combine(executableDirectoryPath, "resources\\bg_music_1.mp3");

        public void PlayBackgroundMusic()
        {
            isPlaying = true;
            background.MediaEnded += new EventHandler(Media_Ended);// Loop Music
            background.Open(new Uri(audioFilePath)); //Open the file for a media playback
            background.Volume = 0.35;
            background.Play(); //Play the media           

        }

        private void Media_Ended(object sender, EventArgs e)
        {
            background.Position = TimeSpan.Zero;
            background.Play();
        }

        public void PauseBackgroundMusic()
        {
            background.Pause();
            isPlaying = false;
        }
        public void UnpauseBackgroundMusic()
        {
            isPlaying = true;
            background.Pause();
        }

    }
}
