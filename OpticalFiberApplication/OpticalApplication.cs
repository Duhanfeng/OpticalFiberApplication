using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAcquisitionModule
{
    /// <summary>
    /// 光纤应用
    /// </summary>
    class OpticalApplication
    {
        #region 通信

        /// <summary>
        /// 数据采集接口
        /// </summary>
        private DataAcquisition acquisition = new DataAcquisition();

        #endregion

        #region 配置参数

        /// <summary>
        /// 基准通道
        /// </summary>
        public int DatumChannel { get; set; } = 0;

        /// <summary>
        /// 触发通道
        /// </summary>
        public int TriggerChannel { get; set; } = 7;

        /// <summary>
        /// 最大误差(单位:mm)
        /// </summary>
        public double MaxError { get; set; } = 0.2;

        /// <summary>
        /// 比例参数(公式: mm = mV * ConverteScale)
        /// </summary>
        public double ConverteScale { get; set; } = 1;

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

        #region 应用
                
        /// <summary>
        /// 检测数据
        /// </summary>
        /// <param name="values">数据</param>
        /// <param name="datumData">基准数据</param>
        /// <param name="maxError">最大误差</param>
        /// <returns>结果</returns>
        private bool CheckError(double[] values, double datumData, double maxError)
        {
            foreach (var item in values)
            {
                double error = item - datumData;

                if (Math.Abs(error) > Math.Abs(maxError))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 工作线程
        /// </summary>
        private Thread thread;

        /// <summary>
        /// 停止标志位
        /// </summary>
        private bool shouldStop = false;

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            shouldStop = true;
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
            }
            else if (isOK == false)
            {
                acquisition.SetOutput(0, false);
                acquisition.SetOutput(1, true);
            }
            else
            {
                acquisition.SetOutput(0, false);
                acquisition.SetOutput(1, false);
            }

        }

        #endregion

    }
}
