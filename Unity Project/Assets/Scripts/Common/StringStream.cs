using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Note:
 * String stream should never need to be trimmed. Always shaves whitespace.
 */
public class StringStream
{
    char[] data;
    int _pos = 0;
    public int Pos { get { return _pos; } }
    public int Available { get { return data.Length - _pos; } }
    public bool Done { get { return Available == 0; } }
    public static System.Text.Encoding ascii = System.Text.Encoding.ASCII;
    public StringStream(string data)
        : this(data.ToCharArray())
    {
    }

    public StringStream(char[] data)
    {
        this.data = data;
    }

    protected void SkipWhitespace()
    {
        while (_pos < data.Length)
        {
            if (!Char.IsWhiteSpace(data[_pos]))
            {
                return;
            }
            _pos++;
        }
    }

    public char ExtractChar()
    {
        SkipWhitespace();
        return data[_pos++];
    }

    public char GetChar()
    {
        SkipWhitespace();
        return data[_pos];
    }

    public void Skip(int amount)
    {
        _pos += amount;
    }

    public string ExtractBetween(char from, params char[] to)
    {
        ExtractUntil(true, from);
        string tmp = ExtractUntil(false, to);
        ExtractChar();
        return tmp;
    }

    public bool IsNext(string str)
    {
        if (Available >= str.Length)
        {
            string tmp = new string(data, _pos, str.Length);
            return str.Equals(tmp);
        }
        return false;
    }

    #region Extract Until
    public string ExtractUntil(bool including, char delim)
    {
        int initial_pos = _pos;
        while (_pos < data.Length)
        {
            if (data[_pos++] == delim)
            {
                if (!including)
                    _pos--;
                return new string(data, initial_pos, _pos - initial_pos).Trim();
            }
        }
        // No match
        return "";
    }

    public string ExtractUntil(bool including, params char[] delims)
    {
        int initial_pos = _pos;
        for (; _pos < data.Length; )
        {
            foreach (byte delim in delims)
            {
                if (data[_pos] == delim)
                {
                    if (including)
                        _pos++;
                    return new string(data, initial_pos, _pos - initial_pos).Trim();
                }
            }
            _pos++;
        }
        // No match
        return "";
    }
    
    public string ExtractUntil(Func<char, bool> func)
    {
        int initial_pos = _pos;
        while (_pos < data.Length)
        {
            if (func(data[_pos++]))
            {
                _pos--;
                return new string(data, initial_pos, _pos - initial_pos).Trim();
            }
        }
        // No match
        return "";
    }

    public string Extract(int amount)
    {
        int initialPos = _pos;
        _pos += amount;
        return new string(data, initialPos, amount).Trim();
    }

    public string ExtractUntil(string delim, bool including = false)
    {
        if (delim.Length == 0)
            return "";

        StringBuilder sb = new StringBuilder();
        sb.Append(ExtractUntil(false, delim[0]));
        while(!IsNext(delim) && !Done)
        {
            sb.Append(ExtractChar());
            sb.Append(ExtractUntil(false, delim[0]));
        }

        if (!Done && including)
            sb.Append(Extract(delim.Length));

        return sb.ToString();
    }

    public string ExtractUntilWhitespace()
    {
        return ExtractUntil(new Func<char, bool>((c) => {
            return Char.IsWhiteSpace(c);
        }));
    }
    #endregion
}
