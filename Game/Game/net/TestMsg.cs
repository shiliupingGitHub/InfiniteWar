using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.ProtocolBuffers;
public  partial class TestMsg : Packet
{
    public string test;
    public int test2;
    public void  Read(CodedInputStream os)
    {
        while(!os.IsAtEnd)
        {
            

            uint tag;
            string fieldName;
            
            if (os.ReadTag(out tag,out fieldName))
            {
                tag = tag >> 3;
                switch (tag)
                {
                    case 1:
                        os.ReadString(ref test);
                        break;
                    case 2:
                        os.ReadInt32(ref test2);
                        break;
                }

            }
        }
    }

    public void Write(CodedOutputStream ins)
    {
        ins.WriteString(1, string.Empty, test);
        ins.WriteInt32(2, string.Empty, test2);
    }
}

