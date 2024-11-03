using System;
using UnityEngine;
using LitJson;

namespace Script.Tool
{
    public class VecJsonConverter
    {
        
        public void RegisterNewType()
        {
            #region vector2
            Action<Vector2, JsonWriter> writeVector2 = (v, w) => {
                w.WriteObjectStart();//开始写入对象
                w.WritePropertyName("x");
                w.Write(v.x.ToString());//写入值
                w.WritePropertyName("y");
                w.Write(v.y.ToString());//写入值
                w.WriteObjectEnd();
            };
         
            JsonMapper.RegisterExporter<Vector2>((v, w) => {
                writeVector2(v, w);
            });
         
            ImporterFunc<JsonReader , Vector2 > readVector2 = (reader) =>
            {
                
                if (reader.Read() && reader.Token == JsonToken.ObjectStart)
                {
                    float x = 0f, y = 0f;
         
                    // 读取对象的属性，直到遇到ObjectEnd
                    while (reader.Read())
                    {
                        if (reader.Token == JsonToken.PropertyName)
                        {
                            string propertyName = reader.Value.ToString();
                            if (reader.Read() && propertyName == "x")
                            {
                                if (float.TryParse(reader.Value.ToString(), out float parsedX))
                                {
                                    x = parsedX;
                                }
                                else
                                {
                                    throw new JsonException("Invalid 'x' value in Vector2.");
                                }
                            }
                            else if (reader.Read() && propertyName == "y")
                            {
                                if (float.TryParse(reader.Value.ToString(), out float parsedY))
                                {
                                    y = parsedY;
                                }
                                else
                                {
                                    throw new JsonException("Invalid 'y' value in Vector2.");
                                }
                            }
                        }
                        else if (reader.Token == JsonToken.ObjectEnd)
                        {
                            break; // 对象结束，跳出循环
                        }
                    }
         
                    return new Vector2(x, y);
                }
                else
                {
                    throw new JsonException("Cannot convert to Vector2! Expected an object.");
                }
            };
         
            JsonMapper.RegisterImporter(readVector2);
            
            #endregion
            
            #region Vector3
            Action<Vector3, JsonWriter> writeVector3 = (v, w) => {
                w.WriteObjectStart();//开始写入对象
         
                w.WritePropertyName("x");
                w.Write(v.x.ToString());//写入值
                w.WritePropertyName("y");
                w.Write(v.y.ToString());//写入值
                w.WritePropertyName("z");
                w.Write(v.z.ToString());//写入值
         
                w.WriteObjectEnd();
            };
         
            JsonMapper.RegisterExporter<Vector3>((v, w) => {
                writeVector2(v, w);
            });
         
            ImporterFunc<JsonReader, Vector3> readVector3 = (reader) =>
            {
               
                if (reader.Read() && reader.Token == JsonToken.ObjectStart)
                {
                    float x = 0f, y = 0f,z = 0f;
         
                    // 读取对象的属性，直到遇到ObjectEnd
                    while (reader.Read())
                    {
                        if (reader.Token == JsonToken.PropertyName)
                        {
                            string propertyName = reader.Value.ToString();
                            if (reader.Read() && propertyName == "x")
                            {
                                if (float.TryParse(reader.Value.ToString(), out float parsedX))
                                {
                                    x = parsedX;
                                }
                                else
                                {
                                    throw new JsonException("Invalid 'x' value in Vector3.");
                                }
                            }
                            else if (reader.Read() && propertyName == "y")
                            {
                                if (float.TryParse(reader.Value.ToString(), out float parsedY))
                                {
                                    y = parsedY;
                                }
                                else
                                {
                                    throw new JsonException("Invalid 'y' value in Vector3.");
                                }
                            }
                            else if (reader.Read() && propertyName == "z")
                            {
                                if (float.TryParse(reader.Value.ToString(), out float parsedZ))
                                {
                                    z = parsedZ;
                                }
                                else
                                {
                                    throw new JsonException("Invalid 'z' value in Vector3.");
                                }
                            }
                        }
                        else if (reader.Token == JsonToken.ObjectEnd)
                        {
                            break; // 对象结束，跳出循环
                        }
                    }
         
                    return new Vector3(x, y, z);
                }
                else
                {
                    throw new JsonException("Cannot convert to Vector3! Expected an object.");
                }
         
            };
         
            JsonMapper.RegisterImporter(readVector3);
         
            #endregion
         
            #region float
         
            JsonMapper.RegisterExporter<float>((v, w) => {
                
                w.Write(v.ToString());//写入值
            });
            JsonMapper.RegisterImporter<string , float>((str) =>
            {
                    // 尝试将字符串解析为float
                    if (float.TryParse(str, out float parsedFloat))
                    {
                        return parsedFloat;
                    }
                    else
                    {
                        throw new JsonException("Invalid float value.");
                    }
            });
            JsonMapper.RegisterImporter<JsonReader,float>((reader) =>
            {
                if (reader.Read() && reader.Token == JsonToken.String)
                {
                    string value = reader.Value.ToString();
                    // 尝试将字符串解析为float
                    if (float.TryParse(value, out float parsedFloat))
                    {
                        return parsedFloat;
                    }
                    else
                    {
                        throw new JsonException("Invalid float value.");
                    }
                }
                else
                {
                    throw new JsonException("Cannot convert to float! Expected a string.");
                }
         
            });
         
         
            #endregion
        }
    }
}