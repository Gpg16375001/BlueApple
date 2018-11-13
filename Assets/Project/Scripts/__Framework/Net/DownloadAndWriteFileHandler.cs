using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class DownloadAndWriteFileHandler : DownloadHandlerScript
{
    private FileStream _stream;
    private int ContentSize;
    private int ReceiveDataLength;

    public bool Result;

    public DownloadAndWriteFileHandler(string path) : base()
    {
        Open (path);
        Result = false;
        ReceiveDataLength = 0;
        ContentSize = 0;
    }

    public DownloadAndWriteFileHandler(string path, byte[] buffer) : base(buffer)
    {
        Open (path);
        Result = false;
    }

    private void Open(string path)
    {
        if (File.Exists (path)) {
            File.Delete (path);
        }
        _stream = File.Open (path, FileMode.CreateNew);
    }

	private void Close()
	{
		if (_stream != null) {
			_stream.Close ();
            _stream.Dispose ();
		}
		_stream = null;
	}

	// Use this for initialization
    protected override void CompleteContent ()
    {
        Result = _stream != null && _stream.Length == ContentSize;
		Close ();
    }

    protected override byte[] GetData ()
    {
        return null;
    }

    protected override float GetProgress ()
    {
        return (float)ReceiveDataLength / (float)ContentSize;
    }

    protected override string GetText ()
    {
        return string.Empty;
    }

    protected override void ReceiveContentLength (int contentLength)
    {
        ContentSize = contentLength;
    }

    protected override bool ReceiveData (byte[] data, int dataLength)
    {
        bool writed = false;
        try {
			_stream.Write(data, 0, dataLength);
            writed = true;
            ReceiveDataLength += dataLength;
        } catch(System.Exception ex) {
			Close ();
            throw ex;
        }
            
        return writed;
    }

}
