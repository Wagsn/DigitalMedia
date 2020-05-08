using System;
using System.Collections.Generic;
using System.Text;

namespace ImageSimilar
{
    /// <summary>
    /// 图像特征（哈希、直方图、SIFT、Gist、ResNet全连接）
    /// </summary>
    public class ImageFeature
    {
        /// <summary>
        /// 文件MD5
        /// </summary>
        public string MD5 { get; set; }
        /// <summary>
        /// 文件SHA256
        /// </summary>
        public string SHA256 { get; set; }
        /// <summary>
        /// 感知HASH值
        /// </summary>
        public string PerceptualHash { get; set; }
        /// <summary>
        /// 平均HASH值
        /// </summary>
        public string AverageHash { get; set; }
        /// <summary>
        /// 差异HASH值
        /// </summary>
        public string DifferentialHash { get; set; }
    }
}
