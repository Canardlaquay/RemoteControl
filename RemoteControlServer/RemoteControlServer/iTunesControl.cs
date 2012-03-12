using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;

namespace RemoteControlServer
{
    public class iTunesControl
    {
        iTunesApp itunes = new iTunesApp();
        public iTunesControl()
        {

        }

        public IITTrack getCurrentTrack
        {
            get { return itunes.CurrentTrack; }
            set { }
        }
        public void stopMusic()
        {
            itunes.Stop();
        }
        public void playMusic()
        {
                itunes.Play();
                itunes.Resume();
        }
        public void playFileMusic(string file)
        {
            itunes.PlayFile(file);
        }
        public void pauseMusic()
        {
            itunes.Pause();
        }
        public void playNextTrack()
        {
            itunes.NextTrack();
        }
        public void playPreviousTrack()
        {
            itunes.PreviousTrack();
        }
        public int volume
        {
            get { return itunes.SoundVolume; }
            set { itunes.SoundVolume += value; }
        }

    }
}
