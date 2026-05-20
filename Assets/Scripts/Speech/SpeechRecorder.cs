using System.Collections;
using System.IO;
using UnityEngine;
using System;

public class SpeechRecorder : MonoBehaviour
{
    public static SpeechRecorder Instance { get; private set; }

    private string _deviceName;
    private AudioClip _recording;
    private bool _isRecording = false;
    private float _startTime;

    public bool IsRecording => _isRecording;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (Microphone.devices.Length > 0)
        {
            _deviceName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    public void StartRecording()
    {
        if (_deviceName == null) return;
        
        _recording = Microphone.Start(_deviceName, false, 10, 16000); // 16kHz is good for Whisper
        _isRecording = true;
        _startTime = Time.time;
        Debug.Log("Recording started...");
    }

    public string StopRecording()
    {
        if (!_isRecording) return null;

        int position = Microphone.GetPosition(_deviceName);
        Microphone.End(_deviceName);
        _isRecording = false;

        if (position == 0) return null;

        // Trim the clip to actual recorded length
        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", position, _recording.channels, _recording.frequency, false);
        float[] data = new float[position * _recording.channels];
        _recording.GetData(data, 0);
        trimmedClip.SetData(data, 0);

        string filePath = Path.Combine(Application.persistentDataPath, "speech.wav");
        SaveAsWav(trimmedClip, filePath);
        
        Debug.Log($"Recording saved to {filePath}");
        return filePath;
    }

    private void SaveAsWav(AudioClip clip, string filePath)
    {
        byte[] wavData = WavUtility.FromAudioClip(clip);
        File.WriteAllBytes(filePath, wavData);
    }
}

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                var samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);

                writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + samples.Length * 2);
                writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)clip.channels);
                writer.Write(clip.frequency);
                writer.Write(clip.frequency * clip.channels * 2);
                writer.Write((short)(clip.channels * 2));
                writer.Write((short)16);
                writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * 32767));
                }
            }
            return stream.ToArray();
        }
    }
}
