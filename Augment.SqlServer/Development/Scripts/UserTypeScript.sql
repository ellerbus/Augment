--
--	GETS THE DEFINITION FOR A GIVEN TYPE{0}
--

select	st.name +
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
		inner join sys.types st
			on	t.system_type_id = st.user_type_id
where	t.is_table_type = 0
  and	t.is_user_defined = 1
  and	t.name = '{0}'