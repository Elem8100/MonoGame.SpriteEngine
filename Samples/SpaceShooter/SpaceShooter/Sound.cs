using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using Un4seen.Bass;
using System.Runtime.InteropServices;
namespace SpaceShooter;

public class BassSound
{
    public static void Init()
    {
        Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
    }

      public static Dictionary<string, int> SoundList = new();
    public static void AddSound(string FileName)
    {
        int Stream = Bass.BASS_StreamCreateFile(FileName, 0L, 0L, BASSFlag.BASS_DEFAULT);
        SoundList.Add(Path.GetFileName(FileName), Stream);
    }
    public static void AddSounds(params string[] FileName)
    {
        foreach (var i in FileName)
            AddSound(i);
    }
    public static void LoadSounds(string Path)
    {
        DirectoryInfo Folder = new DirectoryInfo(Path);
        foreach (FileInfo File in Folder.GetFiles())
        {
            if (File.Extension == ".wav" || File.Extension == ".mp3")
            {
                AddSound(File.FullName);
            }
        }
    }
    public static void Play(string SoundName)
    {
        Bass.BASS_ChannelPlay(SoundList[SoundName], true);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct MidiOutCaps
{
    public UInt16 wMid;
    public UInt16 wPid;
    public UInt32 vDriverVersion;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public String szPname;
    public UInt16 wTechnology;
    public UInt16 wVoices;
    public UInt16 wNotes;
    public UInt16 wChannelMask;
    public UInt32 dwSupport;
    public static void Mci(string command)
    {
        int returnLength = 256;
        StringBuilder reply = new StringBuilder(returnLength);
        mciSendString(command, reply, returnLength, IntPtr.Zero);
    }

    [DllImport("winmm.dll")]
    private static extern long mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr winHandle);
}

public class MidiSound
{
    public static void Play(string FileName)
    {
        string openCommand = "open " + FileName + " alias music";
        MidiOutCaps.Mci(openCommand);
        MidiOutCaps.Mci("play music");
    }
    public static void RePlay()
    {
        MidiOutCaps.Mci("play music from 0");
    }
}