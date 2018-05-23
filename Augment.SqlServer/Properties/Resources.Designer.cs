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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
        ///   Looks up a localized string similar to --
        ///--	GETS COLUMN DEFINITIONS FOR A GIVEN TABLE{0}
        ///--	TO BE USED FOR QUICK COMPARISONS
        ///--
        ///
        ///select	c.name								as [name],
        ///		case
        ///			when c.is_computed = 1 then &apos;as &apos; + cc.definition
        ///			else tp.name +
        ///			case
        ///				when tp.name in (&apos;varchar&apos;, &apos;char&apos;, &apos;varbinary&apos;, &apos;binary&apos;) then &apos;(&apos; + case when c.max_length = -1 then &apos;max&apos; else cast(c.max_length as varchar) end + &apos;)&apos;
        ///				when tp.name in (&apos;nvarchar&apos;, &apos;nchar&apos;) then &apos;(&apos; + case when c.max_length = -1 then &apos;max&apos; else cast(c.max_length/2 as varchar) end + [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ColumnScript {
            get {
                return ResourceManager.GetString("ColumnScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --
        ///--	CREATES A SCRIPT FOR ALL OBJECTS UTILIZED BY Augment.SqlServer
        ///--
        ///
        ///select	definition
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
        ///							when tp.name in (&apos;varchar&apos;, &apos;char&apos;, &apos;varbinary&apos;, &apos;binary&apos;) the [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DatabaseScript {
            get {
                return ResourceManager.GetString("DatabaseScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --
        ///--	LIST ALL OBJECTS AFFECTED BY CHANGES TO A PARENT OBJECT
        ///--
        ///select	distinct e.referenced_schema_name + &apos;.&apos; + e.referenced_entity_name				entity,
        ///		object_schema_name(e.referencing_id) + &apos;.&apos; + object_name(e.referencing_id)		impacts
        ///from	sys.sql_expression_dependencies e
        ///where	e.referenced_id != e.referencing_id
        ///union
        ///select	object_schema_name(k.parent_object_id) + &apos;.&apos; + object_name(k.parent_object_id),
        ///		object_schema_name(k.object_id) + &apos;.&apos; + object_name(k.parent_object_id) + &apos;.&apos; + object_name [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string EntityImpactScript {
            get {
                return ResourceManager.GetString("EntityImpactScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to create table dbo.AugmentRegistry
        ///(
        ///	registry_name			varchar(250) not null,
        ///	sql_script				varchar(max) not null,
        ///	status_enum				varchar(20) not null,
        ///	updated_utc				datetime not null,
        ///	updated_at              as dateadd(mi, datediff(mi, getutcdate(), getdate()), updated_utc),
        ///	updated_by				varchar(50) not null
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
        
        /// <summary>
        ///   Looks up a localized string similar to --
        ///--	GETS COLUMN DEFINITIONS FOR A GIVEN TABLE{0}
        ///--	TO BE MAPPED TO AN INTERNAL OBJECT
        ///--
        ///
        ///select	c.name								column_name,
        ///		isnull(i.is_primary_key, 0)			is_primary_key,
        ///		c.is_computed						is_computed,
        ///		case type_name(c.user_type_id)
        ///			when &apos;timestamp&apos; then 1
        ///			when &apos;rowversion&apos; then 1
        ///			else 0
        ///		end									is_timestamp
        ///from	sys.columns c
        ///		left join sys.index_columns ic
        ///			on	c.object_id = ic.object_id
        ///			and	c.column_id = ic.column_id
        ///		left join sys.indexes i
        ///			on	i.is_prim [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TableDefinitionScript {
            get {
                return ResourceManager.GetString("TableDefinitionScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --
        ///--	GETS THE DEFINITION FOR A GIVEN TYPE{0}
        ///--
        ///
        ///select	st.name +
        ///		case
        ///			when st.name in (&apos;varchar&apos;, &apos;char&apos;, &apos;varbinary&apos;, &apos;binary&apos;) then &apos;(&apos; + case when t.max_length = -1 then &apos;max&apos; else cast(t.max_length as varchar) end + &apos;)&apos;
        ///			when st.name in (&apos;nvarchar&apos;, &apos;nchar&apos;) then &apos;(&apos; + case when t.max_length = -1 then &apos;max&apos; else cast(t.max_length/2 as varchar) end + &apos;)&apos;
        ///			when st.name in (&apos;datetime2&apos;, &apos;time1&apos;, &apos;datetimeoffset&apos;) then &apos;(&apos; + cast(t.scale as varchar) + &apos;)&apos;
        ///			when st.name in (&apos;decimal&apos;) t [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UserTypeScript {
            get {
                return ResourceManager.GetString("UserTypeScript", resourceCulture);
            }
        }
    }
}
