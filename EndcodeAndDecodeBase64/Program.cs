using System.Text;

string EncodeBase64(string str)
{
    byte[] inputs = Encoding.Default.GetBytes(str);
    int lengthInputs = inputs.Length;
    int groupCount = lengthInputs / 3; // Số nhóm (mỗi nhóm có 4 sextet)
    int paddingCount = 0; // Số ký tự đệm

    // Mỗi phần tử trong mảng inputs đại diện cho 8 bit
    // Cần chuyển đổi 6 bit một
    // 8 và 6 có BCNN là 24
    if ((lengthInputs % 3) > 0) // 24/8 = 3 
    {
        paddingCount = 3 - (lengthInputs % 3);
        groupCount += 1;
    }

    int lengthBitConverts = lengthInputs + paddingCount;
    byte[] bitConverts = new byte[lengthBitConverts]; // Mảng chứa các bit sẽ chuyển đổi
    inputs.CopyTo(bitConverts, 0);
    for (int i = lengthInputs; i < lengthBitConverts; i++)
        bitConverts[i] = 0;

    byte sextet1, sextet2, sextet3, sextet4;
    byte temp, temp1, temp2, temp3;
    byte[] sextets = new byte[groupCount * 4];
    char[] result = new char[groupCount * 4];

    for (int i = 0; i < groupCount; i++)
    {
        temp1 = bitConverts[i * 3];
        temp2 = bitConverts[i * 3 + 1];
        temp3 = bitConverts[i * 3 + 2];

        sextet1 = (byte)(temp1 >> 2); // Lấy 6 bit đầu
        
        temp = (byte)((temp1 & 3) << 4); // Lấy 2 bit cuối của temp1. 3: 0011
        sextet2 = (byte)(temp2 >> 4); // Lấy 4 bit đầu của temp2
        sextet2 += temp;

        temp = (byte)((temp2 & 15) << 2); // Lấy 4 bit cuối của temp2. 15: 1111
        sextet3 = (byte)(temp3 >> 6); // Lấy 2 bit đầu của temp3
        sextet3 += temp;

        sextet4 = (byte)(temp3 & 63); // Lấy 6 bit cuối của temp3. 63: 0011 1111

        sextets[i * 4] = sextet1;
        sextets[i * 4 + 1] = sextet2;
        sextets[i * 4 + 2] = sextet3;
        sextets[i * 4 + 3] = sextet4;
    }

    for (int i = 0; i < groupCount * 4; i++)
        result[i] = convertBitToCharBase64(sextets[i]);

    switch (paddingCount)
    {
        case 1:
            result[groupCount * 4 - 1] = '=';
            break;
        case 2:
            result[groupCount * 4 - 1] = '=';
            result[groupCount * 4 - 2] = '=';
            break;
        default:
            break;
    }

    return new string(result);
}

string DecodeBase64(string str)
{
    int lengthStr = str.Length;
    byte[] inputs = new byte[lengthStr];
    for(int i = 0; i < lengthStr; i++)
        inputs[i] = convertCharBase64ToBit(str[i]);

    int groupCount = lengthStr / 4; // 4 sextet

    if ((lengthStr % 4) > 0)
        groupCount += 1;

    byte sextet1, sextet2, sextet3, sextet4;
    byte temp, temp1, temp2, temp3;
    byte[] tmps = new byte[groupCount * 3];
    int paddingCount = str.Length - str.Replace("=", "").Length;
    char[] result = new char[groupCount * 3 - paddingCount];

    for (int i = 0; i < groupCount; i++)
    {
        sextet1 = inputs[i * 4];
        sextet2 = inputs[i * 4 + 1];
        sextet3 = inputs[i * 4 + 2];
        sextet4 = inputs[i * 4 + 3];

        temp1 = (byte)(sextet1 << 2); // Lấy 6 bit cuối
        temp = (byte)(sextet2 >> 4); // Lấy 2 bit đầu của sextet2
        temp1 += temp;

        temp2 = (byte)(sextet2 << 4); // Lấy 4 bit cuối của sextet2
        temp = (byte)(sextet3 >> 2); // Lấy 4 bit đầu của sextet3
        temp2 += temp;

        temp3 = (byte)(sextet3 << 6); // Lấy 2 bit cuối của sextet3
        temp3 += sextet4;

        tmps[i * 3] = temp1;
        tmps[i * 3 + 1] = temp2;
        tmps[i * 3 + 2] = temp3;
    }

    for (int i = 0; i < result.Length; i++)
        result[i] = (char)tmps[i];

    return new string(result);
}

static char convertBitToCharBase64(byte b)
{
    char[] charBase64 = new char[64] {
        'A','B','C','D','E','F','G','H','I','J','K','L','M',
        'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m',
        'n','o','p','q','r','s','t','u','v','w','x','y','z',
        '0','1','2','3','4','5','6','7','8','9','+','/'
    };

    if ((b >= 0) && (b <= 63))
        return charBase64[(int)b];
    return ' ';
}

static byte convertCharBase64ToBit(char c)
{
    char[] charBase64 = new char[64] {
        'A','B','C','D','E','F','G','H','I','J','K','L','M',
        'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m',
        'n','o','p','q','r','s','t','u','v','w','x','y','z',
        '0','1','2','3','4','5','6','7','8','9','+','/'
    };

    if (c.Equals('='))
        return 0;

    int index = Array.FindIndex(charBase64, item => item.Equals(c));
    return (byte)index;
}


string inputString = "softmart";
Console.WriteLine("Chuoi ban dau:");
Console.WriteLine(inputString);

string encodeStr = EncodeBase64(inputString);
Console.WriteLine("\nChuoi base64:");
Console.WriteLine(encodeStr);

string decodeStr = DecodeBase64(encodeStr);
Console.WriteLine("\nChuoi giai ma:");
Console.WriteLine(decodeStr);
