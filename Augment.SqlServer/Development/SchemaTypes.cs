﻿using System;
using System.Diagnostics;
using System.Reflection;

namespace Augment.SqlServer.Development
{
    static class SchemaTypeExtensions
    {
        public static InvalidOperationException UnsupportedException(this SchemaTypes schemaType)
        {
            StackFrame frame = new StackFrame(1, false);
            MethodBase method = frame.GetMethod();
            Type type = method.DeclaringType;

            string msg = $"Unsupported Schema Type '{schemaType}' found '{type.Name}.{method.Name}'";

            return new InvalidOperationException(msg);
        }
    }

    /// <summary>
    /// The type of a database object
    /// </summary>
    /// <remarks>These are in the order that objects need to be created</remarks>
    public enum SchemaTypes
    {
        ///// <summary>
        ///// A user defined type
        ///// </summary>
        //UserDefinedType,
        /// <summary>
        /// A table in the database
        /// </summary>
        Table,
        /// <summary>
        /// 
        /// </summary>
        SystemScript,
        ///// <summary>
        ///// A default in the database
        ///// </summary>
        //Default,
        /// <summary>
        /// A primary key on a table
        /// </summary>
        PrimaryKey,
        /// <summary>
        /// A primary key on a table
        /// </summary>
        UniqueKey,
        ///// <summary>
        ///// Automatically generated Select, Insert, Update, Delete
        ///// </summary>
        //AutoProc,
        ///// <summary>
        ///// A constraint on a table
        ///// </summary>
        //Constraint,
        /// <summary>
        /// A foreign key constraint
        /// </summary>
        ForeignKey,
        ///// <summary>
        ///// A user defined function
        ///// </summary>
        //Function,
        /// <summary>
        /// An index
        /// </summary>
        Index,
        ///// <summary>
        ///// A view on one or more tables
        ///// </summary>
        //View,
        /// <summary>
        /// A stored procedure
        /// </summary>
        StoredProcedure,
        /// <summary>
        /// A table trigger
        /// </summary>
        Trigger,
        ///// <summary>
        ///// General padding things added by SQL Server scripter like SET ANSI NULLS ON
        ///// </summary>
        //Unused,
        ///// <summary>
        ///// 
        ///// </summary>
        //PostScript,
        /// <summary>
        /// An object that contains SQL that we don't support.
        /// </summary>
        Unsupported
    }
}
