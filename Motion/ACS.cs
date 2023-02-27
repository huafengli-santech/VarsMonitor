using ACS.SPiiPlusNET;
using MonitorApp.Logs;
using System;
using System.Net;

namespace MonitorApp
{
    public class ACS
    {
        // 定义一个静态变量来保存类的实例
        private static ACS s_Instance;

        // 定义私有构造函数，使外界不能创建该类实例
        private ACS()
        {
        }

        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static ACS GetInstance()
        {
            // 如果类的实例不存在则创建，否则直接返回
            if (s_Instance == null)
            {
                s_Instance = new ACS();
            }
            return s_Instance;
        }

        private static Api ch = new Api();

        public bool Connect(string ip)
        {
            ch.OpenCommEthernetTCP(ip, 701);
            return ch.IsConnected;
        }

        public bool IsConnected()
        {
            return ch.IsConnected;
        }

        public int GetAxisCount()
        {
            return (int)ch.GetAxesCount();
        }

        public void ChangeSize()
        {
            ch.Transaction("XARRSIZE=10000000");
        }

        public void DeclareAndWriteVariable(object points)
        {
            double[] pointPos = (double[])points;
            ch.DeclareVariable(AcsplVariableType.ACSC_STATIC_REAL_TYPE, $"Test({pointPos.Length})");
            ch.WriteVariable(points, "Test");
        }

        public void ReadVariableAndClear()
        {
            ch.ReadVariable("Test");
            ch.ClearVariables();
        }

        public void EnableFaultEvent()
        {
            ch.EnableEvent(Interrupts.ACSC_INTR_COMM_CHANNEL_CLOSED);
            ch.EnableEvent(Interrupts.ACSC_INTR_EMERGENCY);
            ch.EnableEvent(Interrupts.ACSC_INTR_SYSTEM_ERROR);
            ch.EnableEvent(Interrupts.ACSC_INTR_ETHERCAT_ERROR);
            ch.EnableEvent(Interrupts.ACSC_INTR_MOTION_FAILURE);
            ch.EnableEvent(Interrupts.ACSC_INTR_MOTOR_FAILURE);
            ch.COMMCHANNELCLOSED += Ch_COMMCHANNELCLOSED;
            ch.MOTORFAILURE += Ch_MOTORFAILURE;
            ch.MOTIONFAILURE += Ch_MOTIONFAILURE;
            ch.SYSTEMERROR += Ch_SYSTEMERROR;
            ch.ETHERCATERROR += Ch_ETHERCATERROR;
            ch.EMERGENCY += Ch_EMERGENCY;
        }

        /// <summary>
        /// 获取LOG日志
        /// </summary>
        /// <returns></returns>
        public string GetLOG()
        {
            return ch.Transaction("#LOG");
        }

        /// <summary>
        /// 获取MPU使用率百分比
        /// </summary>
        /// <returns></returns>
        public double GetUsege()
        {
            return (double)ch.ReadVariable("USAGE");
        }

        /// <summary>
        /// 获取控制器RAM百分比
        /// </summary>
        /// <returns></returns>
        public double GetRam()
        {
            return (double)ch.GetVolatileMemoryUsage();
        }

        /// <summary>
        /// 获取CPU/MPU温度
        /// </summary>
        /// <returns></returns>
        public double GetCpuTemperature()
        {
            return (double)ch.GetConf(76, 1);
        }

        /// <summary>
        /// 获取系统温度
        /// </summary>
        /// <returns></returns>
        public double GetSysTemperature()
        {
            return (double)ch.GetConf(76, 0);
        }

        public string[] GetGetEthernetCardsIP()
        {
            // 获取所有IP
            IPAddress[] ipAddresses = ch.GetEthernetCards(IPAddress.Broadcast);
            string[] address = new string[ipAddresses.Length];
            for (int index = 0; index < ipAddresses.Length; index++)
            {
                address[index] = ipAddresses[index].ToString();
            }
            return address;
        }

        private bool CompareTime(DateTime preTime, DateTime afterTime)
        {
            var pretime = preTime.Ticks / 600000000;
            var aftertime = afterTime.Ticks / 600000000;
            if (pretime <= (aftertime - 1))
            {
                return true;
            }
            return false;
        }

        private void Ch_EMERGENCY(ulong param)
        {
            Log.GetInstance().SaveLog($"[EStopError]  Error Message:{ch.GetErrorString((int)param)}", Log.ErrorLevel.Error);
        }

        private void Ch_ETHERCATERROR(ulong param)
        {
            Log.GetInstance().SaveLog($"[EtherCatError]  Error Message:{ch.GetErrorString((int)param)}", Log.ErrorLevel.Error);
        }

        private void Ch_SYSTEMERROR(ulong param)
        {
            //Log.GetInstance().SaveLog($"[SystemError]  Error Message:{ch.GetErrorString((int)param)}", Log.ErrorLevel.Error);
        }

        private void Ch_MOTIONFAILURE(AxisMasks axis)
        {
            for (int i = 0; i < ch.GetAxesCount(); i++)
            {
                if (((int)axis & (int)Math.Pow(2, i)) == Math.Pow(2, i))
                {
                    if (ch.GetMotionError((Axis)i) != 0)
                    {
                        int errorcode = ch.GetMotionError((Axis)i);
                        Log.GetInstance().SaveLog($"[MotionError] Axis:{i} Error Code:{errorcode} Error Message: {ch.GetErrorString(errorcode)}", Log.ErrorLevel.Error);
                    }
                }
            }
        }

        private void Ch_MOTORFAILURE(AxisMasks axis)
        {
            for (int i = 0; i < ch.GetAxesCount(); i++)
            {
                if (((int)axis & (int)Math.Pow(2, i)) == Math.Pow(2, i))
                {
                    int errorcode = ch.GetMotorError((Axis)i);
                    Log.GetInstance().SaveLog($"[MotorError] Axis:{i} Error Code:{errorcode} Error Message:{ch.GetErrorString(errorcode)}", Log.ErrorLevel.Error);
                }
            }
        }

        private void Ch_COMMCHANNELCLOSED(ulong param)
        {
            Log.GetInstance().SaveLog($"[CommError]  Error Message:{ch.GetErrorString((int)param)}", Log.ErrorLevel.Error);
        }
    }
}