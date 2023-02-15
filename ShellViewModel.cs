using Caliburn.Micro;
using LiveCharts;
using LiveCharts.Configurations;
using MonitorApp.Dates;
using MonitorApp.Logs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MonitorApp
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private ObservableCollection<string> _ip;

        public ObservableCollection<string> IP
        {
            get { return _ip; }
            set { _ip = value; NotifyOfPropertyChange(() => IP); }
        }

        private string _selecIp;

        public string SelecIp
        {
            get { return _selecIp; }
            set { _selecIp = value; NotifyOfPropertyChange(() => SelecIp); }
        }

        //ͼ��
        /// <summary>
        /// ʱ���ʽ����
        /// </summary>
        public Func<double, string> DateTimeFormatter { get; set; }

        public ChartValues<MeasureModel> RamChartValues { get; set; }
        public ChartValues<MeasureModel> UsageChartValues { get; set; }
        public ChartValues<MeasureModel> CtempChartValues { get; set; }
        public ChartValues<MeasureModel> StempChartValues { get; set; }

        private ObservableCollection<int> _rate;
        private string _selecRate;

        public string SelecRate
        {
            get { return _selecRate; }
            set { _selecRate = value; NotifyOfPropertyChange(() => SelecRate); }
        }

        public ObservableCollection<int> Rate
        {
            get { return _rate; }
            set { _rate = value; NotifyOfPropertyChange(() => Rate); }
        }

        private string _logs;

        public string Logs
        {
            get { return _logs; }
            set { _logs = value; NotifyOfPropertyChange(() => Logs); }
        }

        private int _faultCount;

        public int FaultCount
        {
            get { return _faultCount; }
            set { _faultCount = value; NotifyOfPropertyChange(() => FaultCount); }
        }

        private Boolean _islog;
        private Boolean _isRam;
        private Boolean _isUsage;
        private Boolean _isStemp;
        private Boolean _isCtemp;

        public Boolean IsCtemp
        {
            get { return _isCtemp; }
            set { _isCtemp = value; NotifyOfPropertyChange(() => IsCtemp); }
        }

        public Boolean IsStemp
        {
            get { return _isStemp; }
            set { _isStemp = value; NotifyOfPropertyChange(() => IsStemp); }
        }

        public Boolean IsUsage
        {
            get { return _isUsage; }
            set { _isUsage = value; NotifyOfPropertyChange(() => IsUsage); }
        }

        public Boolean IsRam
        {
            get { return _isRam; }
            set { _isRam = value; NotifyOfPropertyChange(() => IsRam); }
        }

        public Boolean IsLog
        {
            get { return _islog; }
            set { _islog = value; NotifyOfPropertyChange(() => IsLog); }
        }

        public ShellViewModel()
        {
            //�����߳��쳣
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            UpdateUI();

            IP = new ObservableCollection<string>();
            Rate = new ObservableCollection<int>
            {
                10,//10s
                30,//30s
                60,//1m
                600//10m
            };
            SelecRate = "10";

            //ͼ�����
            // ��ʼ������һ�� X Y ����������ʾ��ͼ����ȷ�� X Y ������ݽṹ
            var mapper = Mappers.Xy<MeasureModel>()
              .X(model => model.DateTime.Ticks)
              .Y(model => model.Value);

            // �������ͼ�����Ա������ط����ض��ķ�ʽ��ʹ��
            Charting.For<MeasureModel>(mapper);

            // ��ʼ�����������ݼ�
            RamChartValues = new ChartValues<MeasureModel>();
            UsageChartValues = new ChartValues<MeasureModel>();
            CtempChartValues = new ChartValues<MeasureModel>();
            StempChartValues = new ChartValues<MeasureModel>();
            IsRam = true; IsUsage = true; IsStemp = true; IsCtemp = true; IsLog = true;

            // ���� X ����ʾ�ı�ǩ��ʽ
            DateTimeFormatter = value => new DateTime((long)value).ToString("hh:mm:ss");
            UpdateCharts();
        }

        private void UpdateCharts()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(Convert.ToInt32(SelecRate) * 1000);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (ACS.GetInstance().IsConnected())
                        {
                            UpdateChart(ACS.GetInstance().GetRam(), ACS.GetInstance().GetUsege(), ACS.GetInstance().GetCpuTemperature(), ACS.GetInstance().GetSysTemperature());
                        }
                    });
                }
            });
        }

        private void UpdateUI()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //AxisStep = TimeSpan.FromSeconds(Convert.ToInt32(SelecRate)).Ticks;
                        if (Log.GetInstance().GetFaultCount() != 0)
                        {
                            FaultCount++;
                            Logs = $"{DateTime.Now}��⵽�жϱ�����鿴LOG��־";
                        }
                    });
                }
            });
        }

        private void UpdateChart(double ram, double usage, double ctemp, double stemp)
        {
            if (IsLog)
                UpdateLog(ACS.GetInstance().GetLOG(), Log.ErrorLevel.Info);
            //test
            if (IsRam)
            {
                RamChartValues.Add(new MeasureModel
                {
                    DateTime = DateTime.Now,
                    Value = ram
                });
                UpdateLog($"�ڴ�:{ACS.GetInstance().GetRam()}", Log.ErrorLevel.Info);
            }

            if (IsUsage)
            {
                UsageChartValues.Add(new MeasureModel
                {
                    DateTime = DateTime.Now,
                    Value = usage
                });
                UpdateLog($"ʹ����:{ACS.GetInstance().GetUsege()}", Log.ErrorLevel.Info);
            }

            if (IsCtemp)
            {
                CtempChartValues.Add(new MeasureModel
                {
                    DateTime = DateTime.Now,
                    Value = ctemp
                });
                UpdateLog($"CPU�¶�:{ACS.GetInstance().GetCpuTemperature()}", Log.ErrorLevel.Info);
            }

            if (IsStemp)
            {
                StempChartValues.Add(new MeasureModel
                {
                    DateTime = DateTime.Now,
                    Value = stemp
                });
                UpdateLog($"ϵͳ�¶�:{ACS.GetInstance().GetSysTemperature()}", Log.ErrorLevel.Info);
            }
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Current_DispatcherUnhandledException:{e.Exception}");
            e.Handled = true;  // ���Ϊ ���Ѵ����������쳣��һ�����ݶ��������
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"CurrentDomain_UnhandledException: {e.ExceptionObject}");
        }

        public void btnRefresh()
        {
            string[] ips = ACS.GetInstance().GetGetEthernetCardsIP();
            foreach (string ip in ips)
            {
                IP.Add(ip);
            }
            if (IP != null && IP.Count > 0)
                SelecIp = IP[0];
        }

        public void btnConnect()
        {
            ACS.GetInstance().Connect(SelecIp);
            UpdateLog("���ӳɹ�", Log.ErrorLevel.Info);
        }

        public void btnEnableFault()
        {
            ACS.GetInstance().EnableFaultEvent();
            UpdateLog("ʹ���жϳɹ�", Log.ErrorLevel.Info);
        }

        public void ClearFaultFunc()
        {
            FaultCount = 0;
            Log.GetInstance().ClearFaultCount();
        }

        public void OpenLogFunc()
        {
            string path = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.log");
            System.Diagnostics.Process.Start("notepad.exe", path);
        }

        private void UpdateLog(string message, Log.ErrorLevel level)
        {
            Logs = $"[{DateTime.Now}] {message}";
            Log.GetInstance().SaveLog(message, level);
        }
    }
}