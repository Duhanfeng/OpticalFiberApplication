using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DhfLib.Serialization;

namespace OpticalFiberApplication
{
    public class SystemParam
    {
        /// <summary>
        /// 串口号
        /// </summary>
        public string SerialPortName { get; set; }

        /// <summary>
        /// 基准通道
        /// </summary>
        public int DatumChannel { get; set; } = 0;

        /// <summary>
        /// 触发通道
        /// </summary>
        public int TriggerChannel { get; set; } = 7;

        /// <summary>
        /// 最大使能的数量
        /// </summary>
        public int EnableChannelCount { get; set; } = 7;

        /// <summary>
        /// 最大误差(mm)
        /// </summary>
        public double MaxError { get; set; } = 0.2;

        /// <summary>
        /// 比例转换参数
        /// </summary>
        public double ConverteScale { get; set; } = 0.1;

    }

    /// <summary>
    /// 系统参数管理器
    /// </summary>
    public class SystemParamManager
    {

        #region 单例模式

        /// <summary>
        /// 私有实例接口
        /// </summary>
        private static readonly SystemParamManager Instance = new SystemParamManager();

        /// <summary>
        /// 创建私有SceneManager新实例,保证外界无法通过new来创建新实例
        /// </summary>
        private SystemParamManager()
        {

        }

        /// <summary>
        /// 获取实例接口
        /// </summary>
        /// <returns></returns>
        public static SystemParamManager GetInstance()
        {

            return Instance;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 参数路径
        /// </summary>
        public readonly string ParamPath = "SystemParams/SystemConfig.json";

        /// <summary>
        /// 系统参数
        /// </summary>
        public SystemParam SystemParam { get; set; } = new SystemParam();

        #endregion

        #region 公共方法

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <returns>执行结果</returns>
        public bool LoadParams()
        {
            return LoadParams(ParamPath);
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns>执行结果</returns>
        public bool LoadParams(string file)
        {
            bool result = true;
            if (File.Exists(file))
            {
                SystemParam = JsonSerialization.DeserializeObjectFromFile<SystemParam>(file);
            }

            if (SystemParam == null)
            {
                SystemParam = new SystemParam();
                result = false;
            }

            //保存参数到默认配置文件
            JsonSerialization.SerializeObjectToFile(SystemParam, ParamPath);

            return result;
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public void SaveParams()
        {
            SaveParams(ParamPath);
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="file">文件路径</param>
        public void SaveParams(string file)
        {
            JsonSerialization.SerializeObjectToFile(SystemParam, file);
        }

        #endregion
    }
}
