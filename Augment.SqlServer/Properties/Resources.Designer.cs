﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Augment.SqlServer.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Augment.SqlServer.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select	c.name								as [Name],
        ///		case
        ///			when c.is_computed = 1 then &apos;as &apos; + cc.definition
        ///			else tp.name +
        ///			case
        ///				when tp.name in (&apos;varchar&apos;, &apos;char&apos;, &apos;varbinary&apos;, &apos;binary&apos;) then &apos;(&apos; + case when c.max_length = -1 then &apos;max&apos; else cast(c.max_length as varchar) end + &apos;)&apos;
        ///				when tp.name in (&apos;nvarchar&apos;, &apos;nchar&apos;) then &apos;(&apos; + case when c.max_length = -1 then &apos;max&apos; else cast(c.max_length/2 as varchar) end + &apos;)&apos;
        ///				when tp.name in (&apos;datetime2&apos;, &apos;time1&apos;, &apos;datetimeoffset&apos;) then &apos;(&apos; + cast(c.scale as va [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ColumnScript {
            get {
                return ResourceManager.GetString("ColumnScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select	definition
        ///from	sys.sql_modules
        ///union
        ///select	&apos;create table &apos; + s.name + &apos;.&apos; + t.name+ &apos; (&apos; + left(col.list, len(col.list) - 1) + &apos;)&apos;
        ///from	sys.tables t
        ///		inner join sys.schemas s
        ///			on	t.schema_id = s.schema_id
        ///		cross apply
        ///		(
        ///			select	c.name + &apos; &apos; + case
        ///						when c.is_computed = 1 then &apos;as &apos; + cc.definition
        ///						else tp.name +
        ///						case
        ///							when tp.name in (&apos;varchar&apos;, &apos;char&apos;, &apos;varbinary&apos;, &apos;binary&apos;) then &apos;(&apos; + case when c.max_length = -1 then &apos;max&apos; else cast(c.max_length as varc [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DatabaseScripts {
            get {
                return ResourceManager.GetString("DatabaseScripts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create table dbo.AugmentRegistry
        ///(
        ///	registry_name			varchar(250) not null,
        ///	sql_script				varchar(max) not null,
        ///	action_enum				varchar(20) not null,
        ///	updated_utc				datetime not null
        ///)
        ///go
        ///
        ///alter table dbo.AugmentRegistry
        ///	add constraint PK_AugmentRegistry
        ///	primary key(registry_name)
        ///go
        ///.
        /// </summary>
        internal static string RegistryMergeScript {
            get {
                return ResourceManager.GetString("RegistryMergeScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create table dbo.AugmentRegistry
        ///(
        ///	registry_name			varchar(250) not null,
        ///	sql_script				varchar(max) not null,
        ///	action_enum				varchar(20) not null,
        ///	updated_utc				datetime not null
        ///)
        ///go
        ///
        ///alter table dbo.AugmentRegistry
        ///	add constraint PK_AugmentRegistry
        ///	primary key(registry_name)
        ///go
        ///.
        /// </summary>
        internal static string RegistryScript {
            get {
                return ResourceManager.GetString("RegistryScript", resourceCulture);
            }
        }
    }
}
