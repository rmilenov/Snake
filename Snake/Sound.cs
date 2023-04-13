using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Resources;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace Snake
{
    public class Sound
    {
        public bool isPlaying = false;
        public static string audioPath = Assembly.GetExecutingAssembly().Location;
        MediaPlayer wowSound = new MediaPlayer(); //Initialize a new instance of MediaPlayer of name wowSound
        public void PlayBackgroundMusic()
        {
            isPlaying = true;
            wowSound.Open(new Uri("C:\\Users\\Radostin.Milenov\\Desktop\\Unterricht\\Programmierung Verteifung\\Snake\\Snake\\resources\\background.wav")); //Open the file for a media playback
            wowSound.Volume = 0.35;
            wowSound.Play(); //Play the media
        }
        public void PauseBackgroundMusic()
        {
            wowSound.Pause();
            isPlaying= false;
        }
        public void UnpauseBackgroundMusic()
        {
            isPlaying= true;
            wowSound.Pause();
        }
        public static readonly SoundPlayer collision = new SoundPlayer("C:\\Users\\Radostin.Milenov\\Desktop\\Unterricht\\Programmierung Verteifung\\Snake\\Snake\\resources\\crash.wav");
        public static readonly SoundPlayer ding = new SoundPlayer("C:\\Users\\Radostin.Milenov\\Desktop\\Unterricht\\Programmierung Verteifung\\Snake\\Snake\\resources\\ding.wav");
        
    }
}
