using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.ProtocolBuffers;
interface Packet
{
    void Write( CodedOutputStream os);
    void  Read(CodedInputStream ns);
}

