using Caliburn.Micro;
using DataAcquisitionModule;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpticalFiberApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mode = new MainWindowViewModel();
            DataContext = mode;
            mode.MessageRaised += Mode_MessageRaised;
        }
        /// <summary>
        /// 消息触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mode_MessageRaised(object sender, MessageRaisedEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        /// <summary>
        /// 按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
            }
        }

        /// <summary>
        /// 失去焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }

    }

    public class MainWindowViewModel : Screen
    {
        #region 初始化

        /// <summary>
        /// 数据采集类
        /// </summary>
        DataAcquisition acquisition = new DataAcquisition();

        /// <summary>
        /// 系统参数管理器
        /// </summary>
        SystemParamManager paramManager;

        public MainWindowViewModel()
        {
            
            
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        public void Loaded()
        {
            //更新串口
            SerialPorts = new ObservableCollection<string>(SerialPort.GetPortNames());

            paramManager = SystemParamManager.GetInstance();
            paramManager.LoadParams();

            if (SerialPorts.Contains(paramManager.SystemParam.SerialPortName))
            {
                SerialPortName = paramManager.SystemParam.SerialPortName;
            }
            else
            {
                if (SerialPorts.Count > 0)
                {
                    SerialPortName = SerialPorts[0];
                }
            }

            EnableChannelCount = paramManager.SystemParam.EnableChannelCount;
            DatumChannel = paramManager.SystemParam.DatumChannel;
            TriggerChannel = paramManager.SystemParam.TriggerChannel;
            MaxError = paramManager.SystemParam.MaxError;
            ConverteScale = paramManager.SystemParam.ConverteScale;

            //更新串口
            //SerialPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            //if (SerialPorts?.Count > 0)
            //{
            //    SerialPortName = SerialPorts[0];
            //}
        }

        #endregion

        #region 通信

        private ObservableCollection<string> serialPorts;

        /// <summary>
        /// 串口列表
        /// </summary>
        public ObservableCollection<string> SerialPorts
        {
            get
            {
                return serialPorts;
            }
            set
            {
                serialPorts = value;
                NotifyOfPropertyChange(() => SerialPorts);
            }
        }

        /// <summary>
        /// 选择的串口
        /// </summary>
        public string SerialPortName
        { 
            get
            {
                return acquisition.SerialPortName;
            }
            set
            {
                acquisition.SerialPortName = value;
                NotifyOfPropertyChange(() => SerialPortName);
                paramManager.SystemParam.SerialPortName = value;
                paramManager.SaveParams();
            }
        }

        #endregion

        #region 配置参数

        private int datumChannel = 0;

        /// <summary>
        /// 基准通道
        /// </summary>
        public int DatumChannel
        {
            get { return datumChannel; }
            set 
            {
                datumChannel = value;
                NotifyOfPropertyChange(() => DatumChannel);
                paramManager.SystemParam.DatumChannel = datumChannel; 
                paramManager.SaveParams();

                if (value >= EnableChannelCount)
                {
                    OnMessageRaised(MessageLevel.Err, "基准通道必须为有效的通道!");
                    //DatumChannel = EnableChannelCount - 1;
                }
            }
        }

        private int triggerChannel = 7;

        /// <summary>
        /// 触发通道
        /// </summary>
        public int TriggerChannel
        {
            get { return triggerChannel; }
            set { triggerChannel = value; NotifyOfPropertyChange(() => TriggerChannel);
                paramManager.SystemParam.TriggerChannel = value; paramManager.SaveParams(); }
        }

        private int enableChannelCount = 4;

        /// <summary>
        /// 使能通道的数量
        /// </summary>
        public int EnableChannelCount
        {
            get { return enableChannelCount; }
            set 
            { 
                enableChannelCount = value; 
                NotifyOfPropertyChange(() => EnableChannelCount);
                paramManager.SystemParam.EnableChannelCount = value;
                paramManager.SaveParams();

                //设置通道
                ChannelColor0 = (EnableChannelCount > 0) ? Brushes .Green: Brushes.LightGray;
                ChannelColor1 = (EnableChannelCount > 1) ? Brushes .Green: Brushes.LightGray;
                ChannelColor2 = (EnableChannelCount > 2) ? Brushes .Green: Brushes.LightGray;
                ChannelColor3 = (EnableChannelCount > 3) ? Brushes .Green: Brushes.LightGray;
                ChannelColor4 = (EnableChannelCount > 4) ? Brushes .Green: Brushes.LightGray;
                ChannelColor5 = (EnableChannelCount > 5) ? Brushes .Green: Brushes.LightGray;
                ChannelColor6 = (EnableChannelCount > 6) ? Brushes .Green: Brushes.LightGray;
                ChannelColor7 = (EnableChannelCount > 7) ? Brushes .Green: Brushes.LightGray;
            }
        }

        private double maxError = 1.0;

        /// <summary>
        /// 最大误差
        /// </summary>
        public double MaxError
        {
            get { return maxError; }
            set { maxError = value; NotifyOfPropertyChange(() => MaxError);
                paramManager.SystemParam.MaxError = value; paramManager.SaveParams(); }
        }

        private double converteScale = 0.1;

        /// <summary>
        /// 转换比例参数
        /// </summary>
        public double ConverteScale
        {
            get { return converteScale; }
            set { converteScale = value; NotifyOfPropertyChange(() => ConverteScale);
                paramManager.SystemParam.ConverteScale = value; paramManager.SaveParams(); }
        }

        #endregion

        #region 通道数据

        #region 通道使能(颜色属性)

        private SolidColorBrush channelColor0;

        /// <summary>
        /// 通道0
        /// </summary>
        public SolidColorBrush ChannelColor0
        {
            get { return channelColor0; }
            set { channelColor0 = value; NotifyOfPropertyChange(() => ChannelColor0); }
        }

        private SolidColorBrush channelColor1;

        /// <summary>
        /// 通道1
        /// </summary>
        public SolidColorBrush ChannelColor1
        {
            get { return channelColor1; }
            set { channelColor1 = value; NotifyOfPropertyChange(() => ChannelColor1); }
        }

        private SolidColorBrush channelColor2;

        /// <summary>
        /// 通道2
        /// </summary>
        public SolidColorBrush ChannelColor2
        {
            get { return channelColor2; }
            set { channelColor2 = value; NotifyOfPropertyChange(() => ChannelColor2); }
        }

        private SolidColorBrush channelColor3;

        /// <summary>
        /// 通道3
        /// </summary>
        public SolidColorBrush ChannelColor3
        {
            get { return channelColor3; }
            set { channelColor3 = value; NotifyOfPropertyChange(() => ChannelColor3); }
        }

        private SolidColorBrush channelColor4;

        /// <summary>
        /// 通道4
        /// </summary>
        public SolidColorBrush ChannelColor4
        {
            get { return channelColor4; }
            set { channelColor4 = value; NotifyOfPropertyChange(() => ChannelColor4); }
        }

        private SolidColorBrush channelColor5;

        /// <summary>
        /// 通道5
        /// </summary>
        public SolidColorBrush ChannelColor5
        {
            get { return channelColor5; }
            set { channelColor5 = value; NotifyOfPropertyChange(() => ChannelColor5); }
        }

        private SolidColorBrush channelColor6;

        /// <summary>
        /// 通道6
        /// </summary>
        public SolidColorBrush ChannelColor6
        {
            get { return channelColor6; }
            set { channelColor6 = value; NotifyOfPropertyChange(() => ChannelColor6); }
        }

        private SolidColorBrush channelColor7;

        /// <summary>
        /// 通道7
        /// </summary>
        public SolidColorBrush ChannelColor7
        {
            get { return channelColor7; }
            set { channelColor7 = value; NotifyOfPropertyChange(() => ChannelColor7); }
        }


        #endregion

        #region 电压值

        private double voltage0;

        /// <summary>
        /// 电压通道0
        /// </summary>
        public double Voltage0
        {
            get { return voltage0; }
            set { voltage0 = value; NotifyOfPropertyChange(() => Voltage0); }
        }

        private double voltage1;

        /// <summary>
        /// 电压通道1
        /// </summary>
        public double Voltage1
        {
            get { return voltage1; }
            set { voltage1 = value; NotifyOfPropertyChange(() => Voltage1); }
        }

        private double voltage2;

        /// <summary>
        /// 电压通道2
        /// </summary>
        public double Voltage2
        {
            get { return voltage2; }
            set { voltage2 = value; NotifyOfPropertyChange(() => Voltage2); }
        }

        private double voltage3;

        /// <summary>
        /// 电压通道3
        /// </summary>
        public double Voltage3
        {
            get { return voltage3; }
            set { voltage3 = value; NotifyOfPropertyChange(() => Voltage3); }
        }

        private double voltage4;

        /// <summary>
        /// 电压通道4
        /// </summary>
        public double Voltage4
        {
            get { return voltage4; }
            set { voltage4 = value; NotifyOfPropertyChange(() => Voltage4); }
        }

        private double voltage5;

        /// <summary>
        /// 电压通道5
        /// </summary>
        public double Voltage5
        {
            get { return voltage5; }
            set { voltage5 = value; NotifyOfPropertyChange(() => Voltage5); }
        }

        private double voltage6;

        /// <summary>
        /// 电压通道6
        /// </summary>
        public double Voltage6
        {
            get { return voltage6; }
            set { voltage6 = value; NotifyOfPropertyChange(() => Voltage6); }
        }

        private double voltage7;

        /// <summary>
        /// 电压通道7
        /// </summary>
        public double Voltage7
        {
            get { return voltage7; }
            set { voltage7 = value; NotifyOfPropertyChange(() => Voltage7); }
        }

        #endregion

        #region 距离值

        private double distance0;

        /// <summary>
        /// 距离0
        /// </summary>
        public double Distance0
        {
            get { return distance0; }
            set { distance0 = value; NotifyOfPropertyChange(() => Distance0); }
        }

        private double distance1;

        /// <summary>
        /// 距离1
        /// </summary>
        public double Distance1
        {
            get { return distance1; }
            set { distance1 = value; NotifyOfPropertyChange(() => Distance1); }
        }

        private double distance2;

        /// <summary>
        /// 距离2
        /// </summary>
        public double Distance2
        {
            get { return distance2; }
            set { distance2 = value; NotifyOfPropertyChange(() => Distance2); }
        }

        private double distance3;

        /// <summary>
        /// 距离3
        /// </summary>
        public double Distance3
        {
            get { return distance3; }
            set { distance3 = value; NotifyOfPropertyChange(() => Distance3); }
        }

        private double distance4;

        /// <summary>
        /// 距离4
        /// </summary>
        public double Distance4
        {
            get { return distance4; }
            set { distance4 = value; NotifyOfPropertyChange(() => Distance4); }
        }

        private double distance5;

        /// <summary>
        /// 距离5
        /// </summary>
        public double Distance5
        {
            get { return distance5; }
            set { distance5 = value; NotifyOfPropertyChange(() => Distance5); }
        }

        private double distance6;

        /// <summary>
        /// 距离6
        /// </summary>
        public double Distance6
        {
            get { return distance6; }
            set { distance6 = value; NotifyOfPropertyChange(() => Distance6); }
        }

        private double distance7;

        /// <summary>
        /// 距离7
        /// </summary>
        public double Distance7
        {
            get { return distance7; }
            set { distance7 = value; NotifyOfPropertyChange(() => Distance7); }
        }

        #endregion

        #region 差值

        private double error0;

        /// <summary>
        /// 误差值0
        /// </summary>
        public double Error0
        {
            get { return error0; }
            set { error0 = value; NotifyOfPropertyChange(() => Error0); }
        }

        private double error1;

        /// <summary>
        /// 误差值1
        /// </summary>
        public double Error1
        {
            get { return error1; }
            set { error1 = value; NotifyOfPropertyChange(() => Error1); }
        }

        private double error2;

        /// <summary>
        /// 误差值2
        /// </summary>
        public double Error2
        {
            get { return error2; }
            set { error2 = value; NotifyOfPropertyChange(() => Error2); }
        }

        private double error3;

        /// <summary>
        /// 误差值3
        /// </summary>
        public double Error3
        {
            get { return error3; }
            set { error3 = value; NotifyOfPropertyChange(() => Error3); }
        }

        private double error4;

        /// <summary>
        /// 误差值4
        /// </summary>
        public double Error4
        {
            get { return error4; }
            set { error4 = value; NotifyOfPropertyChange(() => Error4); }
        }

        private double error5;

        /// <summary>
        /// 误差值5
        /// </summary>
        public double Error5
        {
            get { return error5; }
            set { error5 = value; NotifyOfPropertyChange(() => Error5); }
        }

        private double error6;

        /// <summary>
        /// 误差值6
        /// </summary>
        public double Error6
        {
            get { return error6; }
            set { error6 = value; NotifyOfPropertyChange(() => Error6); }
        }

        private double error7;

        /// <summary>
        /// 误差值7
        /// </summary>
        public double Error7
        {
            get { return error7; }
            set { error7 = value; NotifyOfPropertyChange(() => Error7); }
        }

        #endregion

        #region IO颜色

        private SolidColorBrush okColor = Brushes.LightGray;

        /// <summary>
        /// OK状态栏颜色
        /// </summary>
        public SolidColorBrush OKColor
        {
            get { return okColor; }
            set { okColor = value; NotifyOfPropertyChange(() => OKColor); }
        }

        private SolidColorBrush ngColor = Brushes.LightGray;

        /// <summary>
        /// ng状态栏颜色
        /// </summary>
        public SolidColorBrush NGColor
        {
            get { return ngColor; }
            set { ngColor = value; NotifyOfPropertyChange(() => NGColor); }
        }

        #endregion

        #endregion

        #region 转换

        /// <summary>
        /// 电压转换为长度
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <returns>长度值</returns>
        private double VoltageConverteToLength(double voltage)
        {

            return voltage * ConverteScale;
        }

        /// <summary>
        /// 长度转换为电压
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <returns>长度值</returns>
        private double LengthConverteToVoltage(double length)
        {

            return length / ConverteScale;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 消息触发事件
        /// </summary>
        public event EventHandler<MessageRaisedEventArgs> MessageRaised;

        /// <summary>
        /// 触发消息触发事件
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected void OnMessageRaised(MessageLevel messageLevel, string message, Exception exception = null)
        {
            MessageRaised?.Invoke(this, new MessageRaisedEventArgs(messageLevel, message, exception));
        }

        #endregion

        #region 应用

        private bool isMesuring;

        /// <summary>
        /// 测量标志
        /// </summary>
        public bool IsMesurig
        {
            get { return isMesuring; }
            set { isMesuring = value; NotifyOfPropertyChange(() => IsMesurig); }
        }

        /// <summary>
        /// 停止标志位
        /// </summary>
        private bool _shouldStop = false;

        /// <summary>
        /// 检测数据
        /// </summary>
        /// <param name="voltages">电压数组(mV)</param>
        /// <returns>结果</returns>
        private bool CheckError(double[] voltages)
        {
            if (EnableChannelCount > voltages.Length)
            {
                throw new ArgumentOutOfRangeException("通道数量大于数组的长度");
            }
            if (DatumChannel > EnableChannelCount)
            {
                throw new ArgumentOutOfRangeException("基准通道大于有效的通道数");
            }

            //电压转距离
            double[] distances = voltages.ToList().ConvertAll(x => VoltageConverteToLength(x)).ToArray();

            //检测误差是否超限
            bool result = true;
            double[] error = new double[EnableChannelCount];
            for (int i = 0; i < EnableChannelCount; i++)
            {
                error[i] = distances[i] - distances[DatumChannel];

                if (Math.Abs(error[i]) > Math.Abs(MaxError))
                {
                    result = false;
                }
            }

            //显示结果
            Voltage0 = (EnableChannelCount > 0) ? voltages[0] / 1000.0 : -1;
            Voltage1 = (EnableChannelCount > 1) ? voltages[1] / 1000.0 : -1;
            Voltage2 = (EnableChannelCount > 2) ? voltages[2] / 1000.0 : -1;
            Voltage3 = (EnableChannelCount > 3) ? voltages[3] / 1000.0 : -1;
            Voltage4 = (EnableChannelCount > 4) ? voltages[4] / 1000.0 : -1;
            Voltage5 = (EnableChannelCount > 5) ? voltages[5] / 1000.0 : -1;
            Voltage6 = (EnableChannelCount > 6) ? voltages[6] / 1000.0 : -1;
            Voltage7 = (EnableChannelCount > 7) ? voltages[7] / 1000.0 : -1;

            Distance0 = (EnableChannelCount > 0) ? distances[0] : -1;
            Distance1 = (EnableChannelCount > 1) ? distances[1] : -1;
            Distance2 = (EnableChannelCount > 2) ? distances[2] : -1;
            Distance3 = (EnableChannelCount > 3) ? distances[3] : -1;
            Distance4 = (EnableChannelCount > 4) ? distances[4] : -1;
            Distance5 = (EnableChannelCount > 5) ? distances[5] : -1;
            Distance6 = (EnableChannelCount > 6) ? distances[6] : -1;
            Distance7 = (EnableChannelCount > 7) ? distances[7] : -1;

            Error0 = (EnableChannelCount > 0) ? error[0] : 0;
            Error1 = (EnableChannelCount > 1) ? error[1] : 0;
            Error2 = (EnableChannelCount > 2) ? error[2] : 0;
            Error3 = (EnableChannelCount > 3) ? error[3] : 0;
            Error4 = (EnableChannelCount > 4) ? error[4] : 0;
            Error5 = (EnableChannelCount > 5) ? error[5] : 0;
            Error6 = (EnableChannelCount > 6) ? error[6] : 0;
            Error7 = (EnableChannelCount > 7) ? error[7] : 0;

            //输出结果
            OutputResultSignal(result);

            return true;
        }

        /// <summary>
        /// 单次执行
        /// </summary>
        public void StartOneshot()
        {
            try
            {
                double[] analogs;
                acquisition.GetAllAnalogChannel(out analogs);
                if (analogs.Length == 8)
                {
                    CheckError(analogs);
                }

            }
            catch (Exception ex)
            {
                //触发报警信号
                OnMessageRaised( MessageLevel.Err, ex.Message);
            }
            
        }

        /// <summary>
        /// 开始触发采集
        /// </summary>
        public void StartTrigger()
        {
            if (IsMesurig)
            {
                return;
            }

            _shouldStop = false;

            //开线程
            new Thread(()=>
            {
                IsMesurig = true;

                while (!_shouldStop)
                {
                    double voltage = -1;
                    try
                    {
                        voltage = acquisition.GetAnalogChannel(TriggerChannel);
                        
                    }
                    catch (Exception ex)
                    {
                        //触发报警信号
                        OnMessageRaised(MessageLevel.Err, ex.Message);
                        break;
                    }
                    if (voltage > 5)
                    {
                        StartOneshot();
                    }
                }

                IsMesurig = false;
            }).Start();

        }

        /// <summary>
        /// 停止触发
        /// </summary>
        public void StopTrigger()
        {
            _shouldStop = true;
        }

        /// <summary>
        /// 输出结果信号
        /// </summary>
        /// <param name="isOK"></param>
        public void OutputResultSignal(bool? isOK)
        {
            if (isOK == true)
            {
                acquisition.SetOutput(0, true);
                acquisition.SetOutput(1, false);
                OKColor = Brushes.Green;
                NGColor = Brushes.LightGray;
            }
            else if (isOK == false)
            {
                acquisition.SetOutput(0, false);
                acquisition.SetOutput(1, true);
                OKColor = Brushes.LightGray;
                NGColor = Brushes.Red;
            }
            else
            {
                acquisition.SetOutput(0, false);
                acquisition.SetOutput(1, false);
                OKColor = Brushes.Green;
                NGColor = Brushes.Green;
            }

        }

        #endregion
    }
}
