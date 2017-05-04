using System;
using System.Diagnostics;
using System.Reflection;

namespace Augment.SqlServer.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{Name,nq}")]
    sealed class ParameterMap
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ParameterMap(int index, ParameterInfo pi)
        {
            Index = index;

            Parameter = pi;

            NormalizedName = pi.Name.ToLower();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ParameterInfo Parameter { get; private set; }

        /// <summary>
        /// Parameter.Name.ToLower()
        /// </summary>
        public string NormalizedName { get; private set; }

        /// <summary>
        /// Gets the Parameter Name
        /// </summary>
        public string Name { get { return Parameter.Name; } }

        /// <summary>
        /// 
        /// </summary>
        public Type Type { get { return Parameter.ParameterType; } }

        #endregion
    }
}