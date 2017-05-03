--
--	GETS COLUMN DEFINITIONS FOR A GIVEN TABLE{0}
--

select	c.name								as [name],
		case
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
		end									as [Definition]
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
where	c.object_id = object_id('{0}')
order by c.column_id
