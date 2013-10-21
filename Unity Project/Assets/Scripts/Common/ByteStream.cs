using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ByteStream
{
    byte[] data;
    int _pos = 0;
    public int Pos { get { return _pos; } }
    public int Available { get { return data.Length - _pos; } }
    public bool Done { get { return Available > 0; } }
    public static System.Text.Encoding ascii = System.Text.Encoding.ASCII;
    public ByteStream(string data)
        : this(System.Text.Encoding.ASCII.GetBytes(data))
    {
    }

    public ByteStream(byte[] data)
    {
        this.data = data;
    }

    public void SkipWhitespace()
    {
        while (_pos < data.Length)
        {
            if (!Char.IsWhiteSpace((char)data[_pos]))
            {
                return;
            }
            _pos++;
        }
    }

    public char ExtractChar()
    {
        return (char)data[_pos++];
    }

    public char GetChar()
    {
        return (char)data[_pos];
    }

    public void Skip(int amount)
    {
        _pos += amount;
    }

    #region Extract Until
    // Code is not reused in order to maintain
    // maximum speed for each specific version
    public string ExtractUntil(byte delim)
    {
        int initial_pos = _pos;
        while (_pos < data.Length)
        {
            if (data[_pos++] == delim)
            {
                _pos--;
                return ascii.GetString(data, initial_pos, _pos - initial_pos);
            }
        }
        // No match
        return "";
    }

    public string ExtractUntilTrim(char delim)
    {
        return ExtractUntilTrim((byte)delim);
    }

    public string ExtractUntilTrim(byte delim)
    {
        int begin_pos = -1;
        int end_pos = -1;
        byte b;
        while (_pos < data.Length)
        { // While in range
            b = data[_pos];
            if (b == delim)
            { // If delim found
                if (begin_pos != -1)
                { // If chars prior to delim were found
                    return ascii.GetString(data, begin_pos, end_pos - begin_pos + 1);
                }
                else
                {
                    return "";
                }
            }

            if (!Char.IsWhiteSpace((char)b))
            { // If whitespace
                if (begin_pos == -1)
                { // If begin_pos hasn't been set, set it
                    begin_pos = Pos;
                }
                // Update end pos
                end_pos = _pos;
            }
            _pos++;
        }
        // No match
        return "";
    }

    public string ExtractUntil(byte delim1, byte delim2)
    {
        int initial_pos = _pos;
        for (; _pos < data.Length; )
        {
            if (data[_pos] == delim1 || data[_pos] == delim2)
            {
                return ascii.GetString(data, initial_pos, _pos - initial_pos);
            }
            _pos++;
        }
        // No match
        return "";
    }

    public string ExtractUntilTrim(byte delim1, byte delim2)
    {
        int begin_pos = -1;
        int end_pos = -1;
        byte b;
        while (_pos < data.Length)
        { // While in range
            b = data[_pos];
            if (b == delim1 || b == delim2)
            { // If delim found
                if (begin_pos != -1)
                { // If chars prior to delim were found
                    return ascii.GetString(data, begin_pos, end_pos - begin_pos + 1);
                }
                else
                {
                    return "";
                }
            }

            if (!Char.IsWhiteSpace((char)b))
            { // If whitespace
                if (begin_pos == -1)
                { // If begin_pos hasn't been set, set it
                    begin_pos = Pos;
                }
                // Update end pos
                end_pos = _pos;
            }
            _pos++;
        }
        // No match
        return "";
    }

    public string ExtractUntil(params byte[] delims)
    {
        int initial_pos = _pos;
        for (; _pos < data.Length; )
        {
            foreach (byte delim in delims)
            {
                if (data[_pos] == delim)
                {
                    return ascii.GetString(data, initial_pos, _pos - initial_pos);
                }
            }
            _pos++;
        }
        // No match
        return "";
    }

    public string ExtractUntil(params char[] delims)
    {
        return ExtractUntil(Encoding.UTF8.GetBytes(delims));
    }

    public string ExtractUntil(char delim)
    {
        return ExtractUntil((byte)delim);
    }

    public string ExtractUntil(Func<byte, bool> func)
    {
        int initial_pos = _pos;
        while (_pos < data.Length)
        {
            if (func(data[_pos++]))
            {
                _pos--;
                return ascii.GetString(data, initial_pos, _pos - initial_pos);
            }
        }
        // No match
        return "";
    }

    public string ExtractUntilWhiteSpace()
    {
        return ExtractUntil(new Func<byte, bool>((b) => {
            return Char.IsWhiteSpace((char)b);
        }));
    }
    #endregion
}
