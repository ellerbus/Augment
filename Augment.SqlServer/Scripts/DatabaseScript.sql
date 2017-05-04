--
--	CREATES A SCRIPT FOR ALL OBJECTS UTILIZED BY Augment.SqlServer
--

select	definition
from	sys.sql_modules
union
select	'create table ' + s.name + '.' + t.name+ ' (' + left(col.list, len(col.list) - 1) + ')'
from	sys.tables t
		inner join sys.schemas s
			on	t.schema_id = s.schema_id
		cross apply
		(
			select	c.name + ' ' + case
						when c.is_computed = 1 then 'as ' + cc.definition
						else tp.name +
						case
							when tp.name in ('varchar', 'char', 'varbinary', 'binary') then '(' + case when c.max_length = -1 then 'max' else cast(c.max_length as varchar) end + ')'
							when tp.name in ('nvarchar', 'nchar') then '(' + case when c.max_length = -1 then 'max' else cast(c.max_length/2 as varchar) end + ')'
							when tp.name in ('datetime2', 'time1', 'datetimeoffset') then '(' + cast(c.scale as varchar) + ')'
							when tp.name in ('decimal') then '(' + cast(c.precision as varchar) + ', ' + cast(c.scale as varchar) + ')'
							else ''
						end +
						case
							when c.collation_name is not null and c.collation_name != 'SQL_Latin1_General_CP1_CI_AS' then ' collate ' + c.collation_name
							else ''
						end +
						case
							when c.is_nullable = 0 then ' not'
							else ''
						end + ' null' +
						case
							when dc.definition is not null then ' default' + dc.definition
							else ''
						end +
						case
							when c.is_identity = 1 then ' identity(' + cast(isnull(ic.seed_value, 0) as varchar) + ', ' + cast(isnull(ic.increment_value, 0) as varchar) + ')'
							else ''
						end +
						''
					end + ', '
			from	sys.columns c
					inner join sys.types tp
						on	c.user_type_id = tp.user_type_id
					left join sys.identity_columns ic
						on	c.is_identity = 1
						and	c.object_id = ic.object_id
						and	c.column_id = ic.column_id
					left join sys.default_constraints dc
						on	c.default_object_id != 0
						and	c.object_id = dc.parent_object_id
						and	c.column_id = dc.parent_column_id
					left join sys.computed_columns cc
						on	c.is_computed = 1
						and	c.object_id = cc.object_id
						and	c.column_id = cc.column_id
			where	c.object_id = t.object_id
			order by c.column_id
			for xml path('')
		) col (list)
union
select	'create type ' + s.name + '.' + t.name+ ' as table (' + left(col.list, len(col.list) - 1) + ')'
from	sys.table_types t
		inner join sys.schemas s
			on	t.schema_id = s.schema_id
			and	t.is_table_type = 1
		cross apply
		(
			select	c.name + ' ' + case
						when c.is_computed = 1 then 'as ' + cc.definition
						else tp.name +
						case
							when tp.name in ('varchar', 'char', 'varbinary', 'binary') then '(' + case when c.max_length = -1 then 'max' else cast(c.max_length as varchar) end + ')'
							when tp.name in ('nvarchar', 'nchar') then '(' + case when c.max_length = -1 then 'max' else cast(c.max_length/2 as varchar) end + ')'
							when tp.name in ('datetime2', 'time1', 'datetimeoffset') then '(' + cast(c.scale as varchar) + ')'
							when tp.name in ('decimal') then '(' + cast(c.precision as varchar) + ', ' + cast(c.scale as varchar) + ')'
							else ''
						end +
						case
							when c.collation_name is not null and c.collation_name != 'SQL_Latin1_General_CP1_CI_AS' then ' collate ' + c.collation_name
							else ''
						end +
						case
							when c.is_nullable = 0 then ' not'
							else ''
						end + ' null' +
						case
							when dc.definition is not null then ' default' + dc.definition
							else ''
						end +
						case
							when c.is_identity = 1 then ' identity(' + cast(isnull(ic.seed_value, 0) as varchar) + ', ' + cast(isnull(ic.increment_value, 0) as varchar) + ')'
							else ''
						end +
						''
					end + ', '
			from	sys.columns c
					inner join sys.types tp
						on	c.user_type_id = tp.user_type_id
					left join sys.identity_columns ic
						on	c.is_identity = 1
						and	c.object_id = ic.object_id
						and	c.column_id = ic.column_id
					left join sys.default_constraints dc
						on	c.default_object_id != 0
						and	c.object_id = dc.parent_object_id
						and	c.column_id = dc.parent_column_id
					left join sys.computed_columns cc
						on	c.is_computed = 1
						and	c.object_id = cc.object_id
						and	c.column_id = cc.column_id
			where	c.object_id = t.type_table_object_id
			order by c.column_id
			for xml path('')
		) col (list)
union
select	'create type ' + s.name + '.' + t.name + ' from ' + st.name +
		case
			when st.name in ('varchar', 'char', 'varbinary', 'binary') then '(' + case when t.max_length = -1 then 'max' else cast(t.max_length as varchar) end + ')'
			when st.name in ('nvarchar', 'nchar') then '(' + case when t.max_length = -1 then 'max' else cast(t.max_length/2 as varchar) end + ')'
			when st.name in ('datetime2', 'time1', 'datetimeoffset') then '(' + cast(t.scale as varchar) + ')'
			when st.name in ('decimal') then '(' + cast(t.precision as varchar) + ', ' + cast(t.scale as varchar) + ')'
			else ''
		end +
		case
			when t.is_nullable = 0 then ' not'
			else ''
		end + ' null' +
		case
			when t.collation_name is not null and t.collation_name != 'SQL_Latin1_General_CP1_CI_AS' then ' collate ' + t.collation_name
			else ''
		end
from	sys.types t
		inner join sys.schemas s
			on	t.schema_id = s.schema_id
		inner join sys.types st
			on	t.system_type_id = st.user_type_id
where	t.is_table_type = 0
  and	t.is_user_defined = 1
union

select	'alter table ' + s.name + '.' + t.name +
		' add constraint ' + k.name + ' ' +
		case k.type
			when 'PK' then 'primary key'
			when 'UQ' then 'unique'
		end + ' (' + left(col.list, len(col.list) - 1) + ')'
from	sys.key_constraints k
		inner join sys.tables t
			on	k.parent_object_id = t.object_id
			and	k.type in ('PK', 'UQ')
		inner join sys.schemas s
			on	t.schema_id = s.schema_id
		inner join sys.indexes i
			on	t.object_id = i.object_id
			and	k.unique_index_id = i.index_id
		cross apply
		(
			select	c.name + ', '
			from	sys.index_columns ic 
					inner join sys.columns c
						on	ic.object_id = c.object_id
						and	ic.column_id = c.column_id
			where	ic.object_id = t.object_id
			  and	ic.index_id = i.index_id
			order by ic.key_ordinal
			for xml path('')
		) col (list)
union
select	'alter table ' + s.name + '.' + t.name +
		' add constraint ' + k.name +
		' foreign key (' + left(fkcol.list, len(fkcol.list) - 1) + ')' +
		' references ' + rs.name + '.' + rt.name +'(' + left(fkcol.list, len(fkcol.list) - 1) + ')' +
		case
			when k.delete_referential_action = 1 then ' on delete cascade'
			when k.delete_referential_action = 2 then ' on delete set null'
			when k.delete_referential_action = 3 then ' on delete set default'
			else ''
		end +
		case
			when k.update_referential_action = 1 then ' on update cascade'
			when k.update_referential_action = 2 then ' on update set null'
			when k.update_referential_action = 3 then ' on update set default'
			else ''
		end
from	sys.foreign_keys k
		inner join sys.tables t
			on	k.parent_object_id = t.object_id
			and	k.type in ('F')
		inner join sys.schemas s
			on	t.schema_id = s.schema_id
		inner join sys.tables rt
			on	k.referenced_object_id = rt.object_id
		inner join sys.schemas rs
			on	rt.schema_id = rs.schema_id
		cross apply
		(
			select	c.name + ', '
			from	sys.foreign_key_columns fc 
					inner join sys.columns c
						on	fc.parent_object_id = c.object_id
						and	fc.parent_column_id = c.column_id
			where	fc.parent_object_id = k.parent_object_id
			order by c.column_id
			for xml path('')
		) fkcol (list)
		cross apply
		(
			select	c.name + ', '
			from	sys.foreign_key_columns fc 
					inner join sys.columns c
						on	fc.referenced_object_id = c.object_id
						and	fc.referenced_column_id = c.column_id
			where	fc.referenced_object_id = k.referenced_object_id
			order by c.column_id
			for xml path('')
		) rfcol (list)
union
select	'create index ' + i.name + ' on ' + s.name + '.' + t.name + ' (' + left(icol.list, len(icol.list) - 1) + ')'
from	sys.indexes i
		inner join sys.tables t
			on	i.object_id = t.object_id
			and	i.is_primary_key = 0
			and	i.is_unique_constraint = 0
		inner join sys.schemas s
			on	t.schema_id = s.schema_id
		cross apply
		(
			select	c.name + ', '
			from	sys.index_columns ic 
					inner join sys.columns c
						on	ic.object_id = c.object_id
						and	ic.column_id = c.column_id
			where	ic.object_id = t.object_id
			  and	ic.index_id = i.index_id
			order by ic.key_ordinal
			for xml path('')
		) icol (list)