using System;
using System.Collections.Generic;
using System.Text;

namespace SimilarPhoto
{
    /// <summary>
    /// 图像特征（哈希、直方图、SIFT、Gist、ResNet全连接）
    /// </summary>
    class ImageFeature
    {
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
