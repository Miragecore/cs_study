using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Json_Util
{
    public class Json_FileHandler
    {   
        public JObject Load(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    StreamReader myFile = new StreamReader(filename);
                    string myString = myFile.ReadToEnd();
                    myFile.Close();

                    JObject jsonList = JObject.Parse(myString);

                    return jsonList;
                }
                catch (Exception) { }
            }
            return null;
        }

        public void Save(string filename,JObject jobj)
        {
            try
            {
                StreamWriter myFile = new StreamWriter(filename);
                string myString = jobj.ToString();
                myFile.Write(myString);
                myFile.Close();
            }
            catch (Exception) { }
        }
    }

    public class AutoPwrCtrlSetting
    {
        public string strTargetIp;


        JObject settings;
        public void Load()
        {
            Json_FileHandler jf = new Json_FileHandler();    
            settings = jf.Load("setting.json");
            if (settings == null)
            {
                strTargetIp = "127.0.0.1";
            }
            else
                strTargetIp = (string)settings["TargetIP"];
        }

        public void Save()
        {
            JObject jo = new JObject();
            jo.Add("TargetIP", strTargetIp);
            //string str = jo.ToString();

            Json_FileHandler jf = new Json_FileHandler();
            jf.Save("setting.json",jo);
        }

    }
}