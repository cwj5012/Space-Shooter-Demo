using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using global::Google.Protobuf;

namespace ProtoMsg {
    public class Util {

        public static byte[] SerializeToByte(Message msg)
        {
            MemoryStream ms = new MemoryStream();
            msg.WriteTo(ms);
            byte[] bytes = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public static ProtoMsg.Message ParseFromByte(byte[] bytes)
        {
            ProtoMsg.Message msg = new ProtoMsg.Message();
            try {
                msg.MergeFrom(bytes);
            } catch (Exception e) {
                Debug.Log(e);
            }
            return msg;
        }
    }
}