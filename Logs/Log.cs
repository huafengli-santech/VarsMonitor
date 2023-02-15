using System;
using System.IO;

namespace MonitorApp.Logs
{
    public class Log
    {
        // 定义一个静态变量来保存类的实例
        private static Log s_Instance;

        // 定义私有构造函数，使外界不能创建该类实例
        private Log()
        {
        }

        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static Log GetInstance()
        {
            // 如果类的实例不存在则创建，否则直接返回
            if (s_Instance == null)
            {
                s_Instance = new Log();
            }
            return s_Instance;
        }

        public enum ErrorLevel
        { Error = 0, Warning = 1, Info = 2 };

        private ErrorLevel m_level = ErrorLevel.Info;
        private int m_faultConut = 0;
        private string filename = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.log");

        public void SaveLog(string message, ErrorLevel level)
        {
            m_level = level;
            if (m_level == ErrorLevel.Error)
            {
                SaveFile($"[{DateTime.Now}][Error]：\n{message}\n", filename); m_faultConut++;
            }
            else if (m_level == ErrorLevel.Warning)
            {
                SaveFile($"[{DateTime.Now}][Warning]：\n{message}\n", filename); m_faultConut++;
            }
            else if (m_level == ErrorLevel.Info)
                SaveFile($"[{DateTime.Now}][Info]：\n{message} \n", filename);
        }

        public int GetFaultCount()
        {
            return m_faultConut;
        }

        public void ClearFaultCount()
        {
            m_faultConut = 0;
        }

        private void SaveFile(string text, string fileName)
        {
            using (StreamWriter w = new StreamWriter(fileName, true))
            {
                w.WriteLine(text);
            }
        }
    }
}